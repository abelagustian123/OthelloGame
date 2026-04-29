using GameBoardOthello.BackEnd.BackEnd.Enums;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Structs;
using GameBoardOthello.BackEnd.Interface;
using Serilog;

namespace GameBoardOthello.BackEnd.BackEnd.Models;

public class GameController : IGameController
{
    private IBoard _board;
    private Dictionary<IPlayer, int> _playerScore = new Dictionary<IPlayer, int>();
    private Dictionary<IPlayer, List<Position>> _validPlacesToMove = new Dictionary<IPlayer, List<Position>>();
    private List<IPlayer> _players;
    
    //bentuk format json loggingnya
    
    public event Action<IPlayer>? OnTurnSkipped;
    public event Action<IPlayer>? OnTurnSwitched;
    public event Action<IBoard, IPlayer>? OnMoveMade;
    public event Action<IBoard>? OnGameConcluded;

    private IPlayer _currentPlayer;
    private Position? _lastMovePosition;

    private readonly ILogger _logger = Log.ForContext<GameController>();
    
    public GameController(List<IPlayer> players, IBoard board)
    {
        _players = players;
        _board = board;
        _currentPlayer = players[0];
        
        _logger.Debug("GameController diinisiasi dengan {PlayerCount} pemain", players.Count);
    }
    
    public bool StartGame()
    {
        if (_players == null || _players.Count < 2)
        {
            _logger.Warning("Game gagal dimulai, pemain kurang dari 2 atau null");
            return false;
        }

        InitializeGame(_players, _board);
        _logger.Information($"Game dimulai dengan {_players.Count} pemain");
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
        _logger.Information($"Board {rows}x{cols} diinisialisasi");
        _logger.Debug($"Current Player {_currentPlayer.Name} dengan warna disk {_currentPlayer.PlayerColors}");
    }

    public List<IPlayer> GetPlayers() 
    {
        _logger.Debug($"Method GetPlayers: {_players.Count}");
        return _players;
    }

    public IPlayer GetCurrentPlayer()
    {
        _logger.Debug($"Method GetCurrentPlayer: {_currentPlayer.Name}");

        return _currentPlayer;
    }

    public IBoard GetBoard()
    {
        _logger.Debug($"Method GetBoard: {_board}");
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
        
        _logger.Debug($"Total disk saat ini {total}");
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

        _logger.Warning($"{currentPlayer.Name} tidak memiliki move valid");
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
        
        _logger.Debug($"{currentPlayer.Name} memiliki {validMoves.Count} valid move");

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
                        if (_currentPlayer.PlayerColors == playerColor)
                        {
                            _logger.Debug($"Move valid di [{square.Position.Row + 1},{square.Position.Col + 1}] untuk {currentPlayer.Name}");
                        }
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
            _logger.Warning($"Square dengan [{square.Position.Row}, {square.Position.Col}] sudah terisi");
            return false;
        }

        if (!IsMoveValid(player, square))
        {
            _logger.Warning($"Move tidak valid di [{square.Position.Row}, {square.Position.Col}] oleh {player.Name}");
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
            _logger.Warning($"Disk flip batal, move dibatalkan di [{square.Position.Row}, {square.Position.Col}]");
            return false;
        }

        _logger.Information($"{player.Name} meletakan disk di [{square.Position.Row}, {square.Position.Col}]");
        
        NotifyMoveMade(_board, player);
        return true;
    }

    public bool DiskFlip(IPlayer player, IBoard board)
    {
        if (_lastMovePosition  == null)
        {
            _logger.Warning($"Disk Flip gagal dipanggil karena lastMovePosition null");
            return false;
        }

        Colors playerColor = player.PlayerColors;
        Colors opponentColor = playerColor == Colors.Black ? Colors.White : Colors.Black;

        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);
        int startRow = _lastMovePosition.Value.Row;
        int startCol = _lastMovePosition.Value.Col;

        bool anyFlipped = false;  

        // Scan 8 arah menggunakan Direction 
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
                            _logger.Debug($"{disksToFlip.Count} disk di flip oleh {player.Name}");
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
        bool isFull = GetTotalDisks() >= totalSquares;

        if (isFull)
        {
            _logger.Information("Board Penuh");
        }
        
        return isFull;
    }

    public bool IsBothPlayersCannotMove()
    {
        foreach (IPlayer player in _players)
        {
            if (HasAnyValidMove(player))
            {
                return false;  
            }
        }
        
        _logger.Information($"Kedua pemain tidak bisa menempatkan disk di board");
        return true;  
    }

    public bool EndGame()
    {
        if (!IsBoardFull() && !IsBothPlayersCannotMove())
        {
            _logger.Warning("Game masih belum selesai, board masih ada bisa diisi dan masih ada valid move");
            return false;
        }

        CheckWinner();
        _logger.Information("Game over");
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

        foreach (KeyValuePair<IPlayer, int> score in _playerScore)
        {
            _logger.Information($"Skor {score.Key.Name} adalah {score.Value}");
        }
        return _playerScore;
    }

    public Position? GetLastMovePosition()
    {
        if (_lastMovePosition.HasValue)
            _logger.Debug($"Last move position adalah [{_lastMovePosition.Value.Row + 1},{_lastMovePosition.Value.Col + 1}]" 
                );
        else
            _logger.Debug("Last move position adalah null");
        return _lastMovePosition;
    }
    
    public void NotifyTurnSkipped(IPlayer player)
    {
        _logger.Information($"Player {player.Name} di skip, tidak ada move valid");
        OnTurnSkipped?.Invoke(player);
    }

    public void NotifyTurnSwitched(IPlayer player)
    {
        _logger.Information($"Player {player.Name}, sekarang giliran untuk menempatkan disk di board");
        OnTurnSwitched?.Invoke(player);
    }

    public void NotifyMoveMade(IBoard board, IPlayer player)
    {
        _logger.Information($"Move telah dibuat oleh {player.Name}");
        OnMoveMade?.Invoke(board, player);
    }
}