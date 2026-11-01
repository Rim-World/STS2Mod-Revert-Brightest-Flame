# 构建产物与发布包索引

> 最后更新：2026-11-01
> 本目录内容不入库（`.gitignore` 已忽略，仅保留本文件）。

| 时间 | 名称 | 版本号 | 大小 | 说明 |
|---|---|---|---|---|
| 2026-08-15 | `01-构建产物/RevertBrightestFlame（1.0版本存档）/` | 1.0 | 285 KB（dll 7,168 / json 374 / pck 219,776 / pdb 12,820 B） | v1.0 安装包快照（单补丁 `CanonicalVars`），含创意工坊上传状态目录 |
| 2026-08-16 | `01-构建产物/RevertBrightestFlame（1.1版本存档）/` | 1.1 | 300 KB（dll 15,360 / json 374 / pck 225,072 / pdb 14,880 B） | v1.1 安装包快照（彻底写死，含 16 语言 pck），含创意工坊上传状态目录 |

## 产物构成

| 文件 | 说明 |
|---|---|
| `RevertBrightestFlame.dll` | Harmony 补丁程序集（13 个补丁类 + 形状守卫） |
| `RevertBrightestFlame.pck` | `mod_image.png` 占位图标 + 16 语言 `cards.json` |
| `RevertBrightestFlame.json` | mod 清单（id / 版本 / `min_game_version` / 依赖） |
| `RevertBrightestFlame.pdb` | 调试符号（可选分发） |
| `.sts2-workshop-uploader/` | 创意工坊上传器的本地状态（含 `state.workshop` 与预览图），**不对外分发** |

## 发布包

Release 资产由 `gh release create` 直接从游戏安装目录与源码目录打包生成，
命名规则见 `4.SDD/001-总体设计.md` 与仓库根 `README.md`；打包结果不入库。
