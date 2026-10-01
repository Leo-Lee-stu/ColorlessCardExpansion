using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 睡眠果实：消耗 1 张[gold]手牌[/gold]。获得 1 层人工制品。保留。消耗。[gold]升级[/gold]后费用 1→0。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ShuiMianGuoShi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Artifact", 1m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Retain,
        CardKeyword.Exhaust,
    };

    public ShuiMianGuoShi()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "睡眠果实";

    protected override string CardDescription =>
        "消耗 1 张[gold]手牌[/gold]。获得 {Artifact:diff()} 层人工制品。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = await CommonActions.SelectSingleCard(this, CardSelectorPrefs.ExhaustSelectionPrompt, choiceContext, PileType.Hand);
        if (card != null)
        {
            await CardCmd.Exhaust(choiceContext, card, false, false);
        }
        await PowerCmd.Apply<ArtifactPower>(choiceContext, Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this, false);
    }
}
