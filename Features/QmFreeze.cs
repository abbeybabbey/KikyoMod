using ABI_RC.Core.InteractionSystem;
using ABI_RC.Systems.MovementSystem;

namespace KikyoMod.Features;

internal class QmFreeze : FeatureComponent
{
    /*
     * original project source: https://github.com/xKiraiChan/CVRPlugins/blob/master/MenuFreeze/Plugin.cs
     */

    public override string FeatureName => GetType().Name;
    public override string OriginalAuthor => "xKiraiChan";

    public QmFreeze()
    {
        var types = new[] { typeof(bool) };

        Harmony.Patch(typeof(ViewManager).GetMethod(nameof(ViewManager.UiStateToggle), types), postfix: GetLocalPatch(nameof(UiTogglePatch)));
        Harmony.Patch(typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu), types), postfix: GetLocalPatch(nameof(UiTogglePatch)));
    }

    private static void UiTogglePatch(bool __0) => MovementSystem.Instance.controller.enabled = !__0;
}