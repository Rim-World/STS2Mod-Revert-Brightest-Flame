# -*- coding: utf-8 -*-
"""Extract BrightestFlame localization keys from STS2 main pck (Godot pck v3).

用法: python TOOLS/scripts/extract-brightest-localization.py
输出: languages/raw/<lang>/cards.json + languages/原文汇总.md
"""
import struct, json, os, io

PCK = r"C:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
OUT = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "languages")

with open(PCK, "rb") as f:
    magic = f.read(4)
    assert magic == b"GDPC", f"bad magic {magic}"
    version = struct.unpack("<I", f.read(4))[0]
    ver_major, ver_minor, ver_patch = struct.unpack("<III", f.read(12))
    flags = struct.unpack("<I", f.read(4))[0]
    file_base, dir_offset = struct.unpack("<QQ", f.read(16))
    print(f"pck v{version}, godot {ver_major}.{ver_minor}.{ver_patch}, flags={flags}, file_base={file_base}, dir_offset={dir_offset}")

    f.seek(dir_offset)
    file_count = struct.unpack("<I", f.read(4))[0]
    print(f"file_count={file_count}")

    entries = []
    for _ in range(file_count):
        plen = struct.unpack("<I", f.read(4))[0]
        path = f.read(plen).decode("utf-8").rstrip("\x00").rstrip()
        off, size = struct.unpack("<QQ", f.read(16))
        md5 = f.read(16)
        eflags = struct.unpack("<I", f.read(4))[0]
        entries.append((path, off, size, eflags))
    print(f"parsed {len(entries)} entries")

    targets = []
    for path, off, size, eflags in entries:
        if path.endswith("cards.json") and "localization/" in path:
            targets.append((path, off, size))
    targets.sort()
    print(f"localization cards.json files: {len(targets)}")
    for t in targets:
        print("  ", t[0], t[2])

    os.makedirs(OUT, exist_ok=True)
    summary = []
    for path, off, size in targets:
        f.seek(file_base + off)
        data = f.read(size)
        lang = path.split("/")[-2]
        try:
            obj = json.loads(data.decode("utf-8"))
        except Exception as e:
            print(f"!! {path}: JSON parse failed: {e}")
            continue
        keys = sorted(k for k in obj.keys() if "BRIGHTEST" in k.upper())
        if not keys:
            continue
        raw_dir = os.path.join(OUT, "raw", lang)
        os.makedirs(raw_dir, exist_ok=True)
        subset = {k: obj[k] for k in keys}
        with io.open(os.path.join(raw_dir, "cards.json"), "w", encoding="utf-8") as out:
            json.dump(subset, out, ensure_ascii=False, indent=2)
        summary.append((lang, keys, subset))
        print(f"== {lang}: {keys}")

    md = ["# 至亮之焰（Brightest Flame）本地化原文（游戏 v0.111.0 pck 解包）", ""]
    md.append("> 来源：`SlayTheSpire2.pck` 内 `localization/<lang>/cards.json`；仅抽取含 `BRIGHTEST` 的键。")
    md.append("> `{...}` 为 SmartFormat/DynamicVar 占位符，渲染时由卡牌动态变量填充。")
    md.append("")
    for lang, keys, subset in summary:
        md.append(f"## {lang}")
        md.append("")
        for k in keys:
            md.append(f"### `{k}`")
            md.append("")
            md.append("```text")
            md.append(str(subset[k]))
            md.append("```")
            md.append("")
    with io.open(os.path.join(OUT, "原文汇总.md"), "w", encoding="utf-8") as out:
        out.write("\n".join(md))
    print("done ->", OUT)
