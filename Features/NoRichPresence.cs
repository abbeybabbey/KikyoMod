using ABI_RC.Helpers;
using Steamworks;

namespace KikyoMod.Features;

internal class NoRichPresence : FeatureComponent
{
    public override string FeatureName => GetType().Name;
    public override string OriginalAuthor => "abbey";

    public NoRichPresence()
    {
        Harmony.Patch(typeof(SteamFriends).GetMethod(nameof(SteamFriends.SetRichPresence)),
            GetLocalPatch(nameof(SetRichPresencePatch)));

        Harmony.Patch(typeof(PresenceManager).GetMethod(nameof(PresenceManager.UpdatePresence)),
            GetLocalPatch(nameof(SetRichPresencePatch)));
    }

    private static bool SetRichPresencePatch() => false;
}