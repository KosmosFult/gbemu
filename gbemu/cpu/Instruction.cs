namespace gbemu.cpu;


public enum AddrMode
{
    AM_IMP,
    AM_R_D16,
    AM_R_R,
    AM_MR_R,
    AM_R,
    AM_R_D8,
    AM_R_MR,
    AM_R_HLI,
    AM_R_HLD,
    AM_HLI_R,
    AM_HLD_R,
    AM_R_A8,
    AM_A8_R,
    AM_HL_SPR,
    AM_D16,
    AM_D8,
    AM_D16_R,
    AM_MR_D8,
    AM_MR,
    AM_A16_R,
    AM_R_A16
}

public enum RegType
{
    RT_NONE,
    RT_A,
    RT_F,
    RT_B,
    RT_C,
    RT_D,
    RT_E,
    RT_H,
    RT_L,
    RT_AF,
    RT_BC,
    RT_DE,
    RT_HL,
    RT_SP,
    RT_PC
}

public enum InType
{
    IN_NONE,
    IN_NOP,
    IN_LD,
    IN_INC,
    IN_DEC,
    IN_RLCA,
    IN_ADD,
    IN_RRCA,
    IN_STOP,
    IN_RLA,
    IN_JR,
    IN_RRA,
    IN_DAA,
    IN_CPL,
    IN_SCF,
    IN_CCF,
    IN_HALT,
    IN_ADC,
    IN_SUB,
    IN_SBC,
    IN_AND,
    IN_XOR,
    IN_OR,
    IN_CP,
    IN_POP,
    IN_JP,
    IN_PUSH,
    IN_RET,
    IN_CB,
    IN_CALL,
    IN_RETI,
    IN_LDH,
    IN_JPHL,
    IN_DI,
    IN_EI,
    IN_RST,
    IN_ERR,
    //CB instructions...
    IN_RLC, 
    IN_RRC,
    IN_RL, 
    IN_RR,
    IN_SLA, 
    IN_SRA,
    IN_SWAP, 
    IN_SRL,
    IN_BIT, 
    IN_RES, 
    IN_SET
}

public enum CondType
{
    CT_NONE, CT_NZ, CT_Z, CT_NC, CT_C
    // ... 其他成员
}

public class Instruction
{
    public InType Type;
    public AddrMode Mode;
    public RegType Reg1;
    public RegType Reg2;
    public CondType Cond;
    public byte Param;
}

public static class InstructionSet
{
    private static readonly Dictionary<byte, Instruction> Instructions = new Dictionary<byte, Instruction>
    {
        { 0x00, new Instruction { Type = InType.IN_NOP, Mode = AddrMode.AM_IMP } },
        { 0x05, new Instruction { Type = InType.IN_DEC, Mode = AddrMode.AM_R, Reg1 = RegType.RT_B } },
        { 0x0E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_C, Param = 0xFF } },
        { 0xAF, new Instruction { Type = InType.IN_XOR, Mode = AddrMode.AM_R, Reg1 = RegType.RT_A } },
        { 0xC3, new Instruction { Type = InType.IN_JP, Mode = AddrMode.AM_D16 } },
        { 0xF3, new Instruction { Type = InType.IN_DI } }
    }; 
    
    // 怎么才能返回只读引用
    public static Instruction GetInstructionByOpcode(byte opcode)
    {
        return Instructions[opcode];
    }
}