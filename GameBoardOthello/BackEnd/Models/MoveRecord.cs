using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

public class MoveRecord
{
    public int MoveNumber { get; set; }
    public string PlayerName { get; set; }
    public string PlayerColor { get; set; }
    public Position Position { get; set; }
    public int DisksFlipped { get; set; }
    public int BlackScoreAfter { get; set; }
    public int WhiteScoreAfter { get; set; }
    public DateTime Timestamp { get; set; }

    public MoveRecord(
        int moveNumber, 
        string playerName, 
        string playerColor, 
        Position position, 
        int disksFlipped, 
        int blackScore, 
        int whiteScore)
    {
        MoveNumber = moveNumber;
        PlayerName = playerName;
        PlayerColor = playerColor;
        Position = position;
        DisksFlipped = disksFlipped;
        BlackScoreAfter = blackScore;
        WhiteScoreAfter = whiteScore;
        Timestamp = DateTime.UtcNow;
    }
}