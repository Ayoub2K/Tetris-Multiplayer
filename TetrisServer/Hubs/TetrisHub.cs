using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TetrisServer.Hubs
{
    public class TetrisHub : Hub
    {
        public async Task DropShape()
        {
            await Clients.Others.SendAsync("DropShape");
        }

        public async Task RotateShape(string direction)
        {
            await Clients.Others.SendAsync("RotateShape", direction);
        }

        public async Task MoveShape(string moveDirection)
        {
            await Clients.Others.SendAsync("MoveShape", moveDirection);
        }

        public async Task ReadyUp(int seed)
        {
            await Clients.Others.SendAsync("ReadyUp", seed);
        }

        public async Task SendScore(int score)
        {
            await Clients.Others.SendAsync("SendScore", score);
        }

        public async Task SendLines(int lines)
        {
            await Clients.Others.SendAsync("SendLines", lines);
        }

        public async Task SendLevel(int level)
        {
            await Clients.Others.SendAsync("SendLevel", level);
        }

        public async Task SendBoardstate(string boardState)
        {
            await Clients.Others.SendAsync("SendBoardstate", boardState);
        }

        public async Task SendTetromino(string tetromino)
        {
            await Clients.Others.SendAsync("SendTetromino", tetromino);
        }

        public async Task SendGameState(string gameState)
        {
            await Clients.Others.SendAsync("SendGameState", gameState);
        }

        public async Task SendNextTetromino(string nextTetromino)
        {
            await Clients.Others.SendAsync("SendNextTetromino", nextTetromino);
        }
    }
}