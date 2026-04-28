using GameBoardOthello.BackEnd.BackEnd.Enums;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

public class Disk
{
    public Colors DiskColor { get; set; }

    public Disk(Colors diskColor)
    {
        DiskColor = diskColor;
    }
}