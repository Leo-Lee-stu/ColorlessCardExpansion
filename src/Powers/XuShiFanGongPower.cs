using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 蓄势反攻的计数器：记录本场战斗中使用次数，每使用 1 次，下次使用时额外获得 1 层[gold]活力[/gold]。
/// </summary>
public sealed class XuShiFanGongPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/XuShiFanGong.png";

    public override string? CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/XuShiFanGong.png";

    public override List<(string, string)>? Localization => new()
    {
        ("title", "蓄势反攻"),
        ("description", "每使用 1 次蓄势反攻，下次使用时额外获得 1 层[gold]活力[/gold]。"),
    };
}
