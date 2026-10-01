using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using ColorlessCardExpansion.src.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 甲虫：回合结束时，获得 5（[gold]升级[/gold] 7）点[gold]格挡[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JiaChong : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(5m, ValueProp.Move),
    };

    public JiaChong()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "甲虫";

    protected override string CardDescription => "回合结束时，获得 {Block:diff()} 点[gold]格挡[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars.Block.UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 复用原版 Power 层数机制：层数 = 格挡数，多张叠加（基础 5 + 升级 7 = 12）。
        await PowerCmd.Apply<JiaChongPower>(choiceContext, Owner.Creature, DynamicVars.Block.BaseValue, Owner.Creature, this, false);
    }
}
