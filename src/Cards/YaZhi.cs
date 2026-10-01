using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 压制：对全体敌人造成 5（[gold]升级[/gold] 6）点伤害 3 次。将 2 张[gold]眩晕[/gold]加入[gold]抽牌堆[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class YaZhi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5m, ValueProp.Move),
    };

    public YaZhi()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override string CardTitle => "压制";

    protected override string CardDescription =>
        "对全体敌人造成 {Damage:diff()} 点伤害 3 次。将 2 张[gold]眩晕[/gold]加入[gold]抽牌堆[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < 3; i++)
        {
            await CreatureCmd.Damage(choiceContext, CombatState!.HittableEnemies, DynamicVars["Damage"].BaseValue, ValueProp.Move, Owner.Creature, this, cardPlay);
        }
        // [gold]眩晕[/gold]牌：复用原版机制（同原版"高速脱离"卡）——
        // 用 CombatState.CreateCard<Dazed> 泛型模板生成独立实例，不依赖 ModelId
        // （ModelDb.GetById 找不到会抛异常导致卡住，切勿再用）。
        for (int i = 0; i < 2; i++)
        {
            var dazed = CombatState!.CreateCard<Dazed>(cardPlay.Player);
            await CardPileCmd.AddGeneratedCardToCombat(dazed, PileType.Draw, cardPlay.Player, CardPilePosition.Random);
        }
    }
}
