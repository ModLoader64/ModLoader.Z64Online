using OoT.API;
using OoT;
using System.Reflection.PortableExecutable;
using OoT.API.Enums;

namespace Z64Online.OoTOnline
{
    [BootstrapFilter]
    public class OoTOnlineGuiHandler : IBootstrapFilter
    {
        public static N64RomHeader? romHeader;

        public static InventoryItem[] InventoryTest =
        {
            InventoryItem.DEKU_STICKS ,
            InventoryItem.DEKU_NUTS,
            InventoryItem.BOMBS,
            InventoryItem.FAIRY_BOW,
            InventoryItem.FIRE_ARROWS,
            InventoryItem.DINS_FIRE,
            InventoryItem.FAIRY_SLINGSHOT,
            InventoryItem.FAIRY_OCARINA,
            InventoryItem.BOMBCHUS,
            InventoryItem.BOMBCHUS,
            InventoryItem.HOOKSHOT,
            InventoryItem.ICE_ARROWS,
            InventoryItem.FARORES_WIND,
            InventoryItem.BOOMERANG,
            InventoryItem.LENS_OF_TRUTH,
            InventoryItem.MAGIC_BEANS,
            InventoryItem.MEGATON_HAMMER,
            InventoryItem.LIGHT_ARROWS,
            InventoryItem.NAYRUS_LOVE,
            InventoryItem.RED_POTION,
            InventoryItem.RED_POTION,
            InventoryItem.RED_POTION,
            InventoryItem.RED_POTION,
            InventoryItem.COJIRO ,
            InventoryItem.POCKET_EGG ,
        };


        public static bool DoesLoad(byte[] e)
        {
            romHeader = Core.GetRomHeader(e);

            if (romHeader?.id == OoT.API.Enums.RomRegions.NTSC_OOT)
            {
                Z64Online.currentGame.OoT = true;
            }
            else if (romHeader?.id == OoT.API.Enums.RomRegions.DEBUG_OOT)
            {
                Z64Online.currentGame.OoTDBG = true;
            }
            return Z64Online.currentGame.OoT || Z64Online.currentGame.OoTDBG;
        }

        [OnInit]
        public static void OnInit(EventPluginsLoaded evt)
        {
            Console.WriteLine("OoTO ImGui: Init");
        }

        [OnFrame]
        public static void OnTick(EventNewFrame e)
        {

        }

        [OnViUpdate]
        public static void OnViUpdate(EventNewVi e)
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("OoTO"))
                {

                    if (ImGui.BeginMenu("Status"))
                    {
                        ImGui.Text("Max Health: " + Core.save.healthCapacity);
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Increase##MaxHealthUp", ImGuiNET.ImGuiDir.Up))
                        {
                            Core.save.healthCapacity += 1;
                            Core.save.health = Core.save.healthCapacity;
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Decrease##MaxHealthDown", ImGuiNET.ImGuiDir.Down))
                        {
                            if (Core.save.healthCapacity > 1)
                            {
                                Core.save.healthCapacity -= 1;
                            }
                            Core.save.health = Core.save.healthCapacity;
                        }

                        ImGui.Text("Heart Pieces: " + Core.save.inventory.questStatus.heartPieces);
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Increase##HeartPieceUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.questStatus.heartPieces)
                            {
                                case 0:
                                    Core.save.inventory.questStatus.heartPieces = 1;
                                    break;
                                case 1:
                                    Core.save.inventory.questStatus.heartPieces = 2;
                                    break;
                                case 2:
                                    Core.save.inventory.questStatus.heartPieces = 3;
                                    break;
                                case 3:
                                    Core.save.healthCapacity += 1;
                                    Core.save.health = Core.save.healthCapacity;
                                    Core.save.inventory.questStatus.heartPieces = 0;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Decrease##HeartPieceDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.questStatus.heartPieces)
                            {
                                case 1:
                                    Core.save.inventory.questStatus.heartPieces = 0;
                                    break;
                                case 2:
                                    Core.save.inventory.questStatus.heartPieces = 1;
                                    break;
                                case 3:
                                    Core.save.inventory.questStatus.heartPieces = 2;
                                    break;
                            }
                        }

                        ImGui.Text("Double Defense: " + Core.save.isDoubleDefenseAcquired);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##DoubleDefense"))
                        {
                            Core.save.isDoubleDefenseAcquired = !Core.save.isDoubleDefenseAcquired;
                        }

                        ImGui.Text("Max Magic: " + Core.save.magicLevel);
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Increase##MaxMagicUp", ImGuiNET.ImGuiDir.Up))
                        {
                            if (Core.save.magicLevel >= 2) return;
                            Core.save.magicLevel++;
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Decrease##MaxMagicDown", ImGuiNET.ImGuiDir.Down))
                        {
                            if (Core.save.magicLevel <= 0) return;
                            Core.save.magicLevel--;
                        }
                        ImGui.EndMenu();
                    }


                    if (ImGui.BeginMenu("Inventory"))
                    {
                        for (int i = 0; i < 24; i++)
                        {
                            string s = ((InventorySlot)i).ToString();
                            string v = Core.save.inventory.InventoryItems[(InventorySlot)i].ToString("X");
                            ImGui.Text(s + ": 0x" + v);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##" + i))
                            {
                                Core.save.inventory.InventoryItems[(InventorySlot)i] = InventoryTest[i];
                            }
                        }
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Dungeon"))
                    {
                        for (int i = 0; i < 0x14; i++)
                        {
                            if (ImGui.BeginMenu($"{(VANILLA_DUNGEON_ITEM_INDEXES)i}"))
                            {
                                ImGui.Text($"Keys: {Core.save.inventory.dungeon.keys[i].count}");
                                ImGui.SameLine();
                                if(ImGui.ArrowButton($"KeysUp##{i}", ImGuiNET.ImGuiDir.Up))
                                {
                                    DungeonKeys key = Core.save.inventory.dungeon.keys[i];
                                    key.count += 1;
                                    Core.save.inventory.dungeon.keys[i] = key;
                                }
                                ImGui.SameLine();
                                if (ImGui.ArrowButton($"KeysDown##{i}", ImGuiNET.ImGuiDir.Down))
                                {
                                    DungeonKeys key = Core.save.inventory.dungeon.keys[i];
                                    key.count -= 1;
                                    Core.save.inventory.dungeon.keys[i] = key;
                                }

                                ImGui.Text($"Boss Key: {Core.save.inventory.dungeon.items[i].bossKey}");
                                ImGui.SameLine();
                                if (ImGui.Button($"Set##BossKey{i}"))
                                {
                                    DungeonItems items = Core.save.inventory.dungeon.items[i];
                                    items.bossKey = !items.bossKey;
                                    Core.save.inventory.dungeon.items[i] = items;
                                }

                                ImGui.Text($"Compass: {Core.save.inventory.dungeon.items[i].compass}");
                                ImGui.SameLine();
                                if (ImGui.Button($"Set##Compass{i}"))
                                {
                                    DungeonItems items = Core.save.inventory.dungeon.items[i];
                                    items.compass = !items.compass;
                                    Core.save.inventory.dungeon.items[i] = items;
                                }

                                ImGui.Text($"Map: {Core.save.inventory.dungeon.items[i].map}");
                                ImGui.SameLine();
                                if (ImGui.Button($"Set##Map{i}"))
                                {
                                    DungeonItems items = Core.save.inventory.dungeon.items[i];
                                    items.map = !items.map;
                                    Core.save.inventory.dungeon.items[i] = items;
                                }
                                ImGui.EndMenu();
                            }
                        }
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Equipment"))
                    {
                        ImGui.Text("Kokiri Sword: " + Core.save.inventory.equipment.kokiriSword);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##KokiriSword"))
                        {
                            Core.save.inventory.equipment.kokiriSword = !Core.save.inventory.equipment.kokiriSword;
                        }

                        ImGui.Text("Master Sword: " + Core.save.inventory.equipment.masterSword);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##MasterSword"))
                        {
                            Core.save.inventory.equipment.masterSword = !Core.save.inventory.equipment.masterSword;
                        }

                        ImGui.Text("Giants Knife: " + Core.save.inventory.equipment.giantsKnife);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##GiantsKnife"))
                        {
                            Core.save.inventory.equipment.giantsKnife = !Core.save.inventory.equipment.giantsKnife;
                        }
                        ImGui.SameLine();
                        ImGui.Text("Biggoron Sword: " + Core.save.inventory.equipment.biggoronSword);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##BiggoronSword"))
                        {
                            Core.save.inventory.equipment.biggoronSword = !Core.save.inventory.equipment.biggoronSword;
                        }

                        ImGui.Text("Deku Shield: " + Core.save.inventory.equipment.dekuShield);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##DekuShield"))
                        {
                            Core.save.inventory.equipment.dekuShield = !Core.save.inventory.equipment.dekuShield;
                        }

                        ImGui.Text("Hylian Shield: " + Core.save.inventory.equipment.hylianShield);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##hylianShield"))
                        {
                            Core.save.inventory.equipment.hylianShield = !Core.save.inventory.equipment.hylianShield;
                        }

                        ImGui.Text("Mirror Sword: " + Core.save.inventory.equipment.mirrorShield);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##mirrorShield"))
                        {
                            Core.save.inventory.equipment.mirrorShield = !Core.save.inventory.equipment.mirrorShield;
                        }

                        ImGui.Text("Kokiri Tunic: " + Core.save.inventory.equipment.kokiriTunic);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##kokiriTunic"))
                        {
                            Core.save.inventory.equipment.kokiriTunic = !Core.save.inventory.equipment.kokiriTunic;
                        }

                        ImGui.Text("Goron Tunic: " + Core.save.inventory.equipment.goronTunic);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##goronTunic"))
                        {
                            Core.save.inventory.equipment.goronTunic = !Core.save.inventory.equipment.goronTunic;
                        }

                        ImGui.Text("Zora Tunic: " + Core.save.inventory.equipment.zoraTunic);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##zoraTunic"))
                        {
                            Core.save.inventory.equipment.zoraTunic = !Core.save.inventory.equipment.zoraTunic;
                        }

                        ImGui.Text("Kokiri Boots: " + Core.save.inventory.equipment.kokiriBoots);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##kokiriBoots"))
                        {
                            Core.save.inventory.equipment.kokiriBoots = !Core.save.inventory.equipment.kokiriBoots;
                        }

                        ImGui.Text("Iron Boots: " + Core.save.inventory.equipment.ironBoots);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##ironBoots"))
                        {
                            Core.save.inventory.equipment.ironBoots = !Core.save.inventory.equipment.ironBoots;
                        }

                        ImGui.Text("Hover Boots: " + Core.save.inventory.equipment.hoverBoots);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##hoverBoots"))
                        {
                            Core.save.inventory.equipment.hoverBoots = !Core.save.inventory.equipment.hoverBoots;
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Upgrades"))
                    {
                        ImGui.Text(String.Format("{0, 0} | {1, 5}", "Deku Stick Capacity: ", Core.save.inventory.upgrades.dekuStickCapacity.ToString()));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##DekuStickCapacityUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.dekuStickCapacity)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.None:
                                    Core.save.inventory.upgrades.dekuStickCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.dekuStickCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.dekuStickCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Max;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##DekuStickCapacityDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.dekuStickCapacity)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.dekuStickCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.None;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.dekuStickCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Max:
                                    Core.save.inventory.upgrades.dekuStickCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                            }
                        }

                        ImGui.Text(String.Format("{0, 0} | {1, 7}", "Deku Nut Capacity: ", Core.save.inventory.upgrades.dekuNutCapacity.ToString()));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##DekuNutCapacityUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.dekuNutCapacity)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.None:
                                    Core.save.inventory.upgrades.dekuNutCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.dekuNutCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.dekuNutCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Max;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##DekuNutCapacityDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.dekuNutCapacity)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.dekuNutCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.None;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.dekuNutCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Max:
                                    Core.save.inventory.upgrades.dekuNutCapacity = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                            }
                        }

                        ImGui.Text(String.Format("{0, 0} | {1, 5}", "Bullet Bag Capacity: ", Core.save.inventory.upgrades.bulletBag.ToString()));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##BulletBagUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.bulletBag)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.None:
                                    Core.save.inventory.upgrades.bulletBag = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.bulletBag = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.bulletBag = OoT.API.Enums.Capacity.AmmoUpgrade.Max;
                                    break;
                            }
                            Console.WriteLine(Core.save.inventory.upgrades.bulletBag.ToString());
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##BulletBagDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.bulletBag)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.bulletBag = OoT.API.Enums.Capacity.AmmoUpgrade.None;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.bulletBag = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Max:
                                    Core.save.inventory.upgrades.bulletBag = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                            }
                            Console.WriteLine(Core.save.inventory.upgrades.bulletBag.ToString());
                        }

                        ImGui.Text(String.Format("{0, 0} | {1, 9}", "Quiver Capacity: ", Core.save.inventory.upgrades.quiver.ToString()));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##QuiverUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.quiver)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.None:
                                    Core.save.inventory.upgrades.quiver = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.quiver = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.quiver = OoT.API.Enums.Capacity.AmmoUpgrade.Max;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##QuiverDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.quiver)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.quiver = OoT.API.Enums.Capacity.AmmoUpgrade.None;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.quiver = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Max:
                                    Core.save.inventory.upgrades.quiver = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                            }
                        }

                        ImGui.Text(String.Format("{0, 0} | {1, 7}", "Bomb Bag Capacity: ", Core.save.inventory.upgrades.bombBag.ToString()));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##BombBagUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.bombBag)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.None:
                                    Core.save.inventory.upgrades.bombBag = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.bombBag = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.bombBag = OoT.API.Enums.Capacity.AmmoUpgrade.Max;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##BombBagDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.bombBag)
                            {
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Basic:
                                    Core.save.inventory.upgrades.bombBag = OoT.API.Enums.Capacity.AmmoUpgrade.None;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade:
                                    Core.save.inventory.upgrades.bombBag = OoT.API.Enums.Capacity.AmmoUpgrade.Basic;
                                    break;
                                case OoT.API.Enums.Capacity.AmmoUpgrade.Max:
                                    Core.save.inventory.upgrades.bombBag = OoT.API.Enums.Capacity.AmmoUpgrade.Upgrade;
                                    break;
                            }
                        }

                        ImGui.Text(String.Format("{0, 0} | {1, 16}", "Strength: ", Core.save.inventory.upgrades.strength.ToString()));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##StrengthUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.strength)
                            {
                                case OoT.API.Enums.Capacity.Strength.None:
                                    Core.save.inventory.upgrades.strength = OoT.API.Enums.Capacity.Strength.Silver;
                                    break;
                                case OoT.API.Enums.Capacity.Strength.Silver:
                                    Core.save.inventory.upgrades.strength = OoT.API.Enums.Capacity.Strength.Golden;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##StrengthDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.strength)
                            {
                                case OoT.API.Enums.Capacity.Strength.Silver:
                                    Core.save.inventory.upgrades.strength = OoT.API.Enums.Capacity.Strength.None;
                                    break;
                                case OoT.API.Enums.Capacity.Strength.Golden:
                                    Core.save.inventory.upgrades.strength = OoT.API.Enums.Capacity.Strength.Silver;
                                    break;
                            }
                        }

                        ImGui.Text(String.Format("{0, 0} | {1, 19}", "Scale: ", Core.save.inventory.upgrades.scale));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##ScaleUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.scale)
                            {
                                case OoT.API.Enums.Capacity.Scales.None:
                                    Core.save.inventory.upgrades.scale = OoT.API.Enums.Capacity.Scales.Silver;
                                    break;
                                case OoT.API.Enums.Capacity.Scales.Silver:
                                    Core.save.inventory.upgrades.scale = OoT.API.Enums.Capacity.Scales.Golden;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##ScaleDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.scale)
                            {
                                case OoT.API.Enums.Capacity.Scales.Silver:
                                    Core.save.inventory.upgrades.scale = OoT.API.Enums.Capacity.Scales.None;
                                    break;
                                case OoT.API.Enums.Capacity.Scales.Golden:
                                    Core.save.inventory.upgrades.scale = OoT.API.Enums.Capacity.Scales.Silver;
                                    break;
                            }
                        }

                        ImGui.Text(String.Format("{0, 0} | {1, 18}", "Wallet: ", Core.save.inventory.upgrades.wallet.ToString()));
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##WalletUp", ImGuiNET.ImGuiDir.Up))
                        {
                            switch (Core.save.inventory.upgrades.wallet)
                            {
                                case OoT.API.Enums.Capacity.Wallet.None:
                                    Core.save.inventory.upgrades.wallet = OoT.API.Enums.Capacity.Wallet.Adult;
                                    break;
                                case OoT.API.Enums.Capacity.Wallet.Adult:
                                    Core.save.inventory.upgrades.wallet = OoT.API.Enums.Capacity.Wallet.Giant;
                                    break;
                                case OoT.API.Enums.Capacity.Wallet.Giant:
                                    Core.save.inventory.upgrades.wallet = OoT.API.Enums.Capacity.Wallet.Tycoon;
                                    break;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.ArrowButton("Set##WalletDown", ImGuiNET.ImGuiDir.Down))
                        {
                            switch (Core.save.inventory.upgrades.wallet)
                            {
                                case OoT.API.Enums.Capacity.Wallet.Adult:
                                    Core.save.inventory.upgrades.wallet = OoT.API.Enums.Capacity.Wallet.None;
                                    break;
                                case OoT.API.Enums.Capacity.Wallet.Giant:
                                    Core.save.inventory.upgrades.wallet = OoT.API.Enums.Capacity.Wallet.Adult;
                                    break;
                                case OoT.API.Enums.Capacity.Wallet.Tycoon:
                                    Core.save.inventory.upgrades.wallet = OoT.API.Enums.Capacity.Wallet.Giant;
                                    break;
                            }
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("QuestStatus"))
                    {
                        if (ImGui.BeginMenu("Stones"))
                        {
                            ImGui.Text("Kokiri Emerald: " + Core.save.inventory.questStatus.kokiriEmerald);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##KokiriEmerald"))
                            {
                                Core.save.inventory.questStatus.kokiriEmerald = !Core.save.inventory.questStatus.kokiriEmerald;
                            }

                            ImGui.Text("Goron Ruby: " + Core.save.inventory.questStatus.goronRuby);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##GoronRuby"))
                            {
                                Core.save.inventory.questStatus.goronRuby = !Core.save.inventory.questStatus.goronRuby;
                            }

                            ImGui.Text("Zora Sapphire: " + Core.save.inventory.questStatus.zoraSapphire);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##ZoraSapphire"))
                            {
                                Core.save.inventory.questStatus.zoraSapphire = !Core.save.inventory.questStatus.zoraSapphire;
                            }
                            ImGui.EndMenu();
                        }

                        if (ImGui.BeginMenu("Medallions"))
                        {
                            ImGui.Text("Forest Medallion: " + Core.save.inventory.questStatus.medallionForest);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##ForestMedallion"))
                            {
                                Core.save.inventory.questStatus.medallionForest = !Core.save.inventory.questStatus.medallionForest;
                            }

                            ImGui.Text("Fire Medallion: " + Core.save.inventory.questStatus.medallionFire);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##FireMedallion"))
                            {
                                Core.save.inventory.questStatus.medallionFire = !Core.save.inventory.questStatus.medallionFire;
                            }

                            ImGui.Text("Water Medallion: " + Core.save.inventory.questStatus.medallionWater);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##WaterMedallion"))
                            {
                                Core.save.inventory.questStatus.medallionWater = !Core.save.inventory.questStatus.medallionWater;
                            }

                            ImGui.Text("Spirit Medallion: " + Core.save.inventory.questStatus.medallionSpirit);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##SpiritMedallion"))
                            {
                                Core.save.inventory.questStatus.medallionSpirit = !Core.save.inventory.questStatus.medallionSpirit;
                            }

                            ImGui.Text("Shadow Medallion: " + Core.save.inventory.questStatus.medallionShadow);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##ShadowMedallion"))
                            {
                                Core.save.inventory.questStatus.medallionShadow = !Core.save.inventory.questStatus.medallionShadow;
                            }

                            ImGui.Text("light Medallion: " + Core.save.inventory.questStatus.medallionLight);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##LightMedallion"))
                            {
                                Core.save.inventory.questStatus.medallionLight = !Core.save.inventory.questStatus.medallionLight;
                            }
                            ImGui.EndMenu();
                        }

                        if (ImGui.BeginMenu("Songs"))
                        {
                            ImGui.Text("Zelda's Lullaby: " + Core.save.inventory.questStatus.songLullaby);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##ZeldasLullaby"))
                            {
                                Core.save.inventory.questStatus.songLullaby = !Core.save.inventory.questStatus.songLullaby;
                            }

                            ImGui.Text("Epona Song: " + Core.save.inventory.questStatus.songEpona);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##EponaSong"))
                            {
                                Core.save.inventory.questStatus.songEpona = !Core.save.inventory.questStatus.songEpona;
                            }

                            ImGui.Text("Saria's Song: " + Core.save.inventory.questStatus.songSaria);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##SariaSong"))
                            {
                                Core.save.inventory.questStatus.songSaria = !Core.save.inventory.questStatus.songSaria;
                            }

                            ImGui.Text("Song of Time: " + Core.save.inventory.questStatus.songTime);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##SongOfTime"))
                            {
                                Core.save.inventory.questStatus.songTime = !Core.save.inventory.questStatus.songTime;
                            }

                            ImGui.Text("Song of Storms: " + Core.save.inventory.questStatus.songStorms);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##SongStorms"))
                            {
                                Core.save.inventory.questStatus.songStorms = !Core.save.inventory.questStatus.songStorms;
                            }

                            ImGui.Text("Sun Song: " + Core.save.inventory.questStatus.songSun);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##SunSong"))
                            {
                                Core.save.inventory.questStatus.songSun = !Core.save.inventory.questStatus.songSun;
                            }

                            ImGui.Text("Minuet of Forest: " + Core.save.inventory.questStatus.songMinuet);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##MinuetForest"))
                            {
                                Core.save.inventory.questStatus.songMinuet = !Core.save.inventory.questStatus.songMinuet;
                            }

                            ImGui.Text("Bolero of Fire: " + Core.save.inventory.questStatus.songBolero);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##BoleroFire"))
                            {
                                Core.save.inventory.questStatus.songBolero = !Core.save.inventory.questStatus.songBolero;
                            }

                            ImGui.Text("Serenade of Water: " + Core.save.inventory.questStatus.songSerenade);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##SerenadeWater"))
                            {
                                Core.save.inventory.questStatus.songSerenade = !Core.save.inventory.questStatus.songSerenade;
                            }

                            ImGui.Text("Nocturne of Shadow: " + Core.save.inventory.questStatus.songNocturne);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##NocturneShadow"))
                            {
                                Core.save.inventory.questStatus.songNocturne = !Core.save.inventory.questStatus.songNocturne;
                            }

                            ImGui.Text("Requiem of Spirit:" + Core.save.inventory.questStatus.songRequiem);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##RequiemSpirit"))
                            {
                                Core.save.inventory.questStatus.songRequiem = !Core.save.inventory.questStatus.songRequiem;
                            }

                            ImGui.Text("Prelude of Light:" + Core.save.inventory.questStatus.songPrelude);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##PreludeLight"))
                            {
                                Core.save.inventory.questStatus.songPrelude = !Core.save.inventory.questStatus.songPrelude;
                            }
                            ImGui.EndMenu();

                        }

                        if (ImGui.BeginMenu("Misc"))
                        {
                            ImGui.Text("Stone of Agony: " + Core.save.inventory.questStatus.stoneAgony);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##StoneAgony"))
                            {
                                Core.save.inventory.questStatus.stoneAgony = !Core.save.inventory.questStatus.stoneAgony;
                            }

                            ImGui.Text("Gerudo Card: " + Core.save.inventory.questStatus.gerudoCard);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##GerudoCard"))
                            {
                                Core.save.inventory.questStatus.gerudoCard = !Core.save.inventory.questStatus.gerudoCard;
                            }

                            ImGui.Text("Skull Counter Enabled: " + Core.save.inventory.questStatus.hasGoldSkull);
                            ImGui.SameLine();
                            if (ImGui.Button("Set##SkullEnable"))
                            {
                                Core.save.inventory.questStatus.hasGoldSkull = !Core.save.inventory.questStatus.hasGoldSkull;
                            }

                            ImGui.Text("Gold Skulltulas: " + Core.save.inventory.gsTokens);
                            ImGui.SameLine();
                            if (ImGui.ArrowButton("Add##GoldSkulltulasUp", ImGuiNET.ImGuiDir.Up))
                            {
                                if (Core.save.inventory.gsTokens < 100)
                                {
                                    Core.save.inventory.questStatus.hasGoldSkull = true;
                                    Core.save.inventory.gsTokens++;
                                }
                            }
                            ImGui.SameLine();
                            if (ImGui.ArrowButton("Add##GoldSkulltulasDown", ImGuiNET.ImGuiDir.Down))
                            {
                                if (Core.save.inventory.gsTokens > 0)
                                {
                                    Core.save.inventory.gsTokens--;
                                }
                                else Core.save.inventory.questStatus.hasGoldSkull = false;
                            }

                            ImGui.EndMenu();
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.End();
                }
            }

        }
    }
}
