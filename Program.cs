using System;
using OpenTK.Windowing.Desktop;

namespace ConwaysGameOfLife2
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game(GameWindowSettings.Default, NativeWindowSettings.Default))
                game.Run();
        }
    }
}
