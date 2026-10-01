using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 灵光乍现：回合开始时，在[gold]手牌[/gold]中生成 1 张无色牌，并赋予其消耗与虚无。
/// [gold]升级[/gold]后生成的牌为强化版。
/// </summary>
public sealed class LingGuangZhaXianPower : CustomPowerModel
{
    /// <summary>来源卡是否为[gold]升级[/gold]版（生成强化牌）。</summary>
    public bool Upgraded { get; set; }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/LingGuangZhaXian.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/LingGuangZhaXian.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "灵光乍现"),
        ("description", "回合开始时，在[gold]手牌[/gold]中生成 1 张无色牌，并赋予其消耗与虚无。"),
    };

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        if (player != Owner.Player)
        {
            return;
        }
        var pool = ModelDb.AllCardPools.OfType<ColorlessCardPool>().FirstOrDefault();
        if (pool == null)
        {
            return;
        }
        var candidates = pool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint);
        // 层数 Amount = 打出张数，多张灵光乍现每回合生成多张
        for (int i = 0; i < Amount; i++)
        {
            var generated = CardFactory.GetDistinctForCombat(player, candidates, 1, player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (generated == null)
            {
                continue;
            }
            if (Upgraded)
            {
                CardCmd.Upgrade(generated);
            }
            CardCmd.ApplyKeyword(generated, CardKeyword.Exhaust, CardKeyword.Ethereal);
            await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, player, CardPilePosition.Top);
        }
    }
}
