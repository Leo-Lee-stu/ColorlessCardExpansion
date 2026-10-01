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
/// 精密瞄准：造成 12（[gold]升级[/gold] 15）点伤害。抽 1 张牌。选择 1 张[gold]手牌[/gold]放到[gold]抽牌堆[/gold]顶。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JingMiMiaoZhun : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(12m, ValueProp.Move),
    };

    public JingMiMiaoZhun()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "精密瞄准";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。抽 1 张牌。选择 1 张[gold]手牌[/gold]放到[gold]抽牌堆[/gold]顶。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(3m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, 1m, cardPlay.Player, false);
        var hand = PileTypeExtensions.GetPile(PileType.Hand, cardPlay.Player).Cards;
        if (hand.Count > 0)
        {
            var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_PLACE_ON_TOP"), 1);
            var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, cardPlay.Player, prefs)).FirstOrDefault();
            if (picked != null)
            {
                await CardPileCmd.Add(picked, PileType.Draw, CardPilePosition.Top, this, false);
            }
        }
    }
}
