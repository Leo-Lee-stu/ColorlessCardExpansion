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
/// 高速连射：造成 2 点伤害 3 次。抽 1 张牌。[gold]升级[/gold]后造成 2 点伤害 4 次。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class GaoSuLianShe : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(2m, ValueProp.Move),
        new DynamicVar("Hits", 3m),
    };

    public GaoSuLianShe()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override string CardTitle => "高速连射";

    protected override string CardDescription =>
        "造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。抽 1 张牌。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Hits"].UpgradeValueBy(1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        for (int i = 0; i < DynamicVars["Hits"].IntValue; i++)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars["Damage"].BaseValue, ValueProp.Move, Owner.Creature, this, cardPlay);
        }
        await CardPileCmd.Draw(choiceContext, 1m, cardPlay.Player, false);
    }
}
