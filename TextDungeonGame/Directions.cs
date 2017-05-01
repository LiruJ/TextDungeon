using System;
using System.Collections.Generic;

namespace TextDungeonGame
{
    public struct Direction
    {
        #region Private Properties
        /// <summary>The directions the value can be</summary>
        private enum Directions { none = -1, left, up, right, down }

        /// <summary>The value of the direction</summary>
        private Directions Value { get; set; }
        #endregion
        
        #region Public Properties
        /// <summary>The opposite of this direction</summary>
        public Direction Opposite
        {
            get
            {
                switch (Value)
                {
                    case Directions.none:
                        return None;
                    case Directions.left:
                        return Right;
                    case Directions.right:
                        return Left;
                    case Directions.down:
                        return Up;
                    case Directions.up:
                        return Down;
                    default:
                        return Down;
                }
            }
        }

        /// <summary>The normal position of this direction</summary>
        public Position Normal
        {
            get
            {
                switch (Value)
                {
                    case Directions.none:
                        return new Position(0, 0);
                    case Directions.left:
                        return new Position(-1, 0);
                    case Directions.right:
                        return new Position(1, 0);
                    case Directions.down:
                        return new Position(0, 1);
                    case Directions.up:
                        return new Position(0, -1);
                    default:
                        return new Position(0, 0);
                }
            }
        }
        #endregion

        #region Static Functions
        /// <summary>Gets a random direction</summary>
        /// <param name="random">The random number generator</param>
        /// <returns>A random direction</returns>
        public static Direction RandomDirection(Random random)
        {
            return new Direction((Directions)random.Next(0, 4));
        }

        /// <summary>Gets a list of the 4 directions in a random order</summary>
        /// <param name="random">The random number generator</param>
        /// <returns>A list of 4 random unique directions</returns>
        public static List<Direction> GetRandomDirections(Random random)
        {
            //Creates a new list for the directions
            List<Direction> directionList = new List<Direction>(4);

            //Repeats until each element of the list is a unique direction
            while (directionList.Count < 4)
            {
                //Creates a random direction
                Direction randomDir = RandomDirection(random);

                //If the direction is not in the list, add it
                if (!directionList.Exists(item => item.Value == randomDir.Value)) directionList.Add(randomDir);
            }

            //Return the list of random directions
            return directionList;
        }
        #endregion

        #region Static Properties
        /// <summary>A direction pointing nowhere</summary>
        public static Direction None { get { return new Direction(Directions.none); } }

        /// <summary>A direction pointing left</summary>
        public static Direction Left { get { return new Direction(Directions.left); } }

        /// <summary>A direction pointing right</summary>
        public static Direction Right { get { return new Direction(Directions.right); } }

        /// <summary>A direction pointing down</summary>
        public static Direction Down { get { return new Direction(Directions.down); } }

        /// <summary>A direction pointing up</summary>
        public static Direction Up { get { return new Direction(Directions.up); } }
        #endregion

        #region Constructors
        /// <summary>Creates a new direction</summary>
        /// <param name="direction">The value of the new direction</param>
        private Direction(Directions direction)
        {
            Value = direction;
        }
        #endregion

        #region Operator Overrides
        public static bool operator ==(Direction left, Direction right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return this == (Direction)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
