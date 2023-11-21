using ModLoader.API;
using Network.Packets;

namespace Z64Online;

public class Z64O_ScenePacket
{
    public u16 scene { get; set; }
    public int age { get; set; }
    public NetworkPlayer player { get; set; }
    public string lobby { get; set; }

    public Z64O_ScenePacket(u16 scene, int age, string lobby, NetworkPlayer player)
    {
        this.scene = scene;
        this.age = age;
        this.player = player;
        this.lobby = lobby;
    }
}

public class Z64O_DownloadResponsePacket
{
    public OoTOSyncSave save { get; set; }
    public IKeyRing keys { get; set; }
    public string lobby { get; set; }
    public NetworkPlayer player { get; set; }

    public Z64O_DownloadResponsePacket(string lobby)
    {
        this.lobby = lobby;
    }
}

public class Z64O_DownloadRequestPacket
{
    public OoTOSyncSave save { get; set; }
    public string lobby { get; set; }
    public NetworkPlayer player { get; set; }

    public Z64O_DownloadRequestPacket(OoTOSyncSave save, string lobby, NetworkPlayer player)
    {
        this.save = save;
        this.lobby = lobby;
        this.player = player;
    }
}

public class Z64O_UpdateSaveDataPacket
{
    public OoTOSyncSave save { get; set; }
    public int world { get; set; }
    public string lobby { get; set; }
    public NetworkPlayer player { get; set; }

    public Z64O_UpdateSaveDataPacket(OoTOSyncSave save, int world, NetworkPlayer player)
    {
        this.save = save;
        this.world = world;
        this.lobby = lobby;
        this.player = player;
    }
}

public class Z64O_RupeePacket
{
    public u16 rupees { get; set; }
    public string lobby { get; set; }
    public NetworkPlayer player { get; set; }

    public Z64O_RupeePacket(u16 rupees, string lobby, NetworkPlayer player)
    {
        this.rupees = rupees;
        this.lobby = lobby;
        this.player = player;
    }
}
