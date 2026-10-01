using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Cards;
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
/// 能量护盾：获得 5（[gold]升级[/gold] 7）点[gold]格挡[/gold]和 1 点能量。移除。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class NengLiangHuDun : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Block", 5m),
        new EnergyVar(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public NengLiangHuDun()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "能量护盾";

    protected override string CardDescription =>
        "获得 {Block:diff()} 点[gold]格挡[/gold]和 {Energy:energyIcons()}。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Block"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].BaseValue, ValueProp.Move, cardPlay, false);
        await PlayerCmd.GainEnergy(1m, Owner);
    }
}
