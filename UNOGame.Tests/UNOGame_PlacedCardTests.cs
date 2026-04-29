using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class PlacedTest
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
    public void PlacedCard_NormalCard_ShouldReduceHandAndChangeTurn()
    {
        //getcurrentplayer
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //masukin kartu ke hand
        ICard selectedCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red && card.CardType == CardType.Zero);
        //masukin kartu ke tangan
        hand.Add(selectedCard);

        //set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red);
        _board.UsedCards.Add(topCard);
        
        _gameController.PlacedCard(selectedCard);

        Assert.That(_gameController.GetTopCard(), Is.EqualTo(selectedCard));
        Assert.That(hand.Count, Is.EqualTo(0));
        Assert.That(_gameController.GetNextPlayer, Is.Not.EqualTo(currentPlayer));
          
    }

    [Test]
    public void PlacedCard_SkipCard_ShouldTheNextPlayer()
    {
        //getcurrentplayer
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        //int indexCurrentPlayer = 0;

        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard selectedCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Skip);
        //masukin kartu ke tangan
        hand.Add(selectedCard);

        //set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Skip);
        _board.UsedCards.Add(topCard);

        _gameController.PlacedCard(selectedCard);

        IPlayer nextPlayer = _gameController.GetCurrentPlayer();
        Assert.That(nextPlayer, Is.EqualTo(_players[2]));

    }
    
   [Test]
    public void PlacedCard_ReverseCard_ShouldChangeDirection()
    {
        //getcurrentplayer
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();

        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard selectedCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Reverse);
        //masukin kartu ke tangan
        hand.Add(selectedCard);

        //set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Reverse);
        _board.UsedCards.Add(topCard);

        _gameController.PlacedCard(selectedCard);

        Assert.That(_gameController.IsClockWise, Is.False);   
    }
    [Test]
    public void PlacedCard_ReverseCardTwoPlayer_ShouldSkipPlayer()
    {
        List<IPlayer> twoPlayers = new List<IPlayer>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        _gameController = new GameController(twoPlayers, _deck, _board);


        IPlayer currentPlayer = _gameController.GetCurrentPlayer();
        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard selectedCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Reverse);
        //masukin kartu ke tangan
        hand.Add(selectedCard);

        //set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Reverse);
        _board.UsedCards.Add(topCard);

        _gameController.PlacedCard(selectedCard);

        Assert.That(_gameController.GetCurrentPlayer().Name, Is.EqualTo("Player 1"));
    }

    [Test]
    public void PlacedCard_DrawTwoCards_ShouldAddTwoCardandSkipNextPlayer()
    {
        //getcurrentplayer
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();

        //player victim, karena dimulai dr playe[0] jadi victim player[1]
        IPlayer victim = _players[1];
        int victimCardsCountBeforeDraw = _gameController.PlayerHandCount(victim);

        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard selectedCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Draw && card.CardColor == CardColor.Red);
        //masukin kartu ke tangan
        hand.Add(selectedCard);

        //set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red);
        _board.UsedCards.Add(topCard);

        _gameController.PlacedCard(selectedCard);

        int victimCardsCountAfterDraw = _gameController.PlayerHandCount(victim);
        Assert.That(victimCardsCountAfterDraw, Is.EqualTo(victimCardsCountBeforeDraw +2));
        Assert.That(_gameController.GetCurrentPlayer(), Is.EqualTo(_players[2]));
       
    }
    [Test]
    public void PlacedCard_Wild_ShouldChangeTopCardColor()
    {
        //getcurrentplayer
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();

        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard selectedCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.Wild);
        //masukin kartu ke tangan
        hand.Add(selectedCard);

        //set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red);
        _board.UsedCards.Add(topCard);

        //pilihan warna
        CardColor chosenColor = CardColor.Green;
        _gameController.OnRequestColorSelection = () =>chosenColor;

        _gameController.PlacedCard(selectedCard);

        Assert.That(_gameController.GetTopCard().CardColor, Is.EqualTo(chosenColor));
       
    }
    [Test]
    public void PlacedCard_WildDraw_ShouldChangeTopCardColorAddFourCardsAndSkipNextPlayer()
    {
        //getcurrentplayer
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();

        //player victim, karena dimulai dr playe[0] jadi victim player[1]
        IPlayer victim = _players[1];
        int victimCardsCountBeforeDraw = _gameController.PlayerHandCount(victim);

        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        ICard selectedCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardType == CardType.WildDraw);
        //masukin kartu ke tangan
        hand.Add(selectedCard);

        //set top card
        _board.UsedCards.Clear();
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Red);
        _board.UsedCards.Add(topCard);

        //pilihan warna
        CardColor chosenColor = CardColor.Green;
        _gameController.OnRequestColorSelection = () => chosenColor;

        _gameController.PlacedCard(selectedCard);

        int victimCardsCountAfterDraw = _gameController.PlayerHandCount(victim);
        Assert.That(victimCardsCountAfterDraw, Is.EqualTo(victimCardsCountBeforeDraw +4));
        Assert.That(_gameController.GetCurrentPlayer(), Is.EqualTo(_players[2]));
        Assert.That(_gameController.GetTopCard().CardColor, Is.EqualTo(chosenColor));
       
    } 

}
