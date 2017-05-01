namespace TextDungeonGame
{
    class Program
    {
        #region Private Properties
        static private Game Game;
        #endregion

        #region Start
        static void Main(string[] args)
        {
            //Starts a new game
            Game = new Game(60);
        }
        #endregion
    }
}
