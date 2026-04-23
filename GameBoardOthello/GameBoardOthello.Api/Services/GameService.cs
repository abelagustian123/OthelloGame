using GameBoardOthello.Api.DTOs.Responses;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;

namespace GameBoardOthello.Api.Services;

public class GameService
{
    // In-memory storage untuk active games
    private readonly Dictionary<string, GameController> _activeGames = new();
    private readonly Dictionary<string, (IBoard Board, List<IPlayer> Players)> _gameData = new();
    private readonly Dictionary<string, GameSnapshot> _savedGames = new();
    private readonly List<GameStatEntry> _gameStatistics = new();
    
    public string CreateGame(string player1Name, string player2Name)
    {
        var gameId = Guid.NewGuid().ToString();
        
        var player1 = new Player(player1Name, Colors.Black);
        var player2 = new Player(player2Name, Colors.White);
        var players = new List<IPlayer> { player1, player2 };
        
        var board = new Board(new Square[8, 8]);
        var game = new GameController(players, board);
        
        game.StartGame();
        
        _activeGames[gameId] = game;
        _gameData[gameId] = (board, players);
        
        // Record game start in statistics
        _gameStatistics.Add(new GameStatEntry
        {
            GameId = gameId,
            Player1Name = player1Name,
            Player2Name = player2Name,
            StartTime = DateTime.UtcNow,
            IsCompleted = false
        });
        
        return gameId;
    }

    public GameController? GetGame(string gameId)
    {
        _activeGames.TryGetValue(gameId, out var game);
        return game;
    }

    public (IBoard Board, List<IPlayer> Players)? GetGameData(string gameId)
    {
        _gameData.TryGetValue(gameId, out var data);
        return data;
    }

    public void RemoveGame(string gameId)
    {
        _activeGames.Remove(gameId);
        _gameData.Remove(gameId);
    }

    public int GetActiveGamesCount()
    {
        return _activeGames.Count;
    }

    // Save/Load functionality
    public string SaveGame(string gameId, string saveName)
    {
        var game = GetGame(gameId);
        if (game == null)
            return null;

        var snapshot = game.CreateSnapshot(gameId);
        var saveId = Guid.NewGuid().ToString();
        
        _savedGames[saveId] = snapshot;
        
        return saveId;
    }

    public GameController? LoadGame(string saveId, out string gameId)
    {
        gameId = null;
        
        if (!_savedGames.TryGetValue(saveId, out var snapshot))
            return null;

        // Create new game with same players
        gameId = Guid.NewGuid().ToString();
        
        var players = new List<IPlayer>();
        for (int i = 0; i < snapshot.PlayerNames.Count; i++)
        {
            players.Add(new Player(snapshot.PlayerNames[i], snapshot.PlayerColors[i]));
        }
        
        var board = new Board(new Square[8, 8]);
        var game = new GameController(players, board);
        
        // Load snapshot
        game.LoadFromSnapshot(snapshot);
        
        _activeGames[gameId] = game;
        _gameData[gameId] = (board, players);
        
        return game;
    }

    public List<SavedGameDto> GetSavedGames()
    {
        var result = new List<SavedGameDto>();
        
        foreach (var kvp in _savedGames)
        {
            var saveId = kvp.Key;
            var snapshot = kvp.Value;
            
            result.Add(new SavedGameDto(
                SaveId: saveId,
                GameId: snapshot.GameId,
                SaveName: $"Game {snapshot.GameId.Substring(0, 8)}",
                Player1Name: snapshot.PlayerNames[0],
                Player2Name: snapshot.PlayerNames[1],
                TotalMoves: snapshot.MoveHistory.Count,
                CurrentPlayerName: snapshot.CurrentPlayerName,
                IsGameOver: snapshot.IsGameOver,
                SavedAt: snapshot.SavedAt
            ));
        }
        
        return result.OrderByDescending(s => s.SavedAt).ToList();
    }

    // Statistics
    public void RecordGameCompletion(string gameId, string winnerName)
    {
        var statEntry = _gameStatistics.FirstOrDefault(s => s.GameId == gameId);
        if (statEntry != null)
        {
            statEntry.IsCompleted = true;
            statEntry.WinnerName = winnerName;
            statEntry.EndTime = DateTime.UtcNow;
        }
    }

    public GameStatisticsDto GetStatistics()
    {
        var totalGames = _gameStatistics.Count;
        var completedGames = _gameStatistics.Count(s => s.IsCompleted);
        var inProgressGames = totalGames - completedGames;

        // Calculate per-player stats
        var playerStats = new Dictionary<string, PlayerStatistics>();
        
        foreach (var stat in _gameStatistics)
        {
            UpdatePlayerStats(playerStats, stat, stat.Player1Name);
            UpdatePlayerStats(playerStats, stat, stat.Player2Name);
        }

        return new GameStatisticsDto(
            TotalGames: totalGames,
            CompletedGames: completedGames,
            InProgressGames: inProgressGames,
            PlayerStats: playerStats
        );
    }

    private void UpdatePlayerStats(
        Dictionary<string, PlayerStatistics> playerStats, 
        GameStatEntry stat, 
        string playerName)
    {
        if (!playerStats.ContainsKey(playerName))
        {
            playerStats[playerName] = new PlayerStatistics(
                PlayerName: playerName,
                GamesPlayed: 0,
                Wins: 0,
                Losses: 0,
                Draws: 0,
                WinRate: 0,
                TotalMoves: 0,
                AverageMovesPerGame: 0
            );
        }

        var current = playerStats[playerName];
        var gamesPlayed = current.GamesPlayed + 1;
        var wins = current.Wins + (stat.WinnerName == playerName ? 1 : 0);
        var losses = current.Losses + (stat.IsCompleted && stat.WinnerName != playerName && stat.WinnerName != null ? 1 : 0);
        var draws = current.Draws + (stat.IsCompleted && stat.WinnerName == null ? 1 : 0);
        var winRate = gamesPlayed > 0 ? (double)wins / gamesPlayed * 100 : 0;

        playerStats[playerName] = new PlayerStatistics(
            PlayerName: playerName,
            GamesPlayed: gamesPlayed,
            Wins: wins,
            Losses: losses,
            Draws: draws,
            WinRate: Math.Round(winRate, 2),
            TotalMoves: current.TotalMoves,
            AverageMovesPerGame: current.AverageMovesPerGame
        );
    }
}

// Helper class for statistics
public class GameStatEntry
{
    public string GameId { get; set; }
    public string Player1Name { get; set; }
    public string Player2Name { get; set; }
    public string? WinnerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsCompleted { get; set; }
}