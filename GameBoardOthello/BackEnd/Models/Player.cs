using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;

public class Player : IPlayer
{
    public string Name { get; set; }
    public Colors PlayerColors { get; set; }

    public Player(string name, Colors playerColor)
    {
        Name = name;
        PlayerColors = playerColor;
    }
}