using Microsoft.Win32.SafeHandles;
using ModLoader.OotR.Sigs;
using Buffer = NodeBuffer.Buffer;

namespace Z64Online.OoTOnline
{
    public class OoTR
    {
        public OoTR_BadSyncData badSyncData = new OoTR_BadSyncData();

        public OoTR(OoTR_BadSyncData badSyncData)
        {
            this.badSyncData = badSyncData;

            badSyncData.BlacklistU32(0x30);
            badSyncData.BlacklistU32(0x31);
            badSyncData.BlacklistU32(0x32);
            badSyncData.BlacklistU32(0x33);
            badSyncData.BlacklistU32(0x34);
            badSyncData.BlacklistU32(0x35);
            badSyncData.BlacklistU32(0x39);
            //badSyncData.BlacklistU32(0xFF);

        }

    }
    public class OoTR_BadSyncData
    {
        public Buffer saveBitMask;

        public OoTR_BadSyncData()
        {
            saveBitMask = Construct();
        }

        private Buffer Construct()
        {
            Buffer mask = new Buffer(0xD90);

            for (int i = 0; i < mask.Size; i += 4)
            {
                mask.WriteU32(i, 0xFFFFFFFF);
            }

            return mask;
        }

        public void BlacklistU32(int offset)
        {
            saveBitMask.WriteU32((int)((offset * 0x1C) + 0x10), 0);
        }
        
    }

}
