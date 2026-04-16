using System.Drawing;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;

public class Player : IPlayer
{
    string Name { get; set; }
    Colors PlayerColor { get; set; }

    public Player(string name, Colors playerColor)
    {
        Name = name;
        PlayerColor = playerColor;
    }
}