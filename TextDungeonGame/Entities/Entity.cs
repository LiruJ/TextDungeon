namespace TextDungeonGame.Entities
{
    public abstract class Entity
    {
        #region Public Properties
        /// <summary>The position of this entity</summary>
        public Position Position { get; protected set; }

        /// <summary>The character of this entity</summary>
        public char character { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>Creates a new entity at the given location, only called by inherited classes</summary>
        /// <param name="pos">The position to place the entity</param>
        public Entity(Position pos)
        {
            Position = pos;
        }
        #endregion
    }
}
