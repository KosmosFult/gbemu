using gbemu.bus;
using Gbemu.cart;
using gbemu.cpu;
using Xunit.Abstractions;

public class CpuTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CpuTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CartLoadTest()
    {
        var tCart = new Cartridge();
        tCart.LoadCart("C:\\Users\\Kosmos\\RiderProjects\\gbemu\\gbemu\\roms\\zelda.gb");
    }

    [Fact]
    public void RegisterTest()
    {
        var tCpu = new Cpu();
        tCpu.Register.BC = 0xf234;
        var tB = tCpu.Register.B;
        var tC = tCpu.Register.C;
        var tBC = tCpu.Register.BC;
        Assert.Equal(0xf234, tBC);
        Assert.Equal(0xf2, tB);
        Assert.Equal(0x34, tC);

    }

    [Fact]
    public void BusReadTest()
    {
        var tBus = new Bus();
        var tCart = new Cartridge();
        tCart.LoadCart("C:\\Users\\Kosmos\\RiderProjects\\gbemu\\gbemu\\roms\\zelda.gb");
        tBus.AddSpace(tCart);
        var v = tBus.Read(0x100);
        Assert.Equal(0, v);
    }
}