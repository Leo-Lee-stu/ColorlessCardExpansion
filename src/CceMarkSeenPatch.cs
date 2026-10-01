using ColorlessCardExpansion.src.cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Saves;

namespace ColorlessCardExpansion.src;

/// <summary>
/// 图鉴刷新可见性时，把本 MOD 的卡标记为"已见"，从而在图鉴中直接解锁显示（无需先获得卡）。
/// </summary>
[HarmonyPatch(typeof(NCardLibraryGrid), "RefreshVisibility")]
internal static class CceMarkSeenPatch
{
    [HarmonyPrefix]
    public static void MarkSeen()
    {
        foreach (CardPoolModel pool in ModelDb.AllCardPools)
        {
            foreach (CardModel card in pool.AllCards)
            {
                if (card is AbstractCceCard)
                {
                    SaveManager.Instance.MarkCardAsSeen(card);
                }
            }
        }
    }
}
