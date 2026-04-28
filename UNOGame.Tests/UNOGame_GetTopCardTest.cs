using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class GetTopCardrTest
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
    //test playe dapet 7 kartu di awal
    [Test]
    public void GetTopCard_WhenBoardIsEmpty_ShouldReturnNull()
    {
        ICard topCard = _gameController.GetTopCard();
        Assert.That(topCard, Is.Null, "Tidak ada kartu di board harunys Null");
    }
}
