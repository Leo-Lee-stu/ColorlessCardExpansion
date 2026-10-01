using BaseLib.Patches.Localization;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace ColorlessCardExpansion;

public static class CceConstants
{
    /// <summary>MOD 内部 ID（manifest id 必须一致，用于资源路径 res:// 前缀和卡牌 ID 前缀）。</summary>
    public const string ModId = "ColorlessCardExpansion";
}

[ModInitializer("Initialize")]
public class CceMod
{
    public static void Initialize()
    {
        // 默认语言设为简体中文：玩家使用英文时也会先加载 zhs 表作为兜底
        DefaultLoc.Set(CceConstants.ModId, "zhs");
        // 图鉴解锁补丁：把本 MOD 的卡标记为"已见"
        new Harmony($"com.{CceConstants.ModId}.mod").PatchAll();
        GD.Print("[INFO] [ColorlessCardExpansion] initialized");
    }
}
