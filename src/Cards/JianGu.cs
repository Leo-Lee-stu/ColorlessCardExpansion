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
/// 坚固：获得 15（[gold]升级[/gold] 18）点护甲。下回合获得 10（[gold]升级[/gold] 12）点护甲。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JianGu : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(15m, ValueProp.Move),
        new DynamicVar("NextBlock", 10m),
    };

    public JianGu()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "坚固";

    protected override string CardDescription =>
        "获得 {Block:diff()} 点[gold]格挡[/gold]。下回合获得 {NextBlock:diff()} 点[gold]格挡[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Block"].UpgradeValueBy(3m);
        DynamicVars["NextBlock"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["NextBlock"].BaseValue, Owner.Creature, this, false);
    }
}
