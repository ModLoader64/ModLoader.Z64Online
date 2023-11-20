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


        [OnEmulatorStart]
        public static void OnEmulatorStart(EventEmulatorStart e)
        {
            clientStorage.saveManager = new OoTOSaveData();
        }

        public static void UpdateInventory(byte[] e)
        {
            
            if (OoTOnline.helper.isTitleScreen() || OoTOnline.helper.isSceneNumberValid() || OoTOnline.helper.isPaused() || !clientStorage.first_time_sync) return;
            //if (OoTOnline.helper.Player_InBlockingCsMode() || !this.LobbyConfig.data_syncing) return;

            OoTOSyncSave save = clientStorage.saveManager.CreateSave();

            if (syncTimer > syncTimerMax)
            {
                clientStorage.lastPushHash = clientStorage.saveManager.GetHashCode().ToString();
                Console.WriteLine("Forcing resync due to timeout.");
            }
            if(clientStorage.lastPushHash != clientStorage.saveManager.hash)
            {
                Z64O_UpdateSaveDataPacket packet = new Z64O_UpdateSaveDataPacket(save, clientStorage.world);
                ModLoader.API.NetworkSenders.Client.SendPacket(packet, ModLoader.API.NetworkClientData.lobby);
                clientStorage.lastPushHash = clientStorage.saveManager.hash;
                syncTimer = 0;
            }

        }

        //------------------------------
        // Lobby Setup
        //------------------------------

        [EventHandler(NetworkEvents.CLIENT_ON_NETWORK_CONNECT)]
        public static void OnConnect()
        {
            Console.WriteLine("Connected to server.");
            clientStorage.first_time_sync = false;
        }

        [EventHandler(NetworkEvents.CLIENT_ON_NETWORK_LOBBY_JOIN)]
        public static void OnLobbyJoin(EventClientNetworkLobbyJoined e)
        {
            clientStorage.first_time_sync = false;
        }

        [OnInit]
        public static void OnInit(EventPluginsLoaded evt)
        {
            Console.WriteLine("OoTOnlineClient: Init");
        }

        public static void Destroy()
        {
            Console.WriteLine("OoTOnlineClient: Destroy");
        }

        [OnFrame]
        public static void OnTick(EventNewFrame e)
        {

        }

        [OnViUpdate]
        public static void OnViUpdate(EventNewVi e)
        {

        }

        [EventHandler("OnRomLoaded")]
        public static void OnRomLoaded(EventRomLoaded e)
        {

        }
    }
}
