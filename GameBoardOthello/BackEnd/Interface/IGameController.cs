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

    bool StartGame();
    void InitializeGame(List<IPlayer> players, IBoard board);
    bool EndGame();

    List<IPlayer> GetPlayers();
    IPlayer GetCurrentPlayer();
    IBoard GetBoard();
    int GetTotalDisks();
    Position? GetLastMovePosition();

    bool HasAnyValidMove(IPlayer currentPlayer);
    List<Position> GetValidMoves(IPlayer currentPlayer);
    bool IsMoveValid(IPlayer currentPlayer, Square square);

    bool PutDiskOnBoard(IPlayer player, Square square);
    bool DiskFlip(IPlayer player, IBoard board);

    IPlayer SwitchTurn();

    bool IsBoardFull();
    bool IsBothPlayersCannotMove();
    Dictionary<IPlayer, int> CheckWinner();

    void NotifyTurnSkipped(IPlayer player);
    void NotifyTurnSwitched(IPlayer player);
    void NotifyMoveMade(IBoard board, IPlayer player);
}