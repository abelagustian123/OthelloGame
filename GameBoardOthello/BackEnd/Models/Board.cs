using GameBoardOthello.BackEnd.BackEnd.Interface;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

public class Board : IBoard
{
     public Square[,] Square { get; set; }

     public Board(Square[,] square)
     {
          Square = square;
     }
}