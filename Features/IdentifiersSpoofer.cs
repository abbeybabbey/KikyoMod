using System;
using System.Linq;
using MelonLoader;
using UnityEngine;
using Random = System.Random;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace KikyoMod.Features;

internal class IdentifiersSpoofer : FeatureComponent
{
    private static string _newHWID;
    private static string _newDeviceName;

    public IdentifiersSpoofer()
    {
        var category = MelonPreferences.CreateCategory("HWIDPatch", "HWID Patch");
        var hwidEntry = category.CreateEntry("HWID", "", is_hidden: true);
        var deviceNameEntry = category.CreateEntry("DeviceName", "", is_hidden: true);

        var newId = hwidEntry.Value;
        var newDeviceName = deviceNameEntry.Value;

        if (newId.Length != SystemInfo.deviceUniqueIdentifier.Length)
        {
            var random = new Random(Environment.TickCount);
            var bytes = new byte[SystemInfo.deviceUniqueIdentifier.Length / 2];
            random.NextBytes(bytes);
            newId = string.Join("", bytes.Select(it => it.ToString("x2")));
            MelonLogger.Msg("Generated and saved a new HWID");
            hwidEntry.Value = newId;
            category.SaveToFile(false);
        }

        if (newDeviceName.Length != SystemInfo.deviceName.Length)
        {
            var random = new Random(Environment.TickCount);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomDeviceName = new string(Enumerable.Repeat(chars, 7)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            var originalDeviceName = SystemInfo.deviceName;
            newDeviceName = originalDeviceName.Replace(SystemInfo.deviceName, $"DESKTOP-{randomDeviceName}");
            deviceNameEntry.Value = newDeviceName;
            category.SaveToFile(false);
        }

        _newHWID = newId;

        _newDeviceName = newDeviceName;

        Harmony.Patch(
            typeof(SystemInfo).GetProperty(nameof(SystemInfo.deviceUniqueIdentifier))?.GetMethod,
            postfix: GetLocalPatch(nameof(HWIDPatch)));

        if (SystemInfo.deviceUniqueIdentifier == newId) KikyoLogger.Msg($"Patched HWID: {newId}");
        else KikyoLogger.Error($"{SystemInfo.deviceUniqueIdentifier} and {newId} don't match, patching failed");

        Harmony.Patch(
            typeof(SystemInfo).GetProperty(nameof(SystemInfo.deviceName))?.GetMethod,
            postfix: GetLocalPatch(nameof(DeviceNamePatch)));

        if (SystemInfo.deviceName == newDeviceName) KikyoLogger.Msg($"Patched DeviceName: {newDeviceName}");
        else KikyoLogger.Error($"{SystemInfo.deviceName} and {newDeviceName} don't match, patching failed");
    }

    public override string FeatureName => GetType().Name;
    public override string OriginalAuthor => "knah";

    /*
     * If you wish to alter the __result, you need to define it by reference like ref string name
     * https://harmony.pardeike.net/articles/patching-injections.html#__result
     */
    private static void DeviceNamePatch(ref string __result)
    {
        __result = _newDeviceName;
    }

    private static void HWIDPatch(ref string __result)
    {
        __result = _newHWID;
    }
}