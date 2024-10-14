using gbemu.bus;
using gbemu.cart;
using gbemu.cpu;
using Xunit.Abstractions;

public class CpuTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly Cpu _Cpu = new Cpu();
    private readonly Cartridge _Cart = new Cartridge();
    private readonly Bus _Bus = new Bus();

    public CpuTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _Cart.LoadCart("C:\\Users\\kosmo\\RiderProjects\\gbemu\\gbemu\\roms\\zelda.gb");
        _Bus.AddSpace(_Cart);
    }

    [Fact]
    public void RegisterTest()
    {
        _Cpu.Register.BC = 0xf234;
        var tB = _Cpu.Register.B;
        var tC = _Cpu.Register.C;
        var tBC = _Cpu.Register.BC;
        Assert.Equal(0xf234, tBC);
        Assert.Equal(0xf2, tB);
        Assert.Equal(0x34, tC);

    }

    [Fact]
    public void BusReadTest()
    {
        var v = _Bus.Read(0x100);
        Assert.Equal(0, v);
    }
    
    [Fact]
    public void CpuAddTest()
    {
        ushort x = 0x00f0;
        ushort y = 0x000f;
        ushort value;
        value = _Cpu.TestAdd(x, y, false);
        Assert.Equal(0x00ff, value);
        Assert.Equal([false, false, false, false], _Cpu.GetFlags());
        
        value = _Cpu.TestAdd((ushort)(x+1), y, false);
        Assert.Equal(0x0000, value);
        Assert.Equal([false, true, false, true], _Cpu.GetFlags());

        x = 0x80ff;
        y = 0x70ff;
        value = _Cpu.TestAdd(x, y, true);
        Assert.Equal(0xf1fe, value);
        Assert.Equal([false, false, false, false], _Cpu.GetFlags());
        
        x = 0x28ff;
        y = 0x17ff;
        value = _Cpu.TestAdd(x, y, true);
        Assert.Equal(0x40fe, value);
        Assert.Equal([false, false, false, true], _Cpu.GetFlags());
        
        x = 0x80ff;
        y = 0x80ff;
        value = _Cpu.TestAdd(x, y, true);
        Assert.Equal(0x01fe, value);
        Assert.Equal([false, true, false, false], _Cpu.GetFlags());
        
        x = 0x88ff;
        y = 0x77ff;
        value = _Cpu.TestAdd(x, y, true);
        Assert.Equal(0x00fe, value);
        Assert.Equal([false, true, false, true], _Cpu.GetFlags());

        x = 0x77ff;
        y = 0xff;
        Assert.Equal(0x77fe, _Cpu._Add_16_8_SignedLogic(x, y));
    }

    [Fact]
    public void InsCycleTest()
    {
    }
}