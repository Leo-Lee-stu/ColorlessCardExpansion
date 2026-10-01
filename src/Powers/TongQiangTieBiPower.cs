using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 铜墙铁壁：获得3点[gold]敏捷[/gold]；回合开始时获得1点[gold]敏捷[/gold]。
/// </summary>
public sealed class TongQiangTieBiPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/TongQiangTieBi.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/TongQiangTieBi.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "铜墙铁壁"),
        ("description", "回合开始时获得1点[gold]敏捷[/gold]。"),
    };

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        if (Owner.Player != player)
        {
            return;
        }
        // 层数 Amount = 打出张数，多张铜墙铁壁每回合敏捷叠加
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, Amount, Owner, null, true);
    }
}
