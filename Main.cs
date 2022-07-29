using System;
using System.Linq;
using System.Reflection;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using ABI_RC.Core.Util;
using DarkRift;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace CVRMods
{
    internal static class ModInfo
    {
        public const string Name = "CVRMods";
        public const string Author = "abbey";
        public const string Version = "1.0.0.2";
    }

    public class Main : MelonMod
    {
        private static string _newHWID;

        private static readonly HarmonyLib.Harmony _harmony = new("CVRMods");
        private static readonly MelonLogger.Instance _loggerInstance = new("CVRMods");

        public override void OnApplicationStart()
        {
           InitializePatches();
        }

        private static HarmonyMethod GetLocalPatch(string name)
        {
            return typeof(Main).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();
        }

        private static void InitializePatches()
        {

            try
            {
                var category = MelonPreferences.CreateCategory("HWIDPatch", "HWID Patch");
                var hwidEntry = category.CreateEntry("HWID", "", is_hidden: true);
                var newId = hwidEntry.Value;
                if (newId.Length != SystemInfo.deviceUniqueIdentifier.Length)
                {
                    var random = new System.Random(Environment.TickCount);
                    var bytes = new byte[SystemInfo.deviceUniqueIdentifier.Length / 2];
                    random.NextBytes(bytes);
                    newId = string.Join("", bytes.Select(it => it.ToString("x2")));
                    MelonLogger.Msg("Generated and saved a new HWID");
                    hwidEntry.Value = newId;
                    category.SaveToFile(false);
                }

                _newHWID = newId;

                _harmony.Patch(
                    typeof(SystemInfo).GetProperty(nameof(SystemInfo.deviceUniqueIdentifier))?.GetMethod,
                    postfix: GetLocalPatch(nameof(HWIDPatch)));

                MelonLogger.Msg("Patched HWID; below two should match:");
                MelonLogger.Msg($"Current: {SystemInfo.deviceUniqueIdentifier}");
                MelonLogger.Msg($"Target:  {newId}");
            }
            catch (Exception e)
            {
                _loggerInstance.Error("An exception occurred while trying to patch a HWID:\n", e);
            }

            try
            {
                _harmony.Patch(
                    typeof(Object).GetMethod(nameof(Object.Instantiate), new []{typeof(Object), typeof(Vector3), typeof(Quaternion)}), 
                    GetLocalPatch(nameof(Instantiate)));
            }
            catch (Exception e)
            {
                _loggerInstance.Error("An exception occurred while trying to patch a Instantiate:\n", e);
            }

            try
            {
                _harmony.Patch(typeof(CVRSyncHelper).GetMethod(nameof(CVRSyncHelper.SpawnPortalFromNetwork)),
                    GetLocalPatch(nameof(SpawnPortalNetwork)));
            }
            catch (Exception e)
            {
                _loggerInstance.Error("An exception occurred while trying to patch a SpawnPortalFromNetwork:\n", e);
            }
        }

        private static void SpawnPortalNetwork(Message message)
        {
            using var reader = message.GetReader();
            var portalOwner = reader.ReadString();
            _loggerInstance.Msg($"Received a spawn portal network request, PortalOwner: {portalOwner}");
        }


        private static bool Instantiate(Object original, Vector3 position, Quaternion rotation)
        {
            if (!position.IsSafe())
            {
                _loggerInstance.Error($"A object {original.name} tried to be instantiated with an invalid position: {position}");
                return false;
            }

            if (!rotation.IsSafe())
            {
                _loggerInstance.Error($"A object {original.name} tried to be instantiated with an invalid rotation: {rotation}");
                return false;
            }

            return true;
        }

        private static void HWIDPatch(ref string __result)
        {
           __result = _newHWID;
        }
    }
}
