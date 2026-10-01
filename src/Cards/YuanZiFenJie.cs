using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 原子分解：所有敌人失去25点生命（[gold]升级[/gold]31）。伤害已按平衡调整下调1。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class YuanZiFenJie : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("HpLoss", 25m),
    };

    public YuanZiFenJie()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "原子分解";

    protected override string CardDescription => "所有敌人失去 {HpLoss:diff()} 点生命。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["HpLoss"].UpgradeValueBy(6m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, CombatState!.HittableEnemies, DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this, cardPlay);
    }
}
