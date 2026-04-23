namespace GameBoardOthello.Api.DTOs.Responses;

public record GameStatisticsDto(
    int TotalGames,
    int CompletedGames,
    int InProgressGames,
    Dictionary<string, PlayerStatistics> PlayerStats
);

public record PlayerStatistics(
    string PlayerName,
    int GamesPlayed,
    int Wins,
    int Losses,
    int Draws,
    double WinRate,
    int TotalMoves,
    double AverageMovesPerGame
);