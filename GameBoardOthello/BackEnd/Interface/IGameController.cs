using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.Interface;

public interface IGameController
{
    event Action<IPlayer>? OnTurnSkipped;
    event Action<IPlayer>? OnTurnSwitched;
    event Action<IBoard, IPlayer>? OnMoveMade;
    event Action<IBoard>? OnGameConcluded;

    void StartGame();
    void InitializeGame(List<IPlayer> players, IBoard board);
    
    List<IPlayer> GetPlayer(List<IPlayer> players);
    IPlayer GetCurrentPlayer(IPlayer currentPlayer);
    IBoard GetBoard(IBoard board);
    int GetTotalDisks(int totalDisksOnBoard);

    bool HasAnyValidMove(IBoard board, IPlayer currentPlayer);
    List<Position> GetValidMoves(IPlayer currentPlayer);
    bool IsMoveValid(IPlayer currentPlayer, Square square);
    
    void PutDiskOnBoard(IPlayer player, Square square, int totalDisksOnBoard);
    void DiskFlip(IPlayer player, IBoard board);

    void SwitchTurn(IPlayer currentPlayer);
    
    bool IsBoardFull(int totalDisksOnBoard);
    bool IsBothPlayersCannotMove();
    
    void EndGame();
    void CheckWinner();

    void NotifyTurnSkipped(IPlayer player);
    void NotifyTurnSwitched(IPlayer player);
    void NotifyMoveMade(IBoard board, IPlayer player);
}