using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertBrightestFlame.Patches;

/// <summary>
/// 打出行为接管：获得能量/抽牌数量读取实例 DynamicVars（升级后自动为 3/3，
/// 与升级机制同源，卡面与实测一致）；最大生命损失固定为 1（本 mod 唯一改动数值）。
/// 基础值（2/2/1）由 CanonicalVars 补丁写死，升级增量（+1/+1）由 DynamicVar
/// UpgradeValueBy 引擎机制记录；这里只做执行，不再把升级状态一并写死。
/// </summary>
[HarmonyPatch(typeof(BrightestFlame), "OnPlay")]
public static class BrightestFlameOnPlayPatch
{
    private static bool Prefix(PlayerChoiceContext choiceContext, BrightestFlame __instance, ref Task __result)
    {
        if (!ModConfig.IsReplaceEnabled || !CreatureCmdCompat.IsSupported)
        {
            return true;
        }

        __result = HardCodedOnPlay(choiceContext, __instance);
        return false;
    }

    private static async Task HardCodedOnPlay(PlayerChoiceContext choiceContext, BrightestFlame instance)
    {
        await PlayerCmd.GainEnergy(instance.DynamicVars.Energy.IntValue, instance.Owner);
        await CardPileCmd.Draw(choiceContext, instance.DynamicVars.Cards.BaseValue, instance.Owner);
        await CreatureCmdCompat.LoseMaxHp(choiceContext, instance.Owner.Creature, 1m, instance);
    }
}
