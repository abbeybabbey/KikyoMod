using UnityEngine;

namespace KikyoMod.Features;

internal class BetterLog : FeatureComponent
{
    public override string FeatureName => GetType().Name;
    public override string OriginalAuthor => "abbey";

    public BetterLog()
    {
        Application.logMessageReceived += LogMessageReceivedAction;
    }

    private static void LogMessageReceivedAction(string condition, string stackTrace, LogType type)
    {
        if (type is LogType.Error)
        {
            KikyoLogger.Error($"Unity Log: {condition}");
        }
    }
}