extends SceneTree

func _init():
    var pck_path = "C:/Users/LI/Desktop/MOD/ColorlessCardExpansion/ColorlessCardExpansion.pck"
    var loaded = ProjectSettings.load_resource_pack(pck_path, true)
    print("PCK loaded: ", loaded)
    if not loaded:
        quit(1)
        return

    var dirs = [
        "res://ColorlessCardExpansion/images",
        "res://ColorlessCardExpansion/images/powers",
        "res://ColorlessCardExpansion/images/powers/Small",
    ]
    var totals = {}
    for d in dirs:
        var dir = DirAccess.open(d)
        if dir == null:
            print("FAIL open: ", d)
            continue
        var n = 0
        dir.list_dir_begin()
        var f = dir.get_next()
        while f != "":
            if f.ends_with(".png"):
                n += 1
            f = dir.get_next()
        dir.list_dir_end()
        totals[d] = n
        print("dir: ", d, " -> ", n, " files")

    var cards = totals.get("res://ColorlessCardExpansion/images", -1)
    var powers = totals.get("res://ColorlessCardExpansion/images/powers", -1)
    var small = totals.get("res://ColorlessCardExpansion/images/powers/Small", -1)
    if cards == 60 and powers >= 13 and small >= 13:
        print("VERIFY OK: cards=", cards, " powers=", powers, " small=", small)
        quit(0)
    else:
        print("VERIFY FAIL: cards=", cards, " powers=", powers, " small=", small)
        quit(1)
