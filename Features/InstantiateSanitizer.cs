using UnityEngine;
using Object = UnityEngine.Object;

namespace KikyoMod.Features;

internal class InstantiateSanitizer : FeatureComponent
{
    public InstantiateSanitizer()
    {
        Harmony.Patch(
            typeof(Object).GetMethod(nameof(Object.Instantiate),
                new[] { typeof(Object), typeof(Vector3), typeof(Quaternion) }),
            GetLocalPatch(nameof(InstantiatePatch)));
    }

    public override string FeatureName => GetType().Name;
    public override string OriginalAuthor => "abbey";

    private static bool InstantiatePatch(Object original, Vector3 position, Quaternion rotation)
    {
        if (!position.IsSafe())
        {
            KikyoLogger.Error(
                $"A object {original.name} tried to be instantiated with an invalid position: {position}");
            return false;
        }

        if (!rotation.IsSafe())
        {
            KikyoLogger.Error(
                $"A object {original.name} tried to be instantiated with an invalid rotation: {rotation}");
            return false;
        }

        return true;
    }
}