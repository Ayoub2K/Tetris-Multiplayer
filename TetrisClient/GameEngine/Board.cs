namespace TetrisClient
{
    public class BoardState
    {
        public int[,] Board;

        /// <summary>
        ///     Constructor that creates multidimensional array as empty board
        /// </summary>
        private static int[,] GenerateNewBoard()
        {
            return new[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
        }

        /// <summary>
        ///     places tetromino in current board
        /// </summary>
        public void setTetrominoInBoard(Tetromino tetromino)
        {
            for (var yLine = 0; yLine < tetromino.MatrixInt.Value.GetLength(0); yLine++)
            {
                for (var xLine = 0; xLine < tetromino.MatrixInt.Value.GetLength(1); xLine++)
                {
                    if (tetromino.MatrixInt.Value[yLine, xLine] == 1)
                        Board[yLine + tetromino.OffsetY, xLine + tetromino.OffsetX] = 1;
                }
            }
        }

        /// <summary>
        ///     creates an empty board
        /// </summary>
        public void newGame()
        {
            Board = GenerateNewBoard();
        }
    }
}