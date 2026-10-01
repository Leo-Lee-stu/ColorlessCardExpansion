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
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 连斩：X 费。造成 2X 次 5（[gold]升级[/gold] 7）点伤害。保留。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class LianZhan : AbstractCceCard
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5m, ValueProp.Move),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Retain };

    public LianZhan()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "连斩";

    protected override string CardDescription =>
        "造成 2X 次 {Damage:diff()} 点伤害。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Damage"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        int x = ResolveEnergyXValue();
        for (int i = 0; i < 2 * x; i++)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars["Damage"].BaseValue, ValueProp.Move, Owner.Creature, this, cardPlay);
        }
    }
}
