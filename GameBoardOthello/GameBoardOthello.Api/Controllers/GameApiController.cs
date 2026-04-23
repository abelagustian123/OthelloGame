using GameBoardOthello.Api.DTOs.Requests;
using GameBoardOthello.Api.DTOs.Responses;
using GameBoardOthello.Api.Mappers;
using GameBoardOthello.Api.Services;
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

    /// <summary>
    /// Start a new game
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(GameStateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult StartGame([FromBody] StartGameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Player1Name) || 
            string.IsNullOrWhiteSpace(request.Player2Name))
        {
            return BadRequest("Player names cannot be empty");
        }

        var gameId = _gameService.CreateGame(request.Player1Name, request.Player2Name);
        var game = _gameService.GetGame(gameId)!;
        var (board, players) = _gameService.GetGameData(gameId)!.Value;

        var state = GameMapper.ToGameStateDto(gameId, game, board, players);

        _logger.LogInformation("New game started: {GameId}", gameId);

        return Ok(state);
    }

    /// <summary>
    /// Get current game state
    /// </summary>
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

    /// <summary>
    /// Make a move
    /// </summary>
    [HttpPost("{gameId}/move")]
    [ProducesResponseType(typeof(MoveResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult MakeMove(string gameId, [FromBody] MakeMoveRequest request)
    {
        var game = _gameService.GetGame(gameId);
        if (game == null)
        {
            return NotFound($"Game {gameId} not found");
        }

        var (board, players) = _gameService.GetGameData(gameId)!.Value;
        var currentPlayer = game.GetCurrentPlayer(null);

        // Validate coordinates
        if (request.Row < 0 || request.Row > 7 || request.Col < 0 || request.Col > 7)
        {
            return Ok(new MoveResultDto(
                Success: false,
                ErrorMessage: "Invalid coordinates. Row and Col must be between 0 and 7.",
                GameState: null
            ));
        }

        var square = board.Square[request.Row, request.Col];

        // Check if move is valid
        if (!game.IsMoveValid(currentPlayer, square))
        {
            return Ok(new MoveResultDto(
                Success: false,
                ErrorMessage: "Invalid move. You must flip at least one opponent disk.",
                GameState: null
            ));
        }

        // Execute move
        var totalDisks = game.GetTotalDisks(0);
        game.PutDiskOnBoard(currentPlayer, square, totalDisks);

        // Switch turn
        game.SwitchTurn(currentPlayer);

        // Return updated state
        var newState = GameMapper.ToGameStateDto(gameId, game, board, players);

        _logger.LogInformation(
            "Move made in game {GameId}: Player={Player}, Position=({Row},{Col})",
            gameId, currentPlayer.Name, request.Row, request.Col);

        return Ok(new MoveResultDto(
            Success: true,
            ErrorMessage: null,
            GameState: newState
        ));
    }

    /// <summary>
    /// Get valid moves for current player
    /// </summary>
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

        var currentPlayer = game.GetCurrentPlayer(null);
        var validMoves = game.GetValidMoves(currentPlayer);

        var result = GameMapper.ToValidMovesDto(validMoves);

        return Ok(result);
    }

    /// <summary>
    /// Delete a game
    /// </summary>
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