using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 主宰者：回合开始时，获得 1 点[gold]力量[/gold]。
/// </summary>
public sealed class ZhuZaiZhePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/ZhuZaiZhe.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/ZhuZaiZhe.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "主宰者"),
        ("description", "回合开始时，获得 1 点[gold]力量[/gold]。"),
    };

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        if (Owner == null || player != Owner.Player)
        {
            return;
        }
        // 层数 Amount = 打出张数，多张主宰者每回合力量叠加
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, null, null, true);
    }
}
