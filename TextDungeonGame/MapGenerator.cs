﻿using System;
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
        private int corridorsToLeave = 700;
        private List<Room> rooms;


        private void generateMap()
        {
            resetMap();

            placeRooms(20);

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
                    int width = random.Next(5, 10), height = random.Next(4, 7);
                    int x = random.Next(3, Width - width - 3), y = random.Next(3, Height - height - 3);

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
            Direction startSide = Direction.RandomDirection(random);

            int startX = 0, startY = 0;
            int currentX = 0, currentY = 0;

            switch (startSide.Value)
            {
                case Directions.left:
                    startX = 0;
                    startY = random.Next(1, Height - 2);
                    currentX = 1;
                    currentY = startY;
                    break;
                case Directions.right:
                    startX = Width - 1;
                    startY = random.Next(1, Height - 2);
                    currentX = startX - 1;
                    currentY = startY;
                    break;
                case Directions.up:
                    startX = random.Next(1, Width - 2);
                    startY = 0;
                    currentX = startX;
                    currentY = 1;
                    break;
                case Directions.down:
                    startX = random.Next(1, Width - 2);
                    startY = Height - 1;
                    currentX = startX;
                    currentY = startY - 1;
                    break;
            }

            data[startX, startY] = ' ';
            corridorCount = 1;
            visitCell(currentX, currentY, startSide.Value);

        }

        private void visitCell(int x, int y, Directions from)
        {
            if (!isCellValid(x, y, from)) return;

            corridorCount++;

            this[x, y] = ' ';

            List<Direction> possibleDirections = Direction.GetRandomDirections(random);
            possibleDirections.Remove(new Direction(from));

            if (random.Next(101) > 60)
            {
                //TODO: Make this less stupid, seriously
                Direction to = new Direction(new Direction(from).Opposite);
                possibleDirections.Remove(to);
                visitCell(to.AdjacentCell(x, y).X, to.AdjacentCell(x, y).Y, from);
            }

            foreach (Direction d in possibleDirections)
            {
                Position adjacentCell = d.AdjacentCell(x, y);
                if (d.Value != from)
                    visitCell(adjacentCell.X, adjacentCell.Y, d.Opposite);
            }
        }

        private bool isCellValid(int x, int y, Directions from)
        {
            //If the left tile needs to be checked and the tile is empty, return false
            if (from != Directions.left && tileIsClear(x - 1, y))
                return false;
            if (from != Directions.right && tileIsClear(x + 1, y))
                return false;
            if (from != Directions.up && tileIsClear(x, y - 1))
                return false;
            if (from != Directions.down && tileIsClear(x, y + 1))
                return false;

            Directions to = new Direction(from).Opposite;

            switch (to)
            {
                case Directions.left:
                    if (tileIsClear(x - 1, y - 1) || tileIsClear(x - 1, y + 1))
                        return false;
                    break;
                case Directions.right:
                    if (tileIsClear(x + 1, y - 1) || tileIsClear(x + 1, y + 1))
                        return false;
                    break;
                case Directions.up:
                    if (tileIsClear(x + 1, y - 1) || tileIsClear(x - 1, y - 1))
                        return false;
                    break;
                case Directions.down:
                    if (tileIsClear(x - 1, y + 1) || tileIsClear(x + 1, y + 1))
                        return false;
                    break;
            }


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
                    if (((tileIsClear(Direction.Left.AdjacentCell(x, y)) && tileIsClear(Direction.Right.AdjacentCell(x, y)))
                        || (tileIsClear(Direction.Up.AdjacentCell(x, y)) && tileIsClear(Direction.Down.AdjacentCell(x, y)))) && this[x, y] == '#')
                        if (random.Next(101) < 85) this[x, y] = ' ';
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
                    int desiredCorridorDoors = Math.Max(1, roomToCorridors.Count / 4);

                    while (currentCorridorDoors < desiredCorridorDoors)
                    {
                        Position doorPosition = roomToCorridors[random.Next(roomToCorridors.Count)];
                        map[doorPosition] = ' ';
                        roomToCorridors.Remove(doorPosition);
                        currentCorridorDoors++;
                    }
                }
                
                if (roomToRooms.Count > 0)
                {
                    int currentRoomDoors = 0;
                    int desiredRoomDoors = Math.Max(1, roomToRooms.Count / 6);

                    while (currentRoomDoors < desiredRoomDoors)
                    {
                        Position doorPosition = roomToRooms[random.Next(roomToRooms.Count)];
                        map[doorPosition] = ' ';
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