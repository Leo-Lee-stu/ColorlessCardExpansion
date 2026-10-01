using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 战略重构：丢弃所有[gold]手牌[/gold]。抽 5（[gold]升级[/gold] 6）张牌。消耗。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhanLveChongGou : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Cards", 5m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public ZhanLveChongGou()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "战略重构";

    protected override string CardDescription =>
        "丢弃所有[gold]手牌[/gold]。抽 {Cards:diff()} 张牌。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Cards"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileTypeExtensions.GetPile(PileType.Hand, cardPlay.Player).Cards.ToList();
        if (hand.Count > 0)
        {
            await CardCmd.Discard(choiceContext, hand);
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, cardPlay.Player, false);
    }
}
