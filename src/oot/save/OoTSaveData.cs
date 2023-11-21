using OoT.API;
using System.Security.Cryptography;
using System.Text;

namespace Z64Online
{
    public class OoTOSaveData
    {
        public string hash = "";
        public OoTOSyncSave CreateSave()
        {
            OoTOSyncSave syncSave = new OoTOSyncSave();
            syncSave.inventory = CreateInventory(syncSave.inventory, OoTOnline.save.inventory);
            hash = ModLoader.API.Utils.GetHashSHA1(ModLoader.API.Utils.ObjectToByteArray(syncSave));
            return syncSave;
        }

        public void ApplySave(OoTOSyncSave incoming)
        {
            //OoTOSyncSave save = new OoTOSyncSave();
            //save.inventory = CreateInventory(incoming.inventory, OoTOnline.save.inventory);
            MergeSave(incoming);
        }

        public void MergeSave(OoTOSyncSave incoming, OoTOSyncSave save = null)
        {
            if(save == null)
            {
                ApplyInventory(OoTOnline.save.inventory, incoming.inventory);
            }
            else
            {
                MergeInventory(incoming.inventory, save.inventory);
            }
        }

        public void MergeInventory(OoTOnlineInventorySync incoming, OoTOnlineInventorySync save)
        {
            save.dekuSticks = incoming.dekuSticks;
            save.dekuNuts = incoming.dekuNuts;
            save.bombs = incoming.bombs;
            save.bow = incoming.bow;
            save.fireArrows = incoming.fireArrows;
            save.dinsFire = incoming.dinsFire;
            save.slingshot = incoming.slingshot;
            save.ocarina = incoming.ocarina;
            save.bombchus = incoming.bombchus;
            save.hookshot = incoming.hookshot;
            save.iceArrows = incoming.iceArrows;
            save.faroresWind = incoming.faroresWind;
            save.boomerang = incoming.boomerang;
            save.lensOfTruth = incoming.lensOfTruth;
            save.magicBeans = incoming.magicBeans;
            save.megatonHammer = incoming.megatonHammer;
            save.lightArrows = incoming.lightArrows;
            save.nayrusLove = incoming.nayrusLove;
            save.bottle1 = incoming.bottle1;
            save.bottle2 = incoming.bottle2;
            save.bottle3 = incoming.bottle3;
            save.bottle4 = incoming.bottle4;
            save.childTrade = incoming.childTrade;
            save.adultTrade = incoming.adultTrade;
        }

        public void forceOverrideSave(OoTOSyncSave incoming, WrapperSaveContext save)
        {
            OverrideInventory(incoming.inventory, save.inventory);

        }

        public void OverrideInventory(OoTOnlineInventorySync incoming, WrapperInventory save)
        {
            save.InventoryItems.SetItemInSlot(InventorySlot.DEKU_STICKS, incoming.dekuSticks);
            save.InventoryItems.SetItemInSlot(InventorySlot.DEKU_NUTS, incoming.dekuNuts);
            save.InventoryItems.SetItemInSlot(InventorySlot.BOMBS, incoming.bombs);
            save.InventoryItems.SetItemInSlot(InventorySlot.FAIRY_BOW, incoming.bow);
            save.InventoryItems.SetItemInSlot(InventorySlot.FIRE_ARROWS, incoming.fireArrows);
            save.InventoryItems.SetItemInSlot(InventorySlot.DINS_FIRE, incoming.dinsFire);
            save.InventoryItems.SetItemInSlot(InventorySlot.FAIRY_SLINGSHOT, incoming.slingshot);
            save.InventoryItems.SetItemInSlot(InventorySlot.OCARINA, incoming.ocarina);
            save.InventoryItems.SetItemInSlot(InventorySlot.BOMBCHUS, incoming.bombchus);
            save.InventoryItems.SetItemInSlot(InventorySlot.HOOKSHOT, incoming.hookshot);
            save.InventoryItems.SetItemInSlot(InventorySlot.ICE_ARROWS, incoming.iceArrows);
            save.InventoryItems.SetItemInSlot(InventorySlot.FARORES_WIND, incoming.faroresWind);
            save.InventoryItems.SetItemInSlot(InventorySlot.BOOMERANG, incoming.boomerang);
            save.InventoryItems.SetItemInSlot(InventorySlot.LENS_OF_TRUTH, incoming.lensOfTruth);
            save.InventoryItems.SetItemInSlot(InventorySlot.MAGIC_BEANS, incoming.magicBeans);
            save.InventoryItems.SetItemInSlot(InventorySlot.MEGATON_HAMMER, incoming.megatonHammer);
            save.InventoryItems.SetItemInSlot(InventorySlot.LIGHT_ARROWS, incoming.lightArrows);
            save.InventoryItems.SetItemInSlot(InventorySlot.NAYRUS_LOVE, incoming.nayrusLove);
            save.InventoryItems.SetItemInSlot(InventorySlot.BOTTLE1, incoming.bottle1);
            save.InventoryItems.SetItemInSlot(InventorySlot.BOTTLE2, incoming.bottle2);
            save.InventoryItems.SetItemInSlot(InventorySlot.BOTTLE3, incoming.bottle3);
            save.InventoryItems.SetItemInSlot(InventorySlot.BOTTLE4, incoming.bottle4);
            save.InventoryItems.SetItemInSlot(InventorySlot.CHILD_TRADE_ITEM, incoming.childTrade);
            save.InventoryItems.SetItemInSlot(InventorySlot.ADULT_TRADE_ITEM, incoming.adultTrade);
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

        public void ApplyInventory(WrapperInventory save, OoTOnlineInventorySync incoming)
        {
            save.InventoryItems[InventorySlot.DEKU_STICKS] = incoming.dekuSticks;
            save.InventoryItems[InventorySlot.DEKU_NUTS] = incoming.dekuNuts;
            save.InventoryItems[InventorySlot.BOMBS] = incoming.bombs;
            save.InventoryItems[InventorySlot.FAIRY_BOW] = incoming.bow;
            save.InventoryItems[InventorySlot.FIRE_ARROWS] = incoming.fireArrows;
            save.InventoryItems[InventorySlot.DINS_FIRE] = incoming.dinsFire;
            save.InventoryItems[InventorySlot.FAIRY_SLINGSHOT] = incoming.slingshot;
            save.InventoryItems[InventorySlot.OCARINA] = incoming.ocarina;
            save.InventoryItems[InventorySlot.BOMBCHUS] = incoming.bombchus;
            save.InventoryItems[InventorySlot.HOOKSHOT] = incoming.hookshot;
            save.InventoryItems[InventorySlot.ICE_ARROWS] = incoming.iceArrows;
            save.InventoryItems[InventorySlot.FARORES_WIND] = incoming.faroresWind;
            save.InventoryItems[InventorySlot.BOOMERANG] = incoming.boomerang;
            save.InventoryItems[InventorySlot.LENS_OF_TRUTH] = incoming.lensOfTruth;
            save.InventoryItems[InventorySlot.MAGIC_BEANS] = incoming.magicBeans;
            save.InventoryItems[InventorySlot.MEGATON_HAMMER] = incoming.megatonHammer;
            save.InventoryItems[InventorySlot.LIGHT_ARROWS] = incoming.lightArrows;
            save.InventoryItems[InventorySlot.NAYRUS_LOVE] = incoming.nayrusLove;
            save.InventoryItems[InventorySlot.BOTTLE1] = incoming.bottle1;
            save.InventoryItems[InventorySlot.BOTTLE2] = incoming.bottle2;
            save.InventoryItems[InventorySlot.BOTTLE3] = incoming.bottle3;
            save.InventoryItems[InventorySlot.BOTTLE4] = incoming.bottle4;
            save.InventoryItems[InventorySlot.CHILD_TRADE_ITEM] = incoming.childTrade;
            save.InventoryItems[InventorySlot.ADULT_TRADE_ITEM] = incoming.adultTrade;
        }
    }
}
