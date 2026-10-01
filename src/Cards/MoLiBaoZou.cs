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
/// 魔力暴走：使你的能量翻倍。本回合获得 1 点临时[gold]力量[/gold]与临时[gold]敏捷[/gold]。消耗。[gold]升级[/gold]后费用 1→0。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class MoLiBaoZou : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("StrDex", 1m),
        new EnergyVar(1),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    public MoLiBaoZou()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "魔力暴走";

    protected override string CardDescription =>
        "使你的 {Energy:energyIcons()} 翻倍。本回合获得 {StrDex:diff()} 点临时[gold]力量[/gold]与临时[gold]敏捷[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(Owner.PlayerCombatState!.Energy, Owner);
        await PowerCmd.Apply<MoLiTempStrengthPower>(choiceContext, Owner.Creature, DynamicVars["StrDex"].BaseValue, Owner.Creature, this, false);
        await PowerCmd.Apply<MoLiTempDexterityPower>(choiceContext, Owner.Creature, DynamicVars["StrDex"].BaseValue, Owner.Creature, this, false);
    }
}
