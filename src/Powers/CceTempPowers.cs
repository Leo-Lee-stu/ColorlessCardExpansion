using BaseLib.Abstracts;
using ColorlessCardExpansion.src.cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ColorlessCardExpansion.src.powers;

/// <summary>
/// 临时[gold]力量[/gold]/[gold]敏捷[/gold] Power 子类集合。
/// 复用 BaseLib 的 CustomTemporaryPowerModelWrapper：自动处理
/// "先加对应属性（Strength/Dexterity）、数值变化同步、回合末扣回并移除"的完整生命周期，
/// 图标使用各来源卡的卡面缩略图（powers/Small 用于角色脚下小图标，powers 用于大头像）。
/// </summary>

public sealed class FenNvTempPower : CustomTemporaryPowerModelWrapper<FenNv, StrengthPower>
{
    public override string CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/FenNv.png";
    public override string CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/FenNv.png";
}

public sealed class BaoNveTempPower : CustomTemporaryPowerModelWrapper<BaoNve, StrengthPower>
{
    public override string CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/BaoNve.png";
    public override string CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/BaoNve.png";
}

public sealed class JueWuTempStrengthPower : CustomTemporaryPowerModelWrapper<JueWu, StrengthPower>
{
    public override string CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/JueWu.png";
    public override string CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/JueWu.png";
}

public sealed class JueWuTempDexterityPower : CustomTemporaryPowerModelWrapper<JueWu, DexterityPower>
{
    public override string CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/JueWu.png";
    public override string CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/JueWu.png";
}

public sealed class MoLiTempStrengthPower : CustomTemporaryPowerModelWrapper<MoLiBaoZou, StrengthPower>
{
    public override string CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/MoLiBaoZou.png";
    public override string CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/MoLiBaoZou.png";
}

public sealed class MoLiTempDexterityPower : CustomTemporaryPowerModelWrapper<MoLiBaoZou, DexterityPower>
{
    public override string CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/MoLiBaoZou.png";
    public override string CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/MoLiBaoZou.png";
}

public sealed class XieShangTempPower : CustomTemporaryPowerModelWrapper<XieShang, StrengthPower>
{
    public override string CustomPackedIconPath => "res://ColorlessCardExpansion/images/powers/Small/XieShang.png";
    public override string CustomBigIconPath => "res://ColorlessCardExpansion/images/powers/XieShang.png";
}
