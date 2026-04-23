namespace GameBoardOthello.Api.DTOs.Responses;

public record GameStateDto(
    string GameId,
    List<List<SquareDto>> Board,
    PlayerDto CurrentPlayer,
    PlayerDto Player1,
    PlayerDto Player2,
    ScoreDto Score,
    bool IsGameOver,
    PlayerDto? Winner
);