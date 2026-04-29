using GameBoardOthello.BackEnd.BackEnd.Enums;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.BackEnd.Structs;
using GameBoardOthello.BackEnd.Interface;

namespace GameBoardOthello.Tests;


public class TestPlayer : IPlayer 
{ 
    public string Name { get; set; }
    public Colors PlayerColors { get; set; } 
}

public class TestBoard : IBoard 
{ 
    public Square[,] Square { get; set; }

    public TestBoard(int size = 8)
    {
        Square = new Square[size, size]; 

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Square[i, j] = new Square(new Position(i, j), null);
            }
        }
    }
}

[TestFixture]
public class StartGameShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer
            {
                Name = "John",
                PlayerColors = Colors.Black
            },
            new TestPlayer
            {
                Name = "Smith",
                PlayerColors = Colors.White
            }
        };
    }

    [Test]
    public void ReturnFalse_WhenPlayersAreLessThanTwo()
    {
        var lonePlayer = new List<IPlayer> { new TestPlayer() };
        _gc = new GameController(lonePlayer, _board!);
        
        bool result = _gc.StartGame();
            
        Assert.IsFalse(result);
    }

    [Test]
    public void ReturnTrue_WhenTwoPlayersAreProvided()
    {
        _gc = new GameController(_players!, _board!);
        
        bool result = _gc.StartGame();
            
        Assert.IsTrue(result);
    }

    [Test]
    public void InitializeBoardWithFourStartingDisks()
    {
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
        
        int diskCount = _gc.GetTotalDisks();

        Assert.That(diskCount, Is.EqualTo(4));
    }
}

[TestFixture]
public class HasAnyValidShould
{
    private GameController? _gc;
    private List<IPlayer>? _players;
    private IBoard? _board;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer
            {
                Name = "John",
                PlayerColors = Colors.Black
            },
            new TestPlayer
            {
                Name = "Smith",
                PlayerColors = Colors.White
            }
        };
    }
    
    [Test]
    public void ReturnTrue_WhenHasAnyValid()
    {
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
        bool hasValidMoves = _gc.HasAnyValidMove(_players[0]);
        
        Assert.True(hasValidMoves, "Return true karena pemain punya langkah valid");
    }
    
    [Test]
    public void ReturnFalse_WhenBoardIsEmpty_BecauseNoDisksToFlip()
    {
        _gc = new GameController(_players!, _board!);
        
        bool result = _gc.HasAnyValidMove(_players[0]);

        Assert.IsFalse(result, "Return false karena board kosong");
    }
}

[TestFixture]
public class IsMoveValidShould
{
    private GameController? _gc;
    private List<IPlayer>? _players;
    private IBoard? _board;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer
            {
                Name = "John",
                PlayerColors = Colors.Black
            },
            new TestPlayer
            {
                Name = "Smith",
                PlayerColors = Colors.White
            }
        };
    }

    [Test]
    public void ReturnTrue_WhenPlayerMakeFirstMoveValid()
    {
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();

        bool isValid = _gc.IsMoveValid(_players![0], _board!.Square[2, 3]);
        
        Assert.IsTrue(isValid, "Langkah valid untuk move pertama");
    }

    
    [Test]
    public void ReturnFalse_WhenPlayerMakeFirstMoveInvalid()
    {
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
        
        bool isInvalid = _gc.IsMoveValid(_players![0], _board!.Square[2, 2]);
        
        Assert.IsFalse(isInvalid, "Langkah tidak valid");
    }
}

[TestFixture]
public class GetValidMoveShould
{
    private GameController? _gc;
    private List<IPlayer>? _players;
    private IBoard? _board;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer
            {
                Name = "John",
                PlayerColors = Colors.Black
            },
            new TestPlayer
            {
                Name = "Smith",
                PlayerColors = Colors.White
            }
        };
    }

    [Test]
    public void ReturnFour_AtStartGame()
    {
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();

        List<Position> validMoves = _gc.GetValidMoves(_players[0]);

        Assert.That(validMoves.Count, Is.EqualTo(4), "Valid move seharusnya ada 4 pilihan");
        
        Assert.IsTrue(validMoves.Exists(p => p.Row == 2 && p.Col == 3), "Position [2,3] merupakan move yang valid");
        Assert.IsTrue(validMoves.Exists(p => p.Row == 3 && p.Col == 2), "Position [3,2] merupakan move yang valid");
        Assert.IsTrue(validMoves.Exists(p => p.Row == 4 && p.Col == 5), "Position [4,5] merupakan move yang valid");
        Assert.IsTrue(validMoves.Exists(p => p.Row == 5 && p.Col == 4), "Position [5,4] merupakan move yang valid");
    }

    [Test]
    public void ReturnEmptyList_WhenNoValidMoves()
    {
        _gc = new GameController(_players!, _board!);

        var result = _gc.GetValidMoves(_players![0]);
        
        Assert.IsEmpty(result, "Harus mengembalikan list kosong, jika tidak ada move yang valid");
    }
}

[TestFixture]
public class PutDiskOnBoardShould
{
    private GameController? _gc;
    private List<IPlayer>? _players;
    private IBoard? _board;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer
            {
                Name = "John",
                PlayerColors = Colors.Black
            },
            new TestPlayer
            {
                Name = "Smith",
                PlayerColors = Colors.White
            }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnTrue_WhenMoveIsValid()
    {
        bool result = _gc.PutDiskOnBoard(_players[0], _board!.Square[2, 3]);
        Assert.True(result, "Langkah valid untuk move pertama");
    }

    [Test]
    public void ReturnFalse_WhenSquareHasBeenOccupied()
    {
        bool result = _gc!.PutDiskOnBoard(_players![0], _board!.Square[3, 3]);
        Assert.IsFalse(result, "Return false karena square sudah terisi");
    }
    
    [Test]
    public void ReturnFalse_WhenMoveIsInvalid()
    {
        bool result = _gc!.PutDiskOnBoard(_players![0], _board!.Square[0, 0]);

        Assert.IsFalse(result, "Return false karena langkah tidak valid");
    }
}


[TestFixture]
public class GetPlayersShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John",  PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnTrue_WhenThereIsAPlayer()
    {
        List<IPlayer> player = _gc.GetPlayers();
        
        Assert.That(player.Count, Is.EqualTo(2), "Return 2 player yang terdaftar");
    }

    [Test]
    public void ReturnPlayerNameAndColor_WhenCalled()
    {
        List<IPlayer> player = _gc.GetPlayers();
        Assert.Multiple(() =>
        {
            Assert.That(player[0].Name, Is.EqualTo("John"), "Return player name yang terdaftar");
            Assert.That(player[0].PlayerColors, Is.EqualTo(Colors.Black), "Disk Black");
            
            Assert.That(player[1].Name, Is.EqualTo("Smith"), "Return player name yang terdaftar");
            Assert.That(player[1].PlayerColors, Is.EqualTo(Colors.White), "Disk White");
        });
    }
}

[TestFixture]
public class GetCurrentPlayersShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John",  PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnBlackPlayer_AtStartGame()
    {
        IPlayer player = _gc.GetCurrentPlayer();
        
        Assert.That(player.PlayerColors, Is.EqualTo(Colors.Black), "Return black player ketika first mover");
    }

    [Test]
    public void ReturnWhitePlayer_AfterSwitchTurn()
    {
        _gc.SwitchTurn();
        IPlayer player = _gc.GetCurrentPlayer();
        
        Assert.That(player, Is.SameAs(_players[1]), "Current player berganti ke putih");
    }
}

[TestFixture]
public class GetBoardShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnWithCorrectSize_WhenBoardCalled()
    {
        IBoard board = _gc.GetBoard();
        
        Assert.That(board.Square.GetLength(0), Is.EqualTo(8), "Jumlah baris 8");
        Assert.That(board.Square.GetLength(1), Is.EqualTo(8), "Jumlah kolom 8");
    }

    [Test]
    public void ReturnBoardWithFourIntialDisk_WhenFirstGame()
    {
        IBoard board = _gc.GetBoard();

        Assert.That(board.Square[3, 3].Disk.DiskColor, Is.EqualTo(Colors.White));
        Assert.That(board.Square[3, 4].Disk.DiskColor, Is.EqualTo(Colors.Black));
        Assert.That(board.Square[4, 3].Disk.DiskColor, Is.EqualTo(Colors.Black));
        Assert.That(board.Square[4, 4].Disk.DiskColor, Is.EqualTo(Colors.White));
    }
}

[TestFixture]
public class IsBoardFullShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnFalse_WhenBoardIsFull()
    {
        bool boardNotFull = _gc.IsBoardFull();
        
        Assert.IsFalse(boardNotFull, "Board tidak full pada awal permainan");
    }

    [Test]
    public void ReturnTrue_WhenBoardIsFull()
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                _board.Square[row, col].Disk = new Disk(Colors.Black);
            }
        }
        
        bool boardFull = _gc.IsBoardFull();
        
        Assert.True(boardFull, "Board Full");
    }
}


[TestFixture]
public class IsBothPlayerCannotMoveShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnFalse_WhenBothPlayerCanMove()
    {
        bool playersCanMove = _gc.IsBothPlayersCannotMove();
        
        Assert.False(playersCanMove, "Player dapat bergerak diawal permainan");
    }

    [Test]
    public void ReturnTrue_WhenBothPlayerCannotMove()
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                _board.Square[row, col].Disk = new Disk(Colors.Black);
            }
        }

        bool playersCannotMove = _gc.IsBothPlayersCannotMove();

        Assert.True(playersCannotMove, "Player tidak dapat bergerak karena board penuh");
    }
}


[TestFixture]
public class IsEndGameShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnFalse_WhenGameNotEnd()
    {
        bool gameNotEnd = _gc.EndGame();
        
        Assert.False(gameNotEnd, "Game belum end karna permainan masih dimulai");
    }

    [Test]
    public void ReturnTrue_WhenGameEndWithFullDiceInBoard()
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row <= 4)
                {
                    _board.Square[row, col].Disk = new Disk(Colors.Black);
                }
                else
                {
                    _board.Square[row, col].Disk = new Disk(Colors.White);
                }
            }
        }
        
        bool gameEnd = _gc.EndGame();
        Assert.True(gameEnd, "Game telah berakhir");

        Dictionary<IPlayer, int> players = _gc.CheckWinner();
        Assert.That(players.Values.Max(), Is.EqualTo(40), "Player black memenangkan dengan 40 piece");
        Assert.That(players.Values.Min(), Is.EqualTo(24), "Player white hanya memasukkan 24 piece");
    }
}

[TestFixture]
public class CheckWinnerShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnPlayerAndScore_WhenGameEnd()
    {
        int rows = _board.Square.GetLength(0);
        int cols = _board.Square.GetLength(1);
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row <= 4)
                {
                    _board.Square[row, col].Disk = new Disk(Colors.Black);
                }
                else
                {
                    _board.Square[row, col].Disk = new Disk(Colors.White);
                }
            }
        }
        
        Dictionary<IPlayer, int> playersWin = _gc.CheckWinner();
        KeyValuePair<IPlayer, int> winner = playersWin.First();
        
        Assert.That(winner.Key.PlayerColors, Is.EqualTo(Colors.Black), "Player black dengan piece terbanyak");
        Assert.That(winner.Value, Is.EqualTo(40), "Total 40 pieces");
    }
}

[TestFixture]
public class GetLastMovePositionShould
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void ReturnNull_WhenPlayerNoMoveMade()
    {
        Position? lastMove = _gc.GetLastMovePosition();
        
        Assert.IsNull(lastMove, "Belum ada move");
    }

    [Test]
    public void ReturnLastPosition_AfterMoveMade()
    {
        _gc.PutDiskOnBoard(_players[0], _board.Square[2, 3]);
        Position? lastMove = _gc.GetLastMovePosition();
        
        Assert.IsNotNull(lastMove, "Player 1 sudah menempatkan disk di board");
        Assert.That(lastMove.Value.Row, Is.EqualTo(2), "Row 2");
        Assert.That(lastMove.Value.Col, Is.EqualTo(3), "Col 3");
    }
}

[TestFixture]
public class DiskFlipShould()
{
    private GameController? _gc;
    private IBoard? _board;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard(8);
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gc = new GameController(_players!, _board!);
        _gc.StartGame();
    }

    [Test]
    public void FlipEnemyColor_WhenDiskFlipCall()
    {
        Colors before = _board.Square[3, 3].Disk.DiskColor;
        
        _gc.PutDiskOnBoard(_players[0], _board.Square[2, 3]);
        _gc.DiskFlip(_players[0], _board);
        
        Colors after = _board.Square[3, 3].Disk.DiskColor;

        Assert.That(before, Is.EqualTo(Colors.White), "Disk White sebelum flip");
        Assert.That(after, Is.EqualTo(Colors.Black), "Disk Black setelah flip");
    }

    [Test]
    public void ReturnFalse_WhenNoLastPosition()
    {
        bool anyDiskFlipped = _gc.DiskFlip(_players[0], _board);
        
        Assert.False(anyDiskFlipped, "Return false karena belum ada disk yang player hitam masukkan");
    }
}