using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Structs;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Models;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

public class GameController : IGameController
{
    private IBoard _board;
    private Dictionary<IPlayer, int> _playerScore = new Dictionary<IPlayer, int>();
    private Dictionary<IPlayer, List<Position>> _validPlacesToMove = new Dictionary<IPlayer, List<Position>>();
    private List<IPlayer> _players;
    
    public event Action<IPlayer>? OnTurnSkipped;
    public event Action<IPlayer>? OnTurnSwitched;
    public event Action<IBoard, IPlayer>? OnMoveMade;
    public event Action<IBoard>? OnGameConcluded;

    private IPlayer _currentPlayer;
    private Position? _lastMovePosition;
    
    public GameController(List<IPlayer> players, IBoard board)
    {
        _players = players;
        _board = board;
        _currentPlayer = players[0];
    }
    
    public bool StartGame()
    {
        if (_players == null || _players.Count < 2)
        {
            return false;
        }

        InitializeGame(_players, _board);
        return true;
    }

    public void InitializeGame(List<IPlayer> players, IBoard board)
    {
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);

        // Kosongkan semua square di papan
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                board.Square[row, col] = new Square(new Position(row, col), null);
            }
        }

        // Letakkan 4 disk awal di tengah papan (pola standar Othello)
        int midRow = rows / 2;
        int midCol = cols / 2;
        board.Square[midRow - 1, midCol - 1].Disk = new Disk(Colors.White);
        board.Square[midRow - 1, midCol].Disk     = new Disk(Colors.Black);
        board.Square[midRow,midCol - 1].Disk = new Disk(Colors.Black);
        board.Square[midRow,midCol].Disk     = new Disk(Colors.White);

        // Inisialisasi score dan valid moves untuk tiap player
        foreach (IPlayer player in players) 
        {
            _playerScore[player] = 2;
            _validPlacesToMove[player] = new List<Position>();
        }

        // Player pertama (biasanya Black) selalu mulai duluan di Othello
        _currentPlayer = players[0];
    }

    public List<IPlayer> GetPlayers() 
    {
        return _players;
    }

    public IPlayer GetCurrentPlayer()
    {
        return _currentPlayer;
    }

    public IBoard GetBoard()
    {
        return _board;
    }

    public int GetTotalDisks()
    {
        int total = 0;
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (_board.Square[row, col].Disk != null)
                {
                    total++;
                }
            }
        }

        return total;
    }

    public bool HasAnyValidMove(IPlayer currentPlayer) 
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (IsMoveValid(currentPlayer, _board.Square[row, col]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<Position> GetValidMoves(IPlayer currentPlayer)
    {
        List<Position> validMoves = new List<Position>();
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (IsMoveValid(currentPlayer, _board.Square[row, col]))
                {
                    validMoves.Add(new Position(row, col));
                }
            }
        }

        _validPlacesToMove[currentPlayer] = validMoves;
        return validMoves;
    }

    public bool IsMoveValid(IPlayer currentPlayer, Square square)
    {
        // Square harus kosong (belum ada disk)
        if (square.Disk != null)
        {
            return false;
        }

        Colors playerColor = currentPlayer.PlayerColors;
        Colors opponentColor = playerColor == Colors.Black ? Colors.White : Colors.Black;

        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        //cek arah
        foreach (Direction direction in Direction.AllDirections)
        {
            int currentRow = square.Position.Row + direction.RowDelta;
            int currentCol = square.Position.Col + direction.ColDelta;
            bool foundOpponent = false;

            // Cek selama masih dalam batas papan
            while (currentRow >= 0 && currentRow < rows && currentCol >= 0 && currentCol < cols)
            {
                Disk? disk = _board.Square[currentRow, currentCol].Disk;

                if (disk == null)
                {
                    break;
                }

                if (disk.DiskColor == opponentColor)
                {
                    foundOpponent = true;
                }
                else
                {
                    // Ketemu disk sendiri — jika sudah lewati lawan berarti valid
                    if (foundOpponent)
                    {
                        return true;
                    }
                    break;
                }

                currentRow += direction.RowDelta;
                currentCol += direction.ColDelta;
            }
        }

        return false;
    }

    public bool PutDiskOnBoard(IPlayer player, Square square)
    {
        if (square.Disk != null)
        {
            return false;
        }

        if (!IsMoveValid(player, square))
        {
            return false;
        }

        // Simpan posisi move untuk tracking riwayat move
        _lastMovePosition = square.Position;

        // Letakkan disk di square yang dituju
        _board.Square[square.Position.Row, square.Position.Col].Disk =
            new Disk(player.PlayerColors);

        
        bool flipped = DiskFlip(player, _board);
    
        if (!flipped)
        {
            _board.Square[square.Position.Row, square.Position.Col].Disk = null;
            _lastMovePosition = null;
            return false;
        }

        // Trigger event move made
        NotifyMoveMade(_board, player);

        return true;
    }

    public bool DiskFlip(IPlayer player, IBoard board)
    {
        if (_lastMovePosition  == null)
        {
            return false;
        }

        Colors playerColor = player.PlayerColors;
        Colors opponentColor = playerColor == Colors.Black ? Colors.White : Colors.Black;

        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);
        int startRow = _lastMovePosition .Value.Row;
        int startCol = _lastMovePosition .Value.Col;

        bool anyFlipped = false;  

        // Scan 8 arah menggunakan Direction struct
        foreach (Direction direction in Direction.AllDirections)
        {
            int currentRow = startRow + direction.RowDelta;
            int currentCol = startCol + direction.ColDelta;

            List<Position> disksToFlip = new List<Position>();

            while (currentRow >= 0 && currentRow < rows && currentCol >= 0 && currentCol < cols)
            {
                Disk? disk = board.Square[currentRow, currentCol].Disk;

                if (disk == null)
                {
                    break;
                }

                if (disk.DiskColor == opponentColor)
                {
                    // Kandidat untuk di-flip, tapi belum pasti
                    disksToFlip.Add(new Position(currentRow, currentCol));
                }
                else
                {
                    // Jepitan valid! Flip semua disk lawan di antaranya
                    if (disksToFlip.Count > 0)  
                    {
                        foreach (Position position in disksToFlip)
                        {
                            board.Square[position.Row, position.Col].Disk = new Disk(playerColor);
                        }
                        anyFlipped = true;  
                    }
                    break;
                }

                currentRow += direction.RowDelta;
                currentCol += direction.ColDelta;
            }
        }

        return anyFlipped;  
    }

    public IPlayer SwitchTurn()
    {
        int currentIndex = _players.IndexOf(_currentPlayer);
        int nextIndex = (currentIndex + 1) % _players.Count;
        _currentPlayer = _players[nextIndex];

        // Trigger event turn switched
        NotifyTurnSwitched(_currentPlayer);

        return _currentPlayer;
    }

    public bool IsBoardFull()
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);
        int totalSquares = rows * cols;

        return GetTotalDisks() >= totalSquares;
    }

    public bool IsBothPlayersCannotMove()
    {
        foreach (IPlayer player in _players)
        {
            if (HasAnyValidMove(player))
            {
                return false;  // Ada 1 pemain yang masih bisa move → belum stuck
            }
        }
        return true;  // Semua pemain tidak bisa move
    }

    public bool EndGame()
    {
        if (!IsBoardFull() && !IsBothPlayersCannotMove())
        {
            return false;
        }

        CheckWinner();
        OnGameConcluded?.Invoke(_board);
        return true;
    }

    public Dictionary<IPlayer, int> CheckWinner()
    {
        // Reset skor semua pemain
        foreach (IPlayer player in _players)
        {
            _playerScore[player] = 0;
        }

        // Hitung ulang jumlah disk tiap player dengan scan seluruh papan
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Disk? disk = _board.Square[row, col].Disk;

                if (disk != null)
                {
                    foreach (IPlayer player in _players)
                    {
                        if (player.PlayerColors == disk.DiskColor)
                        {
                            _playerScore[player]++;
                            break;
                        }
                    }
                }
            }
        }

        return _playerScore;
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