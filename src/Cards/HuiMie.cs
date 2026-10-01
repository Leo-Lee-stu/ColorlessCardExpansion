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
/// 毁灭：丢弃 1 张[gold]手牌[/gold]。造成 18（[gold]升级[/gold] 21）点伤害。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class HuiMie : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(18m, ValueProp.Move),
    };

    public HuiMie()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "毁灭";

    protected override string CardDescription =>
        "丢弃 1 张[gold]手牌[/gold]。造成 {Damage:diff()} 点伤害。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(3m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        var hand = PileTypeExtensions.GetPile(PileType.Hand, cardPlay.Player).Cards;
        if (hand.Count > 0)
        {
            var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_DISCARD"), 1);
            var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, cardPlay.Player, prefs)).FirstOrDefault();
            if (picked != null)
            {
                await CardCmd.Discard(choiceContext, picked);
            }
        }
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
