using System;
using System.Collections.Generic;
using System.Windows.Threading;
using static System.Linq.Enumerable;

namespace TetrisClient
{
    public class Engine
    {
        private DispatcherTimer Timer;
        public GameStateEnum GameState { get; set; }
        public BoardState BoardState { get; set; }
        public int Level { get; set; }
        public int Lines { get; set; }
        public int Score { get; set; }
        public Tetromino CurrTetremino { get; set; }
        public Tetromino NextTetremino { get; set; }

        /// <summary>
        ///     Multridimensional array that creates an empty board
        /// </summary>
        public void Start()
        {
            GameState = GameStateEnum.Playing;
            BoardState = new BoardState();
            BoardState.newGame();
            Level = 1;
            Lines = 0;
            Score = 0;
            CurrTetremino = new Tetromino();
            NextTetremino = new Tetromino();
            Timer_Tick();
        }

        /// <summary>
        ///     starts a dispatchtimer that calls the dispatcherTimer_Tick() funtion every 50 miliseconds
        /// </summary>
        private void Timer_Tick()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += dispatcherTimer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 0, 1);
            Timer.Start();
        }

        /// <summary>
        ///     Checks every tick if its possible to move
        ///     also handles level and score, changes gamespeed if you hit a new level
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //elke tick
            //drop 1 tile if possible
            if (!moveDown(CurrTetremino))
            {
                Score += CheckFullLines() switch
                {
                    1 => 40 * Level,
                    2 => 100 * Level,
                    3 => 300 * Level,
                    4 => 1200 * Level,
                    _ => 0
                };
                NextTetromino();
            }

            // set game speed depending on level
            Timer.Interval = Level switch
            {
                2 => new TimeSpan(0, 0, 0, 0, 800),
                3 => new TimeSpan(0, 0, 0, 0, 600),
                4 => new TimeSpan(0, 0, 0, 0, 400),
                5 => new TimeSpan(0, 0, 0, 0, 200),
                6 => new TimeSpan(0, 0, 0, 0, 50),
                _ => new TimeSpan(0, 0, 0, 1)
            };
        }

        //GAMELOGIC
        /// <summary>
        ///     checks if given tetrino with offset is still in the board
        /// </summary>
        public bool isInBoard(Tetromino tetromino, int moveX, int moveY)
        {
            for (var y = 0; y < tetromino.MatrixInt.Value.GetLength(0); y++)
            for (var x = 0; x < tetromino.MatrixInt.Value.GetLength(1); x++)
                if (tetromino.MatrixInt.Value[y, x] == 1)
                {
                    var yWithNextMove = y + tetromino.OffsetY + moveY;
                    var xWithNextMove = x + tetromino.OffsetX + moveX;

                    if (yWithNextMove >= 16)
                        return false;
                    if (xWithNextMove is > 9 or < 0)
                        return false;
                }

            return true;
        }

        /// <summary>
        ///     checks if given tetrino with offset will colide with other blocks
        /// </summary>
        public bool willNotColide(Tetromino tetromino, int moveX, int moveY)
        {
            for (var y = 0; y < tetromino.MatrixInt.Value.GetLength(0); y++)
            for (var x = 0; x < tetromino.MatrixInt.Value.GetLength(1); x++)
                if (tetromino.MatrixInt.Value[y, x] == 1)
                    if (BoardState.Board[y + tetromino.OffsetY + moveY, x + tetromino.OffsetX + moveX] != 0)
                        return false;


            return true;
        }

        /// <summary>
        ///     swaps CurrentTetromino with nextTetromino and creates a new NextTetromino
        /// </summary>
        public void NextTetromino()
        {
            CurrTetremino = NextTetremino;

            if (!willNotColide(CurrTetremino, 0, 0))
            {
                Timer.Stop();
                GameState = GameStateEnum.Lost;
            }

            NextTetremino = new Tetromino();
        }

        /// <summary>
        ///     update level based on how many lines you cleared
        /// </summary>
        private void UpdateLevel()
        {
            Level = Lines switch
            {
                <= 10 => 1,
                >= 10 and < 20 => 2,
                >= 20 and < 30 => 3,
                >= 30 and < 40 => 4,
                >= 40 and < 50 => 5,
                _ => 6
            };
        }

        //LINE DELETION LOGIC

        /// <summary>
        ///     checks which y coordinate has a full horizontal line
        /// </summary>
        private List<int> FindLines()
        {
            var fullLines = new List<int>();
            for (var yLine = 0; yLine < BoardState.Board.GetLength(0); yLine++)
                if (Range(0, BoardState.Board.GetLength(1)).Select(xLine => BoardState.Board[yLine, xLine]).ToList()
                        .FindAll(xLine => xLine > 0).Count == 10)
                    fullLines.Add(yLine);
            return fullLines;
        }

        /// <summary>
        ///     removes given y coordinate and everyline and calls DropLastTetrimonos()
        ///     with every removed line increases lines scoreboard + runs UpdateLevel()
        /// </summary>
        private int ClearFullLines(ICollection<int> fullLines)
        {
            var deletedLines = 0;
            for (var yLine = 0; yLine < BoardState.Board.GetLength(0); yLine++)
            {
                if (!fullLines.Contains(yLine))
                    continue;
                for (var xLine = 0; xLine < BoardState.Board.GetLength(1); xLine++)
                    BoardState.Board[yLine, xLine] = 0;

                DropLastTetriminos(yLine);
                deletedLines++;
                Lines++;
                UpdateLevel();
                fullLines.Remove(yLine);
            }

            return deletedLines;
        }

        /// <summary>
        ///     based on given Lines every block above will drop
        /// </summary>
        private void DropLastTetriminos(int deletedLine)
        {
            for (var yLine = deletedLine; yLine > 0; yLine--)
            for (var xLine = 0; xLine < BoardState.Board.GetLength(1); xLine++)
                BoardState.Board[yLine, xLine] = BoardState.Board[yLine - 1, xLine];
        }

        /// <summary>
        ///     checkFullLines() calls on ClearFullLines() and FindLines()
        /// </summary>
        public int CheckFullLines()
        {
            return ClearFullLines(FindLines());
        }

        /// <summary>
        ///     Toggle to pause game, stops timer
        /// </summary>
        public bool pauseGame()
        {
            if (GameState == GameStateEnum.Paused)
            {
                Timer.IsEnabled = true;
                GameState = GameStateEnum.Playing;
                return false;
            }

            Timer.IsEnabled = false;
            GameState = GameStateEnum.Paused;
            return true;
        }

        /// <summary>
        ///     stops timer and starts game again
        /// </summary>
        public void restartGame()
        {
            Timer.Stop();
            Start();
        }

        /// <summary>
        ///     Creates ghostpiece with offsety++ till it will collide
        /// </summary>
        public Tetromino CreateGhostPiece()
        {
            var ghostPiece = new Tetromino(
                CurrTetremino.OffsetX,
                CurrTetremino.OffsetY,
                CurrTetremino.Shape,
                CurrTetremino.MatrixInt);

            while (isInBoard(ghostPiece, 0, 1) && willNotColide(ghostPiece, 0, 1)) ghostPiece.OffsetY++;

            return ghostPiece;
        }


        // MOVEMENT, all movement check if they will collide of not movement will happen otherwise block will get stuck
        public bool moveDown(Tetromino tetromino)
        {
            if (isInBoard(tetromino, 0, 1) && willNotColide(tetromino, 0, 1))
            {
                tetromino.OffsetY++;
                return true;
            }

            BoardState.setTetrominoInBoard(tetromino);
            return false;
        }

        public void moveRight(Tetromino tetromino)
        {
            if (isInBoard(tetromino, 1, 0) && willNotColide(tetromino, 1, 0)) tetromino.OffsetX++;
        }

        public void moveLeft(Tetromino tetromino)
        {
            if (isInBoard(tetromino, -1, 0) && willNotColide(tetromino, -1, 0)) tetromino.OffsetX--;
        }

        /// <summary>
        ///     rotation movement makes a new tetromino that will rotate, if it gets stuck the keystroke wont happen
        /// </summary>
        public void Rotate90(Tetromino tetromino)
        {
            var rotate90 = tetromino.MatrixInt.Rotate90();
            var checkerTetromino = new Tetromino(tetromino.OffsetX, tetromino.OffsetY, tetromino.Shape, rotate90);
            if (isInBoard(checkerTetromino, 0, 0) && willNotColide(checkerTetromino, 0, 0))
                tetromino.MatrixInt = tetromino.MatrixInt.Rotate90();
        }

        public void Rotate90CounterClockwise(Tetromino tetromino)
        {
            var rotate90Counter = tetromino.MatrixInt.Rotate90CounterClockwise();
            var checkerTetromino =
                new Tetromino(tetromino.OffsetX, tetromino.OffsetY, tetromino.Shape, rotate90Counter);
            if (isInBoard(checkerTetromino, 0, 0) && willNotColide(checkerTetromino, 0, 0))
                tetromino.MatrixInt = tetromino.MatrixInt.Rotate90CounterClockwise();
        }

        /// <summary>
        ///     drops tetrimono instantly to ghostPiece place
        /// </summary>
        public void HardDrop(Tetromino tetromino)
        {
            while (moveDown(tetromino)) Score++;
        }

        /// <summary>
        ///     extra move down
        /// </summary>
        public bool softDrop(Tetromino tetromino)
        {
            if (moveDown(tetromino))
            {
                Score++;
                return true;
            }

            return false;
        }
    }
}