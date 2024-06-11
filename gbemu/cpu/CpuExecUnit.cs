using System.Reflection;

namespace gbemu.cpu;

public partial class Cpu
{
    private Dictionary<InType, Action> _ExecFuncMap;

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