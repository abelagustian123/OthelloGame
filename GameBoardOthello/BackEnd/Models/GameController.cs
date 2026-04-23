using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.Models;

public class GameController : IGameController
{
    private IBoard _board;
    private Dictionary<IPlayer, int> _playerScore;
    private Dictionary<IPlayer, List<Position>> _validPlacesToMove;
    private List<IPlayer> _players;
    
    public event Action<IPlayer>? OnTurnSkipped;
    public event Action<IPlayer>? OnTurnSwitched;
    public event Action<IBoard, IPlayer>? OnMoveMade;
    public event Action<IBoard>? OnGameConcluded;

    private IPlayer _currentPlayer;
    private int _totalDisksOnBoard;
    private Square? _lastPlacedSquare;

    private Position? _lastMovePosition = null;
    
    public GameController(List<IPlayer> players, IBoard board)
    {
        _players = players;
        _board = board;
        _playerScore = new Dictionary<IPlayer, int>();
        _validPlacesToMove = new Dictionary<IPlayer, List<Position>>();
        _currentPlayer = players[0];
        _totalDisksOnBoard = 0;
    }
    
    public void StartGame()
    {
        InitializeGame(_players, _board);
    }

    public void InitializeGame(List<IPlayer> players, IBoard board)
    {
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);

        // Kosongkan semua square di papan
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                board.Square[r, c] = new Square(new Position(r, c), null);
            }
        }

        // Letakkan 4 disk awal di tengah papan (pola standar Othello)
        int midRow = rows / 2;
        int midCol = cols / 2;
        board.Square[midRow - 1, midCol - 1].Disk = new Disk(Colors.White);
        board.Square[midRow - 1, midCol].Disk     = new Disk(Colors.Black);
        board.Square[midRow,     midCol - 1].Disk = new Disk(Colors.Black);
        board.Square[midRow,     midCol].Disk     = new Disk(Colors.White);

        _totalDisksOnBoard = 4;

        // Inisialisasi score dan valid moves untuk tiap player
        foreach (var player in players)
        {
            _playerScore[player] = 2;
            _validPlacesToMove[player] = new List<Position>();
        }

        // Player pertama (biasanya Black) selalu mulai duluan di Othello
        _currentPlayer = players[0];
    }

    public List<IPlayer> GetPlayer(List<IPlayer> players)
    {
        return _players;
    }

    public IPlayer GetCurrentPlayer(IPlayer currentPlayer)
    {
        return _currentPlayer;
    }

    public IBoard GetBoard(IBoard board)
    {
        return _board;
    }

    public int GetTotalDisks(int totalDisksOnBoard)
    {
        return _totalDisksOnBoard;
    }

    public bool HasAnyValidMove(IBoard board, IPlayer currentPlayer)
    {
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (IsMoveValid(currentPlayer, board.Square[r, c]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<Position> GetValidMoves(IPlayer currentPlayer)
    {
        var validMoves = new List<Position>();
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (IsMoveValid(currentPlayer, _board.Square[r, c]))
                {
                    validMoves.Add(new Position(r, c));
                }
            }
        }

        _validPlacesToMove[currentPlayer] = validMoves;
        return validMoves;
    }

    public bool IsMoveValid(IPlayer currentPlayer, Square square)
    {
        // Square harus kosong (belum ada disk)
        if (square.Disk != null) return false;

        Colors playerColors = currentPlayer.PlayerColors;
        Colors opponentColors = playerColors == Colors.Black ? Colors.White : Colors.Black;

        // 8 arah (horizontal, vertikal, diagonal)
        int[,] directions = 
        {
            { -1, -1 }, { -1, 0 }, { -1, 1 },
            {  0, -1 },            {  0, 1 },
            {  1, -1 }, {  1, 0 }, {  1, 1 }
        };

        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int d = 0; d < 8; d++)
        {
            int dr = directions[d, 0];
            int dc = directions[d, 1];
            int r = square.Position.Row + dr;
            int c = square.Position.Col + dc;
            bool foundOpponent = false;

            while (r >= 0 && r < rows && c >= 0 && c < cols)
            {
                var disk = _board.Square[r, c].Disk;
                if (disk == null) break;

                if (disk.DiskColor == opponentColors)
                {
                    foundOpponent = true;
                }
                else // disk.DiskColor == playerColor
                {
                    if (foundOpponent) return true;
                    break;
                }

                r += dr;
                c += dc;
            }
        }

        return false;
    }

    public void PutDiskOnBoard(IPlayer player, Square square, int totalDisksOnBoard)
    {
        _lastMovePosition = square.Position;
        
        // Letakkan disk di square yang dituju
        _board.Square[square.Position.Row, square.Position.Col].Disk = 
            new Disk(player.PlayerColors);

        // Update state internal
        _totalDisksOnBoard = totalDisksOnBoard + 1;
        _lastPlacedSquare = _board.Square[square.Position.Row, square.Position.Col];

        // Flip disk lawan yang terjepit
        DiskFlip(player, _board);

        // Trigger event move made
        NotifyMoveMade(_board, player);
    }

    public void DiskFlip(IPlayer player, IBoard board)
    {
        if (_lastPlacedSquare == null) return;

        Colors playerColor = player.PlayerColors;
        Colors opponentColor = playerColor == Colors.Black ? Colors.White : Colors.Black;

        // 8 arah (horizontal, vertikal, diagonal)
        int[,] directions = new int[,]
        {
            { -1, -1 }, { -1, 0 }, { -1, 1 },
            {  0, -1 },            {  0, 1 },
            {  1, -1 }, {  1, 0 }, {  1, 1 }
        };

        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);
        int startRow = _lastPlacedSquare.Position.Row;
        int startCol = _lastPlacedSquare.Position.Col;

        for (int d = 0; d < 8; d++)
        {
            int dr = directions[d, 0];
            int dc = directions[d, 1];
            int r = startRow + dr;
            int c = startCol + dc;

            var toFlip = new List<Position>();

            while (r >= 0 && r < rows && c >= 0 && c < cols)
            {
                var disk = board.Square[r, c].Disk;
                if (disk == null) break;

                if (disk.DiskColor == opponentColor)
                {
                    // Kandidat untuk di-flip, tapi belum pasti
                    toFlip.Add(new Position(r, c));
                }
                else // disk.DiskColor == playerColor
                {
                    // Jepitan valid! Flip semua disk lawan di antaranya
                    foreach (var pos in toFlip)
                    {
                        board.Square[pos.Row, pos.Col].Disk = new Disk(playerColor);
                    }
                    break;
                }

                r += dr;
                c += dc;
            }
            // Kalau while loop selesai tanpa break di else → arah ini tidak menjepit,
            // toFlip dibuang (tidak di-apply)
        }
    }

    public void SwitchTurn(IPlayer currentPlayer)
    {
        // Cari index pemain saat ini, lalu ambil pemain berikutnya (rotasi)
        int currentIndex = _players.IndexOf(currentPlayer);
        int nextIndex = (currentIndex + 1) % _players.Count;
        _currentPlayer = _players[nextIndex];

        // Trigger event turn switched
        NotifyTurnSwitched(_currentPlayer);
    }

    public bool IsBoardFull(int totalDisksOnBoard)
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);
        return totalDisksOnBoard >= rows * cols;
    }

    public bool IsBothPlayersCannotMove()
    {
        foreach (var player in _players)
        {
            if (HasAnyValidMove(_board, player))
            {
                return false;  // Ada 1 pemain yang masih bisa move → belum stuck
            }
        }
        return true;  // Semua pemain tidak bisa move
    }

    public void EndGame()
    {
        // Hitung skor final
        CheckWinner();

        // Trigger event bahwa game sudah berakhir
        OnGameConcluded?.Invoke(_board);
    }

    public void CheckWinner()
    {
        // Reset skor semua pemain
        foreach (var player in _players)
        {
            _playerScore[player] = 0;
        }

        // Hitung ulang jumlah disk tiap player dengan scan seluruh papan
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var disk = _board.Square[r, c].Disk;
                if (disk != null)
                {
                    foreach (var player in _players)
                    {
                        if (player.PlayerColors == disk.DiskColor)
                        {
                            _playerScore[player]++;
                        }
                    }
                }
            }
        }
    }

    public Position? GetLastMovePosition()
    {
        return _lastMovePosition;
    }
    
    public void NotifyTurnSkipped(IPlayer player)
    {
        OnTurnSkipped?.Invoke(player);
    }

    public void NotifyTurnSwitched(IPlayer player)
    {
        OnTurnSwitched?.Invoke(player);
    }

    public void NotifyMoveMade(IBoard board, IPlayer player)
    {
        OnMoveMade?.Invoke(board, player);
    }
}