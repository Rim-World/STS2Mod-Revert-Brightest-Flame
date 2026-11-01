using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertBrightestFlame.Patches;

/// <summary>
/// 元数据写死：费用 0 / 类型 技能 / 目标 自身 / 稀有度 古代 / 无关键词 /
/// 无金色高亮 / 悬停提示仅能量 / 卡名与描述固定为本 mod 本地化键。
/// 游戏更新给这张卡添加/修改任何元数据均不影响。
/// </summary>

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class BrightestFlameEnergyCostPatch
{
    private static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = 0;
        }
    }
}

[HarmonyPatch(typeof(CardEnergyCost), "Canonical", MethodType.Getter)]
public static class BrightestFlameEnergyCostCanonicalPatch
{
    private static readonly FieldInfo? CardField = AccessTools.Field(typeof(CardEnergyCost), "_card");

    private static void Postfix(CardEnergyCost __instance, ref int __result)
    {
        if (CardField != null && CardField.GetValue(__instance) is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = 0;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
public static class BrightestFlameTypePatch
{
    private static void Postfix(CardModel __instance, ref CardType __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = CardType.Skill;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
public static class BrightestFlameTargetTypePatch
{
    private static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = TargetType.Self;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
public static class BrightestFlameRarityPatch
{
    private static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = CardRarity.Ancient;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalKeywords", MethodType.Getter)]
public static class BrightestFlameKeywordsPatch
{
    private static void Postfix(CardModel __instance, ref IEnumerable<CardKeyword> __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = Array.Empty<CardKeyword>();
        }
    }
}

[HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
public static class BrightestFlameGlowGoldPatch
{
    private static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = false;
        }
    }
}

[HarmonyPatch(typeof(BrightestFlame), "ExtraHoverTips", MethodType.Getter)]
public static class BrightestFlameHoverTipsPatch
{
    private static void Postfix(BrightestFlame __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = new IHoverTip[] { HoverTipFactory.ForEnergy(__instance) };
        }
    }
}

[HarmonyPatch(typeof(CardModel), "TitleLocString", MethodType.Getter)]
public static class BrightestFlameTitlePatch
{
    private static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = new LocString("cards", "RE_BRIGHTEST_FLAME.title");
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
public static class BrightestFlameDescriptionPatch
{
    private static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is BrightestFlame && ModConfig.IsReplaceEnabled)
        {
            __result = new LocString("cards", "RE_BRIGHTEST_FLAME.description");
        }
    }
}
