using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.BackEnd.Structs;

namespace GameBoardOthello.Tests;

public class TestBoard : IBoard
{
    public TestBoard(int size = 8)
    {
        Square = new Square[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                Square[i, j] = new Square(new Position(i, j), null);
            }
        }
    }

    public Square[,] Square { get; set; }
}