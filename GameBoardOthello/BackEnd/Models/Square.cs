using GameBoardOthello.BackEnd.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

public class Square
{
    public Position Position { get; set; }
    public Disk? Disk { get; set; }

    public Square(Position position, Disk? disk)
    {
        Position = position;
        Disk = disk;
    }
}