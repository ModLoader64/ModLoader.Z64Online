namespace ModLoader.Z64Online;

public class RupeePacket
{
    public short rupees { get; set; }
    public int uuid { get; set; }

    public RupeePacket (short newRupees, int newUuID) {
        rupees = newRupees;
        uuid = newUuID;
    }
}
