extends SceneTree

func _init():
    var pck_path = "D:/Program Files (x86)/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.pck"
    ProjectSettings.load_resource_pack(pck_path, true)
    var dir = DirAccess.open("res://localization")
    if dir == null:
        print("FAIL: no res://localization")
        quit(1)
        return
    dir.list_dir_begin()
    var f = dir.get_next()
    while f != "":
        if dir.current_is_dir() and not f.begins_with("."):
            print("locale dir: ", f)
        f = dir.get_next()
    dir.list_dir_end()
    quit()
