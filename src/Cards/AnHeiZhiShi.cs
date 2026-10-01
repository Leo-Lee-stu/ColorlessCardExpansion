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
/// 暗黑知识：抽 1（[gold]升级[/gold] 2）张牌，并将其立即打出。消耗。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class AnHeiZhiShi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Cards", 1m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public AnHeiZhiShi()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "暗黑知识";

    protected override string CardDescription =>
        "抽 {Cards:diff()} 张牌，并将其立即打出。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Cards"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawn = (await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner, false)).ToList();
        foreach (var c in drawn)
        {
            await CardCmd.AutoPlay(choiceContext, c, null, AutoPlayType.Default, false, false);
        }
    }
}
