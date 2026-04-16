using GameBoardOthello.BackEnd.Models;

namespace GameBoardOthello.BackEnd.Interface;

public interface IBoard
{
    public Square[,] Square { get; set; }
}