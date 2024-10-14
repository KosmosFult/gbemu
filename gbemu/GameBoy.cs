// using System.IO;

using gbemu.bus;
using gbemu.cart;
using gbemu.cpu;
using SDL2;
using gbemu.ram;

namespace gbemu;

public class GameBoy
{
    private bool _paused;
    private bool _running;
    public ulong Ticks;   // 时钟

    private Cpu _cpu;
    private Bus _bus;
    private Cartridge _cart;
    private HighRAM _hram;

    public GameBoy()
    {
        _bus = new Bus();
        _cpu = new Cpu(_bus);
        _cart = new Cartridge();
        _hram = new HighRAM();
        _bus.AddSpace(_cart);
        _bus.AddSpace(_hram);
        _cpu.OnCycles += Cycles;
    }

    public void Run(string[] argv)
    {
        if (argv.Length < 1)
        {
            Console.WriteLine("Usage: emu <rom_file>");
            return;
        }
        
        if (!_cart.LoadCart(argv[0])) return;
        
        
        // SDL2.SDL_ttf.TTF_Init();
        

        _paused = false;
        _running = true;
        Ticks = 0;
        
        Console.WriteLine("Game Boy Running");

        while (_running)
        {
            if (_paused)
            {
                SDL.SDL_Delay(10);
                continue;
            }

            if (!_cpu.Step())
            {
                Console.WriteLine("Cpu Stop");
                return;
            }

            Ticks++; // 为啥需要tick
        }
    }
    
    public void Cycles(int machineCycles)
    {
        for (var i = 0; i < machineCycles; i++)
        {
            for (var j = 0; j < 4; j++)
                Ticks++;
        }
        
    }
}