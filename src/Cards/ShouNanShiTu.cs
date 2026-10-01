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
/// 受难使徒：抽牌直至[gold]手牌[/gold]上限（10 张），每抽 1 张牌受到 2（[gold]升级[/gold] 1）点伤害。
/// 文档 target 原为 enemy，但效果全部作用于自身，故按 Self 实现（打出无需选择目标）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ShouNanShiTu : AbstractCceCard
{
    private const int HandLimit = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("HpLoss", 2m),
    };

    public ShouNanShiTu()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "受难使徒";

    protected override string CardDescription =>
        "抽牌直至[gold]手牌[/gold]上限，每抽 1 张牌受到 {HpLoss:diff()} 点伤害。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["HpLoss"].UpgradeValueBy(-1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Player;
        while (true)
        {
            var hand = PileTypeExtensions.GetPile(PileType.Hand, player).Cards;
            var draw = PileTypeExtensions.GetPile(PileType.Draw, player).Cards;
            if (hand.Count >= HandLimit || draw.Count == 0)
            {
                break;
            }
            var drawn = await CardPileCmd.Draw(choiceContext, 1, player, false);
            if (drawn == null || !drawn.Any())
            {
                break;
            }
            // 不可[gold]格挡[/gold]伤害，等价于"失去生命"式的自伤
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable, null, this, cardPlay);
        }
    }
}
