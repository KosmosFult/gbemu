using System.Reflection;

namespace gbemu.cpu;

public partial class Cpu
{
    private Dictionary<InType, Action> _ExecFuncMap;
    
    // private struct FlagRes
    // {
    //     public readonly bool Z;
    //     public readonly bool N;
    //     public readonly bool C;
    //     public readonly bool H;
    // }

    public ushort TestAdd(ushort x, ushort y, bool add16) => add16 ? _Add16Logic(x, y) : _Add8Logic(x, y);

    public bool[] GetFlags() => [Register.ZF, Register.CF, Register.NF, Register.HF];

    private void InitExecFuncMap()
    {
        _ExecFuncMap = new Dictionary<InType, Action>();

        // 获取当前类的所有方法
        var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(method => method.Name.StartsWith("_Execute") && method.ReturnType == typeof(void) &&
                             method.GetParameters().Length == 0).ToArray();
        foreach (var method in methods)
        {
            // 从方法名中提取 IN_XXX
            var inType = method.Name.Replace("_Execute", "IN_");
            var inTypeEnum = (InType)Enum.Parse(typeof(InType), inType);

            // 添加到字典
            _ExecFuncMap.Add(inTypeEnum, method.CreateDelegate<Action>(this));
            Console.WriteLine($"Add ExecFunc {method.Name}");
        }
    }

    private bool CheckCond()
    {
        return _CurIns.Cond switch
        {
            CondType.CT_NONE => true,
            CondType.CT_C => Register.CF,
            CondType.CT_NC => !Register.CF,
            CondType.CT_Z => Register.ZF,
            CondType.CT_NZ => !Register.ZF,
            _ => false
        };
    }

    private byte _Add8Logic(ushort op1, ushort op2)
    {
        var value = (ushort)(op1 + op2);
        Register.ZF = value == 0;
        Register.CF = value > 0xff;
        Register.HF = (op1 & 0x0f) + (op2 & 0x0f) > 0x0f;
        Register.NF = false;
        return (byte)(value & 0x00ff);
    }
    
    public ushort _Add_16_8_SignedLogic(ushort op1, ushort op2)
    {
        short op2I = (sbyte)(op2 & 0xff);
        ushort value = (ushort)(op1 + op2I);
        var carry = op1 ^ op2I ^ value;
        Register.ZF = value == 0;
        Register.CF = (carry & 0x0100) == 0x0100;
        Register.NF = false;
        Register.HF = (carry & 0x0010) == 0x0010;
        return value;
    }
    
    private ushort _Add16Logic(ushort op1, ushort op2)
    {
        var value = (ushort)(op1 + op2);
        Register.ZF = value == 0;
        // CF highest bit conditions 
        // cond1: (1 + 1)
        // cond2: (1 + 0) -> 0
        // cond3: (0 + 1) -> 0
        // or use 32-bits add to simplify
        bool cond1 = (op1 & op2) >> 15 == 1;
        bool cond23 = ((op1 ^ op2) >> 15 == 1) && (value >> 15 == 0);
        Register.CF = cond1 || cond23;
        Register.HF = (op1 & 0x0fff) + (op2 & 0x0fff) > 0x0fff;
        Register.NF = false;
        return value;
    }

    private void _ExecuteNONE()
    {
        throw new ArgumentException($"Invalid Instruction {_CurOpCode}");
    }

    private void _ExecuteNOP()
    {
        OnCycles?.Invoke(1);
    }

    private void _ExecuteDI()
    {
        _IntMasterEnabled = false;
        OnCycles?.Invoke(1);

    }

    private void _ExecuteLD()
    {
        if (_DestIsMem)
        {
            _Bus.Write(_MemDest, (byte)(_FetchedData & 0x00ff));
            OnCycles.Invoke(1);

            if (!CpuRegisters.DoubleReg(_CurIns.Reg2)) return;
            
            _Bus.Write(_MemDest, (byte)(_FetchedData & 0x00ff));
            OnCycles.Invoke(1);

            return;
        }

        if (_CurIns.Mode == AddrMode.AM_HL_SPR)
        {
            var value = _Add_16_8_SignedLogic(ReadReg(_CurIns.Reg2), _FetchedData);
            OnCycles?.Invoke(1);
            SetReg(_CurIns.Reg1, value);
            OnCycles?.Invoke(1);
            return;
        }
        
        SetReg(_CurIns.Reg1, _FetchedData);
        OnCycles?.Invoke(1);
    }

    private void _ExecuteXOR()
    {
        Register.A ^= (byte)(_FetchedData & 0xFF);
        Register.ZF = (Register.A == 0);
        OnCycles?.Invoke(1);
    }

    private void _ExecuteJP()
    {
        if (CheckCond())
        {
            Register.PC = _FetchedData;
            OnCycles?.Invoke(1);
        }
        
        OnCycles?.Invoke(1);
    }
}