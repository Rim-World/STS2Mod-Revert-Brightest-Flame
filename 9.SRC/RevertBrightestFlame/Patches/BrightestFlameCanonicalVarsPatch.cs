using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertBrightestFlame.Patches;

/// <summary>
/// 数值写死：至亮之焰的 CanonicalVars 整体替换为固定数组
/// （损失最大生命 1 / 获得能量 2 / 抽牌 2，即 0.111.0 之前数值）。
/// 不读取原版任何数值：游戏更新改动卡牌数值/增删变量均不影响。
/// 卡面文字经 {Energy}/{Cards}/{MaxHp} 占位符由本数组渲染，自动显示写死数值。
/// </summary>
[HarmonyPatch(typeof(BrightestFlame), "CanonicalVars", MethodType.Getter)]
public static class BrightestFlameCanonicalVarsPatch
{
    private static void Postfix(BrightestFlame __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = new DynamicVar[]
            {
                new MaxHpVar(1m),
                new EnergyVar(2),
                new CardsVar(2)
            };
        }
    }
}
