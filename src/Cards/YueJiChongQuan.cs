using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 跃击冲拳：造成 9（[gold]升级[/gold] 12）点伤害。抽 1 张牌。奇巧：每打出一张[gold]攻击牌[/gold]，此牌的费用减少 1（最低 0）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class YueJiChongQuan : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Cards", 1m),
        new EnergyVar(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Sly };

    public YueJiChongQuan()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "跃击冲拳";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。抽 {Cards:diff()} 张牌。每打出一张[gold]攻击牌[/gold]，此牌的 {Energy:energyIcons()} 减少 1。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(3m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, cardPlay.Player, false);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        if (cardPlay.Card == null || cardPlay.Card == this)
        {
            return;
        }
        if (cardPlay.Card.Type == CardType.Attack)
        {
            EnergyCost.AddUntilPlayed(-1, true);
        }
    }
}
