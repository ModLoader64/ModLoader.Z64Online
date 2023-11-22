using OoT.API;
using OoT;

namespace Z64Online.OoTOnline;

[BootstrapFilter]
public class OoTOnline : IBootstrapFilter
{

    public static N64RomHeader? romHeader;
    public static bool isOpen = true;
    public static bool isRando = false;
    
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
        // TODO: Core handles this stuff so this is deprecated, however might have to init more things later via OoTO anyway
    }

    [OnInit]
    public static void OnInit(EventPluginsLoaded evt)
    {
        Console.WriteLine("OoTOnline: Init");
    }

    public static void Destroy()
    {
        Console.WriteLine("Destroy");
    }

    [OnFrame]
    public static void OnTick(EventNewFrame e)
    {
        if (Core.helper.isTitleScreen() || Core.helper.isPaused()) return;
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
                        Core.save.inventory.equipment.ironBoots = !  Core.save.inventory.equipment.ironBoots;
                    }

                    ImGui.Text("Hover Boots: " + Core.save.inventory.equipment.hoverBoots);
                    ImGui.SameLine();
                    if (ImGui.Button("Set##hoverBoots"))
                    {
                        Core.save.inventory.equipment.hoverBoots = !Core.save.inventory.equipment.hoverBoots;
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