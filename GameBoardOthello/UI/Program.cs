using GameBoardOthello.BackEnd.BackEnd.Enums;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.Interface;
using Serilog;

namespace GameBoardOthello.UI;

public class Program
{
    private const int BoardSize = 8;

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/myapp.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Console.Title = "Othello Game";
            ConsoleRenderer.ShowWelcome();

            (var p1Name, var p2Name) = ConsoleInputHandler.GetPlayerNames();

            IPlayer blackPlayer = new Player(p1Name, Colors.Black);
            IPlayer whitePlayer = new Player(p2Name, Colors.White);
            var players = new List<IPlayer> { blackPlayer, whitePlayer };
            IBoard board = new Board(new Square[BoardSize, BoardSize]);

            var gameController = new GameController(players, board);

            var consoleApp = new OthelloUi(gameController);

            consoleApp.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Log.Fatal(ex, "Aplikasi berhenti karena fatal error.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}