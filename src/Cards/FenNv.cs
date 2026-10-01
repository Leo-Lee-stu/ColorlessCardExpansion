using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using ColorlessCardExpansion.src.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 愤怒：造成 8（[gold]升级[/gold] 11）点伤害。本回合获得 3 点临时[gold]力量[/gold]；每打出 1 次此牌，获得的[gold]力量[/gold] +1。
/// 描述中的临时[gold]力量[/gold]数值随打出次数动态显示（AngerVar 重写 UpdateCardPreview）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class FenNv : AbstractCceCard
{
    internal int TimesPlayedThisCombat;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),
        new FenNvAngerVar(),
    };

    public FenNv()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "愤怒";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。本回合获得 {AngerStr:diff()} 点临时[gold]力量[/gold]（每打出 1 次此牌，获得的[gold]力量[/gold] +1）。";

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
        decimal amount = 3m + TimesPlayedThisCombat;
        await PowerCmd.Apply<FenNvTempPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this, false);
        TimesPlayedThisCombat++;
    }
}

/// <summary>愤怒的临时[gold]力量[/gold]数值：预览时显示 3 + 已打出次数。</summary>
public sealed class FenNvAngerVar : DynamicVar
{
    public FenNvAngerVar()
        : base("AngerStr", 3m)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
        if (card is FenNv fenNv)
        {
            PreviewValue = 3m + fenNv.TimesPlayedThisCombat;
        }
    }
}
