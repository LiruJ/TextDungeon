namespace TextDungeonGame.Entities
{
    public class Player : Entity
    {
        #region Public Properties
        /// <summary>The distance the player can see</summary>
        public double SightRadius { get; private set; }
        #endregion

        #region Constructors
        /// <summary>Creates a player at the given position</summary>
        /// <param name="pos">The player's position</param>
        public Player(Position pos) : base(pos)
        {
            Character = (char)Entities.Player;
            SightRadius = 5.5;
        }
        #endregion

        #region Public Functions
        /// <summary>Moves the player 1 cell in a given direction</summary>
        /// <param name="direction">The direction to move the player</param>
        public void Move(Direction direction)
        {
            Position += direction.Normal;
        }
        #endregion
    }
}
