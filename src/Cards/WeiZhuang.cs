using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 伪装：获得1层缓冲，下回合抽2张牌。消耗（[gold]升级[/gold]后费用-1）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class WeiZhuang : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Buffer", 1m),
        new DynamicVar("NextDraw", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BufferPower>(null),
        HoverTipFactory.FromPower<DrawCardsNextTurnPower>(null),
    };

    public WeiZhuang()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "伪装";

    protected override string CardDescription =>
        "获得 {Buffer:diff()} 层缓冲。下回合抽 {NextDraw:diff()} 张牌。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BufferPower>(choiceContext, Owner.Creature, DynamicVars["Buffer"].BaseValue, Owner.Creature, this, false);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["NextDraw"].BaseValue, Owner.Creature, this, false);
    }
}
