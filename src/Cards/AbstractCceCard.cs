using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ColorlessCardExpansion.src.cards;

/// <summary>
/// 无色牌扩展的卡片基类。
/// 全卡面（无边框）方案：CustomFrame 返回整卡插画（res://ColorlessCardExpansion/images/&lt;类名&gt;.png），
/// 同时把 Portrait 小窗置为透明，让插画铺满整张卡。
/// </summary>
public abstract class AbstractCceCard : CustomCardModel
{
    public const string ImagePathPrefix = "res://ColorlessCardExpansion/images/";

    private static readonly ImageTexture TransparentTexture = ImageTexture.CreateFromImage(
        Image.CreateEmpty(1, 1, false, Image.Format.Rgba8));

    protected AbstractCceCard(int baseCost, CardType type, CardRarity rarity, TargetType target)
        : base(baseCost, type, rarity, target)
    {
    }

    /// <summary>全卡面插画：文件名必须等于类名（驼峰），例如 LanJie.png。</summary>
    public sealed override Texture2D? CustomFrame => GD.Load<Texture2D>($"{ImagePathPrefix:diff()}{GetType().Name}.png");

    /// <summary>
    /// 全卡面材质：必须与 CustomFrame 成对覆盖。
    /// 原版卡面的彩色效果由 HSV 着色器材质（CardPoolModel.CardFrameMaterialPath → FrameMaterial）对灰度遮罩着色而来；
    /// 我们的卡面是彩色插画，若继续用该 HSV 材质会被当成灰度遮罩处理而显示为黑白。
    /// 返回普通 CanvasItemMaterial 让插画按原始 RGB 直接显示。
    /// </summary>
    public sealed override Material? CreateCustomFrameMaterial => new CanvasItemMaterial();

    /// <summary>隐藏原版 Portrait 小窗，避免盖住全卡面插画。</summary>
    public sealed override Texture2D? CustomPortrait => TransparentTexture;

    protected abstract string CardTitle { get; }
    protected abstract string CardDescription { get; }

    public override List<(string, string)>? Localization => new()
    {
        ("title", CardTitle),
        ("description", CardDescription),
    };

    /// <summary>是否处于[gold]升级[/gold]状态（在 OnUpgrade 中置位）。</summary>
    protected bool IsUpgradedCard { get; private set; }

    /// <summary>
    /// [gold]升级[/gold]时对关键词的增删操作（(关键词, true=添加, false=移除)）。
    /// 注意：不能用 IsUpgradedCard 分支改写 CanonicalKeywords 来实现"[gold]升级[/gold]新增关键词"，
    /// 因为 CanonicalKeywords 只在卡创建时读取，[gold]升级[/gold]后不会重算；必须在这里用 AddKeyword/RemoveKeyword 直接修改卡实例。
    /// </summary>
    protected readonly List<(CardKeyword Keyword, bool Add)> UpgradeKeywordOps = new();

    protected override void OnUpgrade()
    {
        IsUpgradedCard = true;
        foreach (var (kw, add) in UpgradeKeywordOps)
        {
            if (add)
            {
                AddKeyword(kw);
            }
            else
            {
                RemoveKeyword(kw);
            }
        }
    }

    /// <summary>
    /// 条件费用刷新：当"[gold]抽牌堆[/gold]或[gold]弃牌堆[/gold]为空"时把费用降为 <see cref="ReductionWhenPilesEmpty"/>，
    /// 否则恢复原价。在玩家回合开始和任意卡牌打出后调用。
    /// 必须用钩子传入的 <see cref="Player"/> 而非 Owner（Owner 在部分时机可能为 null）。
    /// </summary>
    protected virtual int ReductionWhenPilesEmpty => 0;

    private static bool PilesEmpty(Player player)
    {
        if (player == null)
        {
            return false;
        }
        return PileTypeExtensions.GetPile(PileType.Draw, player).Cards.Count == 0
            || PileTypeExtensions.GetPile(PileType.Discard, player).Cards.Count == 0;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);
        RefreshConditionalCost(player);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        RefreshConditionalCost(cardPlay.Player);
    }

    private void RefreshConditionalCost(Player player)
    {
        if (ReductionWhenPilesEmpty <= 0)
        {
            return;
        }
        bool empty = PilesEmpty(player);
        // 每次都追加绝对费用修饰器：减费用 reduceOnly（只在降低时生效），恢复用无条件绝对费，
        // 这样交替刷新时最后一个修饰器始终决定正确费用，避免旧修饰器状态残留。
        EnergyCost.SetThisTurnOrUntilPlayed(empty ? CanonicalEnergyCost - ReductionWhenPilesEmpty : CanonicalEnergyCost, empty);
    }
}
