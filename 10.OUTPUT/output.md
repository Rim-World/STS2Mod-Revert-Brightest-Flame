# 构建产物与发布包索引

> 最后更新：2026-11-01
> 本目录内容不入库（`.gitignore` 已忽略，仅保留本文件）。

| 时间 | 名称 | 版本号 | 大小 | 说明 |
|---|---|---|---|---|
| 2026-08-15 | `01-构建产物/RevertBrightestFlame（1.0版本存档）/` | 1.0 | 285 KB（dll 7,168 / json 374 / pck 219,776 / pdb 12,820 B） | v1.0 安装包快照（单补丁 `CanonicalVars`），含创意工坊上传状态目录 |
| 2026-08-16 | `01-构建产物/RevertBrightestFlame（1.1版本存档）/` | 1.1 | 300 KB（dll 15,360 / json 374 / pck 225,072 / pdb 14,880 B） | v1.1 安装包快照（彻底写死，含 16 语言 pck），含创意工坊上传状态目录 |
| 2026-11-01 | `02-发布包/RevertBrightestFlame.v1.1.1.zip` | 1.1.1 | 231 KB（236,849 B） | Release `v1.1.1` 安装包：dll + pck + json + pdb，已排除 `.sts2-workshop-uploader/` |
| 2026-11-01 | `02-发布包/RevertBrightestFlameSourceCode.zip` | 1.1.1 | 144 KB（147,382 B） | Release `v1.1.1` 源码包：C#/Godot 工程 + 16 语言 `cards.json`，已排除 `.godot/`、`bin/`、`obj/` 与 dll/pck/pdb |
| 2026-11-13 09:17 | `01-构建产物/RevertBrightestFlame（1.1.2版本存档）/` | 1.1.2 | 253 KB（dll 16,896 / pck 225,120 / json 388 / pdb 16,324 B） | 0.113 兼容修复构建：`LoseMaxHp` 运行时探测；同时已覆盖安装到游戏 `mods/RevertBrightestFlame/`；SHA-256 `208da7d2…02a5b3f`（DLL）、`72b73d82…ab69bc28`（PCK） |

## 产物构成

| 文件 | 说明 |
|---|---|
| `RevertBrightestFlame.dll` | Harmony 补丁程序集（13 个补丁类 + 形状守卫） |
| `RevertBrightestFlame.pck` | `mod_image.png` 占位图标 + 16 语言 `cards.json` |
| `RevertBrightestFlame.json` | mod 清单（id / 版本 / `min_game_version` / 依赖） |
| `RevertBrightestFlame.pdb` | 调试符号（可选分发） |
| `.sts2-workshop-uploader/` | 创意工坊上传器的本地状态（含 `state.workshop` 与预览图），**不对外分发** |

## 发布包

Release 资产由 `gh release create` 直接从游戏安装目录与源码目录打包生成；打包结果不入库。

- `v1.1.1`：<https://github.com/Rim-World/STS2Mod-Revert-Brightest-Flame/releases/tag/v1.1.1>
  （`RevertBrightestFlame.v1.1.1.zip` + `RevertBrightestFlameSourceCode.zip`）
| 2026-11-13 10:21 | `02-发布包/RevertBrightestFlame.v1.1.2.zip` | 1.1.2 | 233 KB（238,840 B） | GitHub Release v1.1.2 安装包；dll + pck + json + pdb |
| 2026-11-13 10:21 | `02-发布包/RevertBrightestFlameSourceCode.v1.1.2.zip` | 1.1.2 | 138 KB（141,134 B） | v1.1.2 源码包；排除 `.godot/`、`bin/`、`obj/` 与二进制产物 |
| 2026-11-13 10:21 | GitHub Release `v1.1.2` | 1.1.2 | 两个资产 | <https://github.com/Rim-World/STS2Mod-Revert-Brightest-Flame/releases/tag/v1.1.2> |
