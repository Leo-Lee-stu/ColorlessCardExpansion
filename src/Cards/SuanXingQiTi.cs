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
/// 酸性气体：给予全体敌人 1（[gold]升级[/gold] 2）层[gold]易伤[/gold]与[gold]虚弱[/gold]。保留。消耗。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class SuanXingQiTi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VulnerablePower>("Vuln", 1m),
        new PowerVar<WeakPower>("Weak", 1m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Retain,
        CardKeyword.Exhaust,
    };

    public SuanXingQiTi()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "酸性气体";

    protected override string CardDescription =>
        "给予全体敌人 {Vuln:diff()} 层[gold]易伤[/gold]与 {Weak:diff()} 层[gold]虚弱[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Vuln"].UpgradeValueBy(1m);
        DynamicVars["Weak"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.HittableEnemies;
        await PowerCmd.Apply<VulnerablePower>(choiceContext, enemies, DynamicVars["Vuln"].BaseValue, Owner.Creature, this, false);
        await PowerCmd.Apply<WeakPower>(choiceContext, enemies, DynamicVars["Weak"].BaseValue, Owner.Creature, this, false);
    }
}
