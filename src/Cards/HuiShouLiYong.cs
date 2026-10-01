using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 回收利用：选择并消耗 2 张[gold]手牌[/gold]。获得 1 点能量与 6（[gold]升级[/gold] 8）点[gold]格挡[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class HuiShouLiYong : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Block", 6m),
        new EnergyVar(1),
    };

    public HuiShouLiYong()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "回收利用";

    protected override string CardDescription =>
        "选择并消耗 2 张[gold]手牌[/gold]。获得 {Energy:energyIcons()} 与 {Block:diff()} 点[gold]格挡[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Block"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileTypeExtensions.GetPile(PileType.Hand, cardPlay.Player).Cards;
        if (hand.Count == 0)
        {
            return;
        }
        int toPick = Math.Min(2, hand.Count);
        var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_EXHAUST"), toPick);
        var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, cardPlay.Player, prefs)).ToList();
        foreach (var c in picked)
        {
            await CardCmd.Exhaust(choiceContext, c);
        }
        await PlayerCmd.GainEnergy(1m, cardPlay.Player);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].BaseValue, ValueProp.Move, cardPlay, false);
    }
}
