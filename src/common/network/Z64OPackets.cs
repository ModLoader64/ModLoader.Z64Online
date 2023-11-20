namespace Z64Online;

public class Z64O_ScenePacket
{
    public u8 scene { get; set; }
    public u8 age { get; set; }

    public Z64O_ScenePacket(u8 scene, u8 age)
    {
        this.scene = scene;
        this.age = age;
    }
}

public class Z64O_UpdateSaveDataPacket
{
    public OoTOSyncSave save { get; set; }
    public int world { get; set; }

    public Z64O_UpdateSaveDataPacket(OoTOSyncSave save, int world)
    {
        this.save = save;
        this.world = world;
    }
}

public class Z64O_RupeePacket
{
    public u16 rupees { get; set; }

    public Z64O_RupeePacket(u16 newRupees)
    {
        rupees = newRupees;
    }
}
