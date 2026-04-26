namespace GameBoardOthello.Api.DTOs.Responses;

public record ScoreDto(
    int BlackScore,
    int WhiteScore
);