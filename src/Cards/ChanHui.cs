using System;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 忏悔：丢弃[gold]手牌[/gold]中的所有牌。每丢弃 1 张，对随机敌人造成 8（[gold]升级[/gold] 10）点伤害。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ChanHui : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),
    };

    public ChanHui()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
    }

    protected override string CardTitle => "忏悔";

    protected override string CardDescription =>
        "丢弃[gold]手牌[/gold]中的所有牌。每丢弃 1 张，对随机敌人造成 {Damage:diff()} 点伤害。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileTypeExtensions.GetPile(PileType.Hand, cardPlay.Player).Cards.ToList();
        if (hand.Count > 0)
        {
            await CardCmd.Discard(choiceContext, hand);
        }
        var enemies = CombatState!.HittableEnemies;
        for (int i = 0; i < hand.Count; i++)
        {
            if (enemies.Count == 0)
            {
                break;
            }
            var target = enemies[Random.Shared.Next(enemies.Count)];
            await CreatureCmd.Damage(choiceContext, target, DynamicVars["Damage"].BaseValue, ValueProp.Move, Owner.Creature, this, cardPlay);
        }
    }
}
