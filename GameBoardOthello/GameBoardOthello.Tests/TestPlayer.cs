using GameBoardOthello.BackEnd.BackEnd.Enums;
using GameBoardOthello.BackEnd.Interface;

namespace GameBoardOthello.Tests;

public class TestPlayer : IPlayer
{
    public string Name { get; set; }
    public Colors PlayerColors { get; set; }
}