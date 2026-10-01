using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 机械人偶士兵：造成 6（[gold]升级[/gold] 7）点伤害 4 次。你受到伤害或打出[gold]能力牌[/gold]后，此牌本场战斗费用永久 -1。保留。
/// [gold]升级[/gold]后能量花费 -1。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JiXieRenOuShiBing : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
        new EnergyVar(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Retain };

    public JiXieRenOuShiBing()
        : base(5, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "机械人偶士兵";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害 4 次。你受到伤害或打出[gold]能力牌[/gold]后，此牌的 {Energy:energyIcons()} 消耗永久 -1。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(1m);
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        for (int i = 0; i < 4; i++)
        {
            await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        if (cardPlay.Card == null || cardPlay.Card == this)
        {
            return;
        }
        if (cardPlay.Card.Type == CardType.Power)
        {
            EnergyCost.AddThisCombat(-1, true);
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature receiver, DamageResult result, ValueProp props, Creature? source, CardModel? sourceCard)
    {
        await base.AfterDamageReceived(choiceContext, receiver, result, props, source, sourceCard);
        if (Owner?.Creature == null || receiver != Owner.Creature)
        {
            return;
        }
        if (result.UnblockedDamage <= 0)
        {
            return;
        }
        EnergyCost.AddThisCombat(-1, true);
    }
}
