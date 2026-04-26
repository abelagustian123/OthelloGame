using GameBoardOthello.Api.DTOs.Responses;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.Api.Mappers;

public static class GameMapper
{
    public static GameStateDto ToGameStateDto(
        string gameId,
        GameController game,
        IBoard board,
        List<IPlayer> players)
    {
        var currentPlayer = game.GetCurrentPlayer();
        
        return new GameStateDto(
            GameId: gameId,
            Board: MapBoard(board, game),
            CurrentPlayer: MapPlayer(currentPlayer),
            Player1: MapPlayer(players[0]),
            Player2: MapPlayer(players[1]),
            Score: MapScore(board),
            IsGameOver: IsGameOver(game),
            Winner: DetermineWinner(game, board, players)
        );
    }

    private static List<List<SquareDto>> MapBoard(IBoard board, GameController game)
    {
        List<List<SquareDto>> result = new List<List<SquareDto>>();
        Position? lastMove = game.GetLastMovePosition();
        
        IPlayer currentPlayer = game.GetCurrentPlayer();
        List<Position> validMoves = game.GetValidMoves(currentPlayer);

        for (int row = 0; row < 8; row++)
        {
            List<SquareDto> rowList = new List<SquareDto>();
            for (int col = 0; col < 8; col++)
            {
                Square square = board.Square[row, col];
                
                rowList.Add(new SquareDto(
                    Row: row,
                    Col: col,
                    Disk: square.Disk != null 
                        ? new DiskDto(square.Disk.DiskColor.ToString()) 
                        : null,
                    IsValidMove: validMoves.Any(p => p.Row == row && p.Col == col),
                    IsLastMove: lastMove != null && lastMove.Value.Row == row && lastMove.Value.Col == col
                ));
            }
            result.Add(rowList);
        }
        
        return result;
    }

    private static PlayerDto MapPlayer(IPlayer player)
    {
        return new PlayerDto(
            Name: player.Name,
            Color: player.PlayerColors.ToString()
        );
    }

    private static ScoreDto MapScore(IBoard board)
    {
        int blackCount = 0;
        int whiteCount = 0;
        
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                var disk = board.Square[r, c].Disk;
                if (disk != null)
                {
                    if (disk.DiskColor == Colors.Black)
                    {
                        blackCount++;
                    }
                    else
                    {
                        whiteCount++;
                    }
                }
            }
        }
        
        return new ScoreDto(blackCount, whiteCount);
    }

    private static bool IsGameOver(GameController game)
    {
        return game.IsBoardFull() || game.IsBothPlayersCannotMove();
    }

    private static PlayerDto? DetermineWinner(
        GameController game,
        IBoard board,
        List<IPlayer> players)
    {
        if (!IsGameOver(game))
        {
            return null;
        }

        Dictionary<IPlayer, int> scores = game.CheckWinner();
        
        IPlayer? blackPlayer = players.FirstOrDefault(p => p.PlayerColors == Colors.Black);
        IPlayer? whitePlayer = players.FirstOrDefault(p => p.PlayerColors == Colors.White);
        
        if (blackPlayer == null || whitePlayer == null)
        {
            return null;
        }
        
        int blackScore = scores[blackPlayer];
        int whiteScore = scores[whitePlayer];
        
        if (blackScore > whiteScore)
        {
            return MapPlayer(blackPlayer);
        }
        
        if (whiteScore > blackScore)
        {
            return MapPlayer(whitePlayer);
        }
        
        return null;
    }

    public static ValidMovesDto ToValidMovesDto(List<Position> positions)
    {
        return new ValidMovesDto(
            positions.Select(p => new PositionDto(p.Row, p.Col)).ToList()
        );
    }
}