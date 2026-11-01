using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using RevertBrightestFlame.Patches;

namespace RevertBrightestFlame;

[ModInitializer("ModLoaded")]
public static class ModEntry
{
    public const string ModId = "RevertBrightestFlame";

    private static Harmony? _harmony;

    public static void ModLoaded()
    {
        try
        {
            Log.Info($"{ModId}: loading...");

            if (!BrightestFlameShapeGuard.ValidateStatic())
            {
                Log.Warn($"{ModId}: Brightest Flame not found or required members missing in this game version; patches are disabled.");
                return;
            }

            _harmony = new Harmony(ModId);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Info($"{ModId}: Harmony patches applied (Brightest Flame fully reverted to pre-0.111.0 behavior)");
        }
        catch (Exception e)
        {
            Log.Error($"{ModId}: failed to apply patches: {e}");
        }
    }
}
