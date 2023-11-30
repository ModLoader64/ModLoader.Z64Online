using OoT.API;
using OoT;
using Buffer = NodeBuffer.Buffer;
using Newtonsoft.Json;
using System.Text;

namespace Z64Online.OoTOnline
{
    public class OoTOSaveData
    {
        public string hash = "";

        private List<InventoryItem> USELESS_MASK = new List<InventoryItem> { InventoryItem.GERUDO_MASK, InventoryItem.ZORA_MASK, InventoryItem.GORON_MASK };
        private List<InventoryItem> ALL_MASKS = new List<InventoryItem> { InventoryItem.KEATON_MASK, InventoryItem.SKULL_MASK, InventoryItem.SPOOKY_MASK, InventoryItem.BUNNY_HOOD, InventoryItem.MASK_OF_TRUTH, InventoryItem.GERUDO_MASK, InventoryItem.ZORA_MASK, InventoryItem.GORON_MASK };

        public OoTOSyncSave CreateSave()
        {
            OoTOSyncSave syncSave = new OoTOSyncSave();
            syncSave.data = CreateInventory(Core.save);
            hash = ModLoader.API.Utils.GetHashSHA1(ModLoader.API.Utils.ObjectToByteArray(syncSave));
            return syncSave;
        }

        public void Apply(OoTOSyncSave incoming)
        {
            ForceOverrideSave(incoming);
        }

        public void Merge(OoTOSyncSave incoming, OoTOSyncSave save = null)
        {
            // Write to local OoTO Save if supplied, otherwise write to save file directly
            if (save != null)
            {
                MergeSave(incoming.data, save.data);
            }
            else
            {
                ForceOverrideSave(incoming);
            }
        }

        public void MergeSave(OoTOnlineSaveSync incoming, OoTOnlineSaveSync save)
        {
            MergeItems(incoming, save);
            MergeEquipment(incoming.equipment, save.equipment);
            MergeQuestStatus(incoming.questStatus, save.questStatus);
            MergeDungeonData(incoming.dungeon, save.dungeon);
            MergeFlags(incoming.flags, save.flags);
        }

        public void MergeItems(OoTOnlineSaveSync incoming, OoTOnlineSaveSync save)
        {
            // TODO: Individual logic for some items & potential downgrading 
            for (int i = 0; i < (int)InventorySlot.COUNT; i++)
            {
                // Do not downgrade any items to nothing
                if (incoming.items[i] != InventoryItem.NONE)
                {
                    if (save.items[i] != incoming.items[i])
                    {
                        // Do not downgrade longshot w/ hookshot
                        if (i == (int)InventorySlot.HOOKSHOT)
                        {
                            if (incoming.items[i] == InventoryItem.HOOKSHOT || incoming.items[i] == InventoryItem.LONGSHOT)
                            {
                                if (save.items[i] != InventoryItem.LONGSHOT)
                                {
                                    save.items[i] = incoming.items[i];
                                }
                            }
                        }
                        // TODO: Make this less prone to overwriting
                        else if (i == (int)InventorySlot.ADULT_TRADE_ITEM)
                        {
                            if (incoming.items[i] != InventoryItem.SOLD_OUT)
                            {
                                if (save.items[i] == InventoryItem.NONE)
                                {
                                    save.items[i] = incoming.items[i];
                                }
                                else if (incoming.items[i] > save.items[i])
                                {
                                    save.items[i] = incoming.items[i];
                                }
                            }
                        }
                        else if (i == (int)InventorySlot.CHILD_TRADE_ITEM)
                        {
                            if (incoming.items[i] != InventoryItem.SOLD_OUT)
                            {
                                if (save.items[i] == InventoryItem.NONE)
                                {
                                    save.items[i] = incoming.items[i];
                                } 
                                else if (incoming.items[i] > save.items[i])
                                {
                                    save.items[i] = incoming.items[i];
                                }
                            }
                        }
                        else
                        {
                            save.items[i] = incoming.items[i];
                        }
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

            if (incoming.dekuNutCapacity > save.dekuNutCapacity)
            {
                save.dekuNutCapacity = incoming.dekuNutCapacity;
            }
            if (incoming.dekuStickCapacity > save.dekuStickCapacity)
            {
                save.dekuStickCapacity = incoming.dekuStickCapacity;
            }
            if (incoming.quiver > save.quiver)
            {
                save.quiver = incoming.quiver;
            }
            if (incoming.bombBag > save.bombBag)
            {
                save.bombBag = incoming.bombBag;
            }
            if (incoming.bulletBag > save.bulletBag)
            {
                save.bulletBag = incoming.bulletBag;
            }
            if (incoming.wallet > save.wallet)
            {
                save.wallet = incoming.wallet;
            }
            if (incoming.strength > save.strength)
            {
                save.strength = incoming.strength;
            }
            if (incoming.scale > save.scale)
            {
                save.scale = incoming.scale;
            }
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

            if (incoming.songLullaby) save.songLullaby = incoming.songLullaby;
            if (incoming.songEpona) save.songEpona = incoming.songEpona;
            if (incoming.songSaria) save.songSaria = incoming.songSaria;
            if (incoming.songTime) save.songTime = incoming.songTime;
            if (incoming.songStorms) save.songStorms = incoming.songStorms;
            if (incoming.songSun) save.songSun = incoming.songSun;
            if (incoming.songMinuet) save.songMinuet = incoming.songMinuet;
            if (incoming.songBolero) save.songBolero = incoming.songBolero;
            if (incoming.songSerenade) save.songSerenade = incoming.songSerenade;
            if (incoming.songNocturne) save.songNocturne = incoming.songNocturne;
            if (incoming.songRequiem) save.songRequiem = incoming.songRequiem;
            if (incoming.songPrelude) save.songPrelude = incoming.songPrelude;

            if (incoming.hasDoubleDefense) save.hasDoubleDefense = incoming.hasDoubleDefense;

            // Health & Magic

            if (incoming.healthCapacity > save.healthCapacity && save.healthCapacity < 20)
            {
                save.healthCapacity = incoming.healthCapacity;
            }
            else if (save.healthCapacity > 20)
            {
                save.healthCapacity = 20;
            }

            if (incoming.heartPieces > save.heartPieces && incoming.heartPieces < 4) // Make sure not to apply a 4th piece
            {
                save.heartPieces = incoming.heartPieces;
            }
            else if (incoming.heartPieces == 0 && save.heartPieces == 3)
            { // Heart Pieces reset back to 0 due to a 4th piece making a new container
                save.heartPieces = incoming.heartPieces;
            }
            else if (incoming.heartPieces >= 4) // Just in case the 4th piece is actually sent 
            {
                save.heartPieces = 0;
            }

            if (incoming.magicLevel > save.magicLevel)
            {
                save.magicLevel = incoming.magicLevel;
            }

            if (incoming.gsTokens > save.gsTokens && incoming.gsTokens <= 100)
            {
                save.gsTokens = incoming.gsTokens;
            }
        }

        public void MergeDungeonData(OoTOnlineDungeonSync incoming, OoTOnlineDungeonSync save)
        {
            // TODO: Do this better, maybe keep keys handled exclusively via network handlers?
            for (u8 i = 0; i < save.keys.Length; i++)
            {
                if (save.keys[i].count < incoming.keys[i].count)
                {
                    save.keys[i] = incoming.keys[i];
                }
                else if (incoming.keys[i].count < save.keys[i].count && (save.keys[i].count - incoming.keys[i].count) == 1) // In case key is used, prone to error though.
                {
                    save.keys[i] = incoming.keys[i];
                }
            }

            for (u8 i = 0; i < save.items.Length; i++)
            {
                if (save.items[i] != incoming.items[i])
                {
                    if (incoming.items[i].bossKey) save.items[i].bossKey = incoming.items[i].bossKey;
                    if (incoming.items[i].compass) save.items[i].compass = incoming.items[i].compass;
                    if (incoming.items[i].map) save.items[i].map = incoming.items[i].map;
                }
            }
        }

        public void MergeFlags(OoTOnlineFlagSync incoming, OoTOnlineFlagSync save)
        {
            for (int i = 0; i < incoming.sceneFlags.Length; i++)
            {
                if (!save.sceneFlags[i].Equals(incoming.sceneFlags[i]))
                {
                    Console.WriteLine($"Scene[{i}] Flag Update: {JsonConvert.SerializeObject(save.sceneFlags[i])} -> {JsonConvert.SerializeObject(incoming.sceneFlags[i])}");

                    save.sceneFlags[i].chest |= incoming.sceneFlags[i].chest;
                    save.sceneFlags[i].clear |= incoming.sceneFlags[i].clear;
                    save.sceneFlags[i].collect |= incoming.sceneFlags[i].collect;
                    save.sceneFlags[i].unk |= incoming.sceneFlags[i].unk;
                    save.sceneFlags[i].rooms |= incoming.sceneFlags[i].rooms;
                    save.sceneFlags[i].floors |= incoming.sceneFlags[i].floors;

                    // Switch Flag specific cases
                    switch (i)
                    {
                        case 3: // Forest Temple
                            save.sceneFlags[i].swch &= 0xF7; // Poe Sisters Cutscene & Elevator Off Switch.
                            break;
                        case 5:  // Water Temple
                            save.sceneFlags[i].swch = save.sceneFlags[i].swch; // Water Level, don't bother syncing for now.
                            break;
                        default:
                            save.sceneFlags[i].swch |= incoming.sceneFlags[i].swch;
                            break;
                    }

                }
            }

            for (int i = 0; i < incoming.eventChkInf.Size; i++)
            {
                u8 s_flg = save.eventChkInf.ReadU8(i);
                u8 i_flg = incoming.eventChkInf.ReadU8(i);
                if (s_flg != i_flg)
                {
                    u8 result = (u8)(s_flg | i_flg);
                    Console.WriteLine($"eventChkInf Flag {i} : 0x{s_flg.ToString("X")} | 0x{i_flg.ToString("X")} : 0x{result.ToString("X")}");
                    save.eventChkInf.WriteU8(i, result);
                }
            }

            for (int i = 0; i < incoming.infTable.Size; i++)
            {
                u8 s_flg = save.infTable.ReadU8(i);
                u8 i_flg = incoming.infTable.ReadU8(i);
                if (s_flg != i_flg)
                {
                    u8 result = (u8)(s_flg | i_flg);
                    Console.WriteLine($"infTable Flag {i} : 0x{s_flg.ToString("X")} | 0x{i_flg.ToString("X")} : 0x{result.ToString("X")}");
                    save.infTable.WriteU8(i, result);
                }
            }

            for (int i = 0; i < incoming.itemGetInf.Size; i++)
            {
                u8 s_flg = save.itemGetInf.ReadU8(i);
                u8 i_flg = incoming.itemGetInf.ReadU8(i);
                if (s_flg != i_flg)
                {
                    u8 result = (u8)(s_flg | i_flg);
                    Console.WriteLine($"itemGetInf Flag {i} : 0x{s_flg.ToString("X")} | 0x{i_flg.ToString("X")} : 0x{result.ToString("X")}");
                    save.itemGetInf.WriteU8(i, result);
                }
            }

            for (int i = 0; i < incoming.gsFlags.Size; i++)
            {
                u8 s_flg = save.gsFlags.ReadU8(i);
                u8 i_flg = incoming.gsFlags.ReadU8(i);
                if (s_flg != i_flg)
                {
                    u8 result = (u8)(s_flg | i_flg);
                    Console.WriteLine($"gsFlags Flag {i} : 0x{s_flg.ToString("X")} | 0x{i_flg.ToString("X")} : 0x{result.ToString("X")}");
                    save.gsFlags.WriteU8(i, result);
                }
            }
        }

        public void ForceOverrideSave(OoTOSyncSave incoming, OoTOSyncSave save = null)
        {
            // Write to local OoTO Items if supplied, otherwise write to save file directly
            if (save != null)
            {
                OverrideItems(incoming.data, save.data);
                OverrideEquipment(incoming.data.equipment, save.data.equipment);
                OverrideQuestStatus(incoming.data.questStatus, save.data.questStatus);
                OverrideDungeonData(incoming.data.dungeon, save.data.dungeon);
                OverrideFlags(incoming.data.flags, save.data.flags);
            }
            else
            {
                OverrideItems(incoming.data);
                OverrideEquipment(incoming.data.equipment);
                OverrideQuestStatus(incoming.data.questStatus);
                OverrideDungeonData(incoming.data.dungeon);
                OverrideFlags(incoming.data.flags);
            }
        }

        public void OverrideItems(OoTOnlineSaveSync incoming, OoTOnlineSaveSync save = null)
        {
            // Write to local OoTO Items if supplied, otherwise write to save file directly
            if (save != null)
            {
                for (int i = 0; i < (int)InventorySlot.COUNT; i++)
                {
                    save.items[i] = incoming.items[i];
                }
            }
            else
            {
                for (int i = 0; i < (int)InventorySlot.COUNT; i++)
                {
                    Core.save.inventory.InventoryItems[i] = incoming.items[i];
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

                save.bombBag = incoming.bombBag;
                save.bulletBag = incoming.bulletBag;
                save.wallet = incoming.wallet;
                save.quiver = incoming.quiver;
                save.dekuNutCapacity = incoming.dekuNutCapacity;
                save.dekuStickCapacity = incoming.dekuStickCapacity;
                save.strength = incoming.strength;
                save.scale = incoming.scale;
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

                Core.save.inventory.upgrades.bombBag = incoming.bombBag;
                Core.save.inventory.upgrades.bulletBag = incoming.bulletBag;
                Core.save.inventory.upgrades.wallet = incoming.wallet;
                Core.save.inventory.upgrades.quiver = incoming.quiver;
                Core.save.inventory.upgrades.dekuNutCapacity = incoming.dekuNutCapacity;
                Core.save.inventory.upgrades.dekuStickCapacity = incoming.dekuStickCapacity;
                Core.save.inventory.upgrades.strength = incoming.strength;
                Core.save.inventory.upgrades.scale = incoming.scale;
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
                save.gsTokens = incoming.gsTokens;
                save.hasDoubleDefense = incoming.hasDoubleDefense;
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
                Core.save.inventory.questStatus.gsTokens = incoming.gsTokens;
                Core.save.isDoubleDefenseAcquired = incoming.hasDoubleDefense;
                Core.save.inventory.gsTokens = incoming.gsTokens;
            }
        }

        public void OverrideDungeonData(OoTOnlineDungeonSync incoming, OoTOnlineDungeonSync save = null)
        {
            if (save != null)
            {
                save.keys = incoming.keys;
                save.items = incoming.items;
            }
            else
            {
                Core.save.inventory.dungeon.keys.SetKeysBuffer(incoming.keys);
                Core.save.inventory.dungeon.items.SetItemsBuffer(incoming.items);
            }
        }

        public void OverrideFlags(OoTOnlineFlagSync incoming, OoTOnlineFlagSync save = null)
        {
            if (save != null)
            {
                for (int i = 0; i < incoming.sceneFlags.Length; i++)
                {
                    save.sceneFlags[i].chest |= incoming.sceneFlags[i].chest;
                    save.sceneFlags[i].swch |= incoming.sceneFlags[i].swch;
                    save.sceneFlags[i].clear |= incoming.sceneFlags[i].clear;
                    save.sceneFlags[i].collect |= incoming.sceneFlags[i].collect;
                    save.sceneFlags[i].unk |= incoming.sceneFlags[i].unk;
                    save.sceneFlags[i].rooms |= incoming.sceneFlags[i].rooms;
                    save.sceneFlags[i].floors |= incoming.sceneFlags[i].floors;
                }

                for (int i = 0; i < incoming.eventChkInf.Size; i++)
                {
                    u8 val = save.eventChkInf.ReadU8(i);
                    u8 val2 = incoming.eventChkInf.ReadU8(i);
                    u8 result = val |= val2;
                    save.eventChkInf.WriteU8(i, result);
                }
                for (int i = 0; i < incoming.infTable.Size; i++)
                {
                    u8 val = save.infTable.ReadU8(i);
                    u8 val2 = incoming.infTable.ReadU8(i);
                    u8 result = val |= val2;
                    save.infTable.WriteU8(i, result);
                }
                for (int i = 0; i < incoming.itemGetInf.Size; i++)
                {
                    u8 val = save.itemGetInf.ReadU8(i);
                    u8 val2 = incoming.itemGetInf.ReadU8(i);
                    u8 result = val |= val2;
                    save.itemGetInf.WriteU8(i, result);
                }
                for (int i = 0; i < incoming.gsFlags.Size; i++)
                {
                    u8 val = save.gsFlags.ReadU8(i);
                    u8 val2 = incoming.gsFlags.ReadU8(i);
                    u8 result = val |= val2;
                    save.gsFlags.WriteU8(i, result);
                }
            }
            else
            {
                for (int i = 0; i < incoming.sceneFlags.Length; i++)
                {
                    Core.save.sceneFlags[i].chest |= incoming.sceneFlags[i].chest;
                    Core.save.sceneFlags[i].swch |= incoming.sceneFlags[i].swch;
                    Core.save.sceneFlags[i].clear |= incoming.sceneFlags[i].clear;
                    Core.save.sceneFlags[i].collect |= incoming.sceneFlags[i].collect;
                    Core.save.sceneFlags[i].unk |= incoming.sceneFlags[i].unk;
                    Core.save.sceneFlags[i].rooms |= incoming.sceneFlags[i].rooms;
                    Core.save.sceneFlags[i].floors |= incoming.sceneFlags[i].floors;
                }

                Buffer _eventChkInf = Core.save.eventChkInf;
                Buffer _infTable = Core.save.infTable;
                Buffer _itemGetInf = Core.save.itemGetInf;
                Buffer _gsFlags = Core.save.gsFlags;

                for (int i = 0; i < incoming.eventChkInf.Size; i++)
                {
                    u8 val = _eventChkInf.ReadU8(i);
                    u8 val2 = incoming.eventChkInf.ReadU8(i);
                    u8 result = val |= val2;
                    _eventChkInf.WriteU8(i, result);
                }
                for (int i = 0; i < incoming.infTable.Size; i++)
                {
                    u8 val = _infTable.ReadU8(i);
                    u8 val2 = incoming.infTable.ReadU8(i);
                    u8 result = val |= val2;
                    _infTable.WriteU8(i, result);
                }
                for (int i = 0; i < incoming.itemGetInf.Size; i++)
                {
                    u8 val = _itemGetInf.ReadU8(i);
                    u8 val2 = incoming.itemGetInf.ReadU8(i);
                    u8 result = val |= val2;
                    _itemGetInf.WriteU8(i, result);
                }
                for (int i = 0; i < incoming.gsFlags.Size; i++)
                {
                    u8 val = _gsFlags.ReadU8(i);
                    u8 val2 = incoming.gsFlags.ReadU8(i);
                    u8 result = val |= val2;
                    _gsFlags.WriteU8(i, result);
                }

                Core.save.eventChkInf = _eventChkInf;
                Core.save.infTable = _infTable;
                Core.save.itemGetInf = _itemGetInf;
                Core.save.gsFlags = _gsFlags;
            }
        }

        public OoTOnlineSaveSync CreateInventory(WrapperSaveContext save)
        {
            OoTOnlineSaveSync sync = new OoTOnlineSaveSync();

            for (int i = 0; i < (int)InventorySlot.COUNT; i++)
            {
                sync.items[i] = save.inventory.InventoryItems[i];
            }

            for (int i = 0; i < sync.dungeon.keys.Length; i++)
            {
                sync.dungeon.items[i] = save.inventory.dungeon.items[i];
                sync.dungeon.keys[i] = save.inventory.dungeon.keys[i];
            }

            Buffer sceneFlags = save.GetSceneFlagsRaw();

            if (RomFlags.isRando)
            {
                for (int i = 0; i < OoTOnline.rando.badSyncData.saveBitMask.Size; i++)
                {
                    sceneFlags.WriteU8(i, (u8)(sceneFlags.ReadU8(i) & OoTOnline.rando.badSyncData.saveBitMask.ReadU8(i)));
                }
            }

            for (int i = 0; i < sync.flags.sceneFlags.Length; i++)
            {
                sync.flags.sceneFlags[i] = new SceneFlagStruct(
                    sceneFlags.ReadU32((i * 0x1C) + 0x0),
                    sceneFlags.ReadU32((i * 0x1C) + 0x4),
                    sceneFlags.ReadU32((i * 0x1C) + 0x8),
                    sceneFlags.ReadU32((i * 0x1C) + 0xC),
                    sceneFlags.ReadU32((i * 0x1C) + 0x10),
                    sceneFlags.ReadU32((i * 0x1C) + 0x14),
                    sceneFlags.ReadU32((i * 0x1C) + 0x18));
            }

            sync.flags.eventChkInf = save.eventChkInf;

            // Mask some event flags so they don't get sent to anyone
            for (int i = 0; i < save.eventChkInf.Size; i++)
            {
                u8 value = save.eventChkInf.ReadU8(i);

                if (i == 2)
                {
                    sync.flags.eventChkInf.WriteU8(i, (u8)(value & 0xF7)); // Rented Horse from Ingo
                }
                if (i == 13)
                {
                    sync.flags.eventChkInf.WriteU8(i, (u8)(value & 0xDF)); // Played Song of Storms in Kakariko Widmill
                }

            }

            sync.flags.itemGetInf = save.itemGetInf;
            sync.flags.infTable = save.infTable;

            // Mask some more flags so they don't get sent
            for (int i = 0; i < save.infTable.Size; i++)
            {
                u8 value = save.infTable.ReadU8(i);

                if (i == 15)
                {
                    sync.flags.infTable.WriteU8(i, (u8)(value & 0xFD)); // Hyrule Castle Gate 
                }
            }

            sync.flags.gsFlags = save.gsFlags;

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

            sync.equipment.bombBag = save.inventory.upgrades.bombBag;
            sync.equipment.bulletBag = save.inventory.upgrades.bulletBag;
            sync.equipment.wallet = save.inventory.upgrades.wallet;
            sync.equipment.quiver = save.inventory.upgrades.quiver;
            sync.equipment.dekuNutCapacity = save.inventory.upgrades.dekuNutCapacity;
            sync.equipment.dekuStickCapacity = save.inventory.upgrades.dekuStickCapacity;
            sync.equipment.strength = save.inventory.upgrades.strength;
            sync.equipment.scale = save.inventory.upgrades.scale;

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
            sync.questStatus.gsTokens = save.inventory.gsTokens;
            sync.questStatus.hasDoubleDefense = save.isDoubleDefenseAcquired;

            return sync;
        }

    }
}
