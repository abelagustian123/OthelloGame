using GameBoardOthello.Api.DTOs.Requests;
using GameBoardOthello.Api.DTOs.Responses;
using GameBoardOthello.Api.Mappers;
using GameBoardOthello.Api.Services;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GameBoardOthello.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameApiController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly ILogger<GameApiController> _logger;

    public GameApiController(
        GameService gameService,
        ILogger<GameApiController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    
    [HttpPost("start")]
    [ProducesResponseType(typeof(GameStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult StartGame([FromBody] StartGameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Player1Name) ||
            string.IsNullOrWhiteSpace(request.Player2Name))
            return BadRequest("Player names cannot be empty");

        var gameId = _gameService.CreateGame(request.Player1Name, request.Player2Name);
        var game = _gameService.GetGame(gameId)!;
        var (board, players) = _gameService.GetGameData(gameId)!.Value;

        var state = GameMapper.ToGameStateDto(gameId, game, board, players);

        _logger.LogInformation("New game started: {GameId}", gameId);

        return Ok(state);
    }

   
    [HttpGet("{gameId}/state")]
    [ProducesResponseType(typeof(GameStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetState(string gameId)
    {
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound($"Game {gameId} not found");
        }

        var (board, players) = _gameService.GetGameData(gameId)!.Value;
        var state = GameMapper.ToGameStateDto(gameId, game, board, players);

        return Ok(state);
    }

    [HttpPost("{gameId}/move")]
    [ProducesResponseType(typeof(MoveResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult MakeMove(string gameId, [FromBody] MakeMoveRequest request)
    {
        GameController? game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound($"Game {gameId} not found");
        }

        (IBoard board, List<IPlayer> players) = _gameService.GetGameData(gameId)!.Value;
        
        IPlayer currentPlayer = game.GetCurrentPlayer();

        // Validate coordinates
        if (request.Row < 0 || request.Row > 7 || request.Col < 0 || request.Col > 7)
        {
            return Ok(new MoveResultDto(
                false,
                "Invalid coordinates. Row and Col must be between 0 and 7.",
                null
            ));
        }

        Square square = board.Square[request.Row, request.Col];

        bool moveSuccess = game.PutDiskOnBoard(currentPlayer, square);
        
        if (!moveSuccess)
        {
            return Ok(new MoveResultDto(
                false,
                "Invalid move. You must flip at least one opponent disk.",
                null
            ));
        }

        IPlayer nextPlayer = game.SwitchTurn();

        // Return updated state
        GameStateDto newState = GameMapper.ToGameStateDto(gameId, game, board, players);

        _logger.LogInformation(
            "Move made in game {GameId}: Player={Player}, Position=({Row},{Col}), NextPlayer={NextPlayer}",
            gameId, currentPlayer.Name, request.Row, request.Col, nextPlayer.Name);

        return Ok(new MoveResultDto(
            true,
            null,
            newState
        ));
    }

    [HttpGet("{gameId}/valid-moves")]
    [ProducesResponseType(typeof(ValidMovesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetValidMoves(string gameId)
    {
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound($"Game {gameId} not found");
        }

        var currentPlayer = game.GetCurrentPlayer();
        var validMoves = game.GetValidMoves(currentPlayer);

        var result = GameMapper.ToValidMovesDto(validMoves);

        return Ok(result);
    }
    
   
    [HttpPost("{gameId}/skip-turn")]
    [ProducesResponseType(typeof(GameStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SkipTurn(string gameId)
    {
        GameController? game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound($"Game {gameId} not found");
        }

        IPlayer currentPlayer = game.GetCurrentPlayer();

        // Hanya boleh skip jika current player benar-benar tidak punya move
        if (game.HasAnyValidMove(currentPlayer))
        {
            return BadRequest("Cannot skip — current player still has valid moves.");
        }

        // Notify skip event lalu switch turn
        game.NotifyTurnSkipped(currentPlayer);
        IPlayer nextPlayer = game.SwitchTurn();

        (IBoard board, List<IPlayer> players) = _gameService.GetGameData(gameId)!.Value;
        GameStateDto newState = GameMapper.ToGameStateDto(gameId, game, board, players);

        _logger.LogInformation(
            "Turn skipped in game {GameId}: {SkippedPlayer} → {NextPlayer}",
            gameId, currentPlayer.Name, nextPlayer.Name);

        return Ok(newState);
    }

    [HttpDelete("{gameId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteGame(string gameId)
    {
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound($"Game {gameId} not found");
        }

        _gameService.RemoveGame(gameId);

        _logger.LogInformation("Game deleted: {GameId}", gameId);

        return NoContent();
    }
    
}