using OoT.API;
using System.Security.Cryptography;

namespace Z64Online
{
    public class OoTOSaveData
    {
        public string hash = "";

        public OoTOSyncSave CreateSave()
        {
            OoTOSyncSave syncSave = new OoTOSyncSave();
            CreateInventory(syncSave.inventory, OoTOnline.save.inventory);

            hash = this.GetHashCode().ToString();

            return syncSave;
        }

        public OoTOnlineInventorySync CreateInventory(OoTOnlineInventorySync sync, WrapperInventory save)
        {
            sync.dekuSticks = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.DEKU_STICKS);
            sync.dekuNuts = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.DEKU_NUTS);
            sync.bombs = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.BOMBS);
            sync.bow = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.FAIRY_BOW);
            sync.fireArrows = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.FIRE_ARROWS);
            sync.dinsFire = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.DINS_FIRE);
            sync.slingshot = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.FAIRY_SLINGSHOT);
            sync.ocarina = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.OCARINA);
            sync.bombchus = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.BOMBCHUS);
            sync.hookshot = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.HOOKSHOT);
            sync.iceArrows = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.ICE_ARROWS);
            sync.faroresWind = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.FARORES_WIND);
            sync.boomerang = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.BOOMERANG);
            sync.lensOfTruth = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.LENS_OF_TRUTH);
            sync.magicBeans = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.MAGIC_BEANS);
            sync.megatonHammer = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.MEGATON_HAMMER);
            sync.lightArrows = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.LIGHT_ARROWS);
            sync.nayrusLove = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.NAYRUS_LOVE);
            sync.bottle1 = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.BOTTLE1);
            sync.bottle2 = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.BOTTLE2);
            sync.bottle3 = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.BOTTLE3);
            sync.bottle4 = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.BOTTLE4);
            sync.childTrade = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.CHILD_TRADE_ITEM);
            sync.adultTrade = save.InventoryItems.GetItemInSlot(OoT.API.InventorySlot.ADULT_TRADE_ITEM);

            return sync;
        }
    }
}
