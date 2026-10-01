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
/// 主宰者：获得 2（[gold]升级[/gold] 3）点[gold]力量[/gold]。回合开始时，获得 1 点[gold]力量[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhuZaiZhe : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Strength", 2m),
    };

    public ZhuZaiZhe()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "主宰者";

    protected override string CardDescription =>
        "获得 {Strength:diff()} 点[gold]力量[/gold]。回合开始时，获得 1 点[gold]力量[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Strength"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this, false);
        await PowerCmd.Apply<ZhuZaiZhePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
}
