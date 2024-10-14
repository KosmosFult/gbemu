using System.Reflection;
using gbemu.bus;

namespace gbemu.cpu;

public partial class Cpu
{
    private Dictionary<AddrMode, Action> _fetchFuncMap;

    private void InitFetchFuncMap()
    {
        _fetchFuncMap = new Dictionary<AddrMode, Action>();

        // 获取当前类的所有方法
        var methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        methods = methods.Where(info => info.Name.StartsWith("_Fetch")).ToArray();
        foreach (var method in methods)
        {
            // 从方法名中提取 IN_XXX
            var amMode = method.Name.Replace("_Fetch", "AM");
            var amModeEnum = (AddrMode)Enum.Parse(typeof(AddrMode), amMode);

            // 添加到字典
            _fetchFuncMap.Add(amModeEnum, method.CreateDelegate<Action>(this));
            Console.WriteLine($"Add FetchFunc {method.Name}");
        }
    }

    private void _Fetch_IMP()
    {
    }

    private void _Fetch_R() => _fetchedData = ReadReg(_curIns.Reg1);

    private void _Fetch_R_R() => _fetchedData = ReadReg(_curIns.Reg2);

    private void _Fetch_R_D8()
    {
        _fetchedData = _bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_R_D16()
    {
        var low = _bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
        var high = _bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
        _fetchedData = (ushort)(low | (high << 8));
    }

    private void _Fetch_D16() => _Fetch_R_D16();

    private void _Fetch_MR_R()
    {
        _fetchedData = ReadReg(_curIns.Reg2);
        _memDest = ReadReg(_curIns.Reg1);
        _destIsMem = true;
        if (_curIns.Reg1 == RegType.RT_C)
            _memDest |= 0xff00;
    }

    private void _Fetch_R_MR()
    {
        var addr = (ushort)(ReadReg(_curIns.Reg2) | 
                   (_curIns.Reg2 == RegType.RT_C ? 0xff00 : 0x0000));
        _fetchedData = _bus.Read(addr);
        OnCycles.Invoke(1);
    }

    private void _Fetch_R_HLI()
    {
        _fetchedData = _bus.Read(ReadReg(_curIns.Reg2));
        OnCycles.Invoke(1);
        Register.HL++;
    }
    
    private void _Fetch_R_HLD()
    {
        _fetchedData = _bus.Read(ReadReg(_curIns.Reg2));
        OnCycles.Invoke(1);
        Register.HL--;
    }

    private void _Fetch_HLI_R()
    {
        _fetchedData = ReadReg(_curIns.Reg2);
        _memDest = ReadReg(_curIns.Reg1);
        _destIsMem = true;
        Register.HL++;
    }
    
    private void _Fetch_HLD_R()
    {
        _fetchedData = ReadReg(_curIns.Reg2);
        _memDest = ReadReg(_curIns.Reg1);
        _destIsMem = true;
        Register.HL--;
    }

    private void _Fetch_R_A8()
    {
        _fetchedData = _bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_A8_R()
    {
        _memDest = (ushort)(Register.PC | 0xff00);
        _destIsMem = true;
        OnCycles.Invoke(1);
        Register.PC++;
    }

    private void _Fetch_HL_SPR()
    {
        _fetchedData = _bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_D8()
    {
        _fetchedData = _bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_A16_R()
    {
        _fetchedData = ReadReg(_curIns.Reg2);
        var low = _bus.Read(Register.PC);
        OnCycles.Invoke(1);
        var high = _bus.Read((ushort)(Register.PC + 1));
        OnCycles.Invoke(1);
        _memDest = (ushort)((high << 8) | low);
        _destIsMem = true;
        Register.PC = (ushort)(Register.PC + 2);
    }

    private void _Fetch_D16_R()
    {
        _fetchedData = ReadReg(_curIns.Reg2);
        var low = _bus.Read(Register.PC);
        OnCycles.Invoke(1);
        var high = _bus.Read((ushort)(Register.PC + 1));
        OnCycles.Invoke(1);
        _memDest = (ushort)((high << 8) | low);
        _destIsMem = true;
        Register.PC += 2;
    }

    private void _Fetch_MR_D8()
    {
        _fetchedData = _bus.Read(Register.PC);
        OnCycles.Invoke(1);
        Register.PC++;
        _memDest = ReadReg(_curIns.Reg1);
        _destIsMem = true;
    }

    private void _Fetch_MR()
    {
        _memDest = ReadReg(_curIns.Reg1);
        _destIsMem = true;
        _fetchedData = _bus.Read(_memDest);
        OnCycles.Invoke(1);
    }

    private void _Fetch_R_A16()
    {
        var low = _bus.Read(Register.PC);
        OnCycles.Invoke(1);
        var high = _bus.Read((ushort)(Register.PC + 1));
        OnCycles.Invoke(1);
        Register.PC += 2;
        _fetchedData = _bus.Read((ushort)((high << 8) | low));
        OnCycles.Invoke(1);
    }
}