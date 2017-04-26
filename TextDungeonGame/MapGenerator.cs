using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDungeonGame
{
    public partial class Map
    {
        private Random random = new Random();
        private int numberOfTries = 300;
        private int corridorCount = 0;
        private int corridorsToLeave = 550;
        private int wallsToRemove = 90;
        private int chanceToBendPath = 75;
        private List<Room> rooms;


        private void generateMap()
        {
            resetMap();

            placeRooms(25);

            generateMaze();

            removeDeadEnds();

            breakWalls();

            generateRooms();
        }


        private void resetMap()
        {
            data = new char[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    data[x, y] = '#';
        }

        private void placeRooms(int numberOfRooms)
        {
            rooms = new List<Room>(numberOfRooms);

            for (int r = 0; r < numberOfRooms; r++)
            {
                for (int t = 0; t < numberOfTries; t++)
                {
                    int width = random.Next(5, 12), height = random.Next(3, 6);
                    int x = random.Next(3, Width - width - 3), y = random.Next(3, Height - height - 3);
                    if (x % 2 == 0) x--;
                    if (y % 2 == 0) y--;
                    if (width % 2 == 0) width--;
                    if (height % 2 == 0) height--;

                    if (isAreaClear(x, y, width, height))
                    {
                        Room newRoom = new Room(x, y, width, height);
                        newRoom.FillRoom(this, 'R');
                        rooms.Add(newRoom);
                        break;
                    }
                        
                }
            }
        }

        private void generateMaze()
        {
            Direction startSide = Direction.Down;//Direction.RandomDirection(random);

            int startX = 0, startY = 0;
            int currentX = 0, currentY = 0;

            switch (startSide.Value)
            {
                case Directions.left:
                    startX = 0;
                    startY = random.Next(1, Height - 2);
                    if (startY % 2 == 0) startY--;
                    currentX = 1;
                    currentY = startY;
                    break;
                case Directions.right:
                    startX = Width - 1;
                    startY = random.Next(1, Height - 2);
                    if (startY % 2 == 0) startY--;
                    currentX = startX - 1;
                    currentY = startY;
                    break;
                case Directions.up:
                    startX = random.Next(1, Width - 2);
                    if (startX % 2 == 0) startX--;
                    startY = 0;
                    currentX = startX;
                    currentY = 1;
                    break;
                case Directions.down:
                    startX = random.Next(1, Width - 2);
                    if (startX % 2 == 0) startX--;
                    startY = Height - 1;
                    currentX = startX;
                    currentY = startY - 1;
                    break;
            }

            SpawnPoint = new Position(startX, startY);

            visitCell(currentX, currentY, startSide);
        }

        private void visitCell(int x, int y, Direction from)
        {
            if (!isCellValid(x, y)) return;

            this[from.AdjacentCell(x, y)] = ' ';

            corridorCount += 2;

            this[x, y] = ' ';

            List<Direction> possibleDirections = Direction.GetRandomDirections(random);
            possibleDirections.RemoveAll(item => item.Value == from.Value);

            if (random.Next(101) > chanceToBendPath)
            {
                //TODO: Make this less stupid, seriously
                Direction to = new Direction(from.Opposite);
                possibleDirections.RemoveAll(item => item.Value == to.Value);
                visitCell(to.AdjacentCell(x, y, 2).X, to.AdjacentCell(x, y, 2).Y, from);
            }

            foreach (Direction d in possibleDirections)
            {
                Position adjacentCell = d.AdjacentCell(x, y, 2);
                visitCell(adjacentCell.X, adjacentCell.Y, new Direction(d.Opposite));
            }
        }

        private bool isCellValid(int x, int y)
        {
            if (!coordsInRange(x, y))
                return false;
            //If the left tile needs to be checked and the tile is empty, return false
            if (tileIsClear(x - 1, y))
                return false;
            if (tileIsClear(x + 1, y))
                return false;
            if (tileIsClear(x, y - 1))
                return false;
            if (tileIsClear(x, y + 1))
                return false;

            return true;






        }

        private bool isAreaClear(int x, int y, int width, int height)
        {
            for (int cx = x - 1; cx < x + width + 1; cx++)
                for (int cy = y - 1; cy < y + height + 1; cy++)
                    if (tileIsClear(cx, cy)) return false;

            return true;
        }

        private bool cellIsDeadEnd(int x, int y)
        {
            if (x == SpawnPoint.X && y == SpawnPoint.Y) return false;

            int emptySides = 0;

            if (!tileIsClear(x, y))return false;
            if (tileIsClear(x - 1, y)) emptySides++;
            if (tileIsClear(x + 1, y)) emptySides++;
            if (tileIsClear(x, y - 1)) emptySides++;
            if (tileIsClear(x, y + 1)) emptySides++;

            return emptySides == 1;
        }

        private void breakWalls()
        {
            List<Position> walls = new List<Position>(data.Length / 100);

            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                {
                    if ((this[Direction.Left.AdjacentCell(x, y)] == ' ' && this[Direction.Right.AdjacentCell(x, y)] == ' ') ||
                        (this[Direction.Up.AdjacentCell(x, y)] == ' ' && this[Direction.Down.AdjacentCell(x, y)] == ' ') && this[x, y] == '#')
                        walls.Add(new Position(x, y));
                }
                    
            for (int w = 0; w < wallsToRemove; w++)
            {
                if (walls.Count == 0) break;
                int index = random.Next(walls.Count);
                this[walls[index]] = ' ';
                walls.RemoveAt(index);
            }
        }

        private List<Position> findDeadEnds()
        {
            List<Position> deadEnds = new List<Position>(data.Length / 100);

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (cellIsDeadEnd(x, y)) deadEnds.Add(new Position(x, y));

            deadEnds.TrimExcess();

            return deadEnds;
        }

        private void removeDeadEnds()
        {
            List<Position> deadEnds = findDeadEnds();

            while (corridorCount > corridorsToLeave && deadEnds.Count > 0)
            {
                int x = deadEnds[0].X, y = deadEnds[0].Y;
                this[x, y] = '#';
                if (cellIsDeadEnd(x - 1, y)) deadEnds.Add(new Position(x - 1, y));
                if (cellIsDeadEnd(x + 1, y)) deadEnds.Add(new Position(x + 1, y));
                if (cellIsDeadEnd(x, y - 1)) deadEnds.Add(new Position(x, y - 1));
                if (cellIsDeadEnd(x, y + 1)) deadEnds.Add(new Position(x, y + 1));
                corridorCount--;
                deadEnds.RemoveAt(0);
            }
        }

        private void generateRooms()
        {

            foreach (Room r in rooms)
                r.FindConnectors(this, random);

            foreach (Room r in rooms)
                r.FillRoom(this, ' ');
        }

        private class Room
        {
            public int X, Y, Width, Height;
            private bool isValidRoom = true;

            public Room(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public void FillRoom(Map map, char fillWith)
            {
                if (!isValidRoom) fillWith = 'X';

                for (int x = X; x < X + Width; x++)
                    for (int y = Y; y < Y + Height; y++)
                        map[x, y] = fillWith;
            }

            public void FindConnectors(Map map, Random random)
            {
                List<Position> roomToCorridors = new List<Position>();
                List<Position> roomToRooms = new List<Position>();
                Position upperTile, lowerTile, wallTile;

                //Searches the left and right walls for connectors
                for (int y = Y; y < Y + Height; y++)
                {
                    upperTile = new Position(X - 2, y);
                    lowerTile = new Position(X, y);
                    wallTile = new Position(X - 1, y);
                    isValidDoor(map, roomToCorridors, roomToRooms, upperTile, wallTile, lowerTile);

                    upperTile = new Position(X + Width - 1, y);
                    lowerTile = new Position(X + Width + 1, y);
                    wallTile = new Position(X + Width, y);
                    isValidDoor(map, roomToCorridors, roomToRooms, upperTile, wallTile, lowerTile);
                }

                //Searches the upper and lower walls for connectors
                for (int x = X; x < X + Width; x++)
                {
                    upperTile = new Position(x, Y - 2);
                    lowerTile = new Position(x, Y);
                    wallTile = new Position(x, Y - 1);
                    isValidDoor(map, roomToCorridors, roomToRooms, upperTile, wallTile, lowerTile);

                    upperTile = new Position(x, Y + Height - 1);
                    lowerTile = new Position(x, Y + Height + 1);
                    wallTile = new Position(x, Y + Height);
                    isValidDoor(map, roomToCorridors, roomToRooms, upperTile, wallTile, lowerTile);
                }

                isValidRoom = roomToCorridors.Count > 0 || roomToRooms.Count > 0;
                
                //If this room can connect to a corridor, make sure it does so at least once
                if (roomToCorridors.Count > 0)
                {
                    int currentCorridorDoors = 0;
                    int desiredCorridorDoors = Math.Max(1, roomToCorridors.Count / 6);

                    while (currentCorridorDoors < desiredCorridorDoors)
                    {
                        Position doorPosition = roomToCorridors[random.Next(roomToCorridors.Count)];
                        map[doorPosition] = '+';
                        roomToCorridors.Remove(doorPosition);
                        currentCorridorDoors++;
                    }
                }
                
                if (roomToRooms.Count > 0)
                {
                    int currentRoomDoors = 0;
                    int desiredRoomDoors = Math.Max(1, roomToRooms.Count / 10);

                    while (currentRoomDoors < desiredRoomDoors)
                    {
                        Position doorPosition = roomToRooms[random.Next(roomToRooms.Count)];
                        map[doorPosition] = '+';
                        roomToRooms.Remove(doorPosition);
                        currentRoomDoors++;
                    }
                }
            }

            private void isValidDoor(Map map, List<Position> roomToCorridors, List<Position> roomToRooms, Position upperTile, Position wallTile, Position lowerTile)
            {
                if (map.tileIsClear(upperTile) && map.tileIsClear(lowerTile))
                    if (map[upperTile] == 'R' && map[lowerTile] == 'R')
                        roomToRooms.Add(wallTile);
                    else
                        roomToCorridors.Add(wallTile);
            }
        }
    }
}
