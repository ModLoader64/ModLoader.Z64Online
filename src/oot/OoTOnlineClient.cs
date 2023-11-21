using Network.Packets;
using OoT.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Z64Online
{
    [BootstrapFilter]
    public class OoTOnlineClient : IBootstrapFilter
    {

        public static OoTOnlineStorageClient clientStorage = new OoTOnlineStorageClient();

        static int syncTimer = 0;
        static int syncTimerMax = 60 * 20;

        public static bool DoesLoad(byte[] e)
        {
            return Z64Online.currentGame.OoT || Z64Online.currentGame.OoTDBG;
        }

        public class test
        {
            public int world { get; set; }
        }

        [OnInit]
        public static void OnInit(EventPluginsLoaded evt)
        {
            Console.WriteLine("OoTOnlineClient: Init");

            NetworkClientData.me.data = new test();
            NetworkClientData.me.data.world = -1;
        }

        [OnEmulatorStart]
        public static void OnEmulatorStart(EventEmulatorStart e)
        {
            clientStorage.saveManager = new OoTOSaveData();
        }

        public static void UpdateInventory()
        {
            Console.WriteLine("UpdateInventory");
            if (OoTOnline.helper.isTitleScreen() || OoTOnline.helper.isSceneNumberValid() || OoTOnline.helper.isPaused() || !clientStorage.first_time_sync) return;
            //if (OoTOnline.helper.Player_InBlockingCsMode() || !this.LobbyConfig.data_syncing) return;

            OoTOSyncSave save = clientStorage.saveManager.CreateSave();

            if (syncTimer > syncTimerMax)
            {
                clientStorage.lastPushHash = clientStorage.saveManager.GetHashCode().ToString();
                Console.WriteLine("Forcing resync due to timeout.");
            }
            if (clientStorage.lastPushHash != clientStorage.saveManager.hash)
            {
                Z64O_UpdateSaveDataPacket packet = new Z64O_UpdateSaveDataPacket(save, clientStorage.world, NetworkClientData.me);
                NetworkSenders.Client.SendPacket(packet, ModLoader.API.NetworkClientData.lobby);
                clientStorage.lastPushHash = clientStorage.saveManager.hash;
                syncTimer = 0;
            }

        }

        //------------------------------
        // Lobby Setup
        //------------------------------

        [EventHandler("CLIENT_ON_NETWORK_CONNECT")]
        public static void OnConnect(EventClientNetworkConnection e)
        {
            Console.WriteLine("Connected to server.");
            clientStorage.first_time_sync = false;
        }

        [EventHandler("CLIENT_ON_NETWORK_LOBBY_JOIN")]
        public static void OnLobbyJoin(EventClientNetworkLobbyJoined e)
        {
            Console.WriteLine("FUCK SHIT PISS DICK ASS");
            clientStorage.first_time_sync = false;
        }


        [ClientNetworkHandler(typeof(Z64O_DownloadResponsePacket))]
        public static void OnDownloadPacket_Client(Z64O_DownloadResponsePacket packet)
        {
            if (OoTOnline.helper.isTitleScreen() || !OoTOnline.helper.isSceneNumberValid())
            {
                return;
            }
            if (packet.save != null)
            {
                clientStorage.saveManager.forceOverrideSave(packet.save, OoTOnline.save);
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
            if (OoTOnline.helper.isTitleScreen() || !OoTOnline.helper.isSceneNumberValid()) { return; }
            if (packet.world != clientStorage.world) { return; }
            if (!clientStorage.first_time_sync) { return; }

            clientStorage.saveManager.ApplySave(packet.save);
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
            NetworkSenders.Client.SendPacket(new Z64O_ScenePacket(OoTOnline.global.sceneID, OoTOnline.save.linkAge, NetworkClientData.lobby, NetworkClientData.me), NetworkClientData.lobby);
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
            NetworkSenders.Client.SendPacket(new Z64O_ScenePacket(OoTOnline.global.sceneID, evt.age, NetworkClientData.lobby, NetworkClientData.me), NetworkClientData.lobby);
        }


        [EventHandler("OnRomLoaded")]
        public static void OnRomLoaded(EventRomLoaded e)
        {

        }

        public static void Destroy()
        {
            Console.WriteLine("OoTOnlineClient: Destroy");
        }

        //------------------------------
        // Tick Update
        //------------------------------

        [OnFrame]
        public static void OnTick(EventNewFrame e)
        {
            if (!OoTOnline.helper.isTitleScreen() && OoTOnline.helper.isSceneNumberValid())
            {
                if (!OoTOnline.helper.isPaused())
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
