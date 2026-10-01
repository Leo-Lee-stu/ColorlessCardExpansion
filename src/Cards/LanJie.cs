using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>拦截：造成 40 点伤害。若[gold]抽牌堆[/gold]或[gold]弃牌堆[/gold]没任何牌，则此牌的费用减少 1（[gold]升级[/gold]后减少 2）。</summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class LanJie : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(40m, ValueProp.Move),
        new DynamicVar("Reduction", 1m),
        new EnergyVar(1),
    };

    public LanJie()
        : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "拦截";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。若[gold]抽牌堆[/gold]或[gold]弃牌堆[/gold]没任何牌，则此牌的 {Energy:energyIcons()} -{Reduction:diff()}。";

    protected override int ReductionWhenPilesEmpty => DynamicVars["Reduction"].IntValue;

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Reduction"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars.Var<DamageVar>().BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
