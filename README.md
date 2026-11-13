# STS2Mod-回调至亮之焰（RevertBrightestFlame）

> 把《杀戮尖塔 2》古代卡牌「至亮之焰」（Brightest Flame）每次使用损失的最大生命值从 2 点回调为 1 点，
> 并把这张卡的数值、元数据与 16 语言文案整体写死为 0.111.0 之前的表现。
> Reverts the "Brightest Flame" card in Slay the Spire 2: the Max HP lost per play goes back from 2 to 1 (the pre-0.111 value), with the card's values, metadata and 16-language text hard-coded.

- 当前版本：**1.1.2**
- mod id：`RevertBrightestFlame`；作者：再玩小卡组找人弄你
- 游戏要求：Slay the Spire 2 **0.111.0 及以上**（清单 `min_game_version = "0.111.0"`）
- 仓库：<https://github.com/Rim-World/STS2Mod-Revert-Brightest-Flame>
- 许可：MIT（见 [LICENSE](LICENSE)）

## 功能

- **数值回调**：每次打出至亮之焰损失的最大生命值 2 → **1**（0.099～0.110 的原数值）。
- **其余效果不变**：0 费，获得 2 点能量、抽 2 张牌；升级后能量与抽牌各 +1，损失量仍为 1。
- **整卡写死**：费用、类型（技能）、目标（自身）、稀有度（古代）、关键词（空）、
  悬停提示（仅能量）、卡名与描述全部由本 mod 固定，游戏更新改动这张卡也不影响。
- **16 语言文案**：文本取自 0.111.0 游戏包 `localization/<lang>/cards.json` 原文，
  键前缀 `RE_BRIGHTEST_FLAME.*`，数值由写死的动态变量渲染。
- **存档生效**：直接修改原卡模型，不新增卡牌、不禁用旧卡；开启 mod 时历史记录中的
  至亮之焰按回调后的数值显示，关闭后恢复官方值。
- **不自伤**：游戏版本低于 0.111.0 时加载器拒绝加载；游戏里找不到这张卡或补丁目标成员缺失时，
  守卫会禁用全部补丁并只记一条警告，游戏保持原版行为。

## 兼容性

- 不依赖任何基础 mod（无 RitsuLib、无第三方库），只用到游戏自带的 Harmony。
- 不写死卡面美术，卡图跟随游戏 pck 素材。
- 不保证与盗版、修改版、移动版、老旧版本或冷门次级 mod 的兼容性。
- 不检测游戏分支：正式版（Default）版本号将来达到 111 时同样自动生效。

## 安装

1. 构建产物为 `RevertBrightestFlame.dll` + `RevertBrightestFlame.pck` + `RevertBrightestFlame.json` 三件套
   （`RevertBrightestFlame.pdb` 为可选调试符号）。
2. 解压到 `<游戏目录>/mods/RevertBrightestFlame/`。
3. 启动游戏，日志出现 `RevertBrightestFlame: Harmony patches applied` 即生效；
   mod 列表应显示 1.1.2。

## 构建

```powershell
cd 9.SRC\RevertBrightestFlame
pwsh .\build.ps1
```

`build.ps1` 依次执行：`dotnet build -c Debug` → Godot `--headless --import` →
`--headless --export-pack "Windows Desktop"` → 把 dll / pdb / json 拷贝到游戏 `mods\RevertBrightestFlame\`。

依赖：.NET SDK 9、Godot 4.5.1（mono 版），以及本机游戏目录里的 `sts2.dll`、`0Harmony.dll`；
路径写在 `*.csproj` 与 `build.ps1` 顶部，换机器需要改这三处。

## 目录结构

```
1.ADR/          决策记录（ADR.md 索引）
2.PLANS/        施工计划（PLANS.md 索引）
3.DOCS/         知识与报告（DOCS.md 索引）
4.SDD/          软件设计（SDD.md 索引）
5.ChangeLog/    版本日志分片（ChangeLog.md 索引）
9.SRC/          源码（Godot + C# 工程）
10.OUTPUT/      构建产物与发布包（output.md 索引，不入库）
PRIVACY/        机密信息索引（不入库）
TMP/            临时与过程文件（TMP.md 索引，不入库）
TOOLS/          自写脚本（TOOLS.md 索引）
description/    创意工坊文案（中/英）
languages/      游戏包解包得到的 16 语言原文
pics/           封面与截图素材
```

## 文档索引

- 决策：[1.ADR/001-至亮之焰数值回调实现.md](1.ADR/001-至亮之焰数值回调实现.md)、
  [002-彻底写死重构.md](1.ADR/002-彻底写死重构.md)、
  [003-OnPlay改读动态变量.md](1.ADR/003-OnPlay改读动态变量.md)
- 计划：[2.PLANS/001-G-回调至亮之焰施工计划.md](2.PLANS/001-G-回调至亮之焰施工计划.md)
- 知识：[3.DOCS/002-T-三版本取证与实施笔记.md](3.DOCS/002-T-三版本取证与实施笔记.md)
- 报告：[3.DOCS/001-R-升级收益被吞审阅报告.md](3.DOCS/001-R-升级收益被吞审阅报告.md)
- 设计：[4.SDD/001-总体设计.md](4.SDD/001-总体设计.md)
- 版本日志：[5.ChangeLog/001-1.1.1-2026-09-07.md](5.ChangeLog/001-1.1.1-2026-09-07.md)、
  [5.ChangeLog/002-1.1.2-2026-11-13.md](5.ChangeLog/002-1.1.2-2026-11-13.md)、
  [CHANGELOG.md](CHANGELOG.md)
- 源码说明：[9.SRC/RevertBrightestFlame/README.md](9.SRC/RevertBrightestFlame/README.md)、
  [9.SRC/RevertBrightestFlame/README.zh.md](9.SRC/RevertBrightestFlame/README.zh.md)

## 版本

| 版本 | 日期 | 要点 |
|---|---|---|
| 1.0 | 2026-08-15 | 单补丁 `CanonicalVars` 把最大生命损失改回 1 |
| 1.1 | 2026-08-16 | 彻底写死：13 个补丁 + 16 语言文案 + 形状守卫 |
| 1.1.1 | 2026-09-07 | 修复「升级后仍只有 2 能量 / 2 抽」：`OnPlay` 改读 `DynamicVars` |
| 1.1.2 | 2026-11-13 | 0.113 兼容：`LoseMaxHp` 第 4 参改为运行时探测；不支持的版本交还原版 |

## 致谢

本 mod 使用 Deepseek Harness（WebUI）+ DeepSeek V4 Flash（0731 GA）完成，感谢梁圣。
感谢《杀戮尖塔 2》制作组为方便玩家制作 mod 所做的努力；感谢 Baselib 制作者提供的文档；
写法参考同系列 STS2 补丁 mod（`CanonicalVars` Postfix 先例）。

## 开源地址

<https://github.com/Rim-World/STS2Mod-Revert-Brightest-Flame>
