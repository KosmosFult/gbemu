using System.Reflection;

namespace gbemu.cpu;

public partial class Cpu
{
    private Dictionary<AddrMode, Action> _FetchFuncMap;
    
    private void InitFetchFuncMap()
    {
        _FetchFuncMap = new Dictionary<AddrMode, Action>();
        
        // 获取当前类的所有方法
        var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        methods = methods.Where(info => info.Name.StartsWith("_Fetch")).ToArray();
        foreach (var method in methods)
        {
            // 从方法名中提取 IN_XXX
            var amMode = method.Name.Replace("_Fetch", "AM");
            var amModeEnum = (AddrMode)Enum.Parse(typeof(AddrMode), amMode);

            // 添加到字典
            _FetchFuncMap.Add(amModeEnum, method.CreateDelegate<Action>(this));
            Console.WriteLine($"Add FetchFunc {method.Name}");
        }
    }

    private void _Fetch_IMP()
    {
    }

    private void _Fetch_R() => _FetchedData = ReadReg(_CurIns.Reg1);
    
    private void _Fetch_R_R() => _FetchedData = ReadReg(_CurIns.Reg2);

    private void _Fetch_R_D8()
    {
        _FetchedData = _Bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_R_D16()
    {
        var low = _Bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
        var high = _Bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
        _FetchedData = (ushort)(low | (high << 8));
    }

    private void _Fetch_D16() => _Fetch_R_D16();

    private void _Fetch_MR_R()
    {
        _FetchedData = ReadReg(_CurIns.Reg2);
        _MemDest = ReadReg(_CurIns.Reg1);
        _DestIsMem = true;
        if (_CurIns.Reg1 == RegType.RT_C)
            _MemDest |= 0xff00;
    }
}