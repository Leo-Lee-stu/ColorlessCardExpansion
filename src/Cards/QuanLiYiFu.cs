using System;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 全力以赴：造成14点伤害2次（[gold]升级[/gold]16）。每有1点[gold]敏捷[/gold]使伤害+2（[gold]升级[/gold]+3）。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class QuanLiYiFu : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(14m, ValueProp.Move),
        new DynamicVar("DexBonus", 2m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<DexterityPower>(null),
    };

    public QuanLiYiFu()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "全力以赴";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害2次。每有1点[gold]敏捷[/gold]使伤害+{DexBonus:diff()}。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(2m);
        DynamicVars["DexBonus"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        decimal dexterity = Owner.Creature.GetPower<DexterityPower>()?.Amount ?? 0;
        decimal damage = DynamicVars["Damage"].BaseValue + dexterity * DynamicVars["DexBonus"].IntValue;
        await DamageCmd.Attack(damage)
            .WithHitCount(2)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
