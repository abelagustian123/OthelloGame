
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;

public class Board : IBoard
{
     public Square[,] Square { get; set; }

     public Board(Square[,] square)
     {
          Square = square;
     }

}