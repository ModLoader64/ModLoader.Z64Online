using OoT.API;
using OoT;
using System.Text;

namespace Z64Online.OoTOnline
{
    public class OoTOSaveData
    {
        public string hash = "";
        public OoTOSyncSave CreateSave()
        {
            OoTOSyncSave syncSave = new OoTOSyncSave();
            syncSave.inventory = CreateInventory(Core.save);
            hash = ModLoader.API.Utils.GetHashSHA1(ModLoader.API.Utils.ObjectToByteArray(syncSave));
            return syncSave;
        }

        public void ApplySave(OoTOSyncSave incoming)
        {
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
                forceOverrideSave(incoming);
            }
        }

        public void MergeInventory(OoTOnlineInventorySync incoming, OoTOnlineInventorySync save)
        {
            MergeItems(incoming.items, save.items);
            MergeEquipment(incoming.equipment, save.equipment);
            MergeQuestStatus(incoming.questStatus, save.questStatus);
        }

        public void MergeItems(InventoryItem[] incoming, InventoryItem[] save)
        {
            // TODO: Individual logic for some items & potential downgrading 
            for (int i = 0; i < (int)InventorySlot.COUNT; i++)
            {
                if (save[i] != incoming[i])
                {
                    // Do not downgrade longshot w/ hookshot
                    if (i == (int)InventorySlot.HOOKSHOT)
                    {
                        if (incoming[i] == InventoryItem.HOOKSHOT || incoming[i] == InventoryItem.LONGSHOT)
                        {
                            if (save[i] != InventoryItem.LONGSHOT)
                            {
                                save[i] = incoming[i];
                            }
                        }
                    }

                    // Do not downgrade any items to nothing
                    if (incoming[i] != InventoryItem.NONE)
                    {
                        save[i] = incoming[i];
                    }
                    else
                    {

                    }

                }
            }
        }

        public void MergeEquipment(OoTOnlineEquipmentSync incoming, OoTOnlineEquipmentSync save)
        {
            // TODO: Make this less stupid
            if (incoming.kokiriSword) save.kokiriSword = incoming.kokiriSword;
            if (incoming.masterSword) save.masterSword = incoming.masterSword;
            if (incoming.giantsKnife) save.giantsKnife = incoming.giantsKnife;
            if (incoming.biggoronSword) save.biggoronSword = incoming.biggoronSword;
            if (incoming.dekuShield) save.dekuShield = incoming.dekuShield;
            if (incoming.hylianShield) save.hylianShield = incoming.hylianShield;
            if (incoming.mirrorShield) save.mirrorShield = incoming.mirrorShield;
            if (incoming.kokiriTunic) save.kokiriTunic = incoming.kokiriTunic;
            if (incoming.goronTunic) save.goronTunic = incoming.goronTunic;
            if (incoming.zoraTunic) save.zoraTunic = incoming.zoraTunic;
            if (incoming.kokiriBoots) save.kokiriBoots = incoming.kokiriBoots;
            if (incoming.ironBoots) save.ironBoots = incoming.ironBoots;
            if (incoming.hoverBoots) save.hoverBoots = incoming.hoverBoots;

        }

        public void MergeQuestStatus(OoTOnlineQuestStatusSync incoming, OoTOnlineQuestStatusSync save)
        {
            // TODO: Make this less stupid
            if (incoming.kokiriEmerald) save.kokiriEmerald = incoming.kokiriEmerald;
            if (incoming.goronRuby) save.goronRuby = incoming.goronRuby;
            if (incoming.zoraSapphire) save.zoraSapphire = incoming.zoraSapphire;
            if (incoming.medallionForest) save.medallionForest = incoming.medallionForest;
            if (incoming.medallionFire) save.medallionFire = incoming.medallionFire;
            if (incoming.medallionWater) save.medallionWater = incoming.medallionWater;
            if (incoming.medallionSpirit) save.medallionSpirit = incoming.medallionSpirit;
            if (incoming.medallionShadow) save.medallionShadow = incoming.medallionShadow;
            if (incoming.medallionLight) save.medallionLight = incoming.medallionLight;
            if (incoming.stoneAgony) save.stoneAgony = incoming.stoneAgony;
            if (incoming.gerudoCard) save.gerudoCard = incoming.gerudoCard;
            if (incoming.hasGoldSkull) save.hasGoldSkull = incoming.hasGoldSkull;

            if (incoming.songLullaby) save.kokiriEmerald = incoming.kokiriEmerald;
            if (incoming.songEpona) save.goronRuby = incoming.goronRuby;
            if (incoming.songSaria) save.hasGoldSkull = incoming.hasGoldSkull;
            if (incoming.songTime) save.medallionForest = incoming.medallionForest;
            if (incoming.songStorms) save.medallionFire = incoming.medallionFire;
            if (incoming.songSun) save.zoraSapphire = incoming.zoraSapphire;
            if (incoming.songMinuet) save.medallionWater = incoming.medallionWater;
            if (incoming.songBolero) save.medallionSpirit = incoming.medallionSpirit;
            if (incoming.songSerenade) save.medallionShadow = incoming.medallionShadow;
            if (incoming.songNocturne) save.medallionLight = incoming.medallionLight;
            if (incoming.songRequiem) save.stoneAgony = incoming.stoneAgony;
            if (incoming.songPrelude) save.gerudoCard = incoming.gerudoCard;


            // Health & Magic

            if (incoming.healthCapacity > save.healthCapacity)
            {
                save.healthCapacity = incoming.healthCapacity;
            }

            if ( incoming.heartPieces >  save.heartPieces && incoming.heartPieces < 4) // Make sure not to apply a 4th piece
            {
                save.heartPieces = incoming.heartPieces;
            } else if (incoming.heartPieces == 0 && save.heartPieces == 3) { // Heart Pieces reset back to 0 due to a 4th piece making a new container
                save.heartPieces = incoming.heartPieces;
            } else if (incoming.heartPieces == 4) // Just in case the 4th piece is actually sent 
            {
                save.heartPieces = 0;
            }

            if (incoming.magicLevel > save.magicLevel)
            {
                save.magicLevel = incoming.magicLevel;
            }
        }

        public void forceOverrideSave(OoTOSyncSave incoming, OoTOSyncSave save = null)
        {
            // Write to local OoTO Items if supplied, otherwise write to save file directly
            if (save != null)
            {
                OverrideItems(incoming.inventory.items, save.inventory.items);
                OverrideEquipment(incoming.inventory.equipment, save.inventory.equipment);
                OverrideQuestStatus(incoming.inventory.questStatus, save.inventory.questStatus);
            }
            else
            {
                OverrideItems(incoming.inventory.items);
                OverrideEquipment(incoming.inventory.equipment);
                OverrideQuestStatus(incoming.inventory.questStatus);
            }
        }

        public void OverrideItems(InventoryItem[] incoming, InventoryItem[] save = null)
        {

            // Write to local OoTO Items if supplied, otherwise write to save file directly
            if (save != null)
            {
                for (int i = 0; i < (int)InventorySlot.COUNT; i++)
                {
                    save[i] = incoming[i];
                }
            }
            else
            {
                for (int i = 0; i < (int)InventorySlot.COUNT; i++)
                {
                    Core.save.inventory.InventoryItems[i] = incoming[i];
                }
            }
        }

        public void OverrideEquipment(OoTOnlineEquipmentSync incoming, OoTOnlineEquipmentSync save = null)
        {
            // TODO: Make this less stupid
            if (save != null)
            {
                save.kokiriSword = incoming.kokiriSword;
                save.masterSword = incoming.masterSword;
                save.giantsKnife = incoming.giantsKnife;
                save.biggoronSword = incoming.biggoronSword;
                save.dekuShield = incoming.dekuShield;
                save.hylianShield = incoming.hylianShield;
                save.mirrorShield = incoming.mirrorShield;
                save.kokiriTunic = incoming.kokiriTunic;
                save.goronTunic = incoming.goronTunic;
                save.zoraTunic = incoming.zoraTunic;
                save.kokiriBoots = incoming.kokiriBoots;
                save.ironBoots = incoming.ironBoots;
                save.hoverBoots = incoming.hoverBoots;
            }
            else
            {
                Core.save.inventory.equipment.kokiriSword = incoming.kokiriSword;
                Core.save.inventory.equipment.masterSword = incoming.masterSword;
                Core.save.inventory.equipment.giantsKnife = incoming.giantsKnife;
                Core.save.inventory.equipment.biggoronSword = incoming.biggoronSword;
                Core.save.inventory.equipment.dekuShield = incoming.dekuShield;
                Core.save.inventory.equipment.hylianShield = incoming.hylianShield;
                Core.save.inventory.equipment.mirrorShield = incoming.mirrorShield;
                Core.save.inventory.equipment.kokiriTunic = incoming.kokiriTunic;
                Core.save.inventory.equipment.goronTunic = incoming.goronTunic;
                Core.save.inventory.equipment.zoraTunic = incoming.zoraTunic;
                Core.save.inventory.equipment.kokiriBoots = incoming.kokiriBoots;
                Core.save.inventory.equipment.ironBoots = incoming.ironBoots;
                Core.save.inventory.equipment.hoverBoots = incoming.hoverBoots;
            }
        }

        public void OverrideQuestStatus(OoTOnlineQuestStatusSync incoming, OoTOnlineQuestStatusSync save = null)
        {
            // TODO: Make this less stupid
            if (save != null)
            {

                save.songLullaby = incoming.songLullaby;
                save.songEpona = incoming.songEpona;
                save.songTime = incoming.songTime;
                save.songSun = incoming.songSun;
                save.songStorms = incoming.songStorms;
                save.songSaria = incoming.songSaria;
                save.songMinuet = incoming.songMinuet;
                save.songBolero = incoming.songBolero;
                save.songSerenade = incoming.songSerenade;
                save.songNocturne = incoming.songNocturne;
                save.songRequiem = incoming.songRequiem;
                save.songPrelude = incoming.songPrelude;

                save.kokiriEmerald = incoming.kokiriEmerald;
                save.goronRuby = incoming.goronRuby;
                save.zoraSapphire = incoming.zoraSapphire;
                save.medallionForest = incoming.medallionForest;
                save.medallionFire = incoming.medallionFire;
                save.medallionWater = incoming.medallionWater;
                save.medallionSpirit = incoming.medallionSpirit;
                save.medallionShadow = incoming.medallionShadow;
                save.medallionLight = incoming.medallionLight;
                save.stoneAgony = incoming.stoneAgony;
                save.gerudoCard = incoming.gerudoCard;
                save.hasGoldSkull = incoming.hasGoldSkull;

                save.heartPieces = incoming.heartPieces;
                save.magicLevel = incoming.magicLevel;
                save.healthCapacity = incoming.healthCapacity;
            }
            else
            {

                Core.save.inventory.questStatus.songLullaby = incoming.songLullaby;
                Core.save.inventory.questStatus.songEpona = incoming.songEpona;
                Core.save.inventory.questStatus.songTime = incoming.songTime;
                Core.save.inventory.questStatus.songSun = incoming.songSun;
                Core.save.inventory.questStatus.songStorms = incoming.songStorms;
                Core.save.inventory.questStatus.songSaria = incoming.songSaria;
                Core.save.inventory.questStatus.songMinuet = incoming.songMinuet;
                Core.save.inventory.questStatus.songBolero = incoming.songBolero;
                Core.save.inventory.questStatus.songSerenade = incoming.songSerenade;
                Core.save.inventory.questStatus.songNocturne = incoming.songNocturne;
                Core.save.inventory.questStatus.songRequiem = incoming.songRequiem;
                Core.save.inventory.questStatus.songPrelude = incoming.songPrelude;

                Core.save.inventory.questStatus.kokiriEmerald = incoming.kokiriEmerald;
                Core.save.inventory.questStatus.goronRuby = incoming.goronRuby;
                Core.save.inventory.questStatus.zoraSapphire = incoming.zoraSapphire;
                Core.save.inventory.questStatus.medallionForest = incoming.medallionForest;
                Core.save.inventory.questStatus.medallionFire = incoming.medallionFire;
                Core.save.inventory.questStatus.medallionWater = incoming.medallionWater;
                Core.save.inventory.questStatus.medallionSpirit = incoming.medallionSpirit;
                Core.save.inventory.questStatus.medallionShadow = incoming.medallionShadow;
                Core.save.inventory.questStatus.medallionLight = incoming.medallionLight;
                Core.save.inventory.questStatus.stoneAgony = incoming.stoneAgony;
                Core.save.inventory.questStatus.gerudoCard = incoming.gerudoCard;
                Core.save.inventory.questStatus.hasGoldSkull = incoming.hasGoldSkull;

                Core.save.inventory.questStatus.heartPieces = incoming.heartPieces;
                Core.save.magicLevel = incoming.magicLevel;
                Core.save.healthCapacity = incoming.healthCapacity;
            }
        }


        public OoTOnlineInventorySync CreateInventory(WrapperSaveContext save)
        {
            OoTOnlineInventorySync sync = new OoTOnlineInventorySync();

            for (int i = 0; i < (int)InventorySlot.COUNT; i++)
            {
                sync.items[i] = save.inventory.InventoryItems[i];
            }

            sync.equipment.kokiriSword = save.inventory.equipment.kokiriSword;
            sync.equipment.masterSword = save.inventory.equipment.masterSword;
            sync.equipment.giantsKnife = save.inventory.equipment.giantsKnife;
            sync.equipment.biggoronSword = save.inventory.equipment.biggoronSword;
            sync.equipment.dekuShield = save.inventory.equipment.dekuShield;
            sync.equipment.hylianShield = save.inventory.equipment.hylianShield;
            sync.equipment.mirrorShield = save.inventory.equipment.mirrorShield;
            sync.equipment.kokiriTunic = save.inventory.equipment.kokiriTunic;
            sync.equipment.goronTunic = save.inventory.equipment.goronTunic;
            sync.equipment.zoraTunic = save.inventory.equipment.zoraTunic;
            sync.equipment.kokiriBoots = save.inventory.equipment.kokiriBoots;
            sync.equipment.ironBoots = save.inventory.equipment.ironBoots;
            sync.equipment.hoverBoots = save.inventory.equipment.hoverBoots;

            sync.questStatus.songLullaby = save.inventory.questStatus.songLullaby;
            sync.questStatus.songEpona = save.inventory.questStatus.songEpona;
            sync.questStatus.songTime = save.inventory.questStatus.songTime;
            sync.questStatus.songSun = save.inventory.questStatus.songSun;
            sync.questStatus.songStorms = save.inventory.questStatus.songStorms;
            sync.questStatus.songSaria = save.inventory.questStatus.songSaria;
            sync.questStatus.songMinuet = save.inventory.questStatus.songMinuet;
            sync.questStatus.songBolero = save.inventory.questStatus.songBolero;
            sync.questStatus.songSerenade = save.inventory.questStatus.songSerenade;
            sync.questStatus.songNocturne = save.inventory.questStatus.songNocturne;
            sync.questStatus.songRequiem = save.inventory.questStatus.songRequiem;
            sync.questStatus.songPrelude = save.inventory.questStatus.songPrelude;


            sync.questStatus.kokiriEmerald = save.inventory.questStatus.kokiriEmerald;
            sync.questStatus.goronRuby = save.inventory.questStatus.goronRuby;
            sync.questStatus.zoraSapphire = save.inventory.questStatus.zoraSapphire;
            sync.questStatus.medallionForest = save.inventory.questStatus.medallionForest;
            sync.questStatus.medallionFire = save.inventory.questStatus.medallionFire;
            sync.questStatus.medallionWater = save.inventory.questStatus.medallionWater;
            sync.questStatus.medallionSpirit = save.inventory.questStatus.medallionSpirit;
            sync.questStatus.medallionShadow = save.inventory.questStatus.medallionShadow;
            sync.questStatus.medallionLight = save.inventory.questStatus.medallionLight;
            sync.questStatus.stoneAgony = save.inventory.questStatus.stoneAgony;
            sync.questStatus.gerudoCard = save.inventory.questStatus.gerudoCard;
            sync.questStatus.hasGoldSkull = save.inventory.questStatus.hasGoldSkull;
            sync.questStatus.heartPieces = save.inventory.questStatus.heartPieces;
            sync.questStatus.magicLevel = save.magicLevel;
            sync.questStatus.healthCapacity = save.healthCapacity;

            return sync;
        }

    }
}
