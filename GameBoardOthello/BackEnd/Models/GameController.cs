using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Structs;
using GameBoardOthello.BackEnd.Enum;
using GameBoardOthello.BackEnd.Interface;
using GameBoardOthello.BackEnd.Structs;

namespace GameBoardOthello.BackEnd.Models;

public class GameController : IGameController
{
    private const int INITIAL_DISK_COUNT = 4;
    private const int INITIAL_SCORE_PER_PLAYER = 2;
    
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
        ClearBoard(board);
        PlaceInitialDisks(board);
        InitializePlayerData(players);

        // Player pertama (biasanya Black) selalu mulai duluan di Othello
        _currentPlayer = players[0];
    }
    
    public List<IPlayer> GetPlayer(List<IPlayer> players) => _players;

    public IPlayer GetCurrentPlayer(IPlayer currentPlayer) => _currentPlayer;

    public IBoard GetBoard(IBoard board) => _board;

    public Position? GetLastMovePosition() => _lastMovePosition;
    
    public int GetTotalDisks(int totalDisksOnBoard) => _totalDisksOnBoard;

    public bool HasAnyValidMove(IBoard board, IPlayer currentPlayer)
    {
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (IsMoveValid(currentPlayer, board.Square[row, col]))
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
        if (square.Disk != null) return false;

        Colors playerColors = currentPlayer.PlayerColors;
        Colors opponentColors = playerColors == Colors.Black ? Colors.White : Colors.Black;

        foreach (var direction in Direction.AllDirection)
        {
            if (HasValidSandwichInDirection(square.Position, direction, playerColors, opponentColors)) return true;
        }

        return false;
    }

    public void PutDiskOnBoard(IPlayer player, Square square, int totalDisksOnBoard)
    {
        var targetSquare = _board.Square[square.Position.Row, square.Position.Col];
        targetSquare.Disk = new Disk(player.PlayerColors);
        
        //update game state
        _totalDisksOnBoard = totalDisksOnBoard + 1;
        _lastPlacedSquare = targetSquare;
        _lastMovePosition = square.Position;

        // Flip disk lawan yang terjepit
        DiskFlip(player, _board);

        // Trigger event move made
        NotifyMoveMade(_board, player);
    }

    public void DiskFlip(IPlayer player, IBoard board)
    {
        if (_lastPlacedSquare == null) return;

        var playerColor = player.PlayerColors;
        var opponentColor = GetOpponentColor(playerColor);
        var startPosition = _lastPlacedSquare.Position;

        foreach (var direction in Direction.AllDirection)
        {
            FlipDisksInDirection(board, startPosition, direction, playerColor, opponentColor);
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
        int totalSquares = rows * cols;
        
        return totalDisksOnBoard >= totalSquares;
    }

    public bool IsBothPlayersCannotMove()
    {
        foreach (var player in _players)
        {
            if (HasAnyValidMove(_board, player)) return false;
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
        foreach (var player in _players)
        {
            _playerScore[player] = 0;
        }

        CountDisksOnBoard();
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


    private void ClearBoard(IBoard board)
    {
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                board.Square[row, col] = new Square(new Position(row, col), null);
            }
        }
    }
    
    private void PlaceInitialDisks(IBoard board)
    {
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);
        int midRow = rows / 2;
        int midCol = cols / 2;

        board.Square[midRow - 1, midCol - 1].Disk = new Disk(Colors.White);
        board.Square[midRow - 1, midCol].Disk     = new Disk(Colors.Black);
        board.Square[midRow,     midCol - 1].Disk = new Disk(Colors.Black);
        board.Square[midRow,     midCol].Disk     = new Disk(Colors.White);

        _totalDisksOnBoard = INITIAL_DISK_COUNT;
    }
    
    private void InitializePlayerData(List<IPlayer> players)
    {
        foreach (var player in players)
        {
            _playerScore[player] = INITIAL_SCORE_PER_PLAYER;
            _validPlacesToMove[player] = new List<Position>();
        }
    }
    
    private Colors GetOpponentColor(Colors playerColor)
    {
        return playerColor == Colors.Black ? Colors.White : Colors.Black;
    }
    
    private bool HasValidSandwichInDirection(
        Position startPosition, 
        Direction direction, 
        Colors playerColor, 
        Colors opponentColor)
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);
        
        int currentRow = startPosition.Row + direction.RowDelta;
        int currentCol = startPosition.Col + direction.ColDelta;
        bool foundOpponentDisk = false;

        while (IsWithinBounds(currentRow, currentCol, rows, cols))
        {
            var disk = _board.Square[currentRow, currentCol].Disk;
            
            if (disk == null)
                break;

            if (disk.DiskColor == opponentColor)
            {
                foundOpponentDisk = true;
            }
            else // disk.DiskColor == playerColor
            {
                // Valid sandwich found: opponent disk(s) between two player disks
                return foundOpponentDisk;
            }

            currentRow += direction.RowDelta;
            currentCol += direction.ColDelta;
        }

        return false;
    }
    
    private void FlipDisksInDirection(
        IBoard board, 
        Position startPosition, 
        Direction direction, 
        Colors playerColor, 
        Colors opponentColor)
    {
        int rows = board.Square.GetLength(0);
        int cols = board.Square.GetLength(1);
        
        int currentRow = startPosition.Row + direction.RowDelta;
        int currentCol = startPosition.Col + direction.ColDelta;
        
        var disksToFlip = new List<Position>();

        while (IsWithinBounds(currentRow, currentCol, rows, cols))
        {
            var disk = board.Square[currentRow, currentCol].Disk;
            
            if (disk == null)
                break;

            if (disk.DiskColor == opponentColor)
            {
                // Candidate for flipping
                disksToFlip.Add(new Position(currentRow, currentCol));
            }
            else // disk.DiskColor == playerColor
            {
                // Valid sandwich! Flip all collected opponent disks
                foreach (var position in disksToFlip)
                {
                    board.Square[position.Row, position.Col].Disk = new Disk(playerColor);
                }
                break;
            }

            currentRow += direction.RowDelta;
            currentCol += direction.ColDelta;
        }
    }

    private bool IsWithinBounds(int row, int col, int maxRows, int maxCols)
    {
        return row >= 0 && row < maxRows && col >= 0 && col < maxCols;
    }
    
    private void CountDisksOnBoard()
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var disk = _board.Square[row, col].Disk;
                
                if (disk != null)
                {
                    foreach (var player in _players)
                    {
                        if (player.PlayerColors == disk.DiskColor)
                        {
                            _playerScore[player]++;
                            break; // Found the player, no need to continue
                        }
                    }
                }
            }
        }
    }
    
}