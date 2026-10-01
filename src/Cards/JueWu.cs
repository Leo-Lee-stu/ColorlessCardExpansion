using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using ColorlessCardExpansion.src.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 觉悟：打出[gold]升级[/gold]后的牌时，本回合获得 1 点临时[gold]力量[/gold]与临时[gold]敏捷[/gold]。[gold]升级[/gold]后固有。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JueWu : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("StrDex", 1m),
    };

    public JueWu()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        UpgradeKeywordOps.Add((CardKeyword.Innate, true));
    }

    protected override string CardTitle => "觉悟";

    protected override string CardDescription =>
        "打出[gold]升级[/gold]后的牌时，本回合获得 {StrDex:diff()} 点临时[gold]力量[/gold]与临时[gold]敏捷[/gold]。";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<JueWuPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
}
