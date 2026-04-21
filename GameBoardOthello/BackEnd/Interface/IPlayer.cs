using GameBoardOthello.BackEnd.Enum;

namespace GameBoardOthello.BackEnd.Interface;

public interface IPlayer
{
    public string Name { get; set; }
    public Colors PlayerColors { get; set; }
}