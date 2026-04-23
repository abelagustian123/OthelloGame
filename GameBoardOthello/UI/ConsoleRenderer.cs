using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.UI;

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

    public static void RenderBoard(IBoard board, List<Position> validMoves, Position? lastMove = null)
    {
        Console.Clear();
        var rows = board.Square.GetLength(0);
        var cols = board.Square.GetLength(1);

        // Cetak header kolom (0 1 2 3...)
        Console.Write("    ");
        for (var c = 0; c < cols; c++)
            Console.Write($" {c} ");
        Console.WriteLine();

        // Cetak garis batas atas
        Console.Write("   +");
        for (var c = 0; c < cols; c++) Console.Write("---");
        Console.WriteLine("+");

        for (var r = 0; r < rows; r++)
        {
            // Cetak nomor baris dan garis batas kiri (Warna Default Terminal)
            Console.Write($" {r} |");

            // ---> MENGAKTIFKAN BACKGROUND HIJAU UNTUK AREA PAPAN <---
            Console.BackgroundColor = ConsoleColor.Gray;

            for (var c = 0; c < cols; c++)
            {
                var sq = board.Square[r, c];
                var isValid = validMoves.Any(p => p.Row == r && p.Col == c);
                var isLastMove = lastMove.HasValue && lastMove.Value.Row == r && lastMove.Value.Col == c;

                if (sq.Disk != null)
                {
                    // Tentukan warna teks (Foreground) Hitam atau Putih sesuai warna pion
                    Console.ForegroundColor =
                        sq.Disk.DiskColor == Colors.Black ? ConsoleColor.Black : ConsoleColor.White;

                    if (isLastMove)
                        Console.Write(sq.Disk.DiskColor == Colors.Black ? "[B]" : "[W]");
                    else
                        Console.Write(sq.Disk.DiskColor == Colors.Black ? " B " : " W ");
                }
                else if (isValid)
                {
                    // Warna kuning untuk penanda langkah yang valid
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(" * ");
                }
                else
                {
                    // Tips UI: Gunakan warna DarkGreen untuk titik kosong agar terlihat menyatu dengan background
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(" . ");
                }
            }

            // ---> RESET WARNA SEBELUM MENCETAK BATAS KANAN <---
            Console.ResetColor();
            Console.WriteLine("|");
        }

        // Cetak garis batas bawah
        Console.Write("   +");
        for (var c = 0; c < cols; c++) Console.Write("---");
        Console.WriteLine("+");
    }

    public static void ShowScore(IBoard board)
    {
        var (black, white) = CountDisks(board);
        Console.WriteLine($"\nSkor -> Black (B): {black} | White (W): {white}");
    }

    public static void ShowGameOver(IBoard board, List<IPlayer> players)
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
        {
            var winner = players.FirstOrDefault(p => p.PlayerColors == Colors.Black);
            Console.WriteLine($">>> PEMENANG: {winner?.Name} - Black (B) <<<");
        }
        else if (white > black)
        {
            var winner = players.FirstOrDefault(p => p.PlayerColors == Colors.White);
            Console.WriteLine($">>> PEMENANG: {winner?.Name} - White (W) <<<");
        }
        else
        {
            Console.WriteLine(">>> HASIL: SERI <<<");
        }
    }

    private static (int black, int white) CountDisks(IBoard board)
    {
        int black = 0, white = 0;
        var rows = board.Square.GetLength(0);
        var cols = board.Square.GetLength(1);

        for (var r = 0; r < rows; r++)
        for (var c = 0; c < cols; c++)
        {
            var disk = board.Square[r, c].Disk;
            if (disk != null)
            {
                if (disk.DiskColor == Colors.Black) black++;
                else white++;
            }
        }

        return (black, white);
    }
}