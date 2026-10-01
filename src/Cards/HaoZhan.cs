using System.Collections.Generic;
using System.Linq;
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
/// 好战：若[gold]手牌[/gold]中没有[gold]攻击牌[/gold]，抽 2 张牌，获得 3 层[gold]活力[/gold]，获得 1（[gold]升级[/gold] 2）点能量。
/// 三个效果都只在[gold]手牌[/gold]中无[gold]攻击牌[/gold]时生效。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class HaoZhan : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(1),
    };

    public HaoZhan()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "好战";

    protected override string CardDescription =>
        "若[gold]手牌[/gold]中没有[gold]攻击牌[/gold]，抽 2 张牌，获得 3 层[gold]活力[/gold]，获得{Energy:energyIcons()}。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Energy"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileTypeExtensions.GetPile(PileType.Hand, cardPlay.Player).Cards;
        if (!hand.Any(c => c.Type == CardType.Attack))
        {
            await CardPileCmd.Draw(choiceContext, 2m, cardPlay.Player, false);
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, 3m, Owner.Creature, this, false);
            await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue, Owner);
        }
    }
}
