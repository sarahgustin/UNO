using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class GetIsClockwiseTest
{
    private GameController _gameController;

    [SetUp]
    public void Setup()
    {
        List<ICard> cards =TestDataHelper.GenerateCardsForTest();
        List<IPlayer> players = new List<IPlayer> { new Player("A"), new Player("B"), new Player ("C") };
        IDeck deck = new Deck(cards);
        IBoard board = new Board();
        
        _gameController = new GameController(players, deck, board);
        
    }

    [Test]
    public void GetIsClockwise_RetrunTrue()
    {
        bool result = _gameController.GetIsClockwise();
        Assert.That(result, Is.True);
        
    }
    [Test]
    public void GetIsClockwise_RetrunFalse()
    {
        _gameController.IsClockWise = false;
        bool result = _gameController.GetIsClockwise();
        Assert.That(result, Is.False);
    }
    

}
