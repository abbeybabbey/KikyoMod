using System;
using ABI.CCK.Components;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using DarkRift;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace KikyoMod.Features;

internal class NetworkInformation : FeatureComponent
{

    public override string FeatureName => GetType().Name;
    public override string OriginalAuthor => "abbey";

    public NetworkInformation()
    {
        Harmony.Patch(typeof(CVRPlayerManager).GetMethod(nameof(CVRPlayerManager.TryCreatePlayer)),
            postfix: GetLocalPatch(nameof(TryCreatePlayerPatch)));

        Harmony.Patch(typeof(CVRPlayerManager).GetMethod(nameof(CVRPlayerManager.TryDeletePlayer)), 
            GetLocalPatch(nameof(TryDeletePlayerPatch)));

        Harmony.Patch(typeof(CVRSyncHelper).GetMethod(nameof(CVRSyncHelper.SpawnPortalFromNetwork)),
            postfix: GetLocalPatch(nameof(SpawnPortalFromNetworkPatch)));

        Harmony.Patch(typeof(PuppetMaster).GetMethod(nameof(PuppetMaster.AvatarInstantiated)), 
            postfix: GetLocalPatch(nameof(AvatarInstantiatedPatch)));

        Harmony.Patch(AccessTools.Method(typeof(CVRWorld), "Start"), postfix: GetLocalPatch(nameof(WorldStartPatch)));
    }

    private static void WorldStartPatch(CVRWorld __instance)
    {
       KikyoLogger.Msg($"World Started: {__instance.gameObject.GetComponent<CVRAssetInfo>().guid}");
    }

    //https://github.com/ljoonal/CVR-Plugins/blob/main/HopLib/Api/Avatar.cs
    private static void AvatarInstantiatedPatch(PuppetMaster __instance)
    {
        var avatarGuid = __instance.avatarObject.GetComponent<CVRAssetInfo>().guid;
        if (string.IsNullOrEmpty(avatarGuid)) return;
        /*
         * HarmonyLib traverse is being used to access a private element in a class, https://harmony.pardeike.net/articles/utilities.html#traverse
         * Although most fields, properties and methods in that class hierarchy are private, Traverse can easily access anything.
         * It has build-in null protection and propagates null as a result if any of the intermediates would encounter null. 
         */
        var playerDescriptor = Traverse.Create(__instance)
            .Field("_playerDescriptor")
            .GetValue<PlayerDescriptor>();
        KikyoLogger.Msg(ConsoleColor.Cyan, $"AvatarInstantiated by {playerDescriptor.userName} with an avatar {__instance.avatarObject.GetComponent<CVRAssetInfo>().guid}");
    }

    private static void SpawnPortalFromNetworkPatch(Message message)
    {
        using var reader = message.GetReader();
        var portalOwner = reader.ReadString();
        var getInstanceId = reader.ReadString();
        if (portalOwner == MetaPort.Instance.ownerId)
        {
            KikyoLogger.Msg(ConsoleColor.Cyan, $"Portal spawned by {MetaPort.Instance.username} to an instance: {getInstanceId}");
        }
        else
        {
            var username = CVRPlayerManager.Instance.TryGetPlayerName(portalOwner);
            KikyoLogger.Msg(ConsoleColor.Cyan, $"Portal spawned by {username} to an instance: {getInstanceId}");
        }
    }

    private static void TryDeletePlayerPatch(CVRPlayerManager __instance, Message message)
    {
        CreatePlayerFromMessage(__instance, message, out var player);
        KikyoLogger.Msg($"[Instance] Player: {player.Username} has left");
    }

    private static void TryCreatePlayerPatch(CVRPlayerManager __instance, Message message)
    {
        CreatePlayerFromMessage(__instance, message, out var player);
        KikyoLogger.Msg($"[Instance] Player: {player.Username} has joined");
    }

    private static void CreatePlayerFromMessage(CVRPlayerManager __instance,Message message, out CVRPlayerEntity playerEntity)
    {
        using var reader = message.GetReader();
        var firstReadString = reader.ReadString();
        if (firstReadString.Length == 36)
        {
            var cvrPlayerEntity = __instance.NetworkPlayers.Find(entity => entity.Uuid == firstReadString);
            playerEntity = cvrPlayerEntity;
        }
        else
        {
            var player = new CVRPlayerEntity
            {
                Uuid = reader.ReadString(),
                Username = reader.ReadString()
            };
            playerEntity = player;
        }
    }
}