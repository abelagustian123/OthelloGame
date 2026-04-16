using GameBoardOthello.BackEnd.Enum;

namespace GameBoardOthello.BackEnd.Interface;

public class IPlayer
{
    public string Name { get; set; }
    public Colors PlayerColors { get; set; }
}