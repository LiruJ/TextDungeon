using System;
using System.Collections.Generic;

namespace TextDungeonGame
{
    public class Camera
    {
        #region Private Properties
        /// <summary> The array of cells that have been uncovered by the player </summary>
        private bool[,] discoveredCells;
        
        /// <summary> The array of characters that should be drawn to the screen </summary>
        private char[,] screenData;
        
        /// <summary> The string that will be drawn to the screen, reduces draw calls </summary>
        private string screenString;

        /// <summary> The width and height of the screen in columns </summary>
        private int screenWidth = 31, screenHeight = 21;

        /// <summary> The map interface, containing all the information needed to draw the map </summary>
        private IDrawableMap drawableMap;

        /// <summary> The centre column and row of the screen, where the player will be drawn </summary>
        private Position screenCentre;
        #endregion

        #region Constructors
        /// <summary> Creates a new Camera based on a map </summary>
        /// <param name="map">The map interface</param>
        public Camera(IDrawableMap map)
        {
            //Sets the map interface
            drawableMap = map;

            //Intiialises the discovered cells and screen data arrays
            discoveredCells = new bool[drawableMap.Width, drawableMap.Height];
            screenData = new char[screenWidth, screenHeight];

            //Sets the centre of the screen
            screenCentre = new Position((screenWidth - 1) / 2, (screenHeight - 1) / 2);
        }
        #endregion

        #region Public Functions
        /// <summary>Draws the current scene</summary>
        public void Draw()
        {
            createInterfaceData();

            createDiscoveredCellData();

            createMapData();

            createEntityData();

            drawToScreen();
        }
        #endregion

        #region Private Functions
        /// <summary>Gets the boolean value at the index of the discovered cell array, if the position is out of range, returns false</summary>
        /// <param name="x">The x position of the cell to check</param>
        /// <param name="y">The y position of the cell to check</param>
        /// <returns>Whether or not this tile has been discovered</returns>
        private bool getDiscoveredTile(int x, int y)
        {
            //Checks that the position is within the range of the array
            if (x < 0 || x >= drawableMap.Width || y < 0 || y >= drawableMap.Height) return false;

            //If the position is in range, return the value of it
            return discoveredCells[x, y];
        }

        /// <summary> Creates the UI elements, such as the map's frame and player's health </summary>
        private void createInterfaceData()
        {
            //Creates the corners of the frame
            screenData[0, 0] = 'O';
            screenData[screenWidth - 1, 0] = 'O';
            screenData[screenWidth - 1, screenHeight - 1] = 'O';
            screenData[0, screenHeight - 1] = 'O';

            //Creates the upper and lower border
            for (int x = 1; x < screenWidth - 1; x++)
            {
                screenData[x, 0] = '-';
                screenData[x, screenHeight - 1] = '-';
            }

            //Creates the left and right border
            for (int y = 1; y < screenHeight - 1; y++)
            {
                screenData[0, y] = '|';
                screenData[screenWidth - 1, y] = '|';
            }
        }

        /// <summary> Calculates the cells that have been discovered, based on a circle surrounding the player </summary>
        private void createDiscoveredCellData()
        {
            //THe radius and circumference of the player's sight radius
            double radius = drawableMap.Player.SightRadius;
            double circumference;

            //If the radius is even, calculate the circumference using a 0.5 offset
            if (radius % 2 == 0)
                circumference = Math.Ceiling(2 * Math.PI * radius - 0.5d);
            else
                circumference = Math.Ceiling(2 * Math.PI * radius);

            //For each cell in the circumference of the player's sight
            for (double theta = 0; theta < Math.PI * 2; theta += (Math.PI * 2) / circumference)
            {
                //The x and y of the cell to be checked
                double x = (drawableMap.Player.Position.X + 0.5d) + radius * Math.Cos(theta);
                double y = (drawableMap.Player.Position.Y + 0.5d) + radius * Math.Sin(theta);

                //Discovers every cell in the line of sight to the target cell
                foreach (Position p in getLineOfSight(drawableMap.Player.Position, new Position((int)x, (int)y)))
                    if (p.X >= 0 && p.X < drawableMap.Width && p.Y >= 0 && p.Y < drawableMap.Height)
                        discoveredCells[p.X, p.Y] = true;
            }
        }

        /// <summary> Creates the map area of the screen </summary>
        private void createMapData()
        {
            //Draws the map inside the frame
            for (int y = 1; y < screenHeight - 1; y++)
                for (int x = 1; x < screenWidth - 1; x++)
                {
                    //X and Y position based on the size of the screen and the player's location in the map
                    int drawX = (drawableMap.Player.Position - screenCentre).X + x;
                    int drawY = (drawableMap.Player.Position - screenCentre).Y + y;

                    //If the tile is discovered, draw what the tile is, otherwise, draw an empty space
                    screenData[x, y] = (getDiscoveredTile(drawX, drawY)) ? (char)drawableMap[drawX, drawY] : (char)Map.Cells.Fog;
                }
        }

        /// <summary> Creates the entity data based on the location of the entities </summary>
        private void createEntityData()
        {
            //Draws the entities
            for (int y = 1; y < screenHeight - 1; y++)
                for (int x = 1; x < screenWidth - 1; x++)
                {
                    //X and Y position based on the size of the screen and the player's location in the map
                    int drawX = (drawableMap.Player.Position - screenCentre).X + x;
                    int drawY = (drawableMap.Player.Position - screenCentre).Y + y;

                    //If the cell has an entity on it, draw it
                    if (drawableMap.CellHasEntity(new Position(drawX, drawY)) && getDiscoveredTile(drawX, drawY))
                        screenData[x, y] = drawableMap.GetEntityAtPosition(new Position(drawX, drawY)).Character;
                }

            //Draws the player in the centre of the screen
            screenData[screenCentre.X, screenCentre.Y] = drawableMap.Player.Character;
        }

        /// <summary>Draws the calculated screen data to the screen</summary>
        private void drawToScreen()
        {
            //Resets the screenString to an empty string
            screenString = string.Empty;
            
            //Constructs the screenString based on screenData
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                    screenString += screenData[x, y];
                screenString += '\n';
            }

            //Clears the console and writes the string, using one draw call for the whole screen as opposed to one for every cell reduces flicker and makes the game run faster
            Console.Clear();
            Console.Write(screenString);
        }

        /// <summary>Gets a list of cell positions between two points, this is blocked by any opaque cells between the start and end positions</summary>
        /// <param name="start">The position at which the line of sight will start</param>
        /// <param name="end">The position at which the line of sight will end</param>
        /// <returns>A list of visited cells between the start and end positions</returns>
        private List<Position> getLineOfSight(Position start, Position end)
        {
            //Makes a new list for the visited cell positions
            List<Position> visitedCells = new List<Position>();

            //The distance between the start and end positions
            int xDist = Math.Abs((start - end).X);
            int yDist = Math.Abs((start - end).Y);

            //How many grid tiles to check based on the distance
            int steps = 1 + xDist + yDist;

            //If the end position is to the right of the start, increment right, otherwise go left
            int xInc = (end.X > start.X) ? 1 : -1;
            int yInc = (end.Y > start.Y) ? 1 : -1;

            //Error is positive when the closest tile is on the x axis and negative on the y axis
            int error = xDist - yDist;

            //The X and Y to change, begins at the start position
            int x = start.X;
            int y = start.Y;

            //Visits every cell between the start and end positions
            for (; steps > 0; --steps)
            {
                //Add this cell to the list of visited cells, this happens before we check if the cell is transparent, which allows walls to be discovered
                visitedCells.Add(new Position(x, y));

                //If the cell isn't transparent, return the list of currently visited cells
                if (!drawableMap.CellIsTransparent(new Position(x, y))) return visitedCells;

                //If error is positive, go right and decrease error by the y distance. This eventually causes error to be negative
                if (error > 0)
                {
                    x += xInc;
                    error -= yDist;
                }
                else
                {
                    y += yInc;
                    error += xDist;
                }
            }

            //If every cell was visited, return the final list
            return visitedCells;
        }
        #endregion
    }
}
