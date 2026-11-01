# -*- coding: utf-8 -*-
import struct, io, sys
PCK = r"C:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"
out_path = sys.argv[1] if len(sys.argv) > 1 else None
with open(PCK, "rb") as f:
    f.read(4); f.read(4); f.read(12); f.read(4)
    fb, do = struct.unpack("<QQ", f.read(16))
    f.seek(do)
    n = struct.unpack("<I", f.read(4))[0]
    paths = []
    for i in range(n):
        plen = struct.unpack("<I", f.read(4))[0]
        p = f.read(plen).decode("utf-8").rstrip("\x00").rstrip()
        f.read(16)  # offset,size
        f.read(16)  # md5
        f.read(4)   # flags
        paths.append(p)
print("total", len(paths))
hits = [p for p in paths if "local" in p.lower() or "loc_" in p.lower()]
print("local-ish count:", len(hits))
if out_path:
    with io.open(out_path, "w", encoding="utf-8") as o:
        o.write("\n".join(hits))
for p in hits[:120]:
    print(p)
