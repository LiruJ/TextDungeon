using System.Collections.Generic;
using TextDungeonGame.Entities;

namespace TextDungeonGame
{
    /// <summary>The interface used by anything wanting to draw the map</summary>
    public interface IDrawableMap
    {
        #region Properites
        int Width { get; }
        int Height { get; }
        Player Player { get; }
        #endregion

        #region Indexers
        Map.Cells this[int x, int y] { get; }
        Map.Cells this[Position pos] { get; }
        #endregion

        #region Functions
        bool CellIsTransparent(Position pos);
        bool CellHasEntity(Position pos);
        Entity GetEntityAtPosition(Position pos);
        #endregion
    }

    public partial class Map : IDrawableMap
    {
        #region Public Properties
        /// <summary>The width of the map</summary>
        public int Width { get; private set; }

        /// <summary>The height of the map</summary>
        public int Height { get; private set; }

        /// <summary>The position at which the player will spawn</summary>
        public Position SpawnPoint { get; private set; }

        /// <summary>The player entity</summary>
        public Player Player { get; private set; }

        /// <summary>The cells and their assigned character</summary>
        public enum Cells { Wall = '#', Floor = '.', Room = 'R', OutOfRange = 'N', Fog = ' ' }
        #endregion

        #region Private Properties
        /// <summary>The map's data, this[] should be used instead of directly changing this array</summary>
        private Cells[,] data;

        /// <summary>List of entities on this map</summary>
        private List<Entity> Entities;

        /// <summary>The amount of ticks the game has run</summary>
        private int currentTick;

        /// <summary>Holds info on if a cell has an entity on it</summary>
        private bool[,] cellEntities;
        #endregion

        #region Indexers
        /// <summary>Gets the cell at x,y</summary>
        /// <param name="x">The x position of the cell</param>
        /// <param name="y">The y position of the cell</param>
        /// <returns>The character at x,y, or 'N' if x,y is out of range</returns>
        public Cells this[int x, int y]
        {
            get { return (coordsInRange(x, y)) ? data[x, y] : Cells.OutOfRange; }
            private set { if (coordsInRange(x, y)) { data[x, y] = value; } }
        }

        /// <summary>Gets the cell at pos</summary>
        /// <param name="pos">The position of the cell</param>
        /// <returns>The character at pos, or 'N' if pos is out of range</returns>
        public Cells this[Position pos]
        {
            get { return this[pos.X, pos.Y]; }
            set { this[pos.X, pos.Y] = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates a new map with the given width and height</summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Map(int width, int height)
        {
            //If the width or height is even, make it odd
            Width = (width % 2 == 0) ? width - 1 : width;
            Height = (height % 2 == 0) ? height - 1 : height;

            //Generates the map
            generateMap();

            //Creates the player at the spawn point
            Player = new Player(SpawnPoint);
        }
        #endregion

        #region Public Functions
        /// <summary>Updates the entities in the map</summary>
        /// <param name="playerMove">The direction to move the player</param>
        public void Update(Direction playerMove)
        {
            //If the player can't move, don't update the map
            if (!cellIsWalkable(Player.Position + playerMove.Normal)) return;

            updateEntities(Player.Position + playerMove.Normal);

            //Move the player in their desired direction
            Player.Move(playerMove);

            //Increment the tick
            currentTick++;
        }

        /// <summary>Finds if a cell allows vision to pass over it</summary>
        /// <param name="pos">The position of the cell to check</param>
        /// <returns>Whether or not the cell is transparent</returns>
        public bool CellIsTransparent(Position pos)
        {
            //The transparency of the entity occupying this cell, if no entity is there, defaults to true
            bool entityTransparent = (CellHasEntity(pos)) ? GetEntityAtPosition(pos).IsTransparent : true;

            //If the cell itself is transparent and the entity on this cell is transparent or does not exist
            return coordsInRange(pos) && (this[pos] == Cells.Floor) && entityTransparent;
        }

        /// <summary>Gets the first entity in the list at the given position</summary>
        /// <param name="pos">The position to check</param>
        /// <returns>The entity at that position, nor null if no entity is found</returns>
        public Entity GetEntityAtPosition(Position pos)
        {
            return Entities.Find(item => item.Position == pos);
        }

        /// <summary>Adds an entity to the map</summary>
        /// <param name="entity">The entity to add</param>
        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
            cellEntities[entity.Position.X, entity.Position.Y] = true; 
        }

        /// <summary>Finds if a cell has an entity on it</summary>
        /// <param name="pos">The position of the cell to check</param>
        /// <returns>Whether or not the cell has an entity on it</returns>
        public bool CellHasEntity(Position pos)
        {
            return coordsInRange(pos) && cellEntities[pos.X, pos.Y];
        }
        #endregion

        #region Private Functions
        /// <summary>Finds if x,y is in range of the map</summary>
        /// <param name="x">The x position to check</param>
        /// <param name="y">The y position to check</param>
        /// <returns>Whether or not x,y is in range of the map</returns>
        private bool coordsInRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Width;
        }

        /// <summary>Finds if pos is in range of the map</summary>
        /// <param name="pos">The position to check</param>
        /// <returns>Whether or not pos is in range of the map</returns>
        private bool coordsInRange(Position pos)
        {
            return coordsInRange(pos.X, pos.Y);
        }

        /// <summary>Finds if the cell at pos can be walked on</summary>
        /// <param name="pos">The position to check</param>
        /// <returns>Whether or not the cell is walkable</returns>
        private bool cellIsWalkable(Position pos)
        {
            return coordsInRange(pos) && (this[pos] == Cells.Floor);
        }

        /// <summary>Updates all the entities within the map and handles special interactions</summary>
        /// <param name="nextPosition">The position of the cell the player will move to</param>
        private void updateEntities(Position nextPosition)
        {
            //The entity that the player is attempting to walk onto
            Entity adjacentEntity = GetEntityAtPosition(nextPosition);

            //Handles updating special entities
            switch (adjacentEntity)
            {
                case Door d:
                    d.OpenDoor(currentTick);
                    break;
            }

            //Updates the entities
            foreach (Entity e in Entities)
                e.Update(currentTick);
        }
        #endregion
    }
}
