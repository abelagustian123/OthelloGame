using GameBoardOthello.BackEnd.Models;

namespace GameBoardOthello.BackEnd.BackEnd.Interface;

public interface IBoard
{
    public Square[,] Square { get; set; }
}