using System;
using ABI.CCK.Components;
using ABI_RC.Core.EventSystem;
using ABI_RC.Core.IO;
using ABI_RC.Core.Networking.IO.Instancing;
using ABI_RC.Core.Networking.IO.Social;
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

        Harmony.Patch(typeof(Instances).GetMethod(nameof(Instances.SetJoinTarget)),
            postfix: GetLocalPatch(nameof(SetJoinTargetPatch)));

        Harmony.Patch(typeof(CVRObjectLoader).GetMethod(nameof(CVRObjectLoader.InstantiateAvatar)),
            postfix: GetLocalPatch(nameof(InstantiateAvatarA)));
    }

    private static void SetJoinTargetPatch(string instanceId, string worldId)
    {
        KikyoLogger.Msg($"SetJoinTarget: {instanceId}, worldId: {worldId}");
    }

    // way better than the previous method, truly gets everyone's avatar instantiate event from my testing
    private static void InstantiateAvatarA(DownloadJob.ObjectType t, AssetManagement.AvatarTags tags, string objectId,
        string instTarget = null, byte[] b = null)
    {
        if (instTarget == "_PLAYERLOCAL") return;
        var player = CVRPlayerManager.Instance.TryGetPlayerName(instTarget);
        KikyoLogger.Msg(ConsoleColor.Cyan, $"InstantiateAvatar by {player} with an avatar {objectId}");
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