// using System.IO;

using gbemu.bus;
using Gbemu.cart;
using gbemu.cpu;
using SDL2;

namespace gbemu;

public class GameBoy
{
    private bool Paused;
    private bool Running;
    public ulong Ticks;   // 时钟

    private Cpu _Cpu;
    private Bus _Bus;
    private Cartridge _Cart;

    public GameBoy()
    {
        _Bus = new Bus();
        _Cpu = new Cpu(_Bus);
        _Cart = new Cartridge();
        _Bus.AddSpace(_Cart);
        _Cpu.OnCycles += Cycles;
    }

    public void Run(string[] argv)
    {
        if (argv.Length < 1)
        {
            Console.WriteLine("Usage: emu <rom_file>");
            return;
        }
        
        if (!_Cart.LoadCart(argv[0])) return;
        
        
        SDL2.SDL_ttf.TTF_Init();
        

        Paused = false;
        Running = true;
        Ticks = 0;
        
        Console.WriteLine("Game Boy Running");

        while (Running)
        {
            if (Paused)
            {
                SDL.SDL_Delay(10);
                continue;
            }

            if (!_Cpu.Step())
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