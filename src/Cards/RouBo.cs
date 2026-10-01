using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 肉搏：造成 8（[gold]升级[/gold] 9）点伤害。获得 7（[gold]升级[/gold] 8）点护甲。获得 4 层[gold]活力[/gold]。下回合获得 6（[gold]升级[/gold] 7）点护甲。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class RouBo : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),
        new BlockVar(7m, ValueProp.Move),
        new DynamicVar("Vigor", 4m),
        new DynamicVar("NextBlock", 6m),
    };

    public RouBo()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "肉搏";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。获得 {Block:diff()} 点[gold]格挡[/gold]。获得 {Vigor:diff()} 层[gold]活力[/gold]。下回合获得 {NextBlock:diff()} 点[gold]格挡[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(1m);
        DynamicVars["Block"].UpgradeValueBy(1m);
        DynamicVars["NextBlock"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["Vigor"].BaseValue, Owner.Creature, this, false);
        await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["NextBlock"].BaseValue, Owner.Creature, this, false);
    }
}
