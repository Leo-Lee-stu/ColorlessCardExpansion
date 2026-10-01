using System.Collections.Generic;
using System.Linq;
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
/// 暴虐：给予所有敌人 1 层[gold]易伤[/gold]。[gold]手牌[/gold]中每有一张[gold]攻击牌[/gold]，本回合获得 2 点临时[gold]力量[/gold]。[gold]升级[/gold]后费用 1→0。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class BaoNve : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Vuln", 1m),
        new DynamicVar("PerAttackStr", 2m),
    };

    public BaoNve()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "暴虐";

    protected override string CardDescription =>
        "给予所有敌人 {Vuln:diff()} 层[gold]易伤[/gold]。[gold]手牌[/gold]中每有一张[gold]攻击牌[/gold]，本回合获得 {PerAttackStr:diff()} 点临时[gold]力量[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars["Vuln"].BaseValue, Owner.Creature, this, false);
        }
        int attackCount = PileTypeExtensions.GetPile(PileType.Hand, Owner).Cards.Count(c => c.Type == CardType.Attack);
        if (attackCount > 0)
        {
            await PowerCmd.Apply<BaoNveTempPower>(choiceContext, Owner.Creature, attackCount * DynamicVars["PerAttackStr"].BaseValue, Owner.Creature, this, false);
        }
    }
}
