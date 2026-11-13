# 三版本兼容修复：`CreatureCmd.LoseMaxHp` 四参签名

- 日期：2026-11-13
- 状态：已完成（构建、部署、0.113 启动验证；实机效果待用户验收）
- 目标版本：v1.1.2
- 关联：`3.DOCS/003-R-v0.113失效评估.md`

## 背景

正式版当前为 0.107.x，官方计划把正式版内容切换到原 beta 0.111.x，beta 线已到 0.113.x。manifest `min_game_version = 0.111.0`，0.107 由加载器拒绝；实际兼容范围为 0.111 与 0.113（0.107 运行日志存在，但不在加载范围）。

本 mod 的目标是把 0.111 起「损失 2 最大生命」改回 1，并保持 2 能量 / 2 抽（升级 3/3）。

## 版本事实

| 版本 | `LoseMaxHp` 第 4 参 | `BrightestFlame.CanonicalVars` | 证据 |
|---|---|---|---|
| 0.107 | `bool isFromCard` | `MaxHpVar(1m)` | `1.STS2-107Reference/.../CreatureCmd.cs:811`；`BrightestFlame.cs:15` |
| 0.111 | `bool isFromCard` | `MaxHpVar(2m)` | `2.STS2-111Reference/.../CreatureCmd.cs:866`；`BrightestFlame.cs:15` |
| 0.113 | `CardModel? cardSource` | `MaxHpVar(2m)` | `3.STS2-113Reference/.../CreatureCmd.cs:942`；`BrightestFlame.cs:15` |

三版本 `OnPlay` 只差第 4 参：107 为 `isFromCard: true`，111 为 `isFromCard: true`，113 为 `this`；`OnUpgrade` 三版本逐字相同。

## 语义核对（2026-11-13 修订）

- 107 / 111：`isFromCard` 只决定伤害是否带 `ValueProp.Move`；内部 `Damage(..., dealer: null, cardSource: null)`。
- 113：`cardSource != null` 决定 `Move`，并把 `cardSource` 传给伤害结算；113 的 6 参单体重载会把 `dealer` 推导为 `cardSource?.Owner.Creature`。
- 因此 `isFromCard: true` 与 `cardSource: 卡牌实例` **不是完全等价**：
  - 传实例：113 原版行为。伤害带 `Move`、`dealer` 为玩家、`cardSource` 为卡；`BufferPower` 对 `cardSource is BrightestFlame` 有特判，不消耗 Buffer；`Rupture` 延迟到 `AfterCardPlayed` 结算。
  - 传 `null`：无 `Move`，`dealer` 与 `cardSource` 均为 `null`，Buffer 会照常消耗、`Rupture` 立即结算；更接近 107/111，但丢失卡牌归因。
- `Rupture` 在两种情况下都会触发，差别只是结算时机；此前「传 null 会让 Rupture 不触发」的说法不成立。
- 本 mod 采用**传卡牌实例**：与 0.113 原版一致，也是 Buffer 特判依赖的形态（传 `null` 会让 113 的至亮之焰改为消耗 Buffer，偏离官方行为）。

## 问题

`Patches/BrightestFlameOnPlayPatch.cs:34` 直接调用：

```csharp
await CreatureCmd.LoseMaxHp(choiceContext, instance.Owner.Creature, 1m, isFromCard: true);
```

- 0.113 上该成员不存在，首次执行 `HardCodedOnPlay` 时 JIT 抛 `MissingMethodException`，出牌无任何效果。
- 以 0.113 的 `sts2.dll` 为编译基线时该行无法编译（`isFromCard` 不存在）。

## 方案 A（保留 ADR-002 的执行层冻结，推荐）

1. 新增 `Patches/CreatureCmdCompat.cs`：
   - 用精确签名数组解析两个重载：
     - `("LoseMaxHp", [PlayerChoiceContext, Creature, decimal, bool])`
     - `("LoseMaxHp", [PlayerChoiceContext, Creature, decimal, CardModel])`
   - 解析过程全部包在 `try/catch` 中，失败返回 `null`；不得在静态字段初始化里让它抛出 `TypeInitializationException`（否则该类型此后永久不可用）。
   - 暴露 `IsSupported` 与 `LoseMaxHp(ctx, creature, amount, card)`：优先 bool 重载，其次 `CardModel` 重载。
2. `BrightestFlameOnPlayPatch.Prefix` 增加守卫：`if (!ModConfig.IsReplaceEnabled || !CreatureCmdCompat.IsSupported) return true;`
   - 不接管时，原版 `OnPlay` 读取被 `CanonicalVars` 补丁钉成 1 的 `DynamicVars.MaxHp.BaseValue`，卡牌仍会失去 1 最大生命，不会半失效。
3. `HardCodedOnPlay` 末行改为 `await CreatureCmdCompat.LoseMaxHp(choiceContext, instance.Owner.Creature, 1m, instance);`
4. 保留 `using MegaCrit.Sts2.Core.Commands;`：`PlayerCmd.GainEnergy`、`CardPileCmd.Draw` 仍在该命名空间。
5. 在 `BrightestFlameShapeGuard.ValidateStatic()` 中增加「存在 4 参 `LoseMaxHp`，且第 4 参为 `bool` 或 `CardModel`」检查；缺失时整组补丁不启用，由原版行为兜底。

## 备选方案 B（更小，需先改 ADR）

删除 `BrightestFlameOnPlayPatch` 与 `BrightestFlameOnUpgradePatch`，其余不动：三版本原版 `OnPlay` 都读 `DynamicVars`，`CanonicalVars` 补丁已把三项钉成 1/2/2，0.113 天然传 `this`。

- 优点：零反射、零版本分支、零新增代码。
- 代价：放弃 ADR-002 要求的「执行层冻结」；未来官方若把数值硬编码进 `OnPlay`，本 mod 会跟随。需要新增 ADR 并经用户批准，不能静默采用。

## 改动清单（方案 A）

| 文件 | 改动 |
|---|---|
| `9.SRC/RevertBrightestFlame/Patches/CreatureCmdCompat.cs` | 新增：精确解析 + 分派 |
| `9.SRC/RevertBrightestFlame/Patches/BrightestFlameOnPlayPatch.cs` | 增加 `IsSupported` 守卫；末行改为兼容层调用 |
| `9.SRC/RevertBrightestFlame/Patches/BrightestFlameShapeGuard.cs` | 增加 `LoseMaxHp` 形状检查 |
| `9.SRC/RevertBrightestFlame/RevertBrightestFlame.json` | 版本号 1.1.1 → 1.1.2；`min_game_version` 维持 `0.111.0` |
| `5.ChangeLog/`、`CHANGELOG.md` | 记录 v1.1.2 |
| `3.DOCS/003-R-v0.113失效评估.md` | 追加修复记录 |

## 不做

- 不改卡面数值、文案与本地化。
- 不新增对 0.113 独有成员的直接引用。
- 不修改 `ModConfig` 的开关语义。
- 不采用方案 B，除非用户明确批准并新增 ADR。

## 验收

**前置：确认实际加载的是新构建。** 本地 `mods/RevertBrightestFlame` 在 0.113 设置里处于禁用状态，实际加载的是创意工坊 `3783912778` 的旧 1.1.1。验收前必须二选一：

1. 启用本地副本并禁用工坊副本；或
2. 把新 DLL/pck 覆盖到工坊目录。

启动后检查日志中 `Loading assembly DLL ...RevertBrightestFlame.dll` 的路径与文件时间，确认命中新构建。

| 版本 | 检查 | 通过标准 |
|---|---|---|
| 0.113 | 打出未升级 / 已升级的至亮之焰 | 失去 1 最大生命；获得 2 / 3 能量、抽 2 / 3 张；日志无 `MissingMethodException` |
| 0.111 | 同上 | 行为与 v1.1.1 一致 |
| 0.107 | 启动 | 加载器按 `min_game_version = 0.111.0` 拒绝加载，日志有明确拒绝记录 |

补充组合测试：持有 `Buffer`（缓冲）与 `Rupture`（破裂）时打出该卡，确认 Buffer 不被消耗、Rupture 延迟结算且总量正确（验证传实例的语义）。

## 验证方法

1. 静态：三份反编译源码核对 `LoseMaxHp`、`BrightestFlame`、`BufferPower`、`RupturePower`。
2. 构建：以当前 0.113 `sts2.dll` 为基线，0 警告 0 错误。
3. 运行：0.113 实机；本机今天 06:51 存在 0.107.1 运行日志，107 可切换分支验证；0.111 无本地记录，标记未验证。

## 风险与回退

- 探测只覆盖 4 参签名；未来若改成 5 参，`IsSupported` 为假 → 不接管，原版仍按钉死的数值执行。
- `CardModel` 参数若被换成接口/基类，精确数组返回 `null` → 同上兜底。
- 工坊 `CustomCardBalance` 也有 `BrightestFlame.CanonicalVars` Postfix，写入相同 1/2/2；两者同时启用时结果相同，但顺序由加载决定，未来数值分叉会互相覆盖。
- 已装 `Loadout` 在 0.113 也会因同样的 `LoseMaxHp(bool)` 问题抛 `MissingMethodException`，排查日志时注意区分。
- 回退为 v1.1.1 源码；0.111 可用，0.113 不可用。

## 已确认

1. `min_game_version` 不降到 0.107.0（2026-11-13 用户决定）。0.107 上由加载器拒绝加载。
2. 不在创意工坊简介补充 0.107 说明（2026-11-13 用户决定）。

## 施工记录回填

| 时间 | 实际施工内容 | 验收状态 |
|---|---|---|
| 2026-11-13 09:17 | 新增 `CreatureCmdCompat`；`BrightestFlameOnPlayPatch` 增加 `IsSupported` 守卫并改用兼容层；`BrightestFlameShapeGuard` 加入 `LoseMaxHp` 检查；manifest 1.1.1 → 1.1.2；构建覆盖安装到 `mods/RevertBrightestFlame` | 构建通过；0.113 启动日志显示补丁应用，无异常；实机出牌待用户验收 |
