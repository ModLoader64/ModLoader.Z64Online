using OoT.API;
using OoT;
using Buffer = NodeBuffer.Buffer;

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

        public static void AutosaveSceneData()
        {
            if (!Core.helper.isLinkEnteringLoadingZone() && Core.global.scene_framecount > 20 && clientStorage.first_time_sync)
            {

                Buffer save = new Buffer(0x1C);
                Buffer saveScene = Core.save.GetSceneFlagsFromIndexRaw(Core.global.sceneID);

                u32 chests = Core.global.liveChests;
                u32 switches = Core.global.liveSwitch;
                u32 clear = Core.global.liveClear;
                u32 collect = Core.global.liveCollect;
                u32 temp = Core.global.liveTemp;
                u32 rooms = saveScene.ReadU32(0x14);
                u32 floors = saveScene.ReadU32(0x18);

                save.WriteU32(0x0, chests);
                save.WriteU32(0x4, switches);
                save.WriteU32(0x8, clear);
                save.WriteU32(0xC, collect);
                save.WriteU32(0x10, temp);
                save.WriteU32(0x14, rooms);
                save.WriteU32(0x18, floors);

                string save_hash_2 = ModLoader.API.Utils.GetHashSHA1(save._buffer);
                if (save_hash_2 != clientStorage.autoSaveHash)
                {
                    Console.WriteLine("AutoSaveSceneData()");
                    Console.WriteLine($"save chest: {saveScene.ReadU32(0x0).ToString("X")}, save swch: {saveScene.ReadU32(0x4).ToString("X")}, save clear: {saveScene.ReadU32(0x8).ToString("X")}, save collect: {saveScene.ReadU32(0xC).ToString("X")}, save temp: {saveScene.ReadU32(0x10).ToString("X")}");
                    Console.WriteLine($"live chest: {save.ReadU32(0x0).ToString("X")}, live swch: {save.ReadU32(0x4).ToString("X")}, live clear: {save.ReadU32(0x8).ToString("X")}, live collect: {save.ReadU32(0xC).ToString("X")}, live temp: {save.ReadU32(0x10).ToString("X")}");
                    for (int i = 0; i < saveScene.Size; i++)
                    {
                        u8 sFlag = saveScene.ReadU8(i);
                        u8 iFlag = save.ReadU8(i);
                        saveScene.WriteU8(i, sFlag |= iFlag);
                    }
                    clientStorage.autoSaveHash = save_hash_2;
                }
                else
                {
                    return;
                }
                Core.save.SetSceneFlagsSetIndexRaw(Core.global.sceneID, saveScene);
                NetworkSenders.Client.SendPacket(new Z64O_ClientSceneContextUpdate(chests, switches, collect, clear, temp, Core.global.sceneID, clientStorage.world, NetworkClientData.lobby, NetworkClientData.me), NetworkClientData.lobby);

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

            if (!RomFlags.isRando) return;
            if (OoTR_PotsanityHelper.HasPotsanity())
            {
                RomFlags.OotR_HasPotsanity = true;
                RomFlags.OotR_PotsanityFlagSize = OoTR_PotsanityHelper.GetFlagArraySize();
            }

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

        [ClientNetworkHandler(typeof(Z64O_ClientSceneContextUpdate))]
        public static void OnSceneContextSync_Client(Z64O_ClientSceneContextUpdate packet)
        {
            if (Core.helper.isTitleScreen() || !Core.helper.isSceneNumberValid() || Core.helper.isLinkEnteringLoadingZone()) { return; }
            if (Core.global.sceneID != packet.scene) { return; }
            if (packet.world != clientStorage.world) return;
            if (packet.player.uuid == NetworkClientData.me.uuid) { return; }
            Console.WriteLine("OnSceneContextSync_Client");

            u32 chests = Core.global.liveChests;
            if((chests |= packet.chests) != 0)
            {
                Core.global.liveChests = chests;
            }
            u32 switches = Core.global.liveSwitch;
            if ((switches |= packet.switches) != 0)
            {
                Core.global.liveSwitch = switches;
            }
            u32 collect = Core.global.liveCollect;
            if ((collect |= packet.collect) != 0)
            {
                Core.global.liveCollect = collect;
            }
            u32 clear = Core.global.liveClear;
            if ((clear |= packet.clear) != 0)
            {
                Core.global.liveClear = clear;
            }
            u32 temp = Core.global.liveTemp;
            if ((temp |= packet.temp) != 0)
            {
                Core.global.liveTemp = temp;
            }

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
                OoTOnline.rando = new OoTR(new OoTR_BadSyncData(), new OoTR_PotsanityHelper(), new OoTR_TriforceHuntHelper());
            }
            else
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

                    AutosaveSceneData();

                    syncTimer++;
                    if (syncTimer % 20 == 0)
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
