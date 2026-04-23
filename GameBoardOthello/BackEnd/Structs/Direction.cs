namespace GameBoardOthello.BackEnd.BackEnd.Structs;

public struct Direction
{
    public int RowDelta { get; }
    public int ColDelta { get; }

    public Direction(int rowDelta, int colDelta)
    {
        RowDelta = rowDelta;
        ColDelta = colDelta;
    }

    public static readonly Direction[] AllDirection = new[]
    {
        new Direction(-1, -1), // ↖ Top-Left
        new Direction(-1, 0), // ↑ Top
        new Direction(-1, 1), // ↗ Top-Right
        new Direction(0, -1), // ← Left
        new Direction(0, 1), // → Right
        new Direction(1, -1), // ↙ Bottom-Left
        new Direction(1, 0), // ↓ Bottom
        new Direction(1, 1) // ↘ Bottom-Right
    };
}