# 更新日志

本文件记录本项目所有重要变更。
格式参考 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/)，版本号遵循语义化版本。
详细的分版本施工记录（含开发人员反馈、失败教训与提交编号）见 `5.ChangeLog/`。

## [1.1.2] - 2026-11-13

### 修复

- 0.113 上打出至亮之焰无效果：`CreatureCmd.LoseMaxHp` 第 4 参由 `bool` 改为 `CardModel?`，原补丁绑定旧签名，首次打出时抛 `MissingMethodException`。
- 新增运行时重载探测与分派；不支持时交还原版 `OnPlay`，不产生半失效。
- 清单版本 `1.1.1` → `1.1.2`。

## [1.1.1] - 2026-09-07

### 修复

- 修复升级过的至亮之焰打出后仍只有 2 能量、抽 2 张牌的问题：`OnPlay` 改为读取实例 `DynamicVars`
  （`Energy.IntValue` / `Cards.BaseValue`），升级后恢复为 3 能量 / 3 抽；最大生命损失仍固定为 1。
- 清单版本号 1.1.0 → 1.1.1。

### 已知问题

- `BrightestFlameShapeGuard.ValidateStatic()` 未检查 `BrightestFlame.CanonicalVars` 与
  `CardEnergyCost.Canonical` 两个 getter；当前版本无影响，未来游戏删改这两个成员时存在
  「部分补丁已应用」的隐患。

## [1.1] - 2026-08-16

### 新增

- 13 个 Harmony 补丁：`CanonicalVars`、`OnPlay`、`OnUpgrade`，以及费用、`CardEnergyCost.Canonical`、
  类型、目标、稀有度、关键词、金色高亮、悬停提示、卡名、描述共 10 个元数据/文本补丁。
- 16 语言本地化 `RE_BRIGHTEST_FLAME.title` / `.description`，文本取自 0.111.0 游戏包解包原文。
- `BrightestFlameShapeGuard`：加载时检查类型与补丁目标成员，运行时确认 `ModelDb` 中存在该卡。

### 变更

- 由「只改一个数值」改为「整卡写死」，游戏更新给这张卡增删属性不再影响本 mod。
- pck 从仅含占位图标（219,776 字节）变为图标 + 16 语言文案（225,072 字节）。

### 已知问题

- `OnPlay` 使用字面量 2m/2m，升级收益被吞（升级后仍 2 能量 / 2 抽）——1.1.1 修复。

## [1.0] - 2026-08-15

### 新增

- 首个版本：Harmony Postfix 补丁把 `BrightestFlame.CanonicalVars` 固定为
  `[MaxHpVar(1m), EnergyVar(2), CardsVar(2)]`，最大生命损失回到 1。
- mod 清单 `RevertBrightestFlame.json`（`min_game_version = "0.111.0"`）、Godot 工程、构建脚本与
  420×420 占位图标。

## 未发布

### 变更

- 仓库整理：源码移入 `9.SRC/`，文档按 AGENTS.md 规范编号并补全 `4.SDD`、`5.ChangeLog`、
  `10.OUTPUT`、`PRIVACY`、`TMP`、`TOOLS` 骨架；同步 v1.1.1 源码到远端仓库。未改动任何代码。
  （提交 `f1247f7`）
