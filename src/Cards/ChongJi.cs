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
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 冲击：对所有敌人造成 16（[gold]升级[/gold] 18）点伤害。所有敌人失去 1 点[gold]力量[/gold]，并获得 2（[gold]升级[/gold] 3）层[gold]易伤[/gold]。消耗。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ChongJi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(16m, ValueProp.Move),
        new DynamicVar("Vuln", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public ChongJi()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "冲击";

    protected override string CardDescription =>
        "对所有敌人造成 {Damage:diff()} 点伤害。所有敌人失去 1 点[gold]力量[/gold]，并获得 {Vuln:diff()} 层[gold]易伤[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(2m);
        DynamicVars["Vuln"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, CombatState!.HittableEnemies, DynamicVars["Damage"].BaseValue, ValueProp.Move, Owner.Creature, this, cardPlay);
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -1m, Owner.Creature, this, true);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars["Vuln"].BaseValue, Owner.Creature, this, false);
        }
    }
}
