using GameBoardOthello.BackEnd.BackEnd.Enums;
using GameBoardOthello.BackEnd.BackEnd.Interface;
using GameBoardOthello.BackEnd.BackEnd.Models;
using GameBoardOthello.BackEnd.BackEnd.Structs;
using GameBoardOthello.BackEnd.Interface;

namespace GameBoardOthello.Tests;

//Dijadiin 1 class
//nama class nya GameController.Test
public class TestPlayer : IPlayer
{
    public string Name { get; set; }
    public Colors PlayerColors { get; set; }
}

public class TestBoard : IBoard
{
    public TestBoard(int size = 8)
    {
        Square = new Square[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                Square[i, j] = new Square(new Position(i, j), null);
            }
        }
    }

    public Square[,] Square { get; set; }
}

[TestFixture]
public class StartGameShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
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
    public void StartGame_TwoPlayersAreNotProvided_False()
    {
        var onePlayer = new List<IPlayer> { new TestPlayer() };
        _gameController = new GameController(onePlayer, _board!);

        var result = _gameController.StartGame();

        Assert.IsFalse(result);
    }

    [Test]
    public void StartGame_TwoPlayersAreProvided_True()
    {
        _gameController = new GameController(_players, _board);

        var result = _gameController.StartGame();

        Assert.IsTrue(result);
    }

    [Test]
    public void StartGame_InitializeBoard_WithFourStartingDisks()
    {
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();

        var diskCount = _gameController.GetTotalDisks();

        Assert.That(diskCount, Is.EqualTo(4));
    }
}

[TestFixture]
public class HasAnyValidShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
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
    public void HasAnyValid_WhenHasAnyValidMove_True()
    {
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
        var hasValidMoves = _gameController.HasAnyValidMove(_players[0]);

        Assert.True(hasValidMoves, "Return true karena pemain punya langkah valid");
    }

    [Test]
    public void HasAnyValid_BoardIsEmptyBecauseNoDisksToFlip_False()
    {
        _gameController = new GameController(_players!, _board!);

        var result = _gameController.HasAnyValidMove(_players[0]);

        Assert.IsFalse(result, "Return false karena board kosong");
    }
}

[TestFixture]
public class IsMoveValidShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
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
    public void IsMoveValid_PlayerMakeFirstMoveValid_True()
    {
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();

        var isValid = _gameController.IsMoveValid(_players![0], _board!.Square[2, 3]);

        Assert.IsTrue(isValid, "Langkah valid untuk move pertama");
    }


    [Test]
    public void IsMoveValid_PlayerMakeFirstMoveInvalid_False()
    {
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();

        var isInvalid = _gameController.IsMoveValid(_players![0], _board!.Square[2, 2]);

        Assert.IsFalse(isInvalid, "Langkah tidak valid");
    }
}

[TestFixture]
public class GetValidMoveShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
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
    public void GetValidMove_AtStartGame_ReturnFour()
    {
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();

        var validMoves = _gameController.GetValidMoves(_players[0]);

        Assert.That(validMoves.Count, Is.EqualTo(4), "Valid move seharusnya ada 4 pilihan");

        Assert.IsTrue(validMoves.Exists(p => p.Row == 2 && p.Col == 3), "Position [2,3] merupakan move yang valid");
        Assert.IsTrue(validMoves.Exists(p => p.Row == 3 && p.Col == 2), "Position [3,2] merupakan move yang valid");
        Assert.IsTrue(validMoves.Exists(p => p.Row == 4 && p.Col == 5), "Position [4,5] merupakan move yang valid");
        Assert.IsTrue(validMoves.Exists(p => p.Row == 5 && p.Col == 4), "Position [5,4] merupakan move yang valid");
    }

    [Test]
    public void GetValidMove_NoValidMove_ReturnEmptyList()
    {
        _gameController = new GameController(_players!, _board!);

        var result = _gameController.GetValidMoves(_players![0]);

        Assert.IsEmpty(result, "Harus mengembalikan list kosong, jika tidak ada move yang valid");
    }
}

[TestFixture]
public class PutDiskOnBoardShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
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
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void PutDiskOnBoard_MoveIsValid_ReturnTrue()
    {
        var result = _gameController.PutDiskOnBoard(_players[0], _board!.Square[2, 3]);
        Assert.True(result, "Langkah valid untuk move pertama");
    }

    [Test]
    public void PutDiskOnBoard_SquareHasBeenOccupied_ReturnFalse()
    {
        var result = _gameController!.PutDiskOnBoard(_players![0], _board!.Square[3, 3]);
        Assert.IsFalse(result, "Return false karena square sudah terisi");
    }

    [Test]
    public void PutDiskOnBoard_MoveIsInvalid_ReturnFalse()
    {
        var result = _gameController!.PutDiskOnBoard(_players![0], _board!.Square[0, 0]);

        Assert.IsFalse(result, "Return false karena langkah tidak valid");
    }
}

[TestFixture]
public class GetPlayersShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void GetPlayers_ThereIsAPlayer_ReturnTrue()
    {
        var player = _gameController.GetPlayers();

        Assert.That(player.Count, Is.EqualTo(2), "Return 2 player yang terdaftar");
    }

    [Test]
    public void GetPlayers_WhenCalled_ReturnPlayerNameAndColor()
    {
        var player = _gameController.GetPlayers();
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
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void GetCurrentPlayers_AtStartGame_ReturnBlackPlayer()
    {
        var player = _gameController.GetCurrentPlayer();

        Assert.That(player.PlayerColors, Is.EqualTo(Colors.Black), "Return black player ketika first mover");
    }

    [Test]
    public void GetCurrentPlayers_AfterSwitchTurn_ReturnWhitePlayer()
    {
        _gameController.SwitchTurn();
        var player = _gameController.GetCurrentPlayer();

        Assert.That(player, Is.SameAs(_players[1]), "Current player berganti ke putih");
    }
}

[TestFixture]
public class GetBoardShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void GetBoard_BoardCalled_ReturnWithCorrectSize()
    {
        var board = _gameController.GetBoard();

        Assert.That(board.Square.GetLength(0), Is.EqualTo(8), "Jumlah baris 8");
        Assert.That(board.Square.GetLength(1), Is.EqualTo(8), "Jumlah kolom 8");
    }

    [Test]
    public void GetBoard_FirstGame_ReturnBoardWithFourInitialDisk()
    {
        var board = _gameController.GetBoard();

        Assert.That(board.Square[3, 3].Disk.DiskColor, Is.EqualTo(Colors.White));
        Assert.That(board.Square[3, 4].Disk.DiskColor, Is.EqualTo(Colors.Black));
        Assert.That(board.Square[4, 3].Disk.DiskColor, Is.EqualTo(Colors.Black));
        Assert.That(board.Square[4, 4].Disk.DiskColor, Is.EqualTo(Colors.White));
    }
}

[TestFixture]
public class IsBoardFullShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void IsBoardFull_BoardNotFull_False()
    {
        var boardNotFull = _gameController.IsBoardFull();

        Assert.IsFalse(boardNotFull, "Board tidak full pada awal permainan");
    }

    [Test]
    public void IsBoardFull_BoardFull_True()
    {
        var rows = _board.Square.GetLength(0);
        var cols = _board.Square.GetLength(1);

        for (var row = 0; row < rows; row++)
        for (var col = 0; col < cols; col++)
            _board.Square[row, col].Disk = new Disk(Colors.Black);

        var boardFull = _gameController.IsBoardFull();

        Assert.True(boardFull, "Board Full");
    }
}

[TestFixture]
public class IsBothPlayerCannotMoveShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void BothPlayerCannotMove_BothPlayerCanMove_False()
    {
        var playersCanMove = _gameController.IsBothPlayersCannotMove();

        Assert.False(playersCanMove, "Player dapat bergerak diawal permainan");
    }

    [Test]
    public void BothPlayerCannotMove_BothPlayerCannotMove_True()
    {
        var rows = _board.Square.GetLength(0);
        var cols = _board.Square.GetLength(1);

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                _board.Square[row, col].Disk = new Disk(Colors.Black);
            }
        }

        var playersCannotMove = _gameController.IsBothPlayersCannotMove();

        Assert.True(playersCannotMove, "Player tidak dapat bergerak karena board penuh");
    }
}

[TestFixture]
public class IsEndGameShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void IsEndGame_GameNotEnd_False()
    {
        var gameNotEnd = _gameController.EndGame();

        Assert.False(gameNotEnd, "Game belum end karna permainan masih dimulai");
    }

    [Test]
    public void IsEndGame_GameEnd_True()
    {
        var rows = _board.Square.GetLength(0);
        var cols = _board.Square.GetLength(1);

        for (var row = 0; row < rows; row++)
        for (var col = 0; col < cols; col++)
            if (row <= 4)
                _board.Square[row, col].Disk = new Disk(Colors.Black);
            else
                _board.Square[row, col].Disk = new Disk(Colors.White);

        var gameEnd = _gameController.EndGame();
        Assert.True(gameEnd, "Game telah berakhir");

        var players = _gameController.CheckWinner();
        Assert.That(players.Values.Max(), Is.EqualTo(40), "Player black memenangkan dengan 40 piece");
        Assert.That(players.Values.Min(), Is.EqualTo(24), "Player white hanya memasukkan 24 piece");
    }
}

[TestFixture]
public class CheckWinnerShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void CheckWinner_GameEnd_ReturnPlayerAndScore()
    {
        var rows = _board.Square.GetLength(0);
        var cols = _board.Square.GetLength(1);

        for (var row = 0; row < rows; row++)
        for (var col = 0; col < cols; col++)
            if (row <= 4)
                _board.Square[row, col].Disk = new Disk(Colors.Black);
            else
                _board.Square[row, col].Disk = new Disk(Colors.White);

        var playersWin = _gameController.CheckWinner();
        var winner = playersWin.MaxBy(player => player.Value);

        Assert.That(winner.Key.PlayerColors, Is.EqualTo(Colors.Black), "Player dengan piece terbanyak");
        Assert.That(winner.Value, Is.EqualTo(40), "Total 40 pieces");
    }
}

[TestFixture]
public class GetLastMovePositionShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void GetLastMovePosition_PlayerNoMoveMade_ReturnNull()
    {
        var lastMove = _gameController.GetLastMovePosition();

        Assert.IsNull(lastMove, "Belum ada move");
    }

    [Test]
    public void GetLastMovePosition_AfterMoveMade_ReturnLastPosition()
    {
        _gameController.PutDiskOnBoard(_players[0], _board.Square[2, 3]);
        var lastMove = _gameController.GetLastMovePosition();

        Assert.IsNotNull(lastMove, "Player 1 sudah menempatkan disk di board");
        Assert.That(lastMove.Value.Row, Is.EqualTo(2), "Row 2");
        Assert.That(lastMove.Value.Col, Is.EqualTo(3), "Col 3");
    }
}

[TestFixture]
public class DiskFlipShould
{
    private IBoard? _board;
    private GameController? _gameController;
    private List<IPlayer>? _players;

    [SetUp]
    public void SetUp()
    {
        _board = new TestBoard();
        _players = new List<IPlayer>
        {
            new TestPlayer { Name = "John", PlayerColors = Colors.Black },
            new TestPlayer { Name = "Smith", PlayerColors = Colors.White }
        };
        _gameController = new GameController(_players!, _board!);
        _gameController.StartGame();
    }

    [Test]
    public void DiskFlip_DiskFlipCall_FlipEnemyColor()
    {
        var before = _board.Square[3, 3].Disk.DiskColor;

        _gameController.PutDiskOnBoard(_players[0], _board.Square[2, 3]);
        _gameController.DiskFlip(_players[0], _board);

        var after = _board.Square[3, 3].Disk.DiskColor;

        Assert.That(before, Is.EqualTo(Colors.White), "Disk White sebelum flip");
        Assert.That(after, Is.EqualTo(Colors.Black), "Disk Black setelah flip");
    }

    [Test]
    public void DiskFlip_DiskCanNotFlip_False()
    {
        _gameController.PutDiskOnBoard(_players[0], _board.Square[2, 4]);

        var canNotFlip = _gameController.DiskFlip(_players[0], _board);

        Assert.False(canNotFlip, "Disk tidak flip karena move tidak valid");
    }

    [Test]
    public void DiskFlip_NoLastPosition_False()
    {
        var anyDiskFlipped = _gameController.DiskFlip(_players[0], _board);

        Assert.False(anyDiskFlipped, "Return false karena belum ada disk yang player hitam masukkan");
    }
}