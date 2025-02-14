using gbemu.bus;

namespace gbemu.cpu;

public partial class Cpu
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

        public ushort AF
        {
            set
            {
                A = (byte)((value >> 8) & 0x00ff);
                F = (byte)(value & 0x00ff);
            }
            get => (ushort)((A << 8) | F);
        }

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

        public static bool DoubleReg(RegType rtype) =>
            rtype == RegType.RT_AF || rtype == RegType.RT_BC || rtype == RegType.RT_DE || rtype == RegType.RT_HL ||
            rtype == RegType.RT_PC || rtype == RegType.RT_SP;
    }

    private ushort _fetchedData; // 双操作数是第二个操作数
    private ushort _memDest;
    private bool _destIsMem;
    private byte _curOpCode;
    private Instruction _curIns;
    private bool _halted = false;
    private bool _stepping;

    private bool _intMasterEnabled;

    public CpuRegisters Register;

    private Bus _bus;

    // 通过事件调用Gameboy的cycle方法
    public event Action<int> OnCycles;

    public Cpu()
    {
        Register = new CpuRegisters();
        Register.PC = 0x0100;
        Register.SP = 0xFFFE;
        Register.AF = 0x01B0;
        Register.BC = 0x0013;
        Register.DE = 0x00D8;
        Register.HL = 0x014D;
        _halted = false;

        InitFetchFuncMap();
        InitExecFuncMap();
    }

    public Cpu(Bus bus) : this()
    {
        _bus = bus;
        // Register = new CpuRegisters();
        // Register.PC = 0x100;
        // Register.A = 0x01;
    }


    public void FetchInstruction()
    {
        _curOpCode = _bus.Read(Register.PC++);
        _curIns = InstructionSet.GetInstructionByOpcode(_curOpCode);
    }

    void FetchData()
    {
        _fetchFuncMap[_curIns.Mode]();
    }


    private void SetReg(RegType rType, ushort value)
    {
        switch (rType)
        {
            case RegType.RT_A:
                Register.A = (byte)value;
                return;
            case RegType.RT_B:
                Register.B = (byte)value;
                return;
            case RegType.RT_C:
                Register.C = (byte)value;
                return;
            case RegType.RT_D:
                Register.D = (byte)value;
                return;
            case RegType.RT_E:
                Register.E = (byte)value;
                return;
            case RegType.RT_F:
                Register.B = (byte)value;
                return;
            case RegType.RT_H:
                Register.B = (byte)value;
                return;
            case RegType.RT_L:
                Register.L = (byte)value;
                return;
            case RegType.RT_AF:
                Register.AF = value;
                return;
            case RegType.RT_BC:
                Register.BC = value;
                return;
            case RegType.RT_DE:
                Register.DE = value;
                return;
            case RegType.RT_HL:
                Register.HL = value;
                return;
        }
    }

    // 可以用反射，但太慢
    private ushort ReadReg(RegType rType)
    {
        return rType switch
        {
            RegType.RT_A => (ushort)Register.A,
            RegType.RT_B => (ushort)Register.B,
            RegType.RT_C => (ushort)Register.C,
            RegType.RT_D => (ushort)Register.D,
            RegType.RT_E => (ushort)Register.E,
            RegType.RT_F => (ushort)Register.F,
            RegType.RT_H => (ushort)Register.H,
            RegType.RT_L => (ushort)Register.L,
            RegType.RT_AF => Register.AF,
            RegType.RT_BC => Register.BC,
            RegType.RT_DE => Register.DE,
            RegType.RT_HL => Register.HL,
            _ => throw new ArgumentException($"Invalid register type {rType}")
        };
    }

    private void StackPush(byte data)
    {
        _bus.Write(--Register.SP, data);
    }

    private void StackPush16(ushort data)
    {
        StackPush((byte)((data >> 8) & 0xFF));
        StackPush((byte)(data & 0xFF));
    }

    private byte StackPop() => _bus.Read(Register.SP++);
    
    

    public void Execute()
    {
        _execFuncMap[_curIns.Type]();
    }

    public bool Step()
    {
        if (!_halted)
        {
            // GameBoy在运算的同时会进行取指，因此可以认为取指不占周期
            var cpc = Register.PC;
            FetchInstruction();
            FetchData();
            Execute();
        }
        else
        {
            OnCycles?.Invoke(1);
        }

        return true;
    }
}