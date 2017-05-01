namespace TextDungeonGame
{
    public class Position
    {
        #region Public Properties
        /// <summary>The X coord</summary>
        public int X { get; private set; }

        /// <summary>The Y coord</summary>
        public int Y { get; private set; }
        #endregion

        #region Constructors
        /// <summary>Creates a new position based off the x and y</summary>
        /// <param name="x">The x coord of the position</param>
        /// <param name="y">The y coord of the position</param>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Operator Overrides
        public static Position operator +(Position left, Position right)
        {
            return new Position(left.X + right.X, left.Y + right.Y);
        }

        public static Position operator -(Position left, Position right)
        {
            return new Position(left.X - right.X, left.Y - right.Y);
        }

        public static Position operator *(Position left, int amount)
        {
            return new Position(left.X * amount, left.Y * amount);
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return this == (Position)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
