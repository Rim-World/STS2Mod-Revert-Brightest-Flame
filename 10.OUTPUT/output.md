# 构建产物与发布包索引

> 最后更新：2026-11-01
> 本目录内容不入库（`.gitignore` 已忽略，仅保留本文件）。

| 时间 | 名称 | 版本号 | 大小 | 说明 |
|---|---|---|---|---|
| 2026-08-15 | `01-构建产物/RevertBrightestFlame（1.0版本存档）/` | 1.0 | 285 KB（dll 7,168 / json 374 / pck 219,776 / pdb 12,820 B） | v1.0 安装包快照（单补丁 `CanonicalVars`），含创意工坊上传状态目录 |
| 2026-08-16 | `01-构建产物/RevertBrightestFlame（1.1版本存档）/` | 1.1 | 300 KB（dll 15,360 / json 374 / pck 225,072 / pdb 14,880 B） | v1.1 安装包快照（彻底写死，含 16 语言 pck），含创意工坊上传状态目录 |
| 2026-11-01 | `02-发布包/RevertBrightestFlame.v1.1.1.zip` | 1.1.1 | 231 KB（236,849 B） | Release `v1.1.1` 安装包：dll + pck + json + pdb，已排除 `.sts2-workshop-uploader/` |
| 2026-11-01 | `02-发布包/RevertBrightestFlameSourceCode.zip` | 1.1.1 | 144 KB（147,382 B） | Release `v1.1.1` 源码包：C#/Godot 工程 + 16 语言 `cards.json`，已排除 `.godot/`、`bin/`、`obj/` 与 dll/pck/pdb |

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
