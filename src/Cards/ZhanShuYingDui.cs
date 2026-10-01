using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 战术应对：从 3 张无色[gold]攻击牌[/gold]中选择 1 张加入[gold]手牌[/gold]。[gold]升级[/gold]后所选为[gold]升级[/gold]版。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhanShuYingDui : AbstractCceCard
{
    public ZhanShuYingDui()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "战术应对";

    protected override string CardDescription =>
        "从 3 张无色[gold]攻击牌[/gold]中选择 {IfUpgraded:show:1 张[gold]升级[/gold]后的|1 张}加入[gold]手牌[/gold]。";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player == null)
        {
            return;
        }
        var pool = ModelDb.AllCardPools.OfType<ColorlessCardPool>().FirstOrDefault();
        if (pool == null)
        {
            return;
        }
        var candidates = pool.GetUnlockedCards(cardPlay.Player.UnlockState, cardPlay.Player.RunState.CardMultiplayerConstraint)
            .Where(c => c.Type == CardType.Attack);
        var options = CardFactory.GetDistinctForCombat(cardPlay.Player, candidates, 3, cardPlay.Player.RunState.Rng.CombatCardGeneration).ToList();
        if (options.Count == 0)
        {
            return;
        }
        // 复用原版"丰饶"逻辑：先升级全部候选（选择界面直接显示[gold]升级[/gold]版），再选择。
        if (IsUpgradedCard)
        {
            foreach (var o in options)
            {
                CardCmd.Upgrade(o);
            }
        }
        var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_ADD_TO_HAND"), 1);
        var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, cardPlay.Player, prefs)).FirstOrDefault();
        if (picked != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(picked, PileType.Hand, cardPlay.Player, (CardPilePosition)1);
        }
    }
}
