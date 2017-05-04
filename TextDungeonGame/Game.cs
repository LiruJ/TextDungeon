using System;

namespace TextDungeonGame
{
    public class Game
    {
        #region Properties
        /// <summary>The game map</summary>
        public Map Map;

        /// <summary>The player's camera</summary>
        public Camera Camera;
        #endregion

        #region Constructors
        /// <summary>Starts a new game with a given window size</summary>
        /// <param name="windowSize">The desired width and height of the window in columns</param>
        public Game(int windowSize)
        {
            drawTitleScreen();

            waitForGameStart();

            initialiseWindow(windowSize);

            initialiseGameWorld();

            updateLoop();
        }
        #endregion

        #region Private Functions
        /// <summary>Waits for space to be pressed before continuing the program</summary>
        private void waitForGameStart()
        {
            //Keeps looping until the space bar is pressed
            ConsoleKey pressedkey;
            do
                pressedkey = Console.ReadKey(true).Key;
            while (pressedkey != ConsoleKey.Spacebar);
        }

        /// <summary>Loads and draws the title screen</summary>
        private void drawTitleScreen()
        {
            //Will add a catch if the file does not exist, will start the game automatically, until then, the file must exist
            string fileTitle = System.IO.File.ReadAllText("Title.txt");

            //Draws the title screen
            Console.Write(fileTitle + '\n');
        }

        /// <summary>Initialises the game window</summary>
        /// <param name="windowSize">The width and height of the game window in columns </param>
        private void initialiseWindow(int windowSize)
        {
            //Sets the window size very low, as the buffer cannot be smaller than the window
            Console.SetWindowSize(1, 1);

            //Sets the size of the buffer and window
            Console.SetBufferSize(windowSize, windowSize);
            Console.SetWindowSize(windowSize, windowSize);

            //Hides the cursor
            Console.CursorVisible = false;
        }

        /// <summary> Initialises the camera and map, then draws </summary>
        private void initialiseGameWorld()
        {
            //Initialises the map
            Map = new Map(55, 55);

            //Initialises the camera
            Camera = new Camera(Map);
        }

        /// <summary>Handles input and updates the world</summary>
        private void updateLoop()
        {
            //Draws the updated scene
            Camera.Draw();

            //Holds the program until a key is pressed, then handles this input
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Spacebar:
                    initialiseGameWorld();
                    break;
                case ConsoleKey.W:
                    Map.Update(Direction.Up);
                    break;
                case ConsoleKey.A:
                    Map.Update(Direction.Left);
                    break;
                case ConsoleKey.D:
                    Map.Update(Direction.Right);
                    break;
                case ConsoleKey.S:
                    Map.Update(Direction.Down);
                    break;
                case ConsoleKey.Escape:
                    return;
            }

            //Calls upon itself in order to create a loop
            updateLoop();
        }
        #endregion
    }
}
