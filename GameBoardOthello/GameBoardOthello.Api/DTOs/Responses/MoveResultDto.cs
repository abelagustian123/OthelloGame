namespace GameBoardOthello.Api.DTOs.Responses;

public record MoveResultDto(
    bool Success,
    string? ErrorMessage,
    GameStateDto? GameState
);