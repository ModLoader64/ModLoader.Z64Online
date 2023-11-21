using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z64Online
{
    public class OoTOnlineServer
    {
        public static OoTOnlineStorage[] lobbyStorage;

        [EventHandler(NetworkEvents.SERVER_ON_NETWORK_CONNECT)]
        public static void OnServerConnect(EventServerNetworkConnection evt)
        {
            Console.WriteLine("OnServerConnected");
            lobbyStorage = new OoTOnlineStorage[0];
        }

        [EventHandler(NetworkEvents.SERVER_ON_NETWORK_LOBBY_CREATED)]
        public static void OnLobbyCreated(EventServerNetworkLobbyCreated evt)
        {
            Console.WriteLine("OnLobbyCreated");
            lobbyStorage.Append(new OoTOnlineStorage(evt.lobby));
            if (lobbyStorage == null || lobbyStorage.Length == 0) { return; }

            OoTOnlineStorage storage = GetLobbyStorage(evt.lobby);
            storage.saveManager = new OoTOSaveData();
            SetLobbyStorage(storage, evt.lobby);
        }

        [EventHandler(NetworkEvents.SERVER_ON_NETWORK_LOBBY_JOIN)]
        public static void OnPlayerJoin_Server(EventServerNetworkLobbyJoined evt)
        {
            Console.WriteLine("OnPlayerJoin_Server");
            OoTOnlineStorage storage = GetLobbyStorage(evt.lobby);
            if(storage ==null) { return; }

            storage.players.Append(new PlayerSceneServer(evt.player.uuid, -1));
            storage.networkPlayerInstances.Append(evt.player);
            SetLobbyStorage(storage, evt.lobby);
        }

        [ServerNetworkHandler(typeof(Z64O_ScenePacket))]
        public static void OnSceneChange_Server(Z64O_ScenePacket packet)
        {
            Console.WriteLine("OnSceneChange_Server");
            OoTOnlineStorage storage = GetLobbyStorage(packet.lobby);
            if (storage == null) { return; }

            foreach (PlayerSceneServer player in storage.players)
            {
                if(player.uuid == packet.player.uuid)
                {
                    player.scene = packet.scene;
                }
            }

            Console.WriteLine($"Server: Player {packet.player.nickname} moved to scene {packet.scene}.");

            PubEventBus.bus.PushEvent(new ServerPlayerChangedScenes(packet.player, packet.scene));
        }

        // Client is logging in and wants to know how to proceed.
        [ServerNetworkHandler (typeof(Z64O_DownloadRequestPacket))]
        public static void OnDownloadPacket(Z64O_DownloadRequestPacket packet)
        {
            Console.WriteLine("OnDownloadPacket");
            OoTOnlineStorage storage = GetLobbyStorage(packet.lobby);
            if (storage == null) { return; }
            if (storage.worlds[packet.player.data.world] == null)
            {
                Console.WriteLine($"Creating world {packet.player.data.world} for lobby {packet.lobby}");
                storage.worlds[packet.player.data.world] = new OotOnlineSave_Server();
            }
            var world = storage.worlds[packet.player.data.worlds];
            if (world.saveGameSetup) {
                // Game is running, get data.
                Z64O_DownloadResponsePacket resp = new Z64O_DownloadResponsePacket(packet.lobby);
                resp.save = world.save;
                resp.keys = world.keys;
                NetworkSenders.Server.SendPacketToSpecificPlayer(resp, packet.player, packet.lobby);
            } else
            {
                // Game is not running, give me your data.
                world.save = packet.save;
                world.saveGameSetup = true;
                Z64O_DownloadResponsePacket response = new Z64O_DownloadResponsePacket(packet.lobby);
                NetworkSenders.Server.SendPacketToSpecificPlayer(response, packet.player, packet.lobby);
            }
        }

        public static OoTOnlineStorage GetLobbyStorage(string lobby)
        {
            Console.WriteLine($"GetLobbyStorage: Storage[] Length {lobbyStorage.Length}");
            foreach (var storage in lobbyStorage)
            {
                if (storage.lobby == lobby)
                {
                    return storage;
                }
            }

            return null;
        }

        public static void SetLobbyStorage(OoTOnlineStorage incoming, string lobby)
        {

            var index = Array.FindIndex(lobbyStorage, storage => lobbyStorage.Contains(storage));
            lobbyStorage[index] = incoming;
            Console.WriteLine($"SetLobbyStorage: {index}");

        }
    }
}
