
namespace Z64Online.OoTOnline
{
    public class OoTOnlineServer
    {
        /// <summary>
        /// List of all the lobby's data on the server.
        /// </summary>
        public static List<OoTOnlineStorage> lobbyStorage = new List<OoTOnlineStorage>();

        /// <summary>
        /// Forwards a packet between players in the same scene.
        /// </summary>
        /// <param name="packet"></param>
        public static void SendPacketToPlayersInScene(dynamic packet)
        {
            if (packet == null) return;
            Console.WriteLine("SendPacketToPlayersInScene");
            OoTOnlineStorage storage = GetLobbyStorage(packet.lobby);
            if (storage == null) { return; }

            foreach (NetworkPlayer networkPlayer in storage.networkPlayerInstances)
            {
                if (networkPlayer.uuid != packet.player.uuid)
                {
                    NetworkSenders.Server.SendPacketToSpecificPlayer(packet, networkPlayer, packet.lobby);
                }
            }
        }


        [EventHandler(NetworkEvents.SERVER_ON_NETWORK_CONNECT)]
        public static void OnServerConnect(EventServerNetworkConnection evt)
        {
            Console.WriteLine("OnServerConnected");
        }

        /// <summary>
        /// Adds the current lobby to the lobby storage list.
        /// </summary>
        /// <param name="evt"></param>
        [EventHandler(NetworkEvents.SERVER_ON_NETWORK_LOBBY_CREATED)]
        public static void OnLobbyCreated(EventServerNetworkLobbyCreated evt)
        {
            Console.WriteLine("OnLobbyCreated");
            lobbyStorage.Add(new OoTOnlineStorage(evt.lobby, new OoTOSaveData()));
            if (lobbyStorage == null || lobbyStorage.Count == 0) { return; }

            OoTOnlineStorage storage = GetLobbyStorage(evt.lobby);
            storage.saveManager = new OoTOSaveData();
            SetLobbyStorage(storage, evt.lobby);
        }

        /// <summary>
        /// Adds the new player to the lobby's list.
        /// </summary>
        /// <param name="evt"></param>
        [EventHandler(NetworkEvents.SERVER_ON_NETWORK_LOBBY_JOIN)]
        public static void OnPlayerJoin_Server(EventServerNetworkLobbyJoined evt)
        {
            Console.WriteLine("OnPlayerJoin_Server");
            OoTOnlineStorage storage = GetLobbyStorage(evt.lobby);
            if(storage == null) { return; }

            storage.players.Append(new PlayerSceneServer(evt.player.uuid, -1));
            storage.networkPlayerInstances.Append(evt.player);
            SetLobbyStorage(storage, evt.lobby);
        }

        /// <summary>
        /// Removes the player from the lobby's list.
        /// </summary>
        /// <param name="evt"></param>
        [EventHandler(NetworkEvents.SERVER_ON_NETWORK_LOBBY_DISCONNECT)]
        public static void OnPlayerLeave_Server(EventServerNetworkLobbyDisconnect evt)
        {
            Console.WriteLine("OnPlayerLeave_Server");
            OoTOnlineStorage storage = GetLobbyStorage(evt.lobby);
            if (storage == null) { return; }

            foreach(var player in storage.players)
            {
                if (player.uuid == evt.player.uuid)
                {
                    storage.players.Remove(player);
                    break;
                }
            }

            storage.networkPlayerInstances.Remove(evt.player);

            Console.WriteLine($"Server: Player {evt.player.nickname} disconnected.");
        }

        /// <summary>
        /// Merges the server's save data with the save data sent by the client, and forwards the result to the other players.
        /// </summary>
        /// <param name="packet"></param>
        [ServerNetworkHandler(typeof(Z64O_UpdateSaveDataPacket))]
        public static void OnUpdateSaveData_Server(Z64O_UpdateSaveDataPacket packet)
        {
            Console.WriteLine("OnUpdateSaveData_Server");
            OoTOnlineStorage storage = GetLobbyStorage(packet.lobby);
            if (storage == null) { return; }

            if (storage.worlds.Count < packet.player.data.world + 1)
            {
                NetworkSenders.Server.SendPacket(new Z64O_ErrorPacket("The server has encountered an error with your world. (world id is undefined)", packet.lobby), packet.lobby);
                return;
            }
            var world = storage.worlds[packet.player.data.world];
            storage.saveManager.Merge(packet.save, world.save);
            storage.worlds[packet.player.data.world] = world;
            NetworkSenders.Server.SendPacket(new Z64O_UpdateSaveDataPacket(world.save, packet.player.data.world, packet.player, packet.lobby), packet.lobby);
        }

        /// <summary>
        /// Updates the client's current scene 
        /// </summary>
        /// <param name="packet"></param>
        [ServerNetworkHandler(typeof(Z64O_ScenePacket))]
        public static void OnSceneChange_Server(Z64O_ScenePacket packet)
        {
            Console.WriteLine("OnSceneChange_Server");
            //Console.WriteLine(packet.player.uuid);
            OoTOnlineStorage storage = GetLobbyStorage(packet.lobby);
            if (storage == null) { return; }

            foreach (PlayerSceneServer player in storage.players)
            {
                if(player.uuid == packet.player.uuid)
                {
                    player.scene = packet.scene;
                    break;
                }
            }

            Console.WriteLine($"Server: Player {packet.player.nickname} moved to scene {packet.scene}.");

            PubEventBus.bus.PushEvent(new ServerPlayerChangedScenes(packet.player, packet.scene));
        }

        /// <summary>
        /// Client is logging in and wants to know how to proceed.
        /// </summary>
        /// <param name="packet"></param>
        [ServerNetworkHandler (typeof(Z64O_DownloadRequestPacket))]
        public static void OnDownloadPacket(Z64O_DownloadRequestPacket packet)
        {
            Console.WriteLine("OnDownloadPacket");
            OoTOnlineStorage storage = GetLobbyStorage(packet.lobby);
            if (storage == null) { return; }


            if (storage.worlds.Count < packet.player.data.world + 1)
            {
                Console.WriteLine($"Creating world {packet.player.data.world} for lobby {packet.lobby}");

                storage.worlds.Add(new OotOnlineSave_Server());
            }
            var world = storage.worlds[packet.player.data.world];
            Console.WriteLine($"World is set up?: {world.saveGameSetup}");
            if (world.saveGameSetup) {
                // Game is running, get data.
                Z64O_DownloadResponsePacket resp = new Z64O_DownloadResponsePacket(packet.lobby, packet.player);
                resp.save = world.save;
                resp.keys = world.keys;
                NetworkSenders.Server.SendPacketToSpecificPlayer(resp, packet.player, packet.lobby);
            } else
            {
                // Game is not running, give me your data.
                world.save = packet.save;
                world.saveGameSetup = true;
                Z64O_DownloadResponsePacket response = new Z64O_DownloadResponsePacket(packet.lobby, packet.player);
                Console.WriteLine("Player UUID: "+ packet.player.uuid);
                NetworkSenders.Server.SendPacketToSpecificPlayer(response, packet.player, packet.lobby);
            }
            storage.worlds[packet.player.data.world] = world;
        }

        /// <summary>
        /// Forwards the live scene data to the lobby.
        /// </summary>
        /// <param name="packet"></param>
        [ServerNetworkHandler (typeof(Z64O_ClientSceneContextUpdate))]
        public static void OnSceneContextSync_Server(Z64O_ClientSceneContextUpdate packet)
        {
            NetworkSenders.Server.SendPacket(packet, packet.lobby);
        }

        /// <summary>
        /// Iterates through the all the lobbies on the server and returns the requested lobby.
        /// </summary>
        /// <param name="lobby">Name of the requested lobby.</param>
        /// <returns>The data of the requested lobby.</returns>
        public static OoTOnlineStorage GetLobbyStorage(string lobby)
        {
            Console.WriteLine($"GetLobbyStorage: Storage[] Length {lobbyStorage.Count}");
            foreach (var storage in lobbyStorage)
            {
                if (storage.lobby == lobby)
                {
                    return storage;
                }
            }

            return null;
        }

        /// <summary>
        /// Iterates through the all the lobbies on the server and applies data to the requested lobby.
        /// </summary>
        /// <param name="incoming">New lobby data too apply.</param>
        /// <param name="lobby">Name of the requested lobby.</param>
        public static void SetLobbyStorage(OoTOnlineStorage incoming, string lobby)
        {
            int index = 0;
            bool success = false;
            foreach (var storage in lobbyStorage)
            {
                if(storage.lobby == lobby)
                {
                    lobbyStorage[lobbyStorage.FindIndex(index => storage.lobby == lobby)] = incoming;
                    break;
                }
            }

            Console.WriteLine($"SetLobbyStorage: {index}");
            
        }
    }
}
