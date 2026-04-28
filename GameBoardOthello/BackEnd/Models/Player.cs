using GameBoardOthello.BackEnd.BackEnd.Enums;
using GameBoardOthello.BackEnd.Interface;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

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