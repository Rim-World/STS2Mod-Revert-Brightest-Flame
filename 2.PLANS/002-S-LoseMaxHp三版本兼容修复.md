# 三版本兼容修复：`CreatureCmd.LoseMaxHp` 四参签名

- 日期：2026-11-13
- 状态：待实施
- 目标版本：v1.1.2
- 关联：`3.DOCS/003-R-v0.113失效评估.md`

## 背景

正式版当前为 0.107.x。官方计划把正式版内容切换到原 beta 0.111.x，beta 线已到 0.113.x。本 mod 需要同一份 DLL 在 0.107 / 0.111 / 0.113 三个版本上都能加载并保持「至亮之焰：失去 1 最大生命、获得 2 能量、抽 2 张」的效果（升级后 3/3）。

## 版本事实

| 游戏版本 | `CreatureCmd.LoseMaxHp` 第 4 参 | 证据 |
|---|---|---|
| 0.107 | `bool isFromCard` | `STS2-Reference/1.STS2-107Reference/decompiled/MegaCrit.Sts2.Core.Commands/CreatureCmd.cs:811` |
| 0.111 | `bool isFromCard` | `STS2-Reference/2.STS2-111Reference/decompiled/MegaCrit.Sts2.Core.Commands/CreatureCmd.cs:866` |
| 0.113 | `CardModel? cardSource` | `STS2-Reference/3.STS2-113Reference/decompiled/MegaCrit.Sts2.Core.Commands/CreatureCmd.cs:942` |

前三个参数（`PlayerChoiceContext`、`Creature`、`decimal`）三版本一致。

## 问题

`9.SRC/RevertBrightestFlame/Patches/BrightestFlameOnPlayPatch.cs:34` 直接调用：

```csharp
await CreatureCmd.LoseMaxHp(choiceContext, instance.Owner.Creature, 1m, isFromCard: true);
```

- 在 0.113 上，方法签名不匹配，首次执行 `HardCodedOnPlay` 时抛 `MissingMethodException`，出牌无任何效果。
- 以 0.113 的 `sts2.dll` 为编译基线时，这一行本身无法编译（`isFromCard` 不存在，CS1739）。因此当前源码也无法针对 113 重新构建。

## 方案

用运行时签名探测替代编译期绑定，一个 DLL 覆盖三个版本。

1. 新增 `9.SRC/RevertBrightestFlame/Patches/CreatureCmdCompat.cs`。
2. 静态初始化时在 `CreatureCmd` 上枚举名为 `LoseMaxHp`、参数数量为 4、第 3 参为 `decimal` 的方法，按第 4 参类型判定分支：
   - `bool` → 旧签名，实参传 `true`；
   - 可赋值为 `CardModel` 的类型 → 新签名，实参传卡牌实例 `instance`（作为 `cardSource`，保持「由卡牌造成」的归因）；
   - 都不匹配 → 记录 `Log.Warn`，跳过最大生命损失，其余效果照常执行。
3. 分派方法 `LoseMaxHp(PlayerChoiceContext, Creature, decimal, CardModel card)` 内部用缓存好的 `MethodInfo.Invoke` 调用，`await` 返回的 `Task`；捕获 `TargetInvocationException` 并抛出内层异常，避免掩盖真实错误。
4. `BrightestFlameOnPlayPatch.HardCodedOnPlay` 改为调用兼容层，删除对游戏方法的直接引用。
5. 其余补丁（`CanonicalVars`、元数据、`OnUpgrade`）不动：它们的目标成员在三个版本都存在，且签名未变。

## 改动清单

| 文件 | 改动 |
|---|---|
| `9.SRC/RevertBrightestFlame/Patches/CreatureCmdCompat.cs` | 新增：签名探测 + 分派 |
| `9.SRC/RevertBrightestFlame/Patches/BrightestFlameOnPlayPatch.cs` | 第 34 行改为调用兼容层；删除 `using` 中不再需要的命名空间 |
| `9.SRC/RevertBrightestFlame/RevertBrightestFlame.json` | 版本号 1.1.1 → 1.1.2；`min_game_version` 是否降到 0.107.0 见「待确认」 |
| `5.ChangeLog/`、`CHANGELOG.md` | 记录 v1.1.2 |
| `3.DOCS/` | 追加修复记录（可并入 003 报告末尾） |

## 不做

- 不改动卡面数值与本地化文本。
- 不新增对 0.113 独有成员的直接引用；后续新增代码同样受此约束。
- 不修改 `ModConfig` 的开关语义。

## 验收

| 版本 | 检查 | 通过标准 |
|---|---|---|
| 0.113 | 加载日志；打出至亮之焰（普通与升级） | 日志无 `MissingMethodException`；失去 1 最大生命，获得 2/3 能量、抽 2/3 张 |
| 0.111 | 同上 | 行为与 v1.1.1 一致 |
| 0.107 | 加载日志；打出该卡 | 若 `min_game_version` 降为 0.107.0：正常加载、无异常、数值与 107 原版一致；否则加载器应明确拒绝加载 |

## 验证方法

1. 静态：对照 `1.STS2-107Reference`、`2.STS2-111Reference`、`3.STS2-113Reference` 三份反编译源码，逐一核对 mod 直接引用的每个游戏成员是否在三个版本都存在。
2. 构建：以当前安装的 0.113 `sts2.dll` 为基线构建，0 警告 0 错误。
3. 运行：当前机器只装了 0.113，先做 113 实机验证；107/111 无本地运行条件，标记未验证，由用户在对应版本上回归。

## 风险与回退

- 反射分派的实参类型必须与目标参数类型精确兼容；`CardModel` 是引用类型，传 `null` 也可行，但本 mod 传卡牌实例。
- 若 0.113 的运行表现与预期不符，回退方式为还原 v1.1.1 源码；该版本在 107/111 仍可用。

## 待确认

1. `min_game_version` 是否从 `0.111.0` 降到 `0.107.0`。降到 0.107.0 后，0.107 上 mod 会加载但效果与 107 原版相同（等同空操作）；不降则 107 上直接不加载。
2. 是否需要同步更新创意工坊简介中的版本说明。

## 施工记录回填

| 时间 | 实际施工内容 | 验收状态 |
|---|---|---|
| — | — | — |
