
using OoT.API;
using OoT.API.Enums;
using Spectre.Console;
using System.Numerics;
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
    
    public static N64RomHeader GetRomHeader(byte[] rom)
    {
        var header = new u8[0x40];
        Array.Copy(rom, header, 0x40);
        GCHandle pinnedBytes = GCHandle.Alloc(header, GCHandleType.Pinned);
        N64RomHeader romHeader = (N64RomHeader)Marshal.PtrToStructure(pinnedBytes.AddrOfPinnedObject(), typeof(N64RomHeader));
        pinnedBytes.Free();
        return romHeader;
    }

}