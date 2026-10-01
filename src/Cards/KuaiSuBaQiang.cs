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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 快速拔枪：造成 7（[gold]升级[/gold] 8）点伤害。从[gold]抽牌堆[/gold]中选择 1 张[gold]攻击牌[/gold]并抽取。固有。消耗。[gold]升级[/gold]后额外给予 1 层[gold]易伤[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class KuaiSuBaQiang : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(7m, ValueProp.Move),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Innate,
        CardKeyword.Exhaust,
    };

    public KuaiSuBaQiang()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "快速拔枪";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。从[gold]抽牌堆[/gold]中选择 1 张[gold]攻击牌[/gold]并抽取{IfUpgraded:show:。给予 1 层[gold]易伤[/gold]。|。}";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (IsUpgradedCard)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, 1m, Owner.Creature, this, false);
        }
        var draw = PileTypeExtensions.GetPile(PileType.Draw, cardPlay.Player).Cards;
        var attacks = draw.Where(c => c.Type == CardType.Attack).ToList();
        if (attacks.Count > 0)
        {
            var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_ADD_TO_HAND"), 1);
            var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, attacks, cardPlay.Player, prefs)).FirstOrDefault();
            if (picked != null)
            {
                await CardPileCmd.Add(picked, PileType.Hand, CardPilePosition.Top, this, false);
            }
        }
    }
}
