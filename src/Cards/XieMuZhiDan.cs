using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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
/// 谢幕之弹：造成 3（[gold]升级[/gold] 5）点伤害。若在[gold]手牌[/gold]中，回合结束时伤害永久 +3（[gold]升级[/gold] +4）。保留。消耗。
/// [gold]升级[/gold]成长值按用户平衡调整由 +6 等比降为 +4，可再调。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class XieMuZhiDan : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new DynamicVar("Growth", 3m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Retain,
        CardKeyword.Exhaust,
    };

    public XieMuZhiDan()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "谢幕之弹";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。若在[gold]手牌[/gold]中，回合结束时伤害 +{Growth:diff()}。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(2m);
        DynamicVars["Growth"].UpgradeValueBy(1m);
    }

    /// <summary>
    /// 卡牌自身监听回合结束（手牌中时 ShouldReceiveCombatHooks 为 true，框架会分发该钩子）。
    /// 在手牌中则永久提升伤害；每张实例独立成长、可叠加；卡不离开手牌 → [gold]保留[/gold]正常；
    /// 不产生额外 Power 图标。
    /// </summary>
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> creatures)
    {
        await base.AfterSideTurnEnd(choiceContext, side, creatures);
        if (side != CombatSide.Player || Owner == null)
        {
            return;
        }
        if (Pile?.Type == PileType.Hand)
        {
            DynamicVars["Damage"].UpgradeValueBy(DynamicVars["Growth"].BaseValue);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
