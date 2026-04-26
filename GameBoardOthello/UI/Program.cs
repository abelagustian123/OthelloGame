using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;
using GameBoardOthello.BackEnd.Structs;
using GameBoardOthello.UI;

namespace GameBoardOthello.UI;

public class Program
{
    private const int BoardSize = 8;

    public static void Main(string[] args)
    {
        Console.Title = "Othello Game";
        ConsoleRenderer.ShowWelcome();

        (string p1Name, string p2Name) = ConsoleInputHandler.GetPlayerNames();

        IPlayer blackPlayer = new Player(p1Name, Colors.Black);
        IPlayer whitePlayer = new Player(p2Name, Colors.White);
        List<IPlayer> players = new List<IPlayer> { blackPlayer, whitePlayer };

        IBoard board = new Board(new Square[BoardSize, BoardSize]);

        GameController game = new GameController(players, board);

        // Subscribe to events
        game.OnTurnSwitched += (player) =>
            Console.WriteLine($"  >> Giliran berpindah ke: {player.Name} ({player.PlayerColors})");

        game.OnTurnSkipped += (player) =>
            Console.WriteLine($"  >> {player.Name} di-skip (tidak ada move valid).");

        game.OnMoveMade += (b, p) =>
            Console.WriteLine($"  >> {p.Name} meletakkan disk.");

        game.OnGameConcluded += (b) =>
            Console.WriteLine("  >> Permainan telah berakhir!");

        // StartGame() now returns bool
        bool started = game.StartGame();
        if (!started)
        {
            Console.WriteLine("Gagal start game. Butuh minimal 2 players.");
            Console.WriteLine("\nTekan ENTER untuk keluar...");
            Console.ReadLine();
            return;
        }

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
            IBoard board = game.GetBoard();
            IPlayer currentPlayer = game.GetCurrentPlayer();
            Position? lastMove = game.GetLastMovePosition();

            if (game.IsBoardFull() || game.IsBothPlayersCannotMove())
            {
                ConsoleRenderer.RenderBoard(board, new List<Position>());
                
                game.EndGame();

                List<IPlayer> players = game.GetPlayers();
                ConsoleRenderer.ShowGameOver(board, players);
                break;
            }

            List<Position> validMoves = game.GetValidMoves(currentPlayer);

            if (validMoves.Count == 0)
            {
                ConsoleRenderer.RenderBoard(board, validMoves, lastMove);
                Console.WriteLine($"\n{currentPlayer.Name} tidak punya move valid. Turn di-skip.");
                game.NotifyTurnSkipped(currentPlayer);
                
                IPlayer nextPlayer = game.SwitchTurn();
                
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
            
            bool moveSuccess = game.PutDiskOnBoard(currentPlayer, targetSquare);
            
            if (!moveSuccess)
            {
                Console.WriteLine("ERROR: Move gagal! (tidak seharusnya terjadi jika validasi benar)");
                Console.WriteLine("Tekan ENTER...");
                Console.ReadLine();
                continue;
            }

            IPlayer nextPlayer2 = game.SwitchTurn();

            Console.WriteLine("\nTekan ENTER...");
            Console.ReadLine();
        }
    }
}