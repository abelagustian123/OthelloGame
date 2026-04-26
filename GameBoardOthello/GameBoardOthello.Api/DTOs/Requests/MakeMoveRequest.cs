namespace GameBoardOthello.Api.DTOs.Requests;

public record MakeMoveRequest(
    int Row,
    int Col
);