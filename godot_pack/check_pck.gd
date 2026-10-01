extends SceneTree

func _init():
    var pck = "D:/Program Files (x86)/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.pck"
    ProjectSettings.load_resource_pack(pck, true)
    # 搜所有本地化/数据 json 里的 DAZED 上下文（找模型 ID 线索）
    var paths = [
        "res://localization/zhs/card_library.json",
        "res://localization/zhs/cards.json",
        "res://localization/zhs/bestiary.json",
    ]
    for p in paths:
        if ResourceLoader.exists(p):
            var fa = FileAccess.open(p, FileAccess.READ)
            var txt = fa.get_as_text()
            var idx = txt.find("DAZED")
            if idx >= 0:
                print("=== ", p, " ===")
                print(txt.substr(max(0, idx-80), 220).replace("\n", "\\n"))
    quit()
