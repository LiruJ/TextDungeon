using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDungeonGame
{
    public enum Directions { left, up, right, down }

    class Direction
    {
        public static Direction RandomDirection(Random random)
        {
            return new Direction((Directions)random.Next(0, 4));
        }

        

        public static List<Direction> GetRandomDirections(Random random)
        {
            List<Direction> directionList = new List<Direction>(4);

            while (directionList.Count < 4)
            {
                Direction randomDir = RandomDirection(random);

                if (!directionList.Exists(item => item.Value == randomDir.Value)) directionList.Add(randomDir);
            }

            return directionList;
        }

        public static Direction Left = new Direction(Directions.left);
        public static Direction Right = new Direction(Directions.right);
        public static Direction Down = new Direction(Directions.down);
        public static Direction Up = new Direction(Directions.up);

        public Directions Value { get; private set; }

        public Direction(Directions direction)
        {
            Value = direction;
        }

        public Position AdjacentCell(int x, int y)
        {
            switch (Value)
            {
                case Directions.left:
                    return new Position(x - 1, y);
                case Directions.right:
                    return new Position(x + 1, y);
                case Directions.up:
                    return new Position(x, y - 1);
                case Directions.down:
                    return new Position(x, y + 1);
            }
            return new Position(0, 0);
        }

        public Directions Opposite
        {
            get
            {
                switch (Value)
                {
                    case Directions.left:
                        return Directions.right;
                    case Directions.right:
                        return Directions.left;
                    case Directions.down:
                        return Directions.up;
                    case Directions.up:
                        return Directions.down;
                    default:
                        return Directions.down;
                }
            }
        }


    }
}
