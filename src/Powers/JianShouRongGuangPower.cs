using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 坚守荣光：回合开始时，受到2点伤害，获得2能量。
/// 图标使用卡面缩略图（powers/Small 用于角色脚下小图标，powers 用于大头像）。
/// </summary>
public sealed class JianShouRongGuangPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/JianShouRongGuang.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/JianShouRongGuang.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "坚守荣光"),
        ("description", "回合开始时，受到2点伤害，获得2能量。"),
    };

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        if (Owner.Player != player)
        {
            return;
        }
        // 层数 Amount = 打出张数，多张坚守荣光每回合能量/自伤按张数叠加
        await CreatureCmd.Damage(choiceContext, Owner, 2m * Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner, null, null);
        await PlayerCmd.GainEnergy(2m * Amount, player);
    }
}
