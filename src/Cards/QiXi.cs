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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 奇袭：造成 4 点伤害。若目标有[gold]易伤[/gold]或[gold]虚弱[/gold]，额外造成 20 点伤害并消耗。[gold]升级[/gold]后获得保留。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class QiXi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("Bonus", 20m),
    };

    public QiXi()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        // [gold]升级[/gold]：获得保留
        UpgradeKeywordOps.Add((CardKeyword.Retain, true));
    }

    protected override string CardTitle => "奇袭";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。若目标有[gold]易伤[/gold]或[gold]虚弱[/gold]，额外造成 {Bonus:diff()} 点伤害并消耗。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Bonus"].UpgradeValueBy(3m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        bool bonus = cardPlay.Target.GetPower<VulnerablePower>() != null || cardPlay.Target.GetPower<WeakPower>() != null;
        decimal dmg = DynamicVars["Damage"].BaseValue + (bonus ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(dmg)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (bonus)
        {
            await CardCmd.Exhaust(choiceContext, this);
        }
    }
}
