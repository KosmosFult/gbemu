using System.Reflection;
using gbemu.bus;

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

    private void _Fetch_R_MR()
    {
        var addr = (ushort)(ReadReg(_CurIns.Reg2) | 
                   (_CurIns.Reg2 == RegType.RT_C ? 0xff00 : 0x0000));
        _FetchedData = _Bus.Read(addr);
        OnCycles.Invoke(1);
    }

    private void _Fetch_R_HLI()
    {
        _FetchedData = _Bus.Read(ReadReg(_CurIns.Reg2));
        OnCycles.Invoke(1);
        Register.HL++;
    }
    
    private void _Fetch_R_HLD()
    {
        _FetchedData = _Bus.Read(ReadReg(_CurIns.Reg2));
        OnCycles.Invoke(1);
        Register.HL--;
    }

    private void _Fetch_HLI_R()
    {
        _FetchedData = ReadReg(_CurIns.Reg2);
        _MemDest = ReadReg(_CurIns.Reg1);
        _DestIsMem = true;
        Register.HL++;
    }
    
    private void _Fetch_HLD_R()
    {
        _FetchedData = ReadReg(_CurIns.Reg2);
        _MemDest = ReadReg(_CurIns.Reg1);
        _DestIsMem = true;
        Register.HL--;
    }

    private void _Fetch_R_A8()
    {
        _FetchedData = _Bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_A8_R()
    {
        _MemDest = (ushort)(Register.PC | 0xff00);
        _DestIsMem = true;
        OnCycles.Invoke(1);
        Register.PC++;
    }

    private void _Fetch_HL_SPR()
    {
        _FetchedData = _Bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_D8()
    {
        _FetchedData = _Bus.Read(Register.PC++);
        OnCycles?.Invoke(1);
    }

    private void _Fetch_A16_R()
    {
        _FetchedData = ReadReg(_CurIns.Reg2);
        var low = _Bus.Read(Register.PC);
        OnCycles.Invoke(1);
        var high = _Bus.Read((ushort)(Register.PC + 1));
        OnCycles.Invoke(1);
        _MemDest = (ushort)((high << 8) | low);
        _DestIsMem = true;
        Register.PC = (ushort)(Register.PC + 2);
    }

    private void _Fetch_D16_R()
    {
        _FetchedData = ReadReg(_CurIns.Reg2);
        var low = _Bus.Read(Register.PC);
        OnCycles.Invoke(1);
        var high = _Bus.Read((ushort)(Register.PC + 1));
        OnCycles.Invoke(1);
        _MemDest = (ushort)((high << 8) | low);
        _DestIsMem = true;
        Register.PC += 2;
    }

    private void _Fetch_MR_D8()
    {
        _FetchedData = _Bus.Read(Register.PC);
        OnCycles.Invoke(1);
        Register.PC++;
        _MemDest = ReadReg(_CurIns.Reg1);
        _DestIsMem = true;
    }

    private void _Fetch_MR()
    {
        _MemDest = ReadReg(_CurIns.Reg1);
        _DestIsMem = true;
        _FetchedData = _Bus.Read(_MemDest);
        OnCycles.Invoke(1);
    }

    private void _Fetch_R_A16()
    {
        var low = _Bus.Read(Register.PC);
        OnCycles.Invoke(1);
        var high = _Bus.Read((ushort)(Register.PC + 1));
        OnCycles.Invoke(1);
        Register.PC += 2;
        _FetchedData = _Bus.Read((ushort)((high << 8) | low));
        OnCycles.Invoke(1);
    }
}