using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;

namespace TetrisClient
{
    public partial class MultiplayerWindow : Window
    {
        private readonly HubConnection _connection;
        
        // private int[,] enemyBoardState;
        // private GameStateEnum enemyGameState;
        // private int enemyLevel;
        // private int enemyLines;
        // private int enemyScore;
        // private Tetromino enemyTetromino;
        // private Tetromino enemyTetrominoNext;

        private Engine EngineP2;
        
        private Engine EngineP1;
        private int time = 0;
        private DispatcherTimer Timer;


        public MultiplayerWindow()
        {
            InitializeComponent();

            // De url waar de meegeleverde TetrisHub op draait:
            const string url = "http://127.0.0.1:5000/TetrisHub";

            // De Builder waarmee de connectie aangemaakt wordt:
            _connection = new HubConnectionBuilder().WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            // De eerste paramater moet gelijk zijn met de methodenaam in TetrisHub.cs
            // Wat er tussen de <..> staat bepaald wat de type van de paramater `seed` is.
            // Op deze manier loopt het onderstaande gelijk met de methode in TetrisHub.cs.

            _connection.On<int>("ReadyUp", seed => Dispatcher.BeginInvoke(new Action(() =>
                StartGame(seed))));

            _connection.On<int>("SendLevel", level => Dispatcher.BeginInvoke(new Action(() =>
                EngineP2.Level = level)));
            _connection.On<int>("SendScore", score => Dispatcher.BeginInvoke(new Action(() =>
                EngineP2.Score = score)));
            _connection.On<int>("SendLines", lines => Dispatcher.BeginInvoke(new Action(() =>
                EngineP2.Lines = lines)));
            _connection.On<string>("SendBoardstate", boardState => Dispatcher.BeginInvoke(new Action(() =>
                EngineP2.BoardState = JsonSerializer.Deserialize<BoardState>(boardState))));
            _connection.On<string>("SendTetromino", currTetremino => Dispatcher.BeginInvoke(new Action(() =>
                EngineP2.CurrTetremino = JsonSerializer.Deserialize<Tetromino>(currTetremino))));
            _connection.On<string>("SendGameState", gameState => Dispatcher.BeginInvoke(new Action(() =>
                EngineP2.GameState = JsonSerializer.Deserialize<GameStateEnum>(gameState))));
            _connection.On<string>("SendNextTetromino", nextTetromino => Dispatcher.BeginInvoke(new Action(() =>
                EngineP2.NextTetremino = JsonSerializer.Deserialize<Tetromino>(nextTetromino))));

            // Let op: het starten van de connectie moet *nadat* alle event listeners zijn gezet!
            // Als de methode waarin dit voorkomt al `async` (asynchroon) is, dan kan `Task.Run` weggehaald worden.
            // In het startersproject staat dit in de constructor, daarom is dit echter wel nodig:
            Task.Run(async () => await _connection.StartAsync());
        }

        private void StartGame(int seed)
        {
            EngineP1 = new Engine();
            EngineP2 = new Engine();
            
            EngineP1.Start();
            EngineP2.Start();
            Timer_Tick();
            ReadyButton.Visibility = Visibility.Hidden;
        }

        // Events kunnen `async` zijn in WPF:
        private async void StartGame_OnClick(object sender, RoutedEventArgs e)
        {
            // Als de connectie nog niet is geïnitialiseerd, dan kan er nog niks verstuurd worden:
            if (_connection.State != HubConnectionState.Connected) return;

            var seed = Guid.NewGuid().GetHashCode();

            // Het aanroepen van de TetrisHub.cs methode `ReadyUp`.
            // Hier geven we de int mee die de methode `ReadyUp` verwacht.
            await _connection.InvokeAsync("ReadyUp", seed);
            StartGame(seed);
        }

        private void Timer_Tick()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += dispatcherTimer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            Timer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            clientToServer();
            updateRender();
        }

        private void checkGameState()
        {
            if (EngineP1.GameState == GameStateEnum.Lost)
            {
                GameOverP1.Visibility = Visibility.Visible;
                GameWonP2.Visibility = Visibility.Visible;
            }else if (EngineP2.GameState == GameStateEnum.Lost)
            {
                GameOverP2.Visibility = Visibility.Visible;
                GameWonP1.Visibility = Visibility.Visible;
                EngineP1.pauseGame();
            }
        }

        
        private void clientToServer()
        {
            Task.Run(async () =>
                await _connection.InvokeAsync("SendLevel", EngineP1.Level));
            Task.Run(async () =>
                await _connection.InvokeAsync("SendLines", EngineP1.Lines));
            Task.Run(async () =>
                await _connection.InvokeAsync("SendScore", EngineP1.Score));
            Task.Run(async () =>
                await _connection.InvokeAsync("SendBoardstate", JsonSerializer.Serialize(EngineP1.BoardState.Board)));
            Task.Run(async () =>
                await _connection.InvokeAsync("SendTetromino", JsonSerializer.Serialize(EngineP1.CurrTetremino)));
            Task.Run(async () =>
                await _connection.InvokeAsync("SendGameState", JsonSerializer.Serialize(EngineP1.GameState)));
            Task.Run(async () =>
                await _connection.InvokeAsync("SendNextTetromino", JsonSerializer.Serialize(EngineP1.NextTetremino)));
        }

        private void updateRender()
        {
            checkGameState();
            TetrisGridP1.Children.Clear();
            TetrisGridP2.Children.Clear();
            nextBlockP1.Children.Clear();
            nextBlockP2.Children.Clear();

            renderScoreboardP1();
            renderScoreboardP2();
            
            DrawLandedTetrominoP1();
            DrawLandedTetrominoP2();
            
            DrawTetronimo(EngineP1.NextTetremino, nextBlockP1);
            DrawTetronimo(EngineP2.NextTetremino, nextBlockP2);
            
            
            DrawTetronimo(EngineP1.CurrTetremino, TetrisGridP1, EngineP1.CurrTetremino.OffsetX,
                EngineP1.CurrTetremino.OffsetY);
            DrawTetronimo(EngineP2.CurrTetremino, TetrisGridP2, EngineP2.CurrTetremino.OffsetX,
                EngineP2.CurrTetremino.OffsetY);
            
            var ghostPieceP1 = EngineP1.CreateGhostPiece();
            var ghostPieceP2 = EngineP2.CreateGhostPiece();
            DrawTetronimo(ghostPieceP1, TetrisGridP1, ghostPieceP1.OffsetX, ghostPieceP1.OffsetY, true);
            DrawTetronimo(ghostPieceP2, TetrisGridP2, ghostPieceP2.OffsetX, ghostPieceP2.OffsetY, true);
        }

        private void renderScoreboardP1()
        {
            LevelValueP1.Content = EngineP1.Level;
            LinesValueP1.Content = EngineP1.Lines;
            ScoreValueP1.Content = EngineP1.Score;
        }

        private void renderScoreboardP2()
        {
            LevelValueP2.Content = EngineP2.Level;
            LinesValueP2.Content = EngineP2.Lines;;
            ScoreValueP2.Content = EngineP2.Score;;
        }

        private void DrawLandedTetrominoP1()
        {
            var board = EngineP1.BoardState.Board;

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
                    TetrisGridP1.Children.Add(rectangle);

                    Grid.SetRow(rectangle, y);
                    Grid.SetColumn(rectangle, x);
                }
            }
        }

        private void DrawLandedTetrominoP2()
        {
            var board = EngineP2.BoardState.Board;;
        
            if (board == null)
            {
                return;
            }
        
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
                    TetrisGridP2.Children.Add(rectangle);
        
                    Grid.SetRow(rectangle, y);
                    Grid.SetColumn(rectangle, x);
                }
            }
        }

        private void DrawTetronimo(Tetromino tetromino, Grid grid, int offsetX = 0, int offsetY = 0, bool ghost = false)
        {

            if (tetromino == null) {
                return;
            }

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
                if (ghost) rectangle.Stroke = Brushes.DimGray;
                grid.Children.Add(rectangle);
                Grid.SetRow(rectangle, i + offsetY);
                Grid.SetColumn(rectangle, j + offsetX);
            }
        }

        // Key Controlls
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                    EngineP1.moveRight(EngineP1.CurrTetremino);
                    updateRender();
                    break;
                case Key.A:
                    EngineP1.moveLeft(EngineP1.CurrTetremino);
                    updateRender();
                    break;
                case Key.E:
                    EngineP1.Rotate90(EngineP1.CurrTetremino);
                    updateRender();
                    break;
                case Key.Q:
                    EngineP1.Rotate90CounterClockwise(EngineP1.CurrTetremino);
                    updateRender();
                    break;
                case Key.W:
                    EngineP1.HardDrop(EngineP1.CurrTetremino);
                    updateRender();
                    break;
                case Key.S:
                    EngineP1.softDrop(EngineP1.CurrTetremino);
                    updateRender();
                    break;
            }
        }
    }
}