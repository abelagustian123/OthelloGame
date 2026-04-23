namespace GameBoardOthello.Api.DTOs.Responses;

public record MoveHistoryDto(
    string GameId,
    List<MoveRecordDto> Moves,
    int TotalMoves
);