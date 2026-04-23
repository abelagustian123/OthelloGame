using GameBoardOthello.BackEnd.Enum;


public class Disk
{
    public Colors DiskColor { get; set; }

    public Disk(Colors diskColor)
    {
        DiskColor = diskColor;
    }
}