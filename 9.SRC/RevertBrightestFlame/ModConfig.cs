using RevertBrightestFlame.Patches;

namespace RevertBrightestFlame;

/// <summary>
/// 替换功能总开关：仅当守卫确认游戏里存在「至亮之焰」且补丁目标成员齐全时启用。
/// 版本门槛由 manifest 的 min_game_version=0.111.0 承担（低于该版本加载器拒绝加载）。
/// </summary>
public static class ModConfig
{
    public static bool IsReplaceEnabled => BrightestFlameShapeGuard.IsSupported();
}
