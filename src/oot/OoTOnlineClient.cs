using OoT.API;
using OoT;

namespace Z64Online.OoTOnline
{
    [BootstrapFilter]
    public class OoTOnlineClient : IBootstrapFilter
    {

        public static OoTOnlineStorageClient clientStorage = new OoTOnlineStorageClient(new OoTOSaveData());

        static int syncTimer = 0;
        static int syncTimerMax = 20 * 20;

        public static bool DoesLoad(byte[] e)
        {
            return Z64Online.currentGame.OoT || Z64Online.currentGame.OoTDBG;
        }

        [OnInit]
        public static void OnInit(EventPluginsLoaded evt)
        {
            DebugFlags.IsDebugEnabled = true;
            //Console.WriteLine("OoTOnlineClient: Init");
            //Console.WriteLine($"Nickname: {NetworkClientData.me.nickname}");

        }

        [OnEmulatorStart]
        public static void OnEmulatorStart(EventEmulatorStart e)
        {
            clientStorage.saveManager = new OoTOSaveData();
        }

        public static void UpdateInventory()
        {
            if (Core.helper.isTitleScreen() || !Core.helper.isSceneNumberValid() || Core.helper.isPaused() || !clientStorage.first_time_sync) return;
            //if (Core.helper.Player_InBlockingCsMode() || !this.LobbyConfig.data_syncing) return;

            OoTOSyncSave save = clientStorage.saveManager.CreateSave();

            if (syncTimer > syncTimerMax)
            {
                clientStorage.lastPushHash = "Reset".GetHashCode().ToString();
                Console.WriteLine("Forcing resync due to timeout.");
                syncTimer = 0;
            }
            if (clientStorage.lastPushHash != clientStorage.saveManager.hash)
            {
                Z64O_UpdateSaveDataPacket packet = new Z64O_UpdateSaveDataPacket(save, clientStorage.world, NetworkClientData.me, NetworkClientData.lobby);
                NetworkSenders.Client.SendPacket(packet, NetworkClientData.lobby);
                clientStorage.lastPushHash = clientStorage.saveManager.hash;
                syncTimer = 0;

                Console.WriteLine("UpdateInventory Hash Update");
            }

        }

        //------------------------------
        // Lobby Setup
        //------------------------------

        [EventHandler(NetworkEvents.CLIENT_ON_NETWORK_CONNECT)]
        public static void OnConnect(EventClientNetworkConnection e)
        {
            Console.WriteLine("Connected to server.");
            clientStorage.first_time_sync = false;
        }

        [EventHandler(NetworkEvents.CLIENT_ON_NETWORK_LOBBY_JOIN)]
        public static void OnLobbyJoin(EventClientNetworkLobbyJoined e)
        {
            Console.WriteLine("Client: OnLobbyJoin");
            NetworkClientData.me.data = new NTWKPlayerData();
            //NetworkClientData.me.data.world = -1;
            clientStorage.first_time_sync = false;
        }


        [ClientNetworkHandler(typeof(Z64O_DownloadResponsePacket))]
        public static void OnDownloadPacket_Client(Z64O_DownloadResponsePacket packet)
        {
            Console.WriteLine("Client: OnDownloadPacket_Client");
            if (Core.helper.isTitleScreen() || !Core.helper.isSceneNumberValid())
            {
                return;
            }
            if (packet.save != null)
            {
                Console.WriteLine("Syncing save with server.");
                clientStorage.saveManager.ForceOverrideSave(packet.save);
                clientStorage.saveManager.CreateSave();
                
                clientStorage.lastPushHash = clientStorage.saveManager.hash;
            }
            else
            {
                Console.WriteLine("The lobby is mine!");
            }
            clientStorage.first_time_sync = true;
        }

        //------------------------------
        // Save Load Handling
        //------------------------------

        [EventHandler("EventSaveLoaded")]
        public static void OnSaveLoad(EventSaveLoaded e)
        {
            Console.WriteLine("OnSaveLoad");
            NetworkSenders.Client.SendPacket(new Z64O_DownloadRequestPacket(new OoTOSaveData().CreateSave(), NetworkClientData.lobby, NetworkClientData.me), NetworkClientData.lobby);

        }

        [EventHandler("EventSoftReset")]
        public static void OnSoftReset(EventSoftReset e)
        {
            Console.WriteLine("OnSoftReset");
            clientStorage.first_time_sync = false;
        }

        //------------------------------
        // Save Handling
        //------------------------------

        [ClientNetworkHandler(typeof(Z64O_UpdateSaveDataPacket))]
        public static void OnSaveUpdate(Z64O_UpdateSaveDataPacket packet)
        {
            if (Core.helper.isTitleScreen() || !Core.helper.isSceneNumberValid()) { return; }
            if (packet.world != clientStorage.world) { return; }
            if (!clientStorage.first_time_sync) { return; }
            if (packet.player.uuid != NetworkClientData.me.uuid) { return; }
            Console.WriteLine("OnSaveUpdate");
            clientStorage.saveManager.Apply(packet.save);
            // Update hash.
            clientStorage.saveManager.CreateSave();
            
            clientStorage.lastPushHash = clientStorage.saveManager.hash;

        }


        //------------------------------
        // Scene Handling
        //------------------------------

        [EventHandler("EventSceneChange")]
        public static void OnSceneChange(EventSceneChange evt)
        {
            NetworkClientData.me.data.world = clientStorage.world;
            NetworkSenders.Client.SendPacket(new Z64O_ScenePacket(Core.global.sceneID, Core.save.linkAge, NetworkClientData.lobby, NetworkClientData.me), NetworkClientData.lobby);
            Console.WriteLine("Client: I moved to scene " + evt.scene + ".");
        }

        [ClientNetworkHandler(typeof(Z64O_ScenePacket))]
        public static void OnScenePacket_Client(Z64O_ScenePacket packet)
        {
            Console.WriteLine("Client Recieve: Player " + packet.player + " moved to scene " + packet.scene + ".");
            PubEventBus.bus.PushEvent(new ClientPlayerChangedScenes(packet.player, packet.scene));
        }

        //------------------------------
        // General Event Handling
        //------------------------------

        [EventHandler("OnAgeChange")]
        public static void OnAgeChange(EventAgeChange evt)
        {
            NetworkSenders.Client.SendPacket(new Z64O_ScenePacket(Core.global.sceneID, evt.age, NetworkClientData.lobby, NetworkClientData.me), NetworkClientData.lobby);
        }


        [EventHandler("EventRomLoaded")]
        public static void OnRomLoaded(EventRomLoaded e)
        {
            CheckOoTR(e.rom);
        }

        public static void Destroy()
        {
            Console.WriteLine("OoTOnlineClient: Destroy");
        }

        //------------------------------
        // OoTRando Handling
        //------------------------------

        public static void CheckOoTR(byte[] rom)
        {
            int start = 0x20;
            int prog = 0;
            int _byte = rom[start];
            int terminator = 0;
            while (_byte != terminator)
            {
                prog++;
                _byte = rom[start + prog];
            }
            prog++;
            if (rom[start + prog] > 0)
            {
                byte[] ver = rom[(start + prog - 1)..(start + prog - 1 + 0x4)];
                Console.WriteLine($"OoT Randomizer detected. Version: {Convert.ToHexString(ver)}");
                RomFlags.isRando = true;
                OoTOnline.rando = new OoTR(new OoTR_BadSyncData());
            } else
            {
                RomFlags.isVanilla = true;
                Console.WriteLine($"Vanilla rom detected.");
            }

        }


        //------------------------------
        // Tick Update
        //------------------------------

        [OnFrame]
        public static void OnTick(EventNewFrame e)
        {
            if (!Core.helper.isTitleScreen() && Core.helper.isSceneNumberValid())
            {
                if (!Core.helper.isPaused())
                {
                    if (!clientStorage.first_time_sync) { return; }
                    syncTimer++;
                    if(syncTimer % 20 == 0)
                    {
                        inventoryUpdateTick();
                    }
                }
            }
        }

        public static void inventoryUpdateTick()
        {
            UpdateInventory();
        }

        [OnViUpdate]
        public static void OnViUpdate(EventNewVi e)
        {

        }

    }
}
