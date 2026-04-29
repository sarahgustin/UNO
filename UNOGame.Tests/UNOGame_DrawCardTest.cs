using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class DrawCardTest
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
    public void DrawCard_DeckEmpty_ShouldRefillFromUsedCards()
    {
      _deck.Cards.Clear();

      List<ICard> cardToTest = TestDataHelper.GenerateCardsForTest().Take(5).ToList();
      _board.UsedCards.AddRange(cardToTest);
      _gameController.DrawCard();

      Assert.That(_deck.Cards.Count, Is.GreaterThan(0));
      Assert.That(_board.UsedCards.Count, Is.EqualTo(1));    
        
    }
    [Test]
    public void DrawCard_ShouldRemoveCardsFromDeck()
    {
        int cardCount = _deck.Cards.Count;

        _gameController.DrawCard();

        Assert.That(_deck.Cards.Count, Is.EqualTo(cardCount -1));
      
    }
    [Test]
    public void DrawCard_WhenRefill_ShouldResetWildCardColors()
    {
        _deck.Cards.Clear();
        _board.UsedCards.Clear();
        
        ICard wildCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Wild);
        wildCard.CardColor = CardColor.Red;
        
        ICard normalCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.One);
        
        _board.UsedCards.Add(wildCard);
        _board.UsedCards.Add(normalCard);

        _gameController.DrawCard();
        Assert.That(wildCard.CardColor, Is.EqualTo(CardColor.Black));
    }
    

}
