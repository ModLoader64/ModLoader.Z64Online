using System.Security.Cryptography;

namespace ModLoader.Z64Online;

[Plugin("Z64Online")]
public class Z64Online : IPlugin
{
    public static Configuration? Configuration { get; set; }
    
    public static void Init()
    {
        Console.WriteLine("Init");
        OoTOnline.Init();
    }

    public static void Destroy()
    {
        Console.WriteLine("Destroy");
        OoTOnline.Destroy();
    }

    [OnFrame]
    public static void OnTick(EventNewFrame e)
    {
        OoTOnline.OnTick(e);
    }

    [OnViUpdate]
    public static void OnViUpdate(EventNewVi e)
    {
        OoTOnline.OnViUpdate(e);
    }

}