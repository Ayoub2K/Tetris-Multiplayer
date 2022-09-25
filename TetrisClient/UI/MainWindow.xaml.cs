using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TetrisClient
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Engine Engine;
        private int time = 0;
        private DispatcherTimer Timer;

        public MainWindow()
        {
            InitializeComponent();
            Timer_Tick();
            Engine = new Engine();
            Engine.Start();
        }

        /// <summary>
        ///     starts a dispatchtimer that calls the dispatcherTimer_Tick() funtion every 50 miliseconds
        /// </summary>
        private void Timer_Tick()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += dispatcherTimer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            Timer.Start();
        }

        /// <summary>
        ///     calls upon updateRender()
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            updateRender();
        }

        /// <summary>
        ///     Clears Grid, Draws falling and landed Tetrimono's, Also checks gamestate every tick to check if the game is lost or paused
        /// </summary>
        private void updateRender()
        {
            TetrisGrid.Children.Clear();
            nextBlock.Children.Clear();

            checkGameState();
            renderScoreboard();
            
            DrawLandedTetromino();
            
            DrawTetronimo(Engine.NextTetremino, nextBlock);
            var ghostPiece = Engine.CreateGhostPiece();
            DrawTetronimo(ghostPiece, TetrisGrid, ghostPiece.OffsetX, ghostPiece.OffsetY, true);
            DrawTetronimo(Engine.CurrTetremino, TetrisGrid, Engine.CurrTetremino.OffsetX, Engine.CurrTetremino.OffsetY);

        }

        /// <summary>
        ///     Renders scoreboard
        /// </summary>
        private void renderScoreboard()
        {
            LevelValue.Content = Engine.Level;
            LinesValue.Content = Engine.Lines;
            ScoreValue.Content = Engine.Score;
        }

        /// <summary>
        ///     Draws tetrimono's that have already landed, takes information from Engine.Boardstate
        /// </summary>
        private void DrawLandedTetromino()
        {
            var board = Engine.BoardState.Board;

            for (var y = 0; y < board.GetLength(0); y++)
            for (var x = 0; x < board.GetLength(1); x++)
            {
                var block = board[y, x];
                if (block == 1)
                {
                    var rectangle = new Rectangle
                    {
                        Width = 25,
                        Height = 25,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Fill = Brushes.White
                    };
                    TetrisGrid.Children.Add(rectangle);

                    Grid.SetRow(rectangle, y);
                    Grid.SetColumn(rectangle, x);
                }
            }
        }

        
        /// <summary>
        ///     Draws tetrimono's based on offset and changes colors depending if its a ghost piece
        /// </summary>
        private void DrawTetronimo(Tetromino tetromino, Grid grid, int offsetX = 0, int offsetY = 0, bool ghost = false)
        {
            var values = tetromino.MatrixInt.Value;

            for (var i = 0; i < values.GetLength(0); i++)
            for (var j = 0; j < values.GetLength(1); j++)
            {
                if (values[i, j] != 1) continue;
                var rectangle = new Rectangle
                {
                    Width = 25,
                    Height = 25,
                    Stroke = Brushes.White,
                    StrokeThickness = 2,
                    Fill = Brushes.Black
                };
                if (ghost)
                {
                    rectangle.Stroke = Brushes.DimGray;
                }
                grid.Children.Add(rectangle);
                Grid.SetRow(rectangle, i + offsetY);
                Grid.SetColumn(rectangle, j + offsetX);
            }
        }

        
        /// <summary>
        ///     Check if gamestate is lost
        /// </summary>
        private void checkGameState()
        {
            if (Engine.GameState == GameStateEnum.Lost) GameOverText.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Toggle to pause game, stops Gametick in engine and in mainWindow
        /// </summary>
        private void pauseGame()
        {
            if (Engine.GameState != GameStateEnum.Paused)
            {
                Timer.IsEnabled = false;
                Engine.pauseGame();
                GamePausedText.Visibility = Visibility.Visible;
            }
            else
            {
                Timer.IsEnabled = true;
                Engine.pauseGame();
                GamePausedText.Visibility = Visibility.Hidden;
            }
        }
        
        /// <summary>
        ///     restarts game
        /// </summary>
        private void restartGame()
        {
            GameOverText.Visibility = Visibility.Hidden;
            Timer.Stop();
            Engine.restartGame();
            Timer_Tick();
            updateRender();
        }

        // Key Controlls
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.P:
                    pauseGame();
                    break;
                case Key.O:
                    restartGame();
                    break;
            }

            if (!Timer.IsEnabled) return;
            
            switch (e.Key)
            {
                case Key.D:
                    Engine.moveRight(Engine.CurrTetremino);
                    updateRender();
                    break;
                case Key.A:
                    Engine.moveLeft(Engine.CurrTetremino);
                    updateRender();
                    break;
                case Key.E:
                    Engine.Rotate90(Engine.CurrTetremino);
                    updateRender();
                    break;
                case Key.Q:
                    Engine.Rotate90CounterClockwise(Engine.CurrTetremino);
                    updateRender();
                    break;
                case Key.W:
                    Engine.HardDrop(Engine.CurrTetremino);
                    updateRender();
                    break;
                case Key.S:
                    Engine.softDrop(Engine.CurrTetremino);
                    updateRender();
                    break;
            }
        }
    }
}