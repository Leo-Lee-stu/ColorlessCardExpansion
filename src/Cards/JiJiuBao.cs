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
/// 急救包：回复 5 点生命。每打出一张牌，此牌的费用减少 1（最低 0）。消耗。[gold]升级[/gold]后费用 3→2。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JiJiuBao : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Heal", 5m),
        new EnergyVar(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public JiJiuBao()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "急救包";

    protected override string CardDescription =>
        "回复 {Heal:diff()} 点生命。每打出一张牌，此牌的 {Energy:energyIcons()} 减少 1（最低 0）。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars["Heal"].BaseValue);
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
