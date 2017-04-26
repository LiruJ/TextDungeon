using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDungeonGame
{
    class Game
    {
        public Map Map;

        public Game(int gameSize)
        {
            //Initialises the console window
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(gameSize, gameSize);
            Console.SetWindowSize(gameSize, gameSize);
            Console.CursorVisible = false;
            

            //Initialises the map
            Map = new Map(55, 55);
            DrawMap();

            ConsoleKey pressedKey;
            do
            {
                pressedKey = Console.ReadKey().Key;
                if (pressedKey == ConsoleKey.Spacebar)
                {
                    Map = new Map(55, 55);
                    DrawMap();
                }
            }
            while (pressedKey != ConsoleKey.Escape);
        }

        public void DrawMap()
        {
            Console.Clear();


            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                    Console.Write(Map[x, y]);
                Console.WriteLine();
            }
        }
    }
}
