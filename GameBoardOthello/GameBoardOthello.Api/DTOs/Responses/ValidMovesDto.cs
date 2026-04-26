namespace GameBoardOthello.Api.DTOs.Responses;

public record ValidMovesDto(
    List<PositionDto> Positions
);