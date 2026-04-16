using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.UI;

public class ConsoleInputHandler
{
    public static (string, string) GetPlayerNames()
    {
        Console.Write("Nama Player 1 (Black): ");
        string p1 = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(p1)) p1 = "Player 1";

        Console.Write("Nama Player 2 (White): ");
        string p2 = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(p2)) p2 = "Player 2";

        return (p1, p2);
    }

    public static Position GetMoveInput(List<Position> validMoves)
    {
        while (true)
        {
            Console.Write("\nPosisi (row col) atau 'q' untuk keluar: ");
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input)) continue;

            if (input.ToLower() == "q")
            {
                Console.WriteLine("Keluar dari game...");
                Environment.Exit(0);
            }

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                Console.WriteLine("Format salah! Contoh: 2 3");
                continue;
            }

            if (!int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
            {
                Console.WriteLine("Input harus angka!");
                continue;
            }

            if (!validMoves.Any(p => p.Row == row && p.Col == col))
            {
                Console.WriteLine("Bukan move valid! Coba lagi.");
                continue;
            }

            return new Position(row, col);
        }
    }
}