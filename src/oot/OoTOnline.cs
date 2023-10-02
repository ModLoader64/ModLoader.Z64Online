using OoT.API;

namespace Z64Online;

public class OoTOnline
{
    public static bool isOpen = true;
    public static OoT.API.WrapperSaveContext? save;
    public static OoT.API.WrapperGlobalContext? global;
    public static OoT.API.WrapperPlayerContext? player;
    public static OoT.API.Helper? helper;

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

    public static void Init()
    {
        Console.WriteLine("Init");
        save = new OoT.API.WrapperSaveContext(0x8011A5D0);
        global = new OoT.API.WrapperGlobalContext(0x801C84A0);
        player = new OoT.API.WrapperPlayerContext(0x801DAA30);
        helper = new OoT.API.Helper(save, global, player);
    }

    public static void Destroy()
    {
        Console.WriteLine("Destroy");
    }

    public static void OnTick(EventNewFrame e)
    {
        if (helper.isTitleScreen() || helper.isPaused()) return;

    }

    public static void OnViUpdate(EventNewVi e)
    {
        if (ImGui.Begin("OoTOnline"))
        {
        /*  ImGui.Text($"isTitleScreen: {helper.isTitleScreen()}");
            ImGui.Text($"isSceneNumberValid: 0x{global.sceneID.ToString("X")} - {helper.isSceneNumberValid()}");
            ImGui.Text($"isPaused: {helper.isPaused()}");
            ImGui.Text($"isInterfaceShown: {helper.isInterfaceShown()}");   */

            for (int i = 0; i < 24; i++ )
            {
                string s = ((InventorySlots)i).ToString();
                string v = save.inventory.items[i].ToString("X");
                ImGui.Text(s + ": 0x" + v);
                ImGui.SameLine();
                if (ImGui.Button("Set##"+i))
                {
                    save.inventory.setItemInSlot((InventorySlots)i, InventoryTest[i]);
                }
            }

            

        }
    }


    [EventHandler(NetworkEvents.CLIENT_ON_NETWORK_LOBBY_JOIN)]
    public static void OnLobbyJoin(EventClientNetworkLobbyJoined e)
    {

    }

    /*    [ServerNetworkHandler(typeof(RupeePacket))]
        public static void Server_RupeePacketGet(RupeePacket packet)
        {

        }

        [ClientNetworkHandler(typeof(RupeePacket))]
        public static void Client_RupeePacketGet(RupeePacket packet)
        {

        }*/

}