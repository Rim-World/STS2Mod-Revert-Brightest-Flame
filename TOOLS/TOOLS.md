# 工具与自写脚本索引

> 最后更新：2026-11-01

## 外部工具依赖

| 时间 | 工具 | 版本 | 用途 | 来源 | 安装方式 |
|---|---|---|---|---|---|
| 2026-08 | .NET SDK | 9.0 | 编译 C#（MSIL） | 系统已装（`C:\DevTools`） | 全局安装 |
| 2026-08 | Godot（mono） | 4.5.1-stable | 资源导入与 `--export-pack` 打包 pck | 本机 `C:\Godot&GDRE` | 免安装 |
| 2026-08 | HarmonyLib | 随游戏 | 运行时方法补丁 | 游戏目录 `data_sts2_windows_x86_64/0Harmony.dll` | 随游戏提供 |

本项目未向 `TOOLS/` 下载安装任何免安装工具。

## 自写脚本

| 时间 | 脚本 | 用途 | 版本 | 大小 | 来源 | 依赖 |
|---|---|---|---|---|---|---|
| 2026-08-16 | `scripts/extract-brightest-localization.py` | 解析游戏主包 `SlayTheSpire2.pck`（Godot pck v3），抽取 16 语言 `localization/<lang>/cards.json` 中含 `BRIGHTEST` 的键，输出 `languages/raw/<lang>/cards.json` 与 `languages/原文汇总.md` | 初版 | 3,342 B | 本项目自写 | Python 3 标准库（`struct`/`json`/`os`/`io`）；需本机游戏 pck 路径 |
| 2026-08-16 | `scripts/list-paths.py` | 枚举 pck 路径表中含 `local`/`loc_` 的条目，用于定位本地化文件位置（可选输出到文件） | 初版 | 991 B | 本项目自写 | Python 3 标准库（`struct`/`io`/`sys`）；需本机游戏 pck 路径 |

两个脚本内的 pck 路径均为本机绝对路径
（`C:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck`），换机需修改。

构建脚本 `9.SRC/RevertBrightestFlame/build.ps1` 属于源码工程的一部分，故留在 `9.SRC/` 内，不在此登记。
