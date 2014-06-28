using System;

namespace GetMoneyGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CubeChaserGame game = new CubeChaserGame())
            {
                game.Run();
            }
        }
    }
#endif
}

