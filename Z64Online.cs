
using OoT.API;
using System.Runtime.InteropServices;

namespace Z64Online;

public class LoadedGames
{
    public bool OoT = false;
    public bool OoTDBG = false;
    public bool OoTMM = false;
    public bool MM = false;
}

[Plugin("Z64Online")]
public class Z64Online : IPlugin
{
    public static Configuration? Configuration { get; set; }
    public static N64RomHeader romHeader;
    public static LoadedGames currentGame = new LoadedGames();


    public static void Init()
    {
        Console.WriteLine("Z64Online: Init");
    }

    public static void Destroy()
    {
        Console.WriteLine("Destroy");
    }

    [OnFrame]
    public static void OnTick(EventNewFrame e)
    {
    }

    [OnViUpdate]
    public static void OnViUpdate(EventNewVi e)
    {
    }

}