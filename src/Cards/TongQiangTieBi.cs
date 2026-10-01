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
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 铜墙铁壁：获得 3 点[gold]敏捷[/gold]；回合开始时获得 1 点[gold]敏捷[/gold]。[gold]升级[/gold]后费用 2→1。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class TongQiangTieBi : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Dex", 3m),
    };

    public TongQiangTieBi()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "铜墙铁壁";

    protected override string CardDescription =>
        "获得 {Dex:diff()} 点[gold]敏捷[/gold]。回合开始时获得 1 点[gold]敏捷[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars["Dex"].BaseValue, Owner.Creature, this, false);
        await PowerCmd.Apply<TongQiangTieBiPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
}
