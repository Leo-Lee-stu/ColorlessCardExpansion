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
/// 尖刺盾牌：获得 6 点护甲。获得 2 层[gold]荆棘[/gold]。[gold]升级[/gold]后 8 点护甲、3 层[gold]荆棘[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JianCiDunPai : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar("Thorns", 2m),
    };

    public JianCiDunPai()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "尖刺盾牌";

    protected override string CardDescription =>
        "获得 {Block:diff()} 点[gold]格挡[/gold]。获得 {Thorns:diff()} 层[gold]荆棘[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Thorns"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars["Thorns"].BaseValue, Owner.Creature, this, false);
    }
}
