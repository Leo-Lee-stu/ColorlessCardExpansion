using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using ColorlessCardExpansion.src.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 协商：获得 2 点[gold]力量[/gold]。所有敌人获得 2 点[gold]力量[/gold]。本回合获得 3 点临时[gold]力量[/gold]。保留。消耗。[gold]升级[/gold]后费用 1→0。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class XieShang : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Str", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Retain,
        CardKeyword.Exhaust,
    };

    public XieShang()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "协商";

    protected override string CardDescription =>
        "获得 {Str:diff()} 点[gold]力量[/gold]。所有敌人获得 {Str:diff()} 点[gold]力量[/gold]。本回合获得 3 点临时[gold]力量[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["Str"].BaseValue, Owner.Creature, this, false);
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, DynamicVars["Str"].BaseValue, Owner.Creature, this, true);
        }
        await PowerCmd.Apply<XieShangTempPower>(choiceContext, Owner.Creature, 3m, Owner.Creature, this, false);
    }
}
