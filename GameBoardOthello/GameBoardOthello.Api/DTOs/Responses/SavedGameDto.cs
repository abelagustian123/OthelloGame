namespace GameBoardOthello.Api.DTOs.Responses;

public record SavedGameDto(
    string SaveId,
    string GameId,
    string SaveName,
    string Player1Name,
    string Player2Name,
    int TotalMoves,
    string CurrentPlayerName,
    bool IsGameOver,
    DateTime SavedAt
);