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
/// 乱和：造成5点伤害3次。若目标有[gold]虚弱[/gold]，伤害+3；若目标有[gold]易伤[/gold]，伤害次数+1（[gold]升级[/gold]后费用-1）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class LuanHe : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("BonusDamage", 3m),
        new DynamicVar("BonusHits", 1m),
    };

    public LuanHe()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "乱和";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害3次。若目标有[gold]虚弱[/gold]，伤害+{BonusDamage:diff()}；若目标有[gold]易伤[/gold]，伤害次数+1。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        int hits = 3 + (cardPlay.Target.GetPower<VulnerablePower>() != null ? DynamicVars["BonusHits"].IntValue : 0);
        decimal damage = DynamicVars["Damage"].BaseValue
            + (cardPlay.Target.GetPower<WeakPower>() != null ? DynamicVars["BonusDamage"].IntValue : 0);
        await DamageCmd.Attack(damage)
            .WithHitCount(hits)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
