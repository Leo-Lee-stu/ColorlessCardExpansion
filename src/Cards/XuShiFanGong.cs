using System.Collections.Generic;
using System.Linq;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 蓄势反攻：[gold]格挡[/gold]翻倍。获得 5（[gold]升级[/gold] 7）层[gold]活力[/gold]。每使用 1 次，下次使用时额外获得 1 层[gold]活力[/gold]。
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class XuShiFanGong : AbstractCceCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Vigor", 5m),
    };

    public XuShiFanGong()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override string CardTitle => "蓄势反攻";

    protected override string CardDescription =>
        "[gold]格挡[/gold]翻倍。获得 {Vigor:diff()} 层[gold]活力[/gold]。每使用 1 次，下次使用时额外获得 1 层[gold]活力[/gold]。";

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Vigor"].UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal curBlock = Owner.Creature.Block;
        if (curBlock > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, curBlock, (ValueProp)0, cardPlay, false);
        }
        var counter = Owner.Creature.Powers.OfType<XuShiFanGongPower>().FirstOrDefault();
        int bonus = counter != null ? (int)counter.Amount : 0;
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["Vigor"].BaseValue + bonus, Owner.Creature, this, false);
        await PowerCmd.Apply<XuShiFanGongPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this, false);
    }
}
