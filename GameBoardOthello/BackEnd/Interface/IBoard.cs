using GameBoardOthello.BackEnd.Models;

namespace GameBoardOthello.BackEnd.Interface;

public interface IBoard
{
    Square[,] Square { get; set; }
}