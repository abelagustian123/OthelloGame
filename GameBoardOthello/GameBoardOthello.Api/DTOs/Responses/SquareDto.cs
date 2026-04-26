namespace GameBoardOthello.Api.DTOs.Responses;

public record SquareDto(
    int Row,
    int Col,
    DiskDto? Disk,
    bool IsValidMove,
    bool IsLastMove
);