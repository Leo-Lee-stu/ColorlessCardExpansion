using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using ColorlessCardExpansion.src.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 灵光乍现：回合开始时，在[gold]手牌[/gold]中生成 1 张无色牌，并赋予其消耗与虚无。[gold]升级[/gold]后生成的牌为强化版。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class LingGuangZhaXian : AbstractCceCard
{
    public LingGuangZhaXian()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "灵光乍现";

    protected override string CardDescription =>
        "回合开始时，在[gold]手牌[/gold]中生成 {IfUpgraded:show:1 张[gold]升级[/gold]后的|1 张}无色牌，并赋予其消耗与虚无。";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<LingGuangZhaXianPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
        if (power != null && IsUpgradedCard)
        {
            power.Upgraded = true;
        }
    }
}
