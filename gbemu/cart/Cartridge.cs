namespace Gbemu.cart;

using System;
using System.IO;

// 定义ROM头结构体
public struct RomHeader
{
    public byte[] Entry; // 入口点代码，长度为4
    public byte[] Logo; // Logo数据，长度为0x30
    public string Title; // 游戏标题，长度为16
    public UInt16 NewLicCode; // 新的许可证代码
    public byte SgbFlag; // SGB标志
    public byte Type; // 游戏类型
    public byte RomSize; // ROM大小
    public byte RamSize; // RAM大小
    public byte DestCode; // 目标代码
    public byte LicCode; // 许可证代码
    public byte Version; // 版本号
    public byte CheckSum; // 校验和
    public UInt16 GlobalChecksum; // 全局校验和
}

public class Cartridge
{
    public string Filename { get; private set; }
    public int RomSize { get; private set; }
    public byte[] RomData { get; private set; }
    public RomHeader Header { get; private set; }

    private static readonly string[] RomTypes = new string[]
    {
        "ROM ONLY",
        "MBC1",
        "MBC1+RAM",
        "MBC1+RAM+BATTERY",
        "0x04 ???",
        "MBC2",
        "MBC2+BATTERY",
        "0x07 ???",
        "ROM+RAM 1",
        "ROM+RAM+BATTERY 1",
        "0x0A ???",
        "MMM01",
        "MMM01+RAM",
        "MMM01+RAM+BATTERY",
        "0x0E ???",
        "MBC3+TIMER+BATTERY",
        "MBC3+TIMER+RAM+BATTERY 2",
        "MBC3",
        "MBC3+RAM 2",
        "MBC3+RAM+BATTERY 2",
        "0x14 ???",
        "0x15 ???",
        "0x16 ???",
        "0x17 ???",
        "0x18 ???",
        "MBC5",
        "MBC5+RAM",
        "MBC5+RAM+BATTERY",
        "MBC5+RUMBLE",
        "MBC5+RUMBLE+RAM",
        "MBC5+RUMBLE+RAM+BATTERY",
        "0x1F ???",
        "MBC6",
        "0x21 ???",
        "MBC7+SENSOR+RUMBLE+RAM+BATTERY"
    };

    private static readonly Dictionary<byte, string> LicCode = new Dictionary<byte, string>
    {
        [0x00] = "None",
        [0x01] = "Nintendo R&D1",
        [0x08] = "Capcom",
        [0x13] = "Electronic Arts",
        [0x18] = "Hudson Soft",
        [0x19] = "b-ai",
        [0x20] = "kss",
        [0x22] = "pow",
        [0x24] = "PCM Complete",
        [0x25] = "san-x",
        [0x28] = "Kemco Japan",
        [0x29] = "seta",
        [0x30] = "Viacom",
        [0x31] = "Nintendo",
        [0x32] = "Bandai",
        [0x33] = "Ocean/Acclaim",
        [0x34] = "Konami",
        [0x35] = "Hector",
        [0x37] = "Taito",
        [0x38] = "Hudson",
        [0x39] = "Banpresto",
        [0x41] = "Ubi Soft",
        [0x42] = "Atlus",
        [0x44] = "Malibu",
        [0x46] = "angel",
        [0x47] = "Bullet-Proof",
        [0x49] = "irem",
        [0x50] = "Absolute",
        [0x51] = "Acclaim",
        [0x52] = "Activision",
        [0x53] = "American sammy",
        [0x54] = "Konami",
        [0x55] = "Hi tech entertainment",
        [0x56] = "LJN",
        [0x57] = "Matchbox",
        [0x58] = "Mattel",
        [0x59] = "Milton Bradley",
        [0x60] = "Titus",
        [0x61] = "Virgin",
        [0x64] = "LucasArts",
        [0x67] = "Ocean",
        [0x69] = "Electronic Arts",
        [0x70] = "Infogrames",
        [0x71] = "Interplay",
        [0x72] = "Broderbund",
        [0x73] = "sculptured",
        [0x75] = "sci",
        [0x78] = "THQ",
        [0x79] = "Accolade",
        [0x80] = "misawa",
        [0x83] = "lozc",
        [0x86] = "Tokuma Shoten Intermedia",
        [0x87] = "Tsukuda Original",
        [0x91] = "Chunsoft",
        [0x92] = "Video system",
        [0x93] = "Ocean/Acclaim",
        [0x95] = "Varie",
        [0x96] = "Yonezawa/s’pal",
        [0x97] = "Kaneko",
        [0x99] = "Pack in soft",
        [0xA4] = "Konami (Yu-Gi-Oh!)"
    };

    // 加载ROM文件
    public bool LoadCart(string path)
    {
        try
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Filename = path;
                RomSize = (int)fs.Length;
                RomData = new byte[RomSize];
                fs.Read(RomData, 0, (int)fs.Length);

                // 从ROM数据中解析ROM头信息
                Header = new RomHeader
                {
                    Entry = new byte[4],
                    Logo = new byte[0x30],
                    Title = System.Text.Encoding.ASCII.GetString(RomData, 0x134, 0x10).TrimEnd('\0'),
                    NewLicCode = BitConverter.ToUInt16(new byte[] { RomData[0x0144], RomData[0x0143] }, 0),
                    SgbFlag = RomData[0x0146],
                    Type = RomData[0x0147],
                    RomSize = RomData[0x0148],
                    RamSize = RomData[0x0149],
                    DestCode = RomData[0x014A],
                    LicCode = RomData[0x014B],
                    Version = RomData[0x014C],
                    CheckSum = RomData[0x014D],
                    GlobalChecksum = BitConverter.ToUInt16(new byte[] { RomData[0x014E], RomData[0x014F] }, 0)
                };

                // 复制entry和logo数据
                Array.Copy(RomData, 0x100, Header.Entry, 0, 4);
                Array.Copy(RomData, 0x104, Header.Logo, 0, 0x30);
            }
            Console.WriteLine("Cartridge Loaded:");
            Console.WriteLine("\t Title    : {0}", Header.Title);
            Console.WriteLine("\t Type     : {0:X2} ({1})", Header.Type, CartTypeName());
            Console.WriteLine("\t ROM Size : {0} KB", 32 << Header.RomSize);
            Console.WriteLine("\t RAM Size : {0:X2}", Header.RamSize);
            Console.WriteLine("\t LIC Code : {0:X2} ({1})", Header.LicCode, CartLicName());
            Console.WriteLine("\t ROM Vers : {0:X2}", Header.Version);

            UInt16 x = 0;
            for (UInt16 i=0x0134; i<=0x014C; i++) {
                x = (UInt16)(x - (UInt16)RomData[i]-1);
            }
            
            Console.WriteLine("\t Checksum : {0:X2} ({1})\n", Header.CheckSum, (x & 0xFF)!=0 ? "PASSED" : "FAILED");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load ROM: {ex.Message}");
            return false;
        }
    }

    private string CartTypeName()
    {
        if (Header.Type < RomTypes.Length)
        {
            return RomTypes[Header.Type];
        }
        return "UNKNOWN";
    }

    private string CartLicName()
    {
        if (LicCode.TryGetValue(Header.LicCode, out string name))
        {
            return name;
        }
        return "UNKNOWN";
    }
    
    // 读取ROM数据
    public byte ReadRom(ushort address)
    {
        return RomData[address];
    }

    // 写入ROM数据
    public void WriteRom(ushort address, byte value)
    {
        // 实现写入逻辑...
        throw new NotImplementedException();
    }
}