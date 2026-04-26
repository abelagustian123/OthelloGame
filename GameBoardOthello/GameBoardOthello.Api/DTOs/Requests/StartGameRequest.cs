namespace GameBoardOthello.Api.DTOs.Requests;

public record StartGameRequest(
    string Player1Name,
    string Player2Name
);