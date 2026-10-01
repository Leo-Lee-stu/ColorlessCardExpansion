import os, re, glob

cards_dir = r"C:\Users\LI\Desktop\MOD\ColorlessCardExpansion\src\Cards"
powers_dir = r"C:\Users\LI\Desktop\MOD\ColorlessCardExpansion\src\Powers"

# 1) energy formatter 加括号（先做，避免被 diff 正则误伤）
def fix_energy(t):
    return t.replace("{Energy:energyIcons}", "{Energy:energyIcons()}")

# 2) 纯变量 {X} -> {X:diff()}（无 formatter 的才加）
var_re = re.compile(r"\{([A-Za-z][A-Za-z0-9]*)\}")
def fix_diff(t):
    return var_re.sub(r"{\1:diff()}", t)

# 3) [gold] 词（长词优先，避免短词嵌套进长词）
gold_words = [
    "抽牌堆", "弃牌堆", "攻击牌", "技能牌", "能力牌",
    "手牌", "易伤", "虚弱", "力量", "敏捷", "活力", "格挡",
    "荆棘", "眩晕", "中毒", "升级",
]
def fix_gold(t):
    for w in gold_words:
        # 跳过已经包在 [gold]...[/gold] 里的
        t = t.replace("[gold]" + w + "[/gold]", "\u0000" + w + "\u0000")
        t = t.replace(w, "[gold]" + w + "[/gold]")
        t = t.replace("\u0000" + w + "\u0000", "[gold]" + w + "[/gold]")
    return t

total = 0
for d in (cards_dir, powers_dir):
    for f in glob.glob(os.path.join(d, "*.cs")):
        with open(f, "r", encoding="utf-8") as fh:
            t = fh.read()
        t2 = fix_energy(t)
        t2 = fix_diff(t2)
        t2 = fix_gold(t2)
        if t2 != t:
            with open(f, "w", encoding="utf-8") as fh:
                fh.write(t2)
            total += 1
print("files modified:", total)
