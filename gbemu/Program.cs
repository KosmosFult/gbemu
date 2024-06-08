// See https://aka.ms/new-console-template for more information

using gbemu;
using SDL2;



class Program
{
    static void Main(string[] args)
    {
        GameBoy myBoy = new GameBoy();
        // nint my_window;
        // nint my_renderer;
        // SDL.SDL_CreateWindowAndRenderer(640, 574, 0, out my_window, out my_renderer);
        // SDL.SDL_Delay(3500);
        // SDL.SDL_RenderPresent(my_renderer);
        // SDL.SDL_DestroyRenderer(my_renderer);
        // SDL.SDL_DestroyWindow(my_window);
        // SDL.SDL_Quit();
        myBoy.Run(args);
        Console.WriteLine("Hello, World!");
    }
}

