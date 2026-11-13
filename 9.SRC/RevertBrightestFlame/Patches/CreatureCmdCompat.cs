using System;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace RevertBrightestFlame.Patches;

/// <summary>
/// CreatureCmd.LoseMaxHp 的 4 参重载在第 4 参上有版本差异：
/// 0.107 / 0.111 为 bool isFromCard，0.113 为 CardModel? cardSource。
/// 用精确签名探测，避免直接引用任一版本的参数类型。
/// </summary>
internal static class CreatureCmdCompat
{
    private static readonly MethodInfo? BoolOverload = Resolve(typeof(bool));

    private static readonly MethodInfo? CardOverload = Resolve(typeof(CardModel));

    public static bool IsSupported => BoolOverload != null || CardOverload != null;

    private static MethodInfo? Resolve(Type fourthParameter)
    {
        try
        {
            return typeof(CreatureCmd).GetMethod("LoseMaxHp", new[]
            {
                typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), fourthParameter,
            });
        }
        catch
        {
            return null;
        }
    }

    public static async Task LoseMaxHp(
        PlayerChoiceContext choiceContext,
        Creature creature,
        decimal amount,
        CardModel card)
    {
        MethodInfo? method = BoolOverload ?? CardOverload;
        if (method == null)
        {
            throw new MissingMethodException(
                "CreatureCmd.LoseMaxHp(PlayerChoiceContext, Creature, decimal, bool|CardModel)");
        }

        object fourthArgument = method == BoolOverload ? true : card;
        var task = (Task)method.Invoke(null, new object?[]
        {
            choiceContext, creature, amount, fourthArgument,
        })!;
        await task;
    }
}
