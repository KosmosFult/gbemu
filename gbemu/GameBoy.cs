// using System.IO;

using Gbemu.cart;
using gbemu.cpu;
using SDL2;

namespace gbemu;

public class GameBoy
{
    public bool Paused;
    public bool Running;
    public ulong Ticks;

    public void Run(string[] argv)
    {
        if (argv.Length < 1)
        {
            Console.WriteLine("Usage: emu <rom_file>");
            return;
        }

        var cart = new Cartridge();
        if (!cart.LoadCart(argv[0])) return;
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
        }
    }

    private void Cycles(int cpuCycle)
    {
        throw new NotImplementedException();
    }
}