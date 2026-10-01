using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 甲虫：回合结束时，获得[gold]格挡[/gold]。
/// 层数 Amount = 格挡数（打出基础 5 / 升级 7 时累加），多张叠加。
/// </summary>
public sealed class JiaChongPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/JiaChong.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/JiaChong.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "甲虫"),
        ("description", "回合结束时，获得[gold]格挡[/gold]。"),
    };

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> creatures)
    {
        await base.AfterSideTurnEnd(choiceContext, side, creatures);
        if (Owner == null || side != Owner.Side)
        {
            return;
        }
        if (!creatures.Contains(Owner))
        {
            return;
        }
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null, false);
    }
}
