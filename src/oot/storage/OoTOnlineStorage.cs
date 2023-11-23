
using OoT.API;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Z64Online.OoTOnline
{
    public class OoTOnlineStorage : OoTOnlineStorageBase
    {
        public string lobby;
        public List<NetworkPlayer> networkPlayerInstances = new List<NetworkPlayer>();
        public List<PlayerSceneServer> players = new List<PlayerSceneServer>();
        public List<OotOnlineSave_Server> worlds = new List<OotOnlineSave_Server>();

        public OoTOnlineStorage(string lobby, OoTOSaveData saveManger) : base(saveManger)
        {
            this.lobby = lobby;
        }
    }

    public class PlayerSceneServer
    {
        public string uuid;
        public int scene;

        public PlayerSceneServer(string uuid, int scene)
        {
            this.uuid = uuid;
            this.scene = scene;
        }
    }
    public class OoTOSyncSave
    {
        public bool isOoTR = false;
        public bool isVanilla = false;
        public OoTOnlineInventorySync? inventory = new OoTOnlineInventorySync();
    }

    public class OoTOnlineInventorySync
    {
        public InventoryItem[] items = new InventoryItem[(int)InventorySlot.COUNT];
        public OoTOnlineEquipmentSync equipment = new OoTOnlineEquipmentSync();
        public OoTOnlineQuestStatusSync questStatus = new OoTOnlineQuestStatusSync();
    }

    public class OoTOnlineEquipmentSync
    {
        public bool kokiriSword = false;
        public bool masterSword = false;
        public bool giantsKnife = false;
        public bool biggoronSword = false;
        public bool dekuShield = false;
        public bool hylianShield = false;
        public bool mirrorShield = false;
        public bool kokiriTunic = false;
        public bool goronTunic = false;
        public bool zoraTunic = false;
        public bool kokiriBoots = false;
        public bool ironBoots = false;
        public bool hoverBoots = false;
    }

    public class OoTOnlineQuestStatusSync
    {

    }

    public class OOTKeyRingServer : IKeyRing
    {
        public byte[] keys { get; set; }
    }

    public class OotOnlineSave_Server
    {
        public bool saveGameSetup = false;
        public OoTOSyncSave save = new OoTOSyncSave();
        public IKeyRing keys = new OOTKeyRingServer();
    }
}
