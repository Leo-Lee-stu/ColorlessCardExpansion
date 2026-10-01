using System;
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
/// 使徒仲裁者：每回合首次受到伤害后，获得等量[gold]活力[/gold]层数。
/// [gold]升级[/gold]：费用 -1（2 → 1）并获得固有。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class ShiTuZhongCaiZhe : AbstractCceCard
{
    public ShiTuZhongCaiZhe()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        // [gold]升级[/gold]：费用 -1 + 获得固有
        UpgradeKeywordOps.Add((CardKeyword.Innate, true));
    }

    protected override string CardTitle => "使徒仲裁者";

    protected override string CardDescription => "每回合首次受到伤害后，获得等量[gold]活力[/gold]层数。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ShiTuZhongCaiZhePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
}
