using OoT.API;
using OoT;
using System.Security.Cryptography;
using System.Text;

namespace Z64Online.OoTOnline
{
    public class OoTOSaveData
    {
        public string hash = "";
        public OoTOSyncSave CreateSave()
        {
            OoTOSyncSave syncSave = new OoTOSyncSave();
            syncSave.inventory = CreateInventory(Core.save.inventory);
            hash = ModLoader.API.Utils.GetHashSHA1(ModLoader.API.Utils.ObjectToByteArray(syncSave));
            return syncSave;
        }

        public void ApplySave(OoTOSyncSave incoming)
        {
            //OoTOSyncSave save = new OoTOSyncSave();
            //save.inventory = CreateInventory(incoming.inventory, Core.save.inventory);
            MergeSave(incoming);
        }

        public void MergeSave(OoTOSyncSave incoming, OoTOSyncSave save = null)
        {
            // Write to local OoTO Save if supplied, otherwise write to save file directly
            if (save != null)
            {
                MergeInventory(incoming.inventory, save.inventory);
            }
            else
            {
                forceOverrideSave(incoming, save);
            }
        }

        public void MergeInventory(OoTOnlineInventorySync incoming, OoTOnlineInventorySync save)
        {
            // TODO: Individual logic for some items & potential downgrading 
            for (int i = 0; i < (int)InventorySlot.COUNT; i++)
            {
                if (save.items[i] != incoming.items[i])
                {
                    save.items[i] = incoming.items[i];
                }
            }

        }

        public void forceOverrideSave(OoTOSyncSave incoming, OoTOSyncSave save = null)
        {
            // Write to local OoTO Save if supplied, otherwise write to save file directly
            if (save != null)
            {
                OverrideInventory(incoming.inventory, save.inventory);
            } else
            {
                OverrideInventory(incoming.inventory);
            }

        }

        public void OverrideInventory(OoTOnlineInventorySync incoming, OoTOnlineInventorySync save = null)
        {
            // Write to local OoTO Inventory if supplied, otherwise write to save file directly
            if (save != null)
            {
                for (int i = 0; i < (int)InventorySlot.COUNT; i++)
                {
                    save.items[i] = incoming.items[i];
                }
            } else
            {
                for (int i = 0; i < (int)InventorySlot.COUNT; i++)
                {
                    Core.save.inventory.InventoryItems[i] = incoming.items[i];
                }
            }
            
        }

        public OoTOnlineInventorySync CreateInventory(WrapperInventory save)
        {
            OoTOnlineInventorySync sync = new OoTOnlineInventorySync();
            for (int i = 0; i < (int)InventorySlot.COUNT; i++)
            {
                sync.items[i] = save.InventoryItems[i];
            }

            return sync;
        }

    }
}
