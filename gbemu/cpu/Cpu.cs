using gbemu.bus;

namespace gbemu.cpu;

public class Cpu
{
    public class CpuRegisters
    {
        public byte A;
        public byte F;
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte H;
        public byte L;
        public ushort PC;
        public ushort SP;

        public ushort BC
        {
            set
            {
                B = (byte)((value >> 8) & 0x00ff);
                C = (byte)(value & 0x00ff);
            }
            get => (ushort)((B << 8) | C);
            // return (ushort)((((ushort)B) << 8 & 0xff00) | ((ushort)C & 0x00ff));
            
        }
        
        public ushort DE
        {
            set
            {
                D = (byte)((value >> 8) & 0x00ff);
                E = (byte)(value & 0x00ff);
            }
            get => (ushort)((D << 8) | E);
        }
        
        public ushort HL
        {
            set
            {
                H = (byte)((value >> 8) & 0x00ff);
                L = (byte)(value & 0x00ff);
            }
            get => (ushort)((H << 8) | L);
            // return (ushort)((((ushort)B) << 8 & 0xff00) | ((ushort)C & 0x00ff));
            
        }

        public bool ZF
        {
            set => F = (byte)(F & 0x7f | (value ? 0x80 : 0x00));
            get => (F & 0x80) == 0x80;
        }
        
        public bool NF
        {
            set => F = (byte)(F & 0xbf | (value ? 0x40 : 0x00));
            get => (F & 0x40) == 0x40;
        }
        
        public bool HF
        {
            set => F = (byte)(F & 0xdf | (value ? 0x20 : 0x00));
            get => (F & 0x20) == 0x20;
        }
        
        public bool CF
        {
            set => F = (byte)(F & 0xef | (value ? 0x10 : 0x00));
            get => (F & 0x10) == 0x10;
        }
    }
    
    private ushort _FetchedData;  // 双操作数是第二个操作数
    private ushort _MemDest;
    private bool _DestIsMem;
    private byte _CurOpCode;
    private Instruction _CurIns;
    private bool _Halted;
    private bool _Stepping;

    private bool _IntMasterEnabled;

    public CpuRegisters Register;

    private Bus _Bus;
    
    // 可以用反射，但太慢
    private ushort ReadReg(RegType rType)
    {
        switch (rType)
        {
            case RegType.RT_A:
                return Register.A;
            case RegType.RT_F:
                return Register.F;
            case RegType.RT_B:
                return Register.B;
            case RegType.RT_C:
                return Register.C;
            case RegType.RT_D:
                return Register.D;
            case RegType.RT_E:
                return Register.E;
            case RegType.RT_H:
                return Register.H;
            case RegType.RT_L:
                return Register.L;
            case RegType.RT_BC:
                return Register.BC;
            case RegType.RT_DE:
                return Register.DE;
            case RegType.RT_HL:
                return Register.HL;
            case RegType.RT_SP:
                return Register.SP;
            case RegType.RT_PC:
                return Register.PC;
            case RegType.RT_NONE:
            default:
                throw new ArgumentException($"Invalid register type {rType}");
        }
    }

    public Cpu()
    {
        Register = new CpuRegisters();
        Register.PC = 0x100;
        Register.A = 0x01;
    }

    public Cpu(Bus bus) : this()
    {
        
        _Bus = bus;
        // Register = new CpuRegisters();
        // Register.PC = 0x100;
        // Register.A = 0x01;
    }

    
    public void FetchInstruction()
    {
        _CurOpCode = _Bus.Read(Register.PC++);
        _CurIns = InstructionSet.GetInstructionByOpcode(_CurOpCode);
    }

    public void FetchData()
    {
        _MemDest = 0;
        _DestIsMem = false;

        switch (_CurIns.Mode)
        {
            case AddrMode.AM_IMP: return;
            
            case AddrMode.AM_R:
                _FetchedData = ReadReg(_CurIns.Reg1);
                return;
            
            case AddrMode.AM_R_D8:
                _FetchedData = _Bus.Read(Register.PC++);
                // Update cycle
                return;
            
            case AddrMode.AM_D16:
                var low = _Bus.Read(Register.PC++);
                // Update cycle
                var high = _Bus.Read(Register.PC++);
                // Update cycle
                _FetchedData = (ushort)(low | (high << 8));
                return;
            default:
                throw new NotSupportedException($"Unknown Addressing Mode {_CurIns.Mode} {_CurOpCode}");
        }
    }
    public bool Step()
    {
        if (!_Halted)
        {
            var cpc = Register.PC;
            FetchInstruction();
            FetchData();
        }
        return true;
    }
}