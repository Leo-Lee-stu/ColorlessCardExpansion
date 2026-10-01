using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 颅骨重击：造成 10（[gold]升级[/gold] 12）点伤害。目标的[gold]易伤[/gold]与[gold]虚弱[/gold]层数翻倍（最多 99）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class LuGuZhongJi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
    };

    public LuGuZhongJi()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "颅骨重击";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。目标的[gold]易伤[/gold]与[gold]虚弱[/gold]层数翻倍（最多 99）。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await DoubleDebuff<VulnerablePower>(choiceContext, cardPlay.Target);
        await DoubleDebuff<WeakPower>(choiceContext, cardPlay.Target);
    }

    private async Task DoubleDebuff<T>(PlayerChoiceContext ctx, Creature target) where T : PowerModel
    {
        var existing = target.Powers.OfType<T>().FirstOrDefault();
        if (existing == null)
        {
            return;
        }
        int cur = (int)existing.Amount;
        int doubled = Math.Min(99, cur * 2);
        if (doubled > cur)
        {
            await PowerCmd.Apply<T>(ctx, target, doubled - cur, Owner.Creature, this, true);
        }
    }
}
