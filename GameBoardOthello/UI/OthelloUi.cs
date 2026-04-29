using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.BackEnd.Structs;

namespace GameBoardOthello.UI;

public class OthelloUi
{
    private readonly GameController _game;

    public OthelloUi(GameController game)
    {
        _game = game;
    }

    public void Run()
    {
        _game.OnTurnSwitched += player =>
            Console.WriteLine($"  >> Giliran berpindah ke: {player.Name} ({player.PlayerColors})");

        _game.OnTurnSkipped += player =>
            Console.WriteLine($"  >> {player.Name} di-skip (tidak ada move valid).");

        _game.OnMoveMade += (b, p) =>
            Console.WriteLine($"  >> {p.Name} meletakkan disk.");

        _game.OnGameConcluded += b =>
            Console.WriteLine("  >> Permainan telah berakhir!");

        var started = _game.StartGame();
        if (!started)
        {
            Console.WriteLine("Gagal start game. Butuh minimal 2 players.");
            Console.WriteLine("\nTekan ENTER untuk keluar...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("\nGame dimulai! Tekan ENTER...");
        Console.ReadLine();

        RunGameLoop();

        Console.WriteLine("\nTekan ENTER untuk keluar...");
        Console.ReadLine();
    }

    private void RunGameLoop()
    {
        while (true)
        {
            var board = _game.GetBoard();
            var currentPlayer = _game.GetCurrentPlayer();
            var lastMove = _game.GetLastMovePosition();

            if (_game.IsBoardFull() || _game.IsBothPlayersCannotMove())
            {
                ConsoleRenderer.RenderBoard(board, new List<Position>());

                _game.EndGame();

                var players = _game.GetPlayers();
                ConsoleRenderer.ShowGameOver(board, players);
                break;
            }

            var validMoves = _game.GetValidMoves(currentPlayer);

            if (validMoves.Count == 0)
            {
                ConsoleRenderer.RenderBoard(board, validMoves, lastMove);
                Console.WriteLine($"\n{currentPlayer.Name} tidak punya move valid. Turn di-skip.");
                _game.NotifyTurnSkipped(currentPlayer);

                _game.SwitchTurn();

                Console.WriteLine("\nTekan ENTER...");
                Console.ReadLine();
                continue;
            }

            ConsoleRenderer.RenderBoard(board, validMoves, lastMove);
            ConsoleRenderer.ShowScore(board);
            Console.WriteLine($"\nGiliran: {currentPlayer.Name} ({currentPlayer.PlayerColors})");
            Console.WriteLine($"Valid moves: {string.Join(", ", validMoves.Select(p => $"({p.Row},{p.Col})"))}");

            var selectedPos = ConsoleInputHandler.GetMoveInput(validMoves);

            var targetSquare = board.Square[selectedPos.Row, selectedPos.Col];

            var moveSuccess = _game.PutDiskOnBoard(currentPlayer, targetSquare);

            if (!moveSuccess)
            {
                Console.WriteLine("ERROR: Move gagal! (tidak seharusnya terjadi jika validasi benar)");
                Console.WriteLine("Tekan ENTER...");
                Console.ReadLine();
                continue;
            }

            _game.SwitchTurn();

            Console.WriteLine("\nTekan ENTER...");
            Console.ReadLine();
        }
    }
}