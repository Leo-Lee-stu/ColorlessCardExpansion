using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using ColorlessCardExpansion.src.powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace ColorlessCardExpansion.src.cards;

/// <summary>坚守荣光：回合开始时，受到2点伤害，获得2能量（[gold]升级[/gold]后费用-1）。</summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class JianShouRongGuang : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Glory", 1m),
        new EnergyVar(2),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<JianShouRongGuangPower>(null),
    };

    public JianShouRongGuang()
        : base(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override string CardTitle => "坚守荣光";

    protected override string CardDescription => "回合开始时，受到2点伤害，获得 {Energy:energyIcons()}。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<JianShouRongGuangPower>(choiceContext, Owner.Creature, DynamicVars["Glory"].BaseValue, Owner.Creature, this, false);
    }
}
