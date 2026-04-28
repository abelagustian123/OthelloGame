using GameBoardOthello.Api.DTOs.Responses;
using GameBoardOthello.BackEnd.BackEnd.Enums;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.Interface;

namespace GameBoardOthello.Api.Services;

public class GameService
{
    private readonly Dictionary<string, GameController> _activeGames = new();
    private readonly Dictionary<string, (IBoard Board, List<IPlayer> Players)> _gameData = new();
    
    public string CreateGame(string player1Name, string player2Name)
    {
        var gameId = Guid.NewGuid().ToString();
        
        var player1 = new Player(player1Name, Colors.Black);
        var player2 = new Player(player2Name, Colors.White);
        var players = new List<IPlayer> { player1, player2 };
        
        var board = new Board(new Square[8, 8]);
        var game = new GameController(players, board);
        
        bool started = game.StartGame();
        if (!started)
        {
            return null;  
        }
        
        _activeGames[gameId] = game;
        _gameData[gameId] = (board, players);
        
        return gameId;
    }

    public GameController? GetGame(string gameId)
    {
        _activeGames.TryGetValue(gameId, out var game);
        return game;
    }

    public (IBoard Board, List<IPlayer> Players)? GetGameData(string gameId)
    {
        _gameData.TryGetValue(gameId, out var data);
        return data;
    }
    
    public void RemoveGame(string gameId)
    {
        _activeGames.Remove(gameId);
        _gameData.Remove(gameId);
    }
}
