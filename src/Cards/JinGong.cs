using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 进攻!：从[gold]抽牌堆[/gold]中选择 1 张[gold]攻击牌[/gold]并抽取。基础版带虚无；[gold]升级[/gold]后去除虚无并增加保留。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JinGong : AbstractCceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Ethereal };

    public JinGong()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        // [gold]升级[/gold]：移除虚无，增加保留
        UpgradeKeywordOps.Add((CardKeyword.Retain, true));
        UpgradeKeywordOps.Add((CardKeyword.Ethereal, false));
    }

    protected override string CardTitle => "进攻!";

    protected override string CardDescription => "从[gold]抽牌堆[/gold]中选择 1 张[gold]攻击牌[/gold]并抽取。";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var draw = PileTypeExtensions.GetPile(PileType.Draw, cardPlay.Player).Cards;
        var attacks = draw.Where(c => c.Type == CardType.Attack).ToList();
        if (attacks.Count == 0)
        {
            return;
        }
        var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_ADD_TO_HAND"), 1);
        var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, attacks, cardPlay.Player, prefs)).FirstOrDefault();
        if (picked != null)
        {
            await CardPileCmd.Add(picked, PileType.Hand, CardPilePosition.Top, this, false);
        }
    }
}
