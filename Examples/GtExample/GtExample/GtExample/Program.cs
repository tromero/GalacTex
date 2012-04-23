using System;

namespace GtExample
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GtExampleGame game = new GtExampleGame())
            {
                game.Run();
            }
        }
    }
#endif
}

