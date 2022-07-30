using System;
using ABI_RC.Core.Util;
using DarkRift;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KikyoMod.Features;

internal class InstantiateSanitizer : FeatureComponent
{
    public InstantiateSanitizer()
    {
        try
        {
            Harmony.Patch(
                typeof(Object).GetMethod(nameof(Object.Instantiate),
                    new[] { typeof(Object), typeof(Vector3), typeof(Quaternion) }),
                GetLocalPatch(nameof(InstantiatePatch)));
        }
        catch (Exception e)
        {
            KikyoLogger.Error("An exception occurred while trying to patch a Instantiate:\n", e);
        }

        try
        {
            Harmony.Patch(typeof(CVRSyncHelper).GetMethod(nameof(CVRSyncHelper.SpawnPortalFromNetwork)),
                GetLocalPatch(nameof(SpawnPortalFromNetworkPatch)));
        }
        catch (Exception e)
        {
            KikyoLogger.Error("An exception occurred while trying to patch a SpawnPortalFromNetwork:\n", e);
        }
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

    private static void SpawnPortalFromNetworkPatch(Message message)
    {
        using var reader = message.GetReader();
        var portalOwner = reader.ReadString();
        KikyoLogger.Msg($"Received a spawn portal network request, PortalOwner: {portalOwner}");
    }
}