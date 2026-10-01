using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Cards;
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
/// 战术射击：对所有敌人造成 7（[gold]升级[/gold] 10）点伤害。选择[gold]抽牌堆[/gold]中的 1 张牌并抽取。固有。移除。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ZhanShuSheJi : AbstractCceCard
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

    public ZhanShuSheJi()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "战术射击";

    protected override string CardDescription =>
        "对所有敌人造成 {Damage:diff()} 点伤害。选择[gold]抽牌堆[/gold]中的 1 张牌并抽取。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(3m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, CombatState!.HittableEnemies, DynamicVars["Damage"].BaseValue, ValueProp.Move, Owner.Creature, this, cardPlay);
        var draw = PileTypeExtensions.GetPile(PileType.Draw, cardPlay.Player).Cards;
        if (draw.Count > 0)
        {
            var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_ADD_TO_HAND"), 1);
            var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, draw, cardPlay.Player, prefs)).FirstOrDefault();
            if (picked != null)
            {
                await CardPileCmd.Add(picked, PileType.Hand, CardPilePosition.Top, this, false);
            }
        }
    }
}
