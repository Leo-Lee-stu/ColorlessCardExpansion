using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 觉悟：打出[gold]升级[/gold]后的牌时，本回合获得1点临时[gold]力量[/gold]与临时[gold]敏捷[/gold]。
/// </summary>
public sealed class JueWuPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/JueWu.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/JueWu.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "觉悟"),
        ("description", "打出[gold]升级[/gold]后的牌时，本回合获得1点临时[gold]力量[/gold]与临时[gold]敏捷[/gold]。"),
    };

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        if (cardPlay.Player == null || cardPlay.Player != Owner.Player)
        {
            return;
        }
        if (cardPlay.Card == null || cardPlay.Card.CurrentUpgradeLevel <= 0)
        {
            return;
        }
        // 层数 Amount = 打出张数，多张觉悟时力敏按张数叠加
        await PowerCmd.Apply<JueWuTempStrengthPower>(choiceContext, Owner, Amount, Owner, null, true);
        await PowerCmd.Apply<JueWuTempDexterityPower>(choiceContext, Owner, Amount, Owner, null, true);
    }
}
