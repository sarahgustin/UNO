using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class GetTopCardrTest
{
    private GameController _gameController;
    private IBoard _board;
    private IDeck _deck;

    [SetUp]
    public void Setup()
    {
        List<ICard> cards =TestDataHelper.GenerateCardsForTest();
        List<IPlayer> players = new List<IPlayer> { new Player("A"), new Player("B"), new Player ("C") };
        _deck = new Deck(cards);
        _board = new Board();
        
        _gameController = new GameController(players, _deck, _board);
        
    }
    //test playe dapet 7 kartu di awal
    [Test]
    public void GetTopCard_WhenBoardIsEmpty_ShouldReturnNull()
    {
        _board.UsedCards.Clear();
        //board kosong bikin pake mock
        ICard topCard = _gameController.GetTopCard();
        Assert.That(topCard, Is.Null, "Tidak ada kartu di board harunys Null");
    }
    [Test]
    public void GetTopCard_WhenBoardHasCard_ShouldReturnThatCard()
    {
        ICard topCard = _gameController.GetTopCard();
        Assert.That(topCard, Is.Not.Null);
    }
    

}
