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
        { 0x01, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D16, Reg1 = RegType.RT_BC } },
        { 0x02, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_BC, Reg2 = RegType.RT_A } },
        { 0x05, new Instruction { Type = InType.IN_DEC, Mode = AddrMode.AM_R, Reg1 = RegType.RT_B } },
        { 0x06, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_B } },
        { 0x08, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_A16_R, Reg1 = RegType.RT_NONE, Reg2 = RegType.RT_SP } },
        { 0x0A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_A, Reg2 = RegType.RT_BC } },
        { 0x0E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_C } },
        
        { 0x11, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D16, Reg1 = RegType.RT_DE } },
        { 0x12, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_DE, Reg2 = RegType.RT_A } },
        { 0x15, new Instruction { Type = InType.IN_DEC, Mode = AddrMode.AM_R, Reg1 = RegType.RT_D } },
        { 0x16, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_D } },
        { 0x1A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_A, Reg2 = RegType.RT_DE } },
        { 0x1E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_E } },
        
        { 0x21, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D16, Reg1 = RegType.RT_HL } },
        { 0x22, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_HLI_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_A } },
        { 0x25, new Instruction { Type = InType.IN_DEC, Mode = AddrMode.AM_R, Reg1 = RegType.RT_H } },
        { 0x26, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_H } },
        { 0x2A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_HLI, Reg1 = RegType.RT_A, Reg2 = RegType.RT_HL } },
        { 0x2E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_L } },
        
        { 0x31, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D16, Reg1 = RegType.RT_SP } },
        { 0x32, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_HLD_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_A } },
        { 0x35, new Instruction { Type = InType.IN_DEC, Mode = AddrMode.AM_R, Reg1 = RegType.RT_HL } },
        { 0x36, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_D8, Reg1 = RegType.RT_HL } },
        { 0x3A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_HLD, Reg1 = RegType.RT_A, Reg2 = RegType.RT_HL } },
        { 0x3E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_D8, Reg1 = RegType.RT_A } },
        
        { 0x40, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_B, Reg2 = RegType.RT_B } },
        { 0x41, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_B, Reg2 = RegType.RT_C } },
        { 0x42, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_B, Reg2 = RegType.RT_D } },
        { 0x43, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_B, Reg2 = RegType.RT_E } },
        { 0x44, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_B, Reg2 = RegType.RT_H } },
        { 0x45, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_B, Reg2 = RegType.RT_L } },
        { 0x46, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_B, Reg2 = RegType.RT_HL } },
        { 0x47, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_B, Reg2 = RegType.RT_A } },
        { 0x48, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_B } },
        { 0x49, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_C } },
        { 0x4A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_D } },
        { 0x4B, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_E } },
        { 0x4C, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_H } },
        { 0x4D, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_L } },
        { 0x4E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_C, Reg2 = RegType.RT_HL } },
        { 0x4F, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_A } },
        
        { 0x50, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_D, Reg2 = RegType.RT_B } },
        { 0x51, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_D, Reg2 = RegType.RT_C } },
        { 0x52, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_D, Reg2 = RegType.RT_D } },
        { 0x53, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_D, Reg2 = RegType.RT_E } },
        { 0x54, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_D, Reg2 = RegType.RT_H } },
        { 0x55, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_D, Reg2 = RegType.RT_L } },
        { 0x56, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_D, Reg2 = RegType.RT_HL } },
        { 0x57, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_D, Reg2 = RegType.RT_A } },
        { 0x58, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_E, Reg2 = RegType.RT_B } },
        { 0x59, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_E, Reg2 = RegType.RT_C } },
        { 0x5A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_E, Reg2 = RegType.RT_D } },
        { 0x5B, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_E, Reg2 = RegType.RT_E } },
        { 0x5C, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_E, Reg2 = RegType.RT_H } },
        { 0x5D, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_E, Reg2 = RegType.RT_L } },
        { 0x5E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_E, Reg2 = RegType.RT_HL } },
        { 0x5F, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_E, Reg2 = RegType.RT_A } },
        
        { 0x60, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_H, Reg2 = RegType.RT_B } },
        { 0x61, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_H, Reg2 = RegType.RT_C } },
        { 0x62, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_H, Reg2 = RegType.RT_D } },
        { 0x63, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_H, Reg2 = RegType.RT_E } },
        { 0x64, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_H, Reg2 = RegType.RT_H } },
        { 0x65, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_H, Reg2 = RegType.RT_L } },
        { 0x66, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_H, Reg2 = RegType.RT_HL } },
        { 0x67, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_H, Reg2 = RegType.RT_A } },
        { 0x68, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_L, Reg2 = RegType.RT_B } },
        { 0x69, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_L, Reg2 = RegType.RT_C } },
        { 0x6A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_L, Reg2 = RegType.RT_D } },
        { 0x6B, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_L, Reg2 = RegType.RT_E } },
        { 0x6C, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_L, Reg2 = RegType.RT_H } },
        { 0x6D, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_L, Reg2 = RegType.RT_L } },
        { 0x6E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_L, Reg2 = RegType.RT_HL } },
        { 0x6F, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_L, Reg2 = RegType.RT_A } },
        
        { 0x70, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_B } },
        { 0x71, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_C } },
        { 0x72, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_D } },
        { 0x73, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_E } },
        { 0x74, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_H } },
        { 0x75, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_L } },
        { 0x76, new Instruction { Type = InType.IN_HALT } },
        { 0x77, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_HL, Reg2 = RegType.RT_A } },
        { 0x78, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_A, Reg2 = RegType.RT_B } },
        { 0x79, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_A, Reg2 = RegType.RT_C } },
        { 0x7A, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_A, Reg2 = RegType.RT_D } },
        { 0x7B, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_A, Reg2 = RegType.RT_E } },
        { 0x7C, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_A, Reg2 = RegType.RT_H } },
        { 0x7D, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_A, Reg2 = RegType.RT_L } },
        { 0x7E, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_A, Reg2 = RegType.RT_HL } },
        { 0x7F, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_R, Reg1 = RegType.RT_A, Reg2 = RegType.RT_A } },
        
        { 0xAF, new Instruction { Type = InType.IN_XOR, Mode = AddrMode.AM_R, Reg1 = RegType.RT_A } },
        
        { 0xC3, new Instruction { Type = InType.IN_JP, Mode = AddrMode.AM_D16 } },
        { 0xCD, new Instruction { Type = InType.IN_CALL, Mode = AddrMode.AM_D16}},
        
        { 0xE2, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_MR_R, Reg1 = RegType.RT_C, Reg2 = RegType.RT_A } },
        { 0xEA, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_A16_R, Reg1 = RegType.RT_NONE, Reg2 = RegType.RT_A } },
        
        // 0xFX 系列
        { 0xF2, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_MR, Reg1 = RegType.RT_A, Reg2 = RegType.RT_C } },
        { 0xF3, new Instruction { Type = InType.IN_DI } },
        { 0xFA, new Instruction { Type = InType.IN_LD, Mode = AddrMode.AM_R_A16, Reg1 = RegType.RT_A }}
    };
    
    // 怎么才能返回只读引用
    public static Instruction GetInstructionByOpcode(byte opcode)
    {
        return Instructions[opcode];
    }
}