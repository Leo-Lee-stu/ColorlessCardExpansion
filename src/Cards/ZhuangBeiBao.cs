using System.Collections.Generic;
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
/// 装备包：抽 2（[gold]升级[/gold] 3）张牌。每打出一张牌，此牌的费用减少 1（最低 0）。消耗。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhuangBeiBao : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Cards", 2m),
        new EnergyVar(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public ZhuangBeiBao()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "装备包";

    protected override string CardDescription =>
        "抽 {Cards:diff()} 张牌。每打出一张牌，此牌的 {Energy:energyIcons()} 减少 1。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Cards"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player == null)
        {
            return;
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, cardPlay.Player, false);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        if (cardPlay.Card == null || cardPlay.Card == this)
        {
            return;
        }
        EnergyCost.AddUntilPlayed(-1, true);
    }
}
