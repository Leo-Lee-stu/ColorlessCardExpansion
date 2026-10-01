using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 战斗准备：随机生成 1 张无色牌加入[gold]手牌[/gold]，本回合其耗能为 0。固有。[gold]升级[/gold]后生成的牌为[gold]升级[/gold]版。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhanDouZhunBei : AbstractCceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Innate };

    public ZhanDouZhunBei()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "战斗准备";

    protected override string CardDescription =>
        "随机生成 {IfUpgraded:show:1 张[gold]升级[/gold]后的|1 张}无色牌加入[gold]手牌[/gold]，本回合其耗能为 0。";

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
        var candidates = pool.GetUnlockedCards(cardPlay.Player.UnlockState, cardPlay.Player.RunState.CardMultiplayerConstraint);
        var generated = CardFactory.GetDistinctForCombat(cardPlay.Player, candidates, 1, cardPlay.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (generated == null)
        {
            return;
        }
        if (IsUpgradedCard)
        {
            CardCmd.Upgrade(generated);
        }
        generated.EnergyCost.SetThisTurnOrUntilPlayed(0, true);
        await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, cardPlay.Player, (CardPilePosition)1);
    }
}
