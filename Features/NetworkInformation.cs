using System;
using ABI_RC.Core.Networking;
using DarkRift;

namespace KikyoMod.Features;

internal class NetworkInformation : FeatureComponent
{
    public override string FeatureName => GetType().Name;
    public override string OriginalAuthor => "abbey";

    public NetworkInformation()
    {
        try
        {
            Harmony.Patch(
                typeof(NetworkManager).GetMethod("OnInstanceWelcomeMessageReceived")?.MakeGenericMethod(typeof(void)),postfix: GetLocalPatch(nameof(OnInstanceWelcomeMessageReceivedPatch)));
        }
        catch (Exception e)
        {
            KikyoLogger.Error("An exception occurred while trying to patch a OnInstanceWelcomeMessageReceived:\n", e);
        }
    }

    private static void OnInstanceWelcomeMessageReceivedPatch(Message message)
    {
        using var reader = message.GetReader();
        KikyoLogger.Msg($"Connected to Instance: {reader.ReadString()}");
    }
}