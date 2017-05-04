using System;
using System.Collections.Generic;
using TextDungeonGame.Entities;

namespace TextDungeonGame
{
    public partial class Map
    {
        #region Private Properties
        /// <summary>The random number generator used to generate a map</summary>
        private Random random = new Random();

        /// <summary>Number of times each room will attempt to be placed</summary>
        private int numberOfTries = 300;

        /// <summary>How many corridor cells are on the map</summary>
        private int corridorCount = 0;

        /// <summary>How many corridor cells to leave when removing dead ends</summary>
        private int corridorsToLeave = 550;

        /// <summary>How many walls should be broken down to make the maze imperfect</summary>
        private int wallsToRemove = 90;

        /// <summary>The probability that a corridor will turn left or right for every cell it goes forwards</summary>
        private int chanceToBendPath = 75;

        /// <summary>The list of rooms on this map</summary>
        private List<Room> rooms;
        #endregion

        #region Private Functions
        /// <summary>Randomly generates a map</summary>
        private void generateMap()
        {
            resetMap();

            placeRooms(25);

            generateMaze();

            removeDeadEnds();

            breakWalls();

            generateRooms();
        }

        /// <summary>Finds if a cell is a corridor</summary>
        /// <param name="pos">The position of the cell to check</param>
        /// <returns>Whether or not the cell is a corridor</returns>
        private bool cellIsClear(Position pos)
        {
            return coordsInRange(pos) && (this[pos] == Cells.Floor || this[pos] == Cells.Room);
        }

        /// <summary>Sets every cell on the map to a wall and initialises the entity list</summary>
        private void resetMap()
        {
            //Initialises the data array again
            data = new Cells[Width, Height];

            //Initialises the entity list
            Entities = new List<Entity>();
            cellEntities = new bool[Width, Height];

            //Iterates over the array and sets each cell to a wall
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    data[x, y] = Cells.Wall;
        }

        /// <summary>Creates rooms in empty places</summary>
        /// <param name="numberOfRooms">The amount of rooms to attempt to place</param>
        private void placeRooms(int numberOfRooms)
        {
            //Creates a list to store the rooms
            rooms = new List<Room>(numberOfRooms);

            //Repeat for the number of rooms
            for (int r = 0; r < numberOfRooms; r++)
            {
                //Keep trying to place the room until a place is found or a certain amount of tries have been made
                for (int t = 0; t < numberOfTries; t++)
                {
                    //Randomises the dimensions of the room
                    int width = random.Next(5, 12), height = random.Next(3, 6);

                    //Randomises the position of the room, ensuring that there is at least 3 cells between the sides of the map
                    int x = random.Next(3, Width - width - 3), y = random.Next(3, Height - height - 3);

                    //If any of the room's properties are even, make them odd
                    if (x % 2 == 0) x--;
                    if (y % 2 == 0) y--;
                    if (width % 2 == 0) width--;
                    if (height % 2 == 0) height--;

                    //If the area is clear, create a new room
                    if (isRoomAreaValid(x, y, width, height))
                    {
                        //Create a new room with the set dimensions
                        Room newRoom = new Room(x, y, width, height);

                        //Fill the space on the map with the placeholder room character
                        newRoom.FillRoom(this, Cells.Room);

                        //Add the room to the list
                        rooms.Add(newRoom);

                        //Break out of the loop, as this try was succesful
                        break;
                    }
                }
            }
        }

        /// <summary>Generates a perfect maze in the area between rooms</summary>
        private void generateMaze()
        {
            //Picks a random side of the map to create the entrance
            Direction startSide = Direction.RandomDirection(random);

            //The doorway into the dungeon, where the player will spawn
            int spawnX = 0, spawnY = 0;

            //The cell the maze starts from
            int startX = 0, startY = 0;

            //Handles left-right spawns, and down-up spawns
            if (startSide == Direction.Left || startSide == Direction.Right)
            {
                //Chooses a random y coord to place the spawn
                spawnY = random.Next(1, Height - 2);

                //If the spawn coord is even, make it odd
                if (spawnY % 2 == 0) spawnY--;

                //Since the entrance goes left-right, the spawn is at the same y coord as the maze start
                startY = spawnY;

                //Makes the spawn and start at the edge of the map depending on whether it starts at the left or right
                spawnX = (startSide == Direction.Left) ? 0 : Width - 1;
                startX = (startSide == Direction.Left) ? 1 : Width - 2;
            }
            else
            {
                //Chooses a random x coord to place the spawn
                spawnX = random.Next(1, Width - 2);

                //If the spawn coord is even, make it odd
                if (spawnX % 2 == 0) spawnX--;

                //Since the entrance goes up-down, the spawn is at the same x coord as the maze start
                startX = spawnX;

                //Makes the spawn and start at the edge of the map depending on whether it starts at the top or bottom
                spawnY = (startSide == Direction.Up) ? 0 : Height - 1;
                startY = (startSide == Direction.Up) ? 1 : Height - 2;
            }

            //Sets the spawn point of the map
            SpawnPoint = new Position(spawnX, spawnY);

            //Starts the maze at the starting position
            visitCell(new Position(startX, startY), startSide);
        }

        /// <summary>Checks if the cell 2 cells adjacent is valid, then carves a path to it. Recursively calls upon itself until there are no valid cells</summary>
        /// <param name="pos">The cell's position</param>
        /// <param name="from">The direction of the previous cell</param>
        private void visitCell(Position pos, Direction from)
        {
            //If this cell is invalid, don't carve a path, instead return
            if (!isCellValidMazePoint(pos)) return;

            //Sets the cell and the cell behind it to a floor
            this[pos] = Cells.Floor;
            this[pos + from.Normal] = Cells.Floor;

            //Increments the corridor count by 2, as 2 corridors have been made
            corridorCount += 2;

            //Gets a list of 4 possible directions in randomised order, and removes the direction that the cell came from
            List<Direction> possibleDirections = Direction.GetRandomDirections(random);
            possibleDirections.RemoveAll(item => item == from);

            //Has a chance to continue on straight instead of randomly bending the path
            if (random.Next(101) > chanceToBendPath)
            {
                //Gets the forward direction of the cell
                Direction to = from.Opposite;

                //Removes this direction from the list of possible directions, as it has already been visited
                possibleDirections.RemoveAll(item => item == to);

                //Visits the cell 2 cells infront of this cell
                visitCell(pos + (to.Normal * 2), from);
            }

            //Recursively calls upon this function for each possible direction the maze could go from this cell
            foreach (Direction d in possibleDirections)
                visitCell(pos + (d.Normal * 2), d.Opposite);
        }

        /// <summary>Finds if a cell is in range of the data array and has a wall on each side</summary>
        /// <param name="pos">The position of the cell to check</param>
        /// <returns>Whether or not this cell is a valid maze point</returns>
        private bool isCellValidMazePoint(Position pos)
        {
            //If pos is out of range of the map, this cell is not valid
            if (!coordsInRange(pos))
                return false;

            //If any of the adjacent cells are floors, this cell is not valid
            if (cellIsClear(pos + Direction.Left.Normal))
                return false;
            if (cellIsClear(pos + Direction.Right.Normal))
                return false;
            if (cellIsClear(pos + Direction.Up.Normal))
                return false;
            if (cellIsClear(pos + Direction.Down.Normal))
                return false;

            //If none of the above is true, the cell is valid
            return true;
        }

        /// <summary>Checks that an area of the map is valid for a room to be placed</summary>
        /// <param name="x">The top-left corner to start from</param>
        /// <param name="y">The top-left corner to start from</param>
        /// <param name="width">The width of the area to check</param>
        /// <param name="height">The height of the area to check</param>
        /// <returns>Whether or not the area is clear</returns>
        private bool isRoomAreaValid(int x, int y, int width, int height)
        {
            //Goes through the area on the map and returns false if a cell is clear
            for (int cx = x - 1; cx < x + width + 1; cx++)
                for (int cy = y - 1; cy < y + height + 1; cy++)
                    if (cellIsClear(new Position(cx, cy))) return false;

            //If none of the cells were clear, this area is suitable for a room
            return true;
        }

        /// <summary>Checks if a cell has 3 adjacent walls, making it a dead end</summary>
        /// <param name="pos">The position of the cell to check</param>
        /// <returns>Whether or not this cell is a dead end</returns>
        private bool cellIsDeadEnd(Position pos)
        {
            //If the cell that is being checked is the spawn point, don't remove it
            if (pos == SpawnPoint) return false;

            //Initialises an int to count how many open cells are adjacent to this cell
            int emptySides = 0;

            //If this cell itself is a wall, it can't be a dead end
            if (!cellIsClear(pos))return false;

            //For every adjacent cell that is clear, increment emptySides by one
            if (cellIsClear(pos + Direction.Left.Normal)) emptySides++;
            if (cellIsClear(pos + Direction.Right.Normal)) emptySides++;
            if (cellIsClear(pos + Direction.Up.Normal)) emptySides++;
            if (cellIsClear(pos + Direction.Down.Normal)) emptySides++;

            //If this cell only has one empty side, it is a dead end
            return emptySides == 1;
        }

        /// <summary>Finds walls that are only 1 cell thick and randomly destroys them to create corridors</summary>
        private void breakWalls()
        {
            //Creates a list of positions for cells that can be broken
            List<Position> breakableWalls = new List<Position>(data.Length / 100);

            //Iterates over the whole map, leaving a 1 cell border
            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                {
                    //The position of the cell that is being checked
                    Position tilePos = new Position(x, y);

                    //If the left and right or top and bottom of the cell as clear, as well is if the cell itself is a wall, add it to the list of breakable walls
                    if ((this[tilePos + Direction.Left.Normal] == Cells.Floor && this[tilePos + Direction.Right.Normal] == Cells.Floor) ||
                        (this[tilePos + Direction.Up.Normal] == Cells.Floor && this[tilePos + Direction.Down.Normal] == Cells.Floor) && this[x, y] == Cells.Wall)
                        breakableWalls.Add(tilePos);
                }

            //Breaks the set amount of walls, or until there are no walls left to break
            for (int w = 0; w < wallsToRemove; w++)
            {
                //If there are no walls left to break, break out of the loop
                if (breakableWalls.Count == 0) break;

                //Randomly chooses a wall from the list
                int index = random.Next(breakableWalls.Count);

                //Breaks the wall and removes it from the list
                this[breakableWalls[index]] = Cells.Floor;
                breakableWalls.RemoveAt(index);
            }
        }

        /// <summary>Finds all of the dead ends on the map</summary>
        /// <returns>A list of positions of cells that are dead ends</returns>
        private List<Position> findDeadEnds()
        {
            //Creates a new list to store the positions of the dead ends
            List<Position> deadEnds = new List<Position>(data.Length / 100);

            //Iterates over the map and checks if each cell is a dead end
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (cellIsDeadEnd(new Position(x, y))) deadEnds.Add(new Position(x, y));

            //Trims the list and returns it
            deadEnds.TrimExcess();
            return deadEnds;
        }

        /// <summary>Removes the dead ends from the map until a certain number of corridors remain</summary>
        private void removeDeadEnds()
        {
            //Gets the dead ends on the map
            List<Position> deadEnds = findDeadEnds();

            //Repeats until there are the desired amount of remaining corridors or until there are no more dead ends to remove
            while (corridorCount > corridorsToLeave && deadEnds.Count > 0)
            {
                //Sets this dead end to a wall and reduces the number of corridors on this map
                this[deadEnds[0]] = Cells.Wall;
                corridorCount--;

                //Checks to see if removing this dead end has created any new dead ends, if so, adds them to the list
                if (cellIsDeadEnd(deadEnds[0] + Direction.Left.Normal)) deadEnds.Add(deadEnds[0] + Direction.Left.Normal);
                if (cellIsDeadEnd(deadEnds[0] + Direction.Right.Normal)) deadEnds.Add(deadEnds[0] + Direction.Right.Normal);
                if (cellIsDeadEnd(deadEnds[0] + Direction.Up.Normal)) deadEnds.Add(deadEnds[0] + Direction.Up.Normal);
                if (cellIsDeadEnd(deadEnds[0] + Direction.Down.Normal)) deadEnds.Add(deadEnds[0] + Direction.Down.Normal);

                //Removes this position from the list, as it is no longer a dead end
                deadEnds.RemoveAt(0);
            }
        }

        /// <summary>Fills the room with floor tiles and any furniture, also connects them to the corridors and other rooms via connectors</summary>
        private void generateRooms()
        {
            //Adds doors to the rooms
            foreach (Room r in rooms)
                r.CreateDoors(this, random);

            //Fills the rooms with floors
            foreach (Room r in rooms)
                r.FillRoom(this, Cells.Floor);
        }
        #endregion

        #region Private Classes
        private class Room
        {
            #region Public Properties
            /// <summary>The dimensions of this room</summary>
            public int X, Y, Width, Height;
            #endregion

            #region Private Properties
            /// <summary>If the room does not connect to the rest of the map, it is invalid and must be removed</summary>
            private bool isValidRoom = true;
            #endregion

            #region Constructors
            /// <summary>Creates a room with the given dimensions</summary>
            /// <param name="x">The x coord of the room</param>
            /// <param name="y">The y coord of the room</param>
            /// <param name="width">The width of the room</param>
            /// <param name="height">The height of the room</param>
            public Room(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
            #endregion

            #region Public Functions
            /// <summary>Fills the area on the map with the given cell</summary>
            /// <param name="map">The map to modify</param>
            /// <param name="fillWith">The cell to fill the room with</param>
            public void FillRoom(Map map, Cells fillWith)
            {
                //If the room is invalid, fill it in with a wall
                if (!isValidRoom) fillWith = Cells.Wall;

                //Iterates through every cell within the room and sets it to the desired cell
                for (int x = X; x < X + Width; x++)
                    for (int y = Y; y < Y + Height; y++)
                        map[x, y] = fillWith;
            }

            /// <summary>Finds cells around the border of the room that can connect to corridors and other rooms</summary>
            /// <param name="map">The map to modify</param>
            /// <param name="random">The random number generator</param>
            public void CreateDoors(Map map, Random random)
            {
                //Creates a new list for the connectors that connect between corridors and other roooms
                List<Position> roomToCorridors = new List<Position>();
                List<Position> roomToRooms = new List<Position>();

                //Positions to check
                Position greaterCell, lesserCell, wallCell;

                //Iterates the height of the room and searches the left and right walls for connectors
                for (int y = Y; y < Y + Height; y++)
                {
                    //Checks the cells to the right and left of the walls on either side of the room
                    greaterCell = new Position(X - 2, y);
                    lesserCell = new Position(X, y);
                    wallCell = new Position(X - 1, y);
                    addValidDoor(map, roomToCorridors, roomToRooms, greaterCell, wallCell, lesserCell);

                    greaterCell = new Position(X + Width - 1, y);
                    lesserCell = new Position(X + Width + 1, y);
                    wallCell = new Position(X + Width, y);
                    addValidDoor(map, roomToCorridors, roomToRooms, greaterCell, wallCell, lesserCell);
                }

                //Iterates the width of the room and searches the upper and lower walls for connectors
                for (int x = X; x < X + Width; x++)
                {
                    //Checks the cells above and below the walls on either side of the room
                    greaterCell = new Position(x, Y - 2);
                    lesserCell = new Position(x, Y);
                    wallCell = new Position(x, Y - 1);
                    addValidDoor(map, roomToCorridors, roomToRooms, greaterCell, wallCell, lesserCell);

                    greaterCell = new Position(x, Y + Height - 1);
                    lesserCell = new Position(x, Y + Height + 1);
                    wallCell = new Position(x, Y + Height);
                    addValidDoor(map, roomToCorridors, roomToRooms, greaterCell, wallCell, lesserCell);
                }

                //If the room has no connectors at all, it is invalid
                isValidRoom = roomToCorridors.Count > 0 || roomToRooms.Count > 0;
                
                //If this room can connect to a corridor, make sure it does so at least once
                if (roomToCorridors.Count > 0)
                {
                    //The amount of doors that connect to a corridor
                    int currentCorridorDoors = 0;

                    //The desired amount of doors that connect to a corridor
                    int desiredCorridorDoors = Math.Max(1, roomToCorridors.Count / 5);

                    //Keep adding doors on random connectors until the amount of doors meets the desired amount
                    while (currentCorridorDoors < desiredCorridorDoors)
                    {
                        //Gets a random connector
                        Position doorPosition = roomToCorridors[random.Next(roomToCorridors.Count)];

                        //Sets this connector to a door and removes it from the list
                        map[doorPosition] = Cells.Floor;
                        map.AddEntity(new Door(doorPosition));
                        roomToCorridors.Remove(doorPosition);

                        //Increases the amount of doors by 1
                        currentCorridorDoors++;
                    }
                }
                
                //If this room can connect to another room, make sure it does so at least once
                if (roomToRooms.Count > 0)
                {
                    //The amount of doors that connect to another room
                    int currentRoomDoors = 0;

                    //The desired amount of doors that connect to another room
                    int desiredRoomDoors = Math.Max(1, roomToRooms.Count / 10);

                    //Keep adding doors on random connectors until the amount of doors reaches the desired amount
                    while (currentRoomDoors < desiredRoomDoors)
                    {
                        //Gets a random connector
                        Position doorPosition = roomToRooms[random.Next(roomToRooms.Count)];

                        //Sets this connector to a door and removes it from the list
                        map[doorPosition] = Cells.Floor;
                        map.AddEntity(new Door(doorPosition));
                        roomToRooms.Remove(doorPosition);

                        //Increases the amount of doors by 1
                        currentRoomDoors++;
                    }
                }
            }
            #endregion

            #region Private Functions
            /// <summary>Finds if a wall is a valid connector, then adds it to the list based on its adjacent cells</summary>
            /// <param name="map">The map to check</param>
            /// <param name="roomToCorridors">The list of connectors from the room to the corridor</param>
            /// <param name="roomToRooms">The list of connectors from the room to other rooms</param>
            /// <param name="greaterCell">The cell below or to the right of the wall</param>
            /// <param name="wallCell">The wall cell</param>
            /// <param name="lesserCell">The cell above or to the left of the wall</param>
            private void addValidDoor(Map map, List<Position> roomToCorridors, List<Position> roomToRooms, Position greaterCell, Position wallCell, Position lesserCell)
            {
                //If both sides of the wall are clear
                if (map.cellIsClear(greaterCell) && map.cellIsClear(lesserCell))
                    //If both sides are rooms, add this connector to the suitable list
                    if (map[greaterCell] == Cells.Room && map[lesserCell] == Cells.Room)
                        roomToRooms.Add(wallCell);
                    //If one side is a corridor, add this connector to the suitable list
                    else
                        roomToCorridors.Add(wallCell);
            }
            #endregion
        }
        #endregion
    }
}
