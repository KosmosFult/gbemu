using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace gbemu.cpu;

public partial class Cpu
{
    private Dictionary<InType, Action> _FuncMap;
    
    private void InitializeFuncMap()
    {
        _FuncMap = new Dictionary<InType, Action>();

        // 获取当前类的所有方法
        var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var method in methods)
        {
            if (method.Name.StartsWith("Execute") && method.ReturnType == typeof(void) &&
                method.GetParameters().Length == 0)
            {
                // 从方法名中提取 IN_XXX
                var inType = method.Name.Replace("Execute", "IN_");
                var inTypeEnum = (InType)Enum.Parse(typeof(InType), inType);

                // 添加到字典
                _FuncMap.Add(inTypeEnum, method.CreateDelegate<Action>(this));
                Console.WriteLine($"Add ExecFunc {method.Name}");
            }
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

    private void ExecuteNONE()
    {
        throw new ArgumentException($"Invalid Instruction {_CurOpCode}");
    }

    private void ExecuteNOP()
    {
        
    }
    
    private void ExecuteDI()
    {
        _IntMasterEnabled = false;
    }
    
    private void ExecuteLD()
    {
        
    }
    
    private void ExecuteXOR()
    {
        Register.A ^= (byte)(_FetchedData & 0xFF);
        Register.ZF = (Register.A == 0);
    }

    private void ExecuteJP()
    {
        if (CheckCond())
        {
            Register.PC = _FetchedData;
            // cycles
        }
    }

}