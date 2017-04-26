using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDungeonGame
{
    

    public partial class Map
    {
        private char[,] data;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Position SpawnPoint { get; private set; }

        public char this[int x, int y]
        {
            get { return (coordsInRange(x, y)) ? data[x, y] : 'N'; }
            set { if (coordsInRange(x, y)) { data[x, y] = value; } }
        }

        public char this[Position p]
        {
            get { return this[p.X, p.Y]; }
            set { this[p.X, p.Y] = value; }
        }

        public Map(int width, int height)
        {
            Width = (width % 2 == 0) ? width - 1 : width;
            Height = (height % 2 == 0) ? height - 1 : height;
            generateMap();
        }

        private bool coordsInRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Width;
        }

        public bool tileIsClear(int x, int y)
        {
            return coordsInRange(x, y) && (this[x, y] == ' ' || this[x, y] == 'R');
        }

        public bool tileIsClear(Position p)
        {
            return tileIsClear(p.X, p.Y);
        }

    }
}
