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

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 消散之风：选择并消耗[gold]抽牌堆[/gold]与[gold]手牌[/gold]中的 1 张牌。所有敌人获得 1（[gold]升级[/gold] 2）层[gold]虚弱[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class XiaoSanZhiFeng : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Weak", 1m),
    };

    public XiaoSanZhiFeng()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "消散之风";

    protected override string CardDescription =>
        "选择并消耗[gold]抽牌堆[/gold]与[gold]手牌[/gold]中的 1 张牌。所有敌人获得 {Weak:diff()} 层[gold]虚弱[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Weak"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileTypeExtensions.GetPile(PileType.Hand, cardPlay.Player).Cards;
        var draw = PileTypeExtensions.GetPile(PileType.Draw, cardPlay.Player).Cards;
        var candidates = hand.Concat(draw).ToList();
        if (candidates.Count > 0)
        {
            var prefs = new CardSelectorPrefs(new LocString("card_selection", "TO_EXHAUST"), 1);
            var picked = (await CardSelectCmd.FromSimpleGrid(choiceContext, candidates, cardPlay.Player, prefs)).FirstOrDefault();
            if (picked != null)
            {
                await CardCmd.Exhaust(choiceContext, picked);
            }
        }
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this, false);
        }
    }
}
