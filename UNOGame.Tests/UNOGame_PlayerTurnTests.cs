using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class PlayerTurnTest
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
    public void PlayerTurn_WithValidChoice_ShouldPlaceCard()
    {
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard playerCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Yellow && card.CardType == CardType.Zero);
        //masukin kartu ke tangan
        hand.Add(playerCard);

        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        _board.UsedCards.Add(topCard);

        _gameController.PlayerTurn(1);
        
        Assert.That(playerCard, Is.EqualTo(_gameController.GetTopCard()));
        
    }
    [Test]
    public void PlayerTurn_NoChoice_DrawMatch_ShouldPlaceAutomatically()
    {
        //ambil player hand
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        List<ICard> hand = _gameController.GetCurrentPlayerHand();

        //hapus kartu di hand
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard playerCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Green && card.CardType == CardType.One);
        //masukin kartu ke tangan
        hand.Add(playerCard);


        // set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        _board.UsedCards.Add(topCard);


        //draw card nya di set sesuai sama top card
        _deck.Cards.Clear();
        ICard drawCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Yellow && card.CardType == CardType.Zero);
        _deck.Cards.Insert(0, drawCard);

        //panggil player turn
        _gameController.PlayerTurn(null);
    
        Assert.That(_gameController.GetTopCard(), Is.EqualTo(drawCard));    
    }
    
    [Test]
    public void PlayerTurn_NoChoice_DrawNoMatch_ShouldSwitchTurn()
    {
        
        //ambil player hand
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        List<ICard> hand = _gameController.GetCurrentPlayerHand();

        //hapus kartu di hand
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard playerCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Green && card.CardType == CardType.One);
        //masukin kartu ke tangan
        hand.Add(playerCard);

        // set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        _board.UsedCards.Add(topCard);


        //draw card nya di set sesuai sama top card
        _deck.Cards.Clear();
        ICard drawCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Yellow && card.CardType == CardType.One);
        _deck.Cards.Insert(0, drawCard);

        //panggil player turn
        _gameController.PlayerTurn(null);
        IPlayer nextPlayer = _gameController.GetCurrentPlayer();
        
        Assert.That(hand.Count, Is.EqualTo(2));
        Assert.That(nextPlayer, Is.Not.EqualTo(currentPlayer));
    }

      
    
    

}
