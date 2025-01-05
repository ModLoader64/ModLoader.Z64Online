using OoT.API;
using OoT;

namespace Z64Online.OoTOnline;

[BootstrapFilter]
public class OoTOnline : IBootstrapFilter
{

    public static N64RomHeader? romHeader;
    public static bool isOpen = true;
    public static OoTR ?rando;
    
    /// <summary>
    /// Determines whether OoTO is constructed by checking the ROM's header to see if it's a version of OoT.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
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