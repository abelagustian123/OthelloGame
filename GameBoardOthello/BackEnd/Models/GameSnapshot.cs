using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

public class GameSnapshot
{
    public string GameId { get; set; }
    public Square[,] BoardState { get; set; }
    public List<string> PlayerNames { get; set; }
    public List<Colors> PlayerColors { get; set; }
    public string CurrentPlayerName { get; set; }
    public int TotalDisksOnBoard { get; set; }
    public List<MoveRecord> MoveHistory { get; set; }
    public DateTime SavedAt { get; set; }
    public bool IsGameOver { get; set; }

    public GameSnapshot(
        string gameId,
        Square[,] boardState,
        List<IPlayer> players,
        IPlayer currentPlayer,
        int totalDisks,
        List<MoveRecord> moveHistory,
        bool isGameOver)
    {
        GameId = gameId;
        BoardState = (Square[,])boardState.Clone();
        PlayerNames = players.Select(p => p.Name).ToList();
        PlayerColors = players.Select(p => p.PlayerColors).ToList();
        CurrentPlayerName = currentPlayer.Name;
        TotalDisksOnBoard = totalDisks;
        MoveHistory = new List<MoveRecord>(moveHistory);
        SavedAt = DateTime.UtcNow;
        IsGameOver = isGameOver;
    }
}