using OoT.API;
using OoT.API.Enums;
using Buffer = NodeBuffer.Buffer;

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
        public OoTOnlineSaveSync? data = new OoTOnlineSaveSync();
    }

    public class OoTOnlineSaveSync
    {
        public InventoryItem[] items = new InventoryItem[(int)InventorySlot.COUNT];
        public OoTOnlineEquipmentSync equipment = new OoTOnlineEquipmentSync();
        public OoTOnlineQuestStatusSync questStatus = new OoTOnlineQuestStatusSync();
        public OoTOnlineDungeonSync dungeon = new OoTOnlineDungeonSync();
        public OoTOnlineFlagSync flags = new OoTOnlineFlagSync();
        public OoTOnlineRandoSync rando = new OoTOnlineRandoSync();

        public bool isOoTR = false;
        public bool isPotsanity = false;
        public bool isVanilla = false;
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

        public Capacity.AmmoUpgrade dekuNutCapacity = Capacity.AmmoUpgrade.None;
        public Capacity.AmmoUpgrade dekuStickCapacity = Capacity.AmmoUpgrade.None;
        public Capacity.AmmoUpgrade bombBag = Capacity.AmmoUpgrade.None;
        public Capacity.AmmoUpgrade bulletBag = Capacity.AmmoUpgrade.None;
        public Capacity.AmmoUpgrade quiver = Capacity.AmmoUpgrade.None;
        public Capacity.Wallet wallet = Capacity.Wallet.None;
        public Capacity.Strength strength = Capacity.Strength.None;
        public Capacity.Scales scale = Capacity.Scales.None;
    }

    public class OoTOnlineQuestStatusSync
    {
        public u16 healthCapacity; 
        public u8 heartPieces; // 0 - 4 , never set 4
        public u8 magicLevel;
        public s16 gsTokens;
        public bool songLullaby = false;
        public bool songEpona = false;
        public bool songSaria = false;
        public bool songSun = false;
        public bool songTime = false;
        public bool songStorms = false;
        public bool songPrelude = false;
        public bool songMinuet = false;
        public bool songBolero = false;
        public bool songSerenade = false;
        public bool songNocturne = false;
        public bool songRequiem = false;
        public bool kokiriEmerald = false;
        public bool goronRuby = false;
        public bool zoraSapphire = false;
        public bool medallionForest = false;
        public bool medallionFire = false;
        public bool medallionWater = false;
        public bool medallionSpirit = false;
        public bool medallionShadow = false;
        public bool medallionLight = false;
        public bool stoneAgony = false;
        public bool gerudoCard = false;
        public bool hasGoldSkull = false;
        public bool hasDoubleDefense = false;
    }

    public class OoTOnlineDungeonSync
    {
        public DungeonItems[] items = new DungeonItems[0x14];
        public DungeonKeys[] keys = new DungeonKeys[0x14];
    }

    public class OoTOnlineFlagSync 
    {
        public SceneFlagStruct[] sceneFlags = new SceneFlagStruct[124];
        public Buffer eventChkInf = new Buffer(0x1C);
        public Buffer itemGetInf = new Buffer(0x8);
        public Buffer infTable = new Buffer(0x3C);
        public Buffer gsFlags = new Buffer(0x18);
    }

    public class SceneFlagStruct
    {
        public u32 chest;
        public u32 swch;
        public u32 clear;
        public u32 collect;
        public u32 unk;
        public u32 rooms;
        public u32 floors;

        public SceneFlagStruct(u32 chest, u32 swch, u32 clear, u32 collect, u32 unk, u32 rooms, u32 floors)
        {
            this.chest = chest;
            this.swch = swch;
            this.clear = clear;
            this.collect = collect;
            this.unk = unk;
            this.rooms = rooms;
            this.floors = floors;
        }

        public bool Equals(SceneFlagStruct other)
        {
            if(this.chest != other.chest) return false;
            if (this.swch != other.swch) return false;
            if (this.clear != other.clear) return false;
            if (this.collect != other.collect) return false;
            if (this.unk != other.unk) return false;
            if (this.rooms != other.rooms) return false;
            if (this.floors != other.floors) return false;
            return true;
        }

    }

    public class OoTOnlineRandoSync
    {
        public Buffer collectible_override_flags = new Buffer(OoTR_PotsanityHelper.GetFlagArraySize());
        public u32 triforcePieces = 0;
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
