# ADR-003：OnPlay 读取 DynamicVars，修复升级收益被吞（v1.1.1）

- 日期：2026-09-07
- 状态：已实施并已重新构建部署（v1.1.1），待实机验证
- 关联计划：`2.PLANS/001-G-回调至亮之焰施工计划.md` §八
- 替代：ADR-002 §决策 1 中「OnPlay → Prefix 整体替换 `GainEnergy(2m)` + `Draw(2m)`」的做法

## 背景

v1.1（ADR-002「彻底写死」）实机表现异常：打出**升级过的**至亮之焰仍只获得
2 能量、抽 2 张牌；预期为 3 能量、3 抽。对照解包，107/110/111 原版
`BrightestFlame.OnPlay` 一律读取 `DynamicVars.Energy.IntValue` /
`DynamicVars.Cards.BaseValue`，升级由 `OnUpgrade` 将 DynamicVar 2→3。
v1.1 把 OnPlay 数值写成字面量 `2m`，与 DynamicVar 升级链路脱钩，导致升级失效。

## 决策

1. **执行层读变量、数值层写死**：`BrightestFlameOnPlayPatch.HardCodedOnPlay`
   改为从 `instance.DynamicVars` 取能量与抽牌数（`Energy.IntValue` /
   `Cards.BaseValue`，与原版表达式一致）；最大生命损失仍固定 `1m`。
   - 基础 2/2/1 的「写死」职责由 `CanonicalVars` 补丁承担（ADR-002 保留）；
   - 升级 +1/+1 的「写死」职责由 `OnUpgrade` 补丁 + DynamicVar
     `UpgradeValueBy` 机制承担（ADR-002 保留）；
   - OnPlay 只负责按 DynamicVars 执行，不再冻结升级状态。
2. **版本号**：manifest `version` 1.1.0 → 1.1.1，重新 build 并部署
   `C:\Game\Steam\steamapps\common\Slay the Spire 2\mods\RevertBrightestFlame`。
3. 其余决策（元数据补丁、16 语言描述、ShapeGuard、min_game_version）不变。

## 备选方案与放弃原因

- OnPlay 改为「读 DynamicVars + 按 CurrentUpgradeLevel 手工 +1」：
  重复实现引擎升级语义，若连续升级/降级/回档易漂移，放弃。
- 删除 OnUpgrade 补丁、放行原版：111 原版升级逻辑即为 +1/+1，功能等价；
  但 ADR-002 的「游戏更新改升级数值也不影响」目标会丢失，保留补丁。

## 影响

- 升级后的卡：3 能量 / 3 抽 / 损失 1 最大生命；未升级：2 能量 / 2 抽 / 损失 1。
- 卡面（DynamicVar 占位符）与实测重新一致（升级前 2/2 → 升级后 3/3）。
- 若未来版本 OnPlay 结构变化但 DynamicVars 语义保留，本补丁仍可工作；
  若 DynamicVars/EnergyVar/CardsVar API 变化，ShapeGuard 需补充对应检查。

## 回归验证要点

1. 未升级：+2 能量、抽 2、-1 最大生命；
2. 升级后：+3 能量、抽 3、-1 最大生命；
3. 升级预览（营地/图鉴/事件）显示 3/3 与实际打出一致；
4. 存档中已升级的旧卡在 mod 更新后按 3/3 生效（升级状态存于 DynamicVars 克隆，不依赖补丁常量）。
