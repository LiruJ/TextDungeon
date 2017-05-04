namespace TextDungeonGame.Entities
{
    public enum Entities { OpenDoor = '\'', ClosedDoor = '%', Player = '@' }

    public abstract class Entity
    {
        #region Public Properties
        /// <summary>The position of this entity</summary>
        public Position Position { get; protected set; }

        /// <summary>The character of this entity</summary>
        public char Character { get; protected set; }

        /// <summary>Whether or not the player can see through this entity</summary>
        public bool IsTransparent { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>Creates a new entity at the given location, only called by inherited classes</summary>
        /// <param name="pos">The position to place the entity</param>
        public Entity(Position pos)
        {
            Position = pos;
        }
        #endregion

        #region Public Functions
        public virtual void Update(int currentTick)
        {

        }
        #endregion
    }
}
