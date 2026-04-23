using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.UI;

public class Program
{
     private const int BoardSize = 8;

    public static void Main(string[] args)
    {
        Console.Title = "Othello Game";
        ConsoleRenderer.ShowWelcome();

        var (p1Name, p2Name) = ConsoleInputHandler.GetPlayerNames();

        IPlayer blackPlayer = new Player(p1Name, Colors.Black);
        IPlayer whitePlayer = new Player(p2Name, Colors.White);
        var players = new List<IPlayer> { blackPlayer, whitePlayer };

        IBoard board = new Board(new Square[BoardSize, BoardSize]);

        var game = new GameController(players, board);

        game.OnTurnSwitched += (player) =>
            Console.WriteLine($"  >> Giliran berpindah ke: {player.Name} ({player.PlayerColors})");

        game.OnTurnSkipped += (player) =>
            Console.WriteLine($"  >> {player.Name} di-skip (tidak ada move valid).");

        game.OnMoveMade += (b, p) =>
            Console.WriteLine($"  >> {p.Name} meletakkan disk.");

        game.OnGameConcluded += (b) =>
            Console.WriteLine("  >> Permainan telah berakhir!");

        game.StartGame();
        Console.WriteLine("\nGame dimulai! Tekan ENTER...");
        Console.ReadLine();

        RunGameLoop(game);

        Console.WriteLine("\nTekan ENTER untuk keluar...");
        Console.ReadLine();
    }

    private static void RunGameLoop(GameController game)
    {
        while (true)
        {
            IBoard board = game.GetBoard(null!);
            IPlayer currentPlayer = game.GetCurrentPlayer(null!);
            int totalDisks = game.GetTotalDisks(0);
            Position? lastMove = game.GetLastMovePosition();
            
            if (game.IsBoardFull(totalDisks) || game.IsBothPlayersCannotMove())
            {
                ConsoleRenderer.RenderBoard(board, new List<Position>());
                game.EndGame();
                
                var players = game.GetPlayer(null!);
                ConsoleRenderer.ShowGameOver(board, players);
                break;
            }

            var validMoves = game.GetValidMoves(currentPlayer);

            if (validMoves.Count == 0)
            {
                ConsoleRenderer.RenderBoard(board, validMoves, lastMove);
                Console.WriteLine($"\n{currentPlayer.Name} tidak punya move valid. Turn di-skip.");
                game.NotifyTurnSkipped(currentPlayer);
                game.SwitchTurn(currentPlayer);
                Console.WriteLine("\nTekan ENTER...");
                Console.ReadLine();
                continue;
            }

            ConsoleRenderer.RenderBoard(board, validMoves, lastMove);
            ConsoleRenderer.ShowScore(board);
            Console.WriteLine($"\nGiliran: {currentPlayer.Name} ({currentPlayer.PlayerColors})");
            Console.WriteLine($"Valid moves: {string.Join(", ", validMoves.Select(p => $"({p.Row},{p.Col})"))}");

            Position selectedPos = ConsoleInputHandler.GetMoveInput(validMoves);

            Square targetSquare = board.Square[selectedPos.Row, selectedPos.Col];
            game.PutDiskOnBoard(currentPlayer, targetSquare, totalDisks);

            game.SwitchTurn(currentPlayer);

            Console.WriteLine("\nTekan ENTER...");
            Console.ReadLine();
        }
    }
}