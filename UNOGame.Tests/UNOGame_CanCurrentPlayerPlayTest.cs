using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class CanCurrentPlayerPlayTest
{
    private GameController _gameController;
    private IBoard _board;
    private IDeck _deck;
    private List<IPlayer> _players;


    [SetUp]
    public void Setup()
    {
        List<ICard> cards =TestDataHelper.GenerateCardsForTest();
        _players = new List<IPlayer> { new Player("A"), new Player("B"), new Player ("C") };
        _deck = new Deck(cards);
        _board = new Board();
        
        _gameController = new GameController(_players,_deck, _board);
        
    }

    [Test]
    public void CanCurrentPlayerPlay_ReturnTrue()
    {
        //getplayerhand
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard playerCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        //masukin kartu ke tangan
        hand.Add(playerCard);

        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        _board.UsedCards.Add(topCard);

        bool canPlay = _gameController.CanCurrentPlayerPlay();

        Assert.That(canPlay, Is.True);
        
    }
    [Test]
    public void CanCurrentPlayerPlay_ReturnFalse()
    {
      
        //getplayerhand
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard playerCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Yellow && card.CardType == CardType.One);
        //masukin kartu ke tangan
        hand.Add(playerCard);

        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        _board.UsedCards.Add(topCard);

        bool canPlay = _gameController.CanCurrentPlayerPlay();
        Assert.That(canPlay, Is.False);

    }
    
    [Test]
    public void CanCurrentPlayerPlay_WhenHasWildCard_ReturnsTrue()
    {
        
        //getplayerhand
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard playerCardWild = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Wild);
        ICard playerCardWildDraw = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.WildDraw);
        //masukin kartu ke tangan
        hand.Add(playerCardWild);
        hand.Add(playerCardWildDraw);


        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        _board.UsedCards.Add(topCard);

        bool canPlay = _gameController.CanCurrentPlayerPlay();

        Assert.That(canPlay, Is.True);
    }

}
