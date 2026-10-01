using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 虚无吞噬者：目标失去 2 点[gold]力量[/gold]。获得 2 点[gold]力量[/gold]。消耗。[gold]升级[/gold]后费用 1→0。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class XuWuTunShiZhe : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("StrDrain", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public XuWuTunShiZhe()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "虚无吞噬者";

    protected override string CardDescription =>
        "目标失去 {StrDrain:diff()} 点[gold]力量[/gold]。获得 {StrDrain:diff()} 点[gold]力量[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target, -DynamicVars["StrDrain"].BaseValue, Owner.Creature, this, false);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["StrDrain"].BaseValue, Owner.Creature, this, false);
    }
}
