using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 致命射击：造成9点伤害（[gold]升级[/gold]12）。若目标有[gold]易伤[/gold]或[gold]虚弱[/gold]，使其失去10点生命，否则给予1层[gold]易伤[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhiMingSheJi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("HpLoss", 10m),
        new DynamicVar("Vuln", 1m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<VulnerablePower>(null),
    };

    public ZhiMingSheJi()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "致命射击";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。若目标有[gold]易伤[/gold]或[gold]虚弱[/gold]，使其失去 {HpLoss:diff()} 点生命，否则给予 {Vuln:diff()} 层[gold]易伤[/gold]。";

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
        if (cardPlay.Target.GetPower<VulnerablePower>() != null || cardPlay.Target.GetPower<WeakPower>() != null)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this, cardPlay);
        }
        else
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars["Vuln"].BaseValue, Owner.Creature, this, false);
        }
    }
}
