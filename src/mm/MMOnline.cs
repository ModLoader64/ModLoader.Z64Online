using OoT.API;

namespace Z64Online;

[BootstrapFilter]
public class MMOnline : IBootstrapFilter
{
    public static bool isOpen = true;

    public static bool DoesLoad(byte[] e)
    {
        N64RomHeader romHeader = Z64Online.GetRomHeader(e);
        return romHeader.id == OoT.API.Enums.RomRegions.NTSC_MM;
    }

    [OnInit]
    public static void OnInit(EventPluginsLoaded evt)
    {
        Console.WriteLine("MMOnline: Init");
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
        if (ImGui.Begin("MMOnline"))
        {

        }
    }
}