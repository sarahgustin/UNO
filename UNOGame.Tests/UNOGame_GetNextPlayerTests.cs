using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class PlayerRotaionTest
{
    private GameController _gameController;

    [SetUp]
    public void Setup()
    {
        List<ICard> cards = TestDataHelper.GenerateCardsForTest();
        List<IPlayer> players = new List<IPlayer> { new Player("A"), new Player("B"), new Player ("C"), new Player ("D")  };
        IDeck deck = new Deck(cards);
        IBoard board = new Board();
        
        _gameController = new GameController(players, deck, board);
        
    }
    [Test]
    public void nextPlayer_NormalRotaion_ShouldIncrementIndex()
    {
        //ambil dulu list player 
        List<IPlayer> players = _gameController.GetPlayerList();
        //current player
        IPlayer expectedNextPlayer = players[1];
        
        //getnextplayer
        IPlayer NextPlayer = _gameController.GetNextPlayer();

        Assert.That(NextPlayer, Is.EqualTo(expectedNextPlayer), "Next player harus index selanjutnya");
    }

        //cek reverse rotation
    [Test]
    public void nextPlayer_ReverseRotaion_ShouldecrementIndex()
    {
        //clock wise is false
        _gameController.IsClockWise = false;

        //list player
        List<IPlayer> players = _gameController.GetPlayerList();

        //expected index 
        IPlayer expectedNextPlayer = players[3];
        //getnext player
        IPlayer nextPlayer = _gameController.GetNextPlayer();

        Assert.That(nextPlayer, Is.EqualTo(expectedNextPlayer), "Next player harus index sebelumnya");
    }

    //cek kalo reverse pemainnya cuma 2 jadi ke skip
    /*
    [Test]
    public void NextPlayer_ReverseTwoPlayers_ShouldSkipNextPlayer()
    {
         //clock wise is false
        _gameController.IsClockWise = false;

        //list player
        List<IPlayer> players = _gameController.GetPlayerList();
        //expected index 
        IPlayer expectedNextPlayer = players[1];
        //getnext player
        IPlayer nextPlayer = _gameController.GetNextPlayer();

        Assert.That(nextPlayer, Is.EqualTo(expectedNextPlayer), "Jika 2 pemain, reverse harus kembali ke pemain satunya");
    }*/
    
    //cek kalo next index setelah dari player max index kembali ke nol
}
