using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertBrightestFlame.Patches;

/// <summary>
/// 升级行为写死：整体替换 BrightestFlame.OnUpgrade，
/// 固定为 能量 +1、抽牌 +1，最大生命损失量不变（仍为 1）。
/// 升级数值经 DynamicVar.UpgradeValueBy 引擎通用机制记录，
/// 升级预览/图鉴渲染自动正确；不读取原版升级逻辑。
/// </summary>
[HarmonyPatch(typeof(BrightestFlame), "OnUpgrade")]
public static class BrightestFlameOnUpgradePatch
{
    private static bool Prefix(BrightestFlame __instance)
    {
        if (!ModConfig.IsReplaceEnabled)
        {
            return true;
        }

        __instance.DynamicVars.Energy.UpgradeValueBy(1m);
        __instance.DynamicVars.Cards.UpgradeValueBy(1m);
        return false;
    }
}
