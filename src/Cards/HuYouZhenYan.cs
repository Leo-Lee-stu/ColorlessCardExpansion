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
/// 护佑箴言：获得 10（[gold]升级[/gold] 13）点护甲。若打出时没有护甲，额外获得 10 点护甲。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class HuYouZhenYan : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(10m, ValueProp.Move),
        new DynamicVar("Bonus", 10m),
    };

    public HuYouZhenYan()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "护佑箴言";

    protected override string CardDescription =>
        "获得 {Block:diff()} 点[gold]格挡[/gold]。若打出时没有[gold]格挡[/gold]，额外获得 {Bonus:diff()} 点[gold]格挡[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Block"].UpgradeValueBy(3m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal total = DynamicVars.Block.BaseValue;
        if (Owner.Creature.Block <= 0)
        {
            total += DynamicVars["Bonus"].BaseValue;
        }
        await CreatureCmd.GainBlock(Owner.Creature, total, ValueProp.Move, cardPlay, false);
    }
}
