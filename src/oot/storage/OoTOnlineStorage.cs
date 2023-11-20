
using OoT.API;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Z64Online
{
    public class OoTOnlineStorage
    {
        public NetworkPlayer[] networkPlayerInstances = { };
        public object players = new { };
        public OotOnlineSave_Server[] worlds = { };
    }

    public class OoTOSyncSave
    {
        public bool isOoTR;
        public bool isVanilla;
        public OoTOnlineInventorySync? inventory;
    }

    public class OoTOnlineInventorySync
    {
        public InventoryItem dekuSticks;
        public InventoryItem dekuNuts;
        public InventoryItem bombs;
        public InventoryItem bow;
        public InventoryItem fireArrows;
        public InventoryItem dinsFire;
        public InventoryItem slingshot;
        public InventoryItem ocarina;
        public InventoryItem bombchus;
        public InventoryItem hookshot;
        public InventoryItem iceArrows;
        public InventoryItem faroresWind;
        public InventoryItem boomerang;
        public InventoryItem lensOfTruth;
        public InventoryItem magicBeans;
        public InventoryItem megatonHammer;
        public InventoryItem lightArrows;
        public InventoryItem nayrusLove;
        public InventoryItem bottle1;
        public InventoryItem bottle2;
        public InventoryItem bottle3;
        public InventoryItem bottle4;
        public InventoryItem childTrade;
        public InventoryItem adultTrade;

        public bool kokiriSword;
        public bool masterSword;
        public bool giantsKnife;
        public bool biggoronSword;

        public bool dekuShield;
        public bool hylianShield;
        public bool mirrorShield;

        public bool kokiriTunic;
        public bool goronTunic;
        public bool zoraTunic;

        public bool kokiriBoots;
        public bool ironBoots;
        public bool hoverBoots;
    }

    class OoTOnlineQuestStatus
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
