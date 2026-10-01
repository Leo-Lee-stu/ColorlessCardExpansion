using ColorlessCardExpansion.src.cards;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace ColorlessCardExpansion.src;

/// <summary>
/// 全卡面视觉补丁：隐藏原版卡片的"插画相框"（PortraitBorder）和 SDF 高亮框（Highlight），
/// 让 CustomFrame 的整卡插画完整露出，实现无边框全卡面效果。
/// </summary>
[HarmonyPatch(typeof(NCard), "UpdatePortrait")]
internal static class CceCardVisualPatch
{
    [HarmonyPostfix]
    private static void HideBorders(NCard __instance)
    {
        if (__instance.Model is not AbstractCceCard)
        {
            return;
        }

        TextureRect? portraitBorder = __instance.GetNodeOrNull<TextureRect>("%PortraitBorder");
        if (portraitBorder != null)
        {
            portraitBorder.Visible = false;
        }

        // SDF 高亮框：图鉴/静止时呈现青蓝色卡片轮廓，属于装饰层，一并隐藏。
        // 悬停高亮暂时保留（NCardHighlight 由脚本控制，若仍有边框残留可再隐藏）。
        NCardHighlight? highlight = __instance.GetNodeOrNull<NCardHighlight>("%Highlight");
        if (highlight != null)
        {
            // 只去掉静止轮廓：把纹理清空并保持节点可用
            highlight.Texture = null;
        }
    }
}
