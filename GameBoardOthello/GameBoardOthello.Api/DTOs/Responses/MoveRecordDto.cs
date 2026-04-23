namespace GameBoardOthello.Api.DTOs.Responses;

public record MoveRecordDto(
    int MoveNumber,
    string PlayerName,
    string PlayerColor,
    PositionDto Position,
    int DisksFlipped,
    int BlackScoreAfter,
    int WhiteScoreAfter,
    DateTime Timestamp
);