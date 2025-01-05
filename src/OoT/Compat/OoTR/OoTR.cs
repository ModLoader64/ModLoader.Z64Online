using Microsoft.Win32.SafeHandles;
using ModLoader.OotR.Sigs;
using OoT;
using OoT.API;
using System.Formats.Tar;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using Buffer = NodeBuffer.Buffer;

namespace Z64Online.OoTOnline
{
    public class OoTR
    {
        public OoTR_BadSyncData badSyncData = new OoTR_BadSyncData();
        public OoTR_PotsanityHelper potsanityHelper = new OoTR_PotsanityHelper();
        public OoTR_TriforceHuntHelper triforceHelper = new OoTR_TriforceHuntHelper();

        public OoTR(OoTR_BadSyncData badSyncData, OoTR_PotsanityHelper potsanityHelper, OoTR_TriforceHuntHelper triforceHelper)
        {
            this.badSyncData = badSyncData;
            this.potsanityHelper = potsanityHelper;
            this.triforceHelper = triforceHelper;
            badSyncData.BlacklistU32(0x30);
            badSyncData.BlacklistU32(0x31);
            badSyncData.BlacklistU32(0x32);
            badSyncData.BlacklistU32(0x33);
            badSyncData.BlacklistU32(0x34);
            badSyncData.BlacklistU32(0x35);
            badSyncData.BlacklistU32(0x39);
            badSyncData.BlacklistU32(0x48);
            badSyncData.BlacklistU32(0x49);
        }

        public void CheckSigs(byte[] rom)
        {
            List<Signature> sigs = new List<Signature>();
            List<Signature> sigs2 = new List<Signature>();

            Span<byte> romBuf = new Span<byte>(rom);
            string header = ModLoader.API.Utils.GetHashSHA1(romBuf.Slice(0x20, 0x20).ToArray()).ToUpper();

            var files = Directory.GetFiles("./mods/Z64Online/OoTR_Sigs/");
            if (files.Length == 0)
            {
                Console.WriteLine("No OoTR Sigs found.");
                return;
            }
            foreach (var sig in files)
            {
                var stream = new FileStream(Path.GetFullPath(sig), FileMode.Open);
                Span<byte> buf = new byte[stream.Length];
                stream.Read(buf.ToArray(), 0, buf.Length);
                stream.Close();

                Console.WriteLine(OotRSigs.collectible_override_flags.ToString("X"));
                Console.WriteLine(OotRSigs.num_override_flags.ToString("X"));

                Buffer sigBuf = new Buffer(buf.ToArray());
                Buffer _header = new Buffer(buf.Slice(0x0, 0x20).ToArray());
                uint items = _header.ReadU32BE(0x0C);
                uint items2 = _header.ReadU32BE(0x10);
                string rom_header_hash = Encoding.Default.GetString(buf.Slice(0x20, 0x30));


                Console.WriteLine($"buf.Length 0x{buf.Length.ToString("X")}");
                Console.WriteLine($"sigBuf.Size 0x{sigBuf.Size.ToString("X")}");
                Console.WriteLine($"_header.Size 0x{_header.Size.ToString("X")}");
                Console.WriteLine($"items 0x{items.ToString("X")}");
                Console.WriteLine($"items2 0x{items2.ToString("X")}");
                Console.WriteLine($"header {header}");
                Console.WriteLine($"rom_header_hash {rom_header_hash}");
                if (rom_header_hash != header) return;

                for (int i = 0; i < items; i++)
                {
                    int s1 = sigBuf.Read32(0x0);
                    int o1 = sigBuf.Read32(0x4);
                    int s2 = sigBuf.Read32(0x8);
                    int o2 = sigBuf.Read32(0xC);

                    Signature signature = new Signature();
                    signature.offset_name = o1;
                    signature.size_name = s1;
                    signature.addr = s2;
                    sigs.Add(signature);
                }

                for (int i = 0; i < items2; i++)
                {
                    int s1 = sigBuf.Read32(0x0);
                    int o1 = sigBuf.Read32(0x4);
                    int s2 = sigBuf.Read32(0x8);
                    int o2 = sigBuf.Read32(0xC);

                    Signature signature = new Signature();
                    signature.offset_name = o1;
                    signature.offset_addr = o2;
                    signature.size_name = s1;
                    signature.addr = s2;
                    sigs2.Add(signature);
                }

                for (int i = 0; i < sigs.Count; i++)
                {
                    sigs[i].name = System.Text.Encoding.ASCII.GetString(new Buffer(sigBuf.Read32(sigs[i].offset_name))._buffer);
                }
                for (int i = 0; i < sigs2.Count; i++)
                {
                    sigs2[i].name = System.Text.Encoding.ASCII.GetString(new Buffer(sigBuf.Read32(sigs2[i].offset_name))._buffer);
                }

                if (sigs.Count > 0 && sigs2.Count > 0)
                {
                    Console.WriteLine($"Loaded symbols:");
                    Console.WriteLine($"{sigs}");
                    Console.WriteLine($"{sigs2}");
                } else
                {

                }
            }
        }

    }
    
    public class Signature
    {
        public string name = "";
        public int size_name = -1;
        public int offset_name = -1;
        public int offset_addr = -1;
        public int addr = -1;
    }

    public class OoTR_PotsanityHelper
    {
        public static bool HasPotsanity()
        {
            return Memory.RAM.ReadU32(OotRSigs.collectible_override_flags) != 0;
        }

        public static int GetFlagArraySize()
        {
            return Memory.RAM.ReadU16(OotRSigs.num_override_flags) + 100;
        }

        public static Buffer GetFlagBuffer()
        {
            if(!HasPotsanity()) { return new Buffer(0); }
            uint ptr = Memory.RAM.ReadU32(OotRSigs.collectible_override_flags);
            Buffer buf = new Buffer(GetFlagArraySize());
            for(int i = 0; i < buf.Size;  i++)
            {
                buf.WriteU8(i, Memory.RAM.ReadU8((uint)(ptr + i)));
            }
            return buf;
        }

        public static void SetFlagsBuffer(Buffer buf)
        {
            if (!HasPotsanity()) return;
            uint ptr = Memory.RAM.ReadU32(OotRSigs.collectible_override_flags);
            if(buf.Size == GetFlagArraySize())
            {
                for(int i = 0; i < buf.Size; i++)
                {
                    Memory.RAM.WriteU8((uint)(ptr + i), buf.ReadU8(i));
                }
            }
        }
    }

    public class OoTR_TriforceHuntHelper
    {
        public static u32 GetTriforcePieces()
        {
            if (RomFlags.isRando)
            {
                u32 val = Core.save.GetSceneFlagsFromIndex(0x48).unk;
                return val;
            }
            else
            {
                return 0;
            }
        } 

        public static void SetTriforcePieces(u32 pieces)
        {
            if (RomFlags.isRando)
            {
                WrapperSavedSceneFlags flags = Core.save.GetSceneFlagsFromIndex(0x48);
                flags.unk = pieces;
                Core.save.SetSceneFlagsToIndex(0x48, flags);
            }
        }

        public static void IncrementTriforcePieces()
        {
            if (RomFlags.isRando)
            {
                WrapperSavedSceneFlags flags = Core.save.GetSceneFlagsFromIndex(0x48);
                flags.unk += 1;
                Core.save.SetSceneFlagsToIndex(0x48, flags);
            }
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
