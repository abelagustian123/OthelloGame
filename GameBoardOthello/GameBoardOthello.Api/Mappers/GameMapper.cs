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
        var currentPlayer = game.GetCurrentPlayer(null!);
        
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
        var result = new List<List<SquareDto>>();
        var lastMove = game.GetLastMovePosition();
        var currentPlayer = game.GetCurrentPlayer(null!);
        var validMoves = game.GetValidMoves(currentPlayer);

        for (int r = 0; r < 8; r++)
        {
            var row = new List<SquareDto>();
            for (int c = 0; c < 8; c++)
            {
                var square = board.Square[r, c];
                
                row.Add(new SquareDto(
                    Row: r,
                    Col: c,
                    Disk: square.Disk != null 
                        ? new DiskDto(square.Disk.DiskColor.ToString()) 
                        : null,
                    IsValidMove: validMoves.Any(p => p.Row == r && p.Col == c),
                    IsLastMove: lastMove != null && lastMove.Value.Row == r && lastMove.Value.Col == c
                ));
            }
            result.Add(row);
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
                        blackCount++;
                    else
                        whiteCount++;
                }
            }
        }
        
        return new ScoreDto(blackCount, whiteCount);
    }

    private static bool IsGameOver(GameController game)
    {
        var totalDisks = game.GetTotalDisks(0);
        return game.IsBoardFull(totalDisks) || game.IsBothPlayersCannotMove();
    }

    private static PlayerDto? DetermineWinner(
        GameController game,
        IBoard board,
        List<IPlayer> players)
    {
        if (!IsGameOver(game))
            return null;

        var score = MapScore(board);
        
        if (score.BlackScore > score.WhiteScore)
            return MapPlayer(players.First(p => p.PlayerColors == Colors.Black));
        
        if (score.WhiteScore > score.BlackScore)
            return MapPlayer(players.First(p => p.PlayerColors == Colors.White));
        
        return null; // Draw
    }

    public static ValidMovesDto ToValidMovesDto(List<Position> positions)
    {
        return new ValidMovesDto(
            positions.Select(p => new PositionDto(p.Row, p.Col)).ToList()
        );
    }
    
    public static MoveHistoryDto ToMoveHistoryDto(string gameId, List<MoveRecord> moves)
    {
        var moveDtos = moves.Select(m => new MoveRecordDto(
            MoveNumber: m.MoveNumber,
            PlayerName: m.PlayerName,
            PlayerColor: m.PlayerColor,
            Position: new PositionDto(m.Position.Row, m.Position.Col),
            DisksFlipped: m.DisksFlipped,
            BlackScoreAfter: m.BlackScoreAfter,
            WhiteScoreAfter: m.WhiteScoreAfter,
            Timestamp: m.Timestamp
        )).ToList();

        return new MoveHistoryDto(
            GameId: gameId,
            Moves: moveDtos,
            TotalMoves: moves.Count
        );
    }
}