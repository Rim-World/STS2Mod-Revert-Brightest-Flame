using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertBrightestFlame.Patches;

/// <summary>
/// 打出行为写死：整体替换 BrightestFlame.OnPlay，
/// 固定为 获得 2 点能量 → 抽 2 张牌 → 损失 1 点最大生命值（0.111.0 之前行为）。
/// 不读取原版 OnPlay 的任何实现细节，游戏更新修改原逻辑不影响。
/// </summary>
[HarmonyPatch(typeof(BrightestFlame), "OnPlay")]
public static class BrightestFlameOnPlayPatch
{
    private static bool Prefix(PlayerChoiceContext choiceContext, BrightestFlame __instance, ref Task __result)
    {
        if (!ModConfig.IsReplaceEnabled)
        {
            return true;
        }

        __result = HardCodedOnPlay(choiceContext, __instance);
        return false;
    }

    private static async Task HardCodedOnPlay(PlayerChoiceContext choiceContext, BrightestFlame instance)
    {
        await PlayerCmd.GainEnergy(2m, instance.Owner);
        await CardPileCmd.Draw(choiceContext, 2m, instance.Owner);
        await CreatureCmd.LoseMaxHp(choiceContext, instance.Owner.Creature, 1m, isFromCard: true);
    }
}
