
using OoT.API;
using Spectre.Console;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Z64Online;

[Plugin("Z64Online")]
public class Z64Online : IPlugin
{
    public static Configuration? Configuration { get; set; }
    public static N64RomHeader romHeader;

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

    [EventHandler("OnRomLoaded")]
    public static void OnRomLoaded(EventRomLoaded e)
    {
        N64RomHeader romHeader = GetRomHeader(e.rom);
        Console.WriteLine("Z64Online - OnRomLoaded() ROM Header: " + romHeader.id);
        if (romHeader.id == OoT.API.Enums.RomRegions.NTSC_OOT && romHeader.version == (int)OoT.API.Enums.ROM_VERSIONS.N0)
        {
            Console.WriteLine("OoT 1.0 NTSC");
            OoTVersionPointers.SaveContext = (Ptr)0x8011A5D0;
            OoTVersionPointers.GlobalContext = (Ptr)0x801C84A0;
            OoTVersionPointers.PlayerContext = (Ptr)0x801DAA30;
        } else if (romHeader.id == OoT.API.Enums.RomRegions.DEBUG_OOT)
        {
            Console.WriteLine("OoT DEBUG");
            OoTVersionPointers.SaveContext = (Ptr)0x8015E660;
            OoTVersionPointers.GlobalContext = (Ptr)0x80212020;
            OoTVersionPointers.PlayerContext = (Ptr)0x802245B0;
        } else if (romHeader.id == OoT.API.Enums.RomRegions.NTSC_MM)
        {
            Console.WriteLine("MM 1.0 NTSC");
        }
    }
}