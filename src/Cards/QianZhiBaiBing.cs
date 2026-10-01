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
/// 钱治百病：回复 4（[gold]升级[/gold] 6）点生命。获得 30 金币。消耗。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class QianZhiBaiBing : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Heal", 4m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public QianZhiBaiBing()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "钱治百病";

    protected override string CardDescription =>
        "回复 {Heal:diff()} 点生命。获得 30 金币。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Heal"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars["Heal"].BaseValue, true);
        await PlayerCmd.GainGold(30m, Owner);
    }
}
