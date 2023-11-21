using OoT.API;
using OoT.API.Enums;

namespace Z64Online;

[BootstrapFilter]
public class OoTOnline : IBootstrapFilter
{
    public static bool isOpen = true;
    public static N64RomHeader? romHeader;
    public static OoT.API.WrapperSaveContext? save;
    public static OoT.API.WrapperGlobalContext? global;
    public static OoT.API.WrapperPlayerContext? player;
    public static Helper? helper;
    public static bool isRando = false;
    
    public static bool DoesLoad(byte[] e)
    {
        romHeader = Z64Online.GetRomHeader(e);

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

    public static void InitOoT()
    {
        if (romHeader?.id == OoT.API.Enums.RomRegions.NTSC_OOT && romHeader?.version == (int)OoT.API.Enums.ROM_VERSIONS.N0)
        {
            Console.WriteLine("OoT 1.0 NTSC");
            OoTVersionPointers.SaveContext = (Ptr)0x8011A5D0;
            OoTVersionPointers.GlobalContext = (Ptr)0x801C84A0;
            OoTVersionPointers.PlayerContext = (Ptr)0x801DAA30;
        }
        else if (romHeader?.id == OoT.API.Enums.RomRegions.DEBUG_OOT)
        {
            Console.WriteLine("OoT DEBUG");
            OoTVersionPointers.SaveContext = (Ptr)0x8015E660;
            OoTVersionPointers.GlobalContext = (Ptr)0x80212020;
            OoTVersionPointers.PlayerContext = (Ptr)0x802245B0;
        }
    }

    [OnInit]
    public static void OnInit(EventPluginsLoaded evt)
    {
        Console.WriteLine("OoTOnline: Init");

        InitOoT();

        save = new OoT.API.WrapperSaveContext((uint)OoTVersionPointers.SaveContext);
        global = new OoT.API.WrapperGlobalContext((uint)OoTVersionPointers.GlobalContext);
        player = new OoT.API.WrapperPlayerContext((uint)OoTVersionPointers.PlayerContext);
        helper = new OoT.API.Helper(save, global, player);
    }

    public static void Destroy()
    {
        Console.WriteLine("Destroy");
    }

    [OnFrame]
    public static void OnTick(EventNewFrame e)
    {
        if (helper.isTitleScreen() || helper.isPaused()) return;
        //ModLoader.API.NetworkSenders.Client.SendPacket();
    }

    [OnViUpdate]
    public static void OnViUpdate(EventNewVi e)
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("OoTO"))
            {

                if (ImGui.BeginMenu("Inventory"))
                {
                    for (int i = 0; i < 24; i++)
                    {
                        string s = ((InventorySlot)i).ToString();
                        string v = save.inventory.InventoryItems[(InventorySlot)i].ToString("X");
                        ImGui.Text(s + ": 0x" + v);
                        ImGui.SameLine();
                        if (ImGui.Button("Set##" + i))
                        {
                            save.inventory.InventoryItems[(InventorySlot)i] = InventoryTest[i];
                        }
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Equipment"))
                {
                    ImGui.Text("Kokiri Sword: " + save.inventory.equipment.kokiriSword);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##KokiriSword"))
                    {
                        save.inventory.equipment.kokiriSword = !save.inventory.equipment.kokiriSword;
                    }

                    ImGui.Text("Master Sword: " + save.inventory.equipment.masterSword);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##MasterSword"))
                    {
                        save.inventory.equipment.masterSword = !save.inventory.equipment.masterSword;
                    }

                    ImGui.Text("Giants Knife: " + save.inventory.equipment.giantsKnife);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##GiantsKnife"))
                    {
                        save.inventory.equipment.giantsKnife = !save.inventory.equipment.giantsKnife;
                    }
                    ImGui.SameLine();
                    ImGui.Text("Biggoron Sword: " + save.inventory.equipment.biggoronSword);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##BiggoronSword"))
                    {
                        save.inventory.equipment.biggoronSword = !save.inventory.equipment.biggoronSword;
                    }

                    ImGui.Text("Deku Shield: " + save.inventory.equipment.dekuShield);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##DekuShield"))
                    {
                        save.inventory.equipment.dekuShield = !save.inventory.equipment.dekuShield;
                    }

                    ImGui.Text("Hylian Shield: " + save.inventory.equipment.hylianShield);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##hylianShield"))
                    {
                        save.inventory.equipment.hylianShield = !save.inventory.equipment.hylianShield;
                    }

                    ImGui.Text("Mirror Sword: " + save.inventory.equipment.mirrorShield);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##mirrorShield"))
                    {
                        save.inventory.equipment.mirrorShield = !save.inventory.equipment.mirrorShield;
                    }

                    ImGui.Text("Kokiri Tunic: " + save.inventory.equipment.kokiriTunic);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##kokiriTunic"))
                    {
                        save.inventory.equipment.kokiriTunic = !save.inventory.equipment.kokiriTunic;
                    }

                    ImGui.Text("Goron Tunic: " + save.inventory.equipment.goronTunic);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##goronTunic"))
                    {
                        save.inventory.equipment.goronTunic = !save.inventory.equipment.goronTunic;
                    }

                    ImGui.Text("Zora Tunic: " + save.inventory.equipment.zoraTunic);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##zoraTunic"))
                    {
                        save.inventory.equipment.zoraTunic = !save.inventory.equipment.zoraTunic;
                    }

                    ImGui.Text("Kokiri Boots: " + save.inventory.equipment.kokiriBoots);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##kokiriBoots"))
                    {
                        save.inventory.equipment.kokiriBoots = !save.inventory.equipment.kokiriBoots;
                    }

                    ImGui.Text("Iron Boots: " + save.inventory.equipment.ironBoots);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##ironBoots"))
                    {
                        save.inventory.equipment.ironBoots = !save.inventory.equipment.ironBoots;
                    }

                    ImGui.Text("Hover Boots: " + save.inventory.equipment.hoverBoots);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##hoverBoots"))
                    {
                        save.inventory.equipment.hoverBoots = !save.inventory.equipment.hoverBoots;
                    }

                    ImGui.EndMenu();
                }
                ImGui.End();
            }
        }
        
    }


    [EventHandler(NetworkEvents.CLIENT_ON_NETWORK_LOBBY_JOIN)]
    public static void OnLobbyJoin(EventClientNetworkLobbyJoined e)
    {

    }

    [ServerNetworkHandler(typeof(Z64O_RupeePacket))]
    public static void Server_RupeePacketGet(Z64O_RupeePacket packet)
    {

    }

    [ClientNetworkHandler(typeof(Z64O_RupeePacket))]
    public static void Client_RupeePacketGet(Z64O_RupeePacket packet)
    {

    }

}