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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 瞄准射击：造成10点伤害（[gold]升级[/gold]14）。若击杀非爪牙敌人，获得40金币（[gold]升级[/gold]50）。保留。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class MiaoZhunSheJi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("Gold", 40m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Retain };

    public MiaoZhunSheJi()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "瞄准射击";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害。若击杀非爪牙敌人，获得 {Gold:diff()} 金币。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(4m);
        DynamicVars["Gold"].UpgradeValueBy(10m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        var results = (await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext)).Results.SelectMany(r => r);
        if (results.Any(r => r.WasTargetKilled) && cardPlay.Target.GetPower<MinionPower>() == null)
        {
            await PlayerCmd.GainGold(DynamicVars["Gold"].IntValue, Owner, false);
        }
    }
}
