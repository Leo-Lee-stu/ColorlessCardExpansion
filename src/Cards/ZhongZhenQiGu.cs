using System;
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
/// 重振旗鼓：从[gold]弃牌堆[/gold]随机将 3 张牌加入[gold]手牌[/gold]。消耗。[gold]升级[/gold]后费用 1→0。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhongZhenQiGu : AbstractCceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public ZhongZhenQiGu()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "重振旗鼓";

    protected override string CardDescription =>
        "从[gold]弃牌堆[/gold]随机将 3 张牌加入[gold]手牌[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discard = PileTypeExtensions.GetPile(PileType.Discard, Owner).Cards.ToList();
        var picked = discard.OrderBy(_ => Random.Shared.Next()).Take(3).ToList();
        foreach (var c in picked)
        {
            await CardPileCmd.Add(c, PileType.Hand, (CardPilePosition)1, this, false);
        }
    }
}
