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
/// 战略起点：从[gold]抽牌堆[/gold]中将 2 张[gold]能力牌[/gold]加入[gold]手牌[/gold]。获得 1（[gold]升级[/gold] 2）点能量。固有。消耗。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhanLveQiDian : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("PowerCards", 2m),
        new EnergyVar(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Innate,
        CardKeyword.Exhaust,
    };

    public ZhanLveQiDian()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "战略起点";

    protected override string CardDescription =>
        "从[gold]抽牌堆[/gold]中将 {PowerCards:diff()} 张[gold]能力牌[/gold]加入[gold]手牌[/gold]。获得 {Energy:energyIcons()}。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Energy"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player == null)
        {
            return;
        }
        var powerCards = PileTypeExtensions.GetPile(PileType.Draw, cardPlay.Player).Cards
            .Where(c => c.Type == CardType.Power)
            .Take((int)DynamicVars["PowerCards"].BaseValue)
            .ToList();
        foreach (var c in powerCards)
        {
            await CardPileCmd.Add(c, PileType.Hand, (CardPilePosition)1, this, false);
        }
        await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue, cardPlay.Player);
    }
}
