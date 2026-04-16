using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.UI;

public class ConsoleRenderer
{
    public static void ShowWelcome()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("       OTHELLO / REVERSI GAME       ");
        Console.WriteLine("====================================");
        Console.WriteLine();
        Console.WriteLine("Legenda:");
        Console.WriteLine("  B = Black    W = White");
        Console.WriteLine("  . = Kosong   * = Move Valid");
        Console.WriteLine();
        Console.WriteLine("Cara main: ketik 'row col' (contoh: 2 3)");
        Console.WriteLine("Ketik 'q' untuk keluar");
        Console.WriteLine();
    }

    public static void RenderBoard(IBoard board, List<Position> validMoves)
    {
        Console.Clear();
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);

        Console.Write("    ");
        for (int c = 0; c < cols; c++)
            Console.Write($" {c} ");
        Console.WriteLine();

        Console.Write("   +");
        for (int c = 0; c < cols; c++) Console.Write("---");
        Console.WriteLine("+");

        for (int r = 0; r < rows; r++)
        {
            Console.Write($" {r} |");
            for (int c = 0; c < cols; c++)
            {
                var sq = board.Square[r, c];
                bool isValid = validMoves.Any(p => p.Row == r && p.Col == c);

                if (sq?.Disk != null)
                {
                    Console.Write(sq.Disk.DiskColor == Colors.Black ? " B " : " W ");
                }
                else if (isValid)
                {
                    Console.Write(" * ");
                }
                else
                {
                    Console.Write(" . ");
                }
            }
            Console.WriteLine("|");
        }

        Console.Write("   +");
        for (int c = 0; c < cols; c++) Console.Write("---");
        Console.WriteLine("+");
    }

    public static void ShowScore(IBoard board)
    {
        var (black, white) = CountDisks(board);
        Console.WriteLine($"\nSkor -> Black (B): {black} | White (W): {white}");
    }

    public static void ShowGameOver(IBoard board)
    {
        var (black, white) = CountDisks(board);

        Console.WriteLine();
        Console.WriteLine("====================================");
        Console.WriteLine("            GAME OVER               ");
        Console.WriteLine("====================================");
        Console.WriteLine();
        Console.WriteLine($"  Black (B): {black}");
        Console.WriteLine($"  White (W): {white}");
        Console.WriteLine();

        if (black > white)
            Console.WriteLine(">>> PEMENANG: Black (B) <<<");
        else if (white > black)
            Console.WriteLine(">>> PEMENANG: White (W) <<<");
        else
            Console.WriteLine(">>> HASIL: SERI <<<");
    }

    private static (int black, int white) CountDisks(IBoard board)
    {
        int black = 0, white = 0;
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var disk = board.Square[r, c]?.Disk;
                if (disk != null)
                {
                    if (disk.DiskColor == Colors.Black) black++;
                    else white++;
                }
            }
        }

        return (black, white);
    }
}