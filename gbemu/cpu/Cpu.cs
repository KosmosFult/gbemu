namespace gbemu.cpu;

public class Cpu
{
    private struct CpuRegisters
    {
        public byte a;
        public byte f;
        public byte b;
        public byte c;
        public byte d;
        public byte e;
        public byte h;
        public byte l;
        public ushort pc;
        public ushort sp;
    }
    
    private ushort FetchedData;
    private ushort MemDest;
    private bool DestIsMem;
    private byte CurOpCode;
    private Instruction CurIns;
    private bool Halted;
    private bool Stepping;

    private bool IntMasterEnabled;

    private CpuRegisters Register;

    public Cpu()
    {
        Register.pc = 0x100;
        Register.a = 0x01;
    }

    public bool Step()
    {
        return true;
    }
}