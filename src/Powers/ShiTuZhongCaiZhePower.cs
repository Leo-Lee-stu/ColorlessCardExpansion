using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 使徒仲裁者：每回合首次受到伤害后，获得等量[gold]活力[/gold]层数。
/// </summary>
public sealed class ShiTuZhongCaiZhePower : CustomPowerModel
{
    private bool _triggeredThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/ShiTuZhongCaiZhe.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/ShiTuZhongCaiZhe.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "使徒仲裁者"),
        ("description", "每回合首次受到伤害后，获得等量[gold]活力[/gold]层数。"),
    };

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature receiver, DamageResult result, ValueProp props, Creature? source, CardModel? sourceCard)
    {
        await base.AfterDamageReceived(choiceContext, receiver, result, props, source, sourceCard);
        if (_triggeredThisTurn || Owner == null || receiver != Owner)
        {
            return;
        }
        if (result.UnblockedDamage <= 0)
        {
            return;
        }
        _triggeredThisTurn = true;
        // 层数 Amount = 打出张数，多张仲裁者时活力按张数翻倍
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner, result.UnblockedDamage * Amount, null, null, true);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> creatures)
    {
        await base.AfterSideTurnEnd(choiceContext, side, creatures);
        if (side == CombatSide.Player)
        {
            _triggeredThisTurn = false;
        }
    }
}
