using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class GameControllerTest
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
    public void GameStart_ShouldGiveSevenCardsToEachPlayer()
    {

        List<ICard> player1Hand = _gameController.GetCurrentPlayerHand();

        Assert.That(player1Hand.Count, Is.EqualTo(7), "Harusnya player dapet 7 kartu pas awal game");
    }
    //kartu udah di shuffle
    [Test]
    public void GameStart_DeckShouldBeShuffled()
    {
        //ambil dulu list deck nya
        List<ICard> cards = TestDataHelper.GenerateCardsForTest();
        List<ICard> shuffledCards = _gameController.GetDeck.Cards;

        //cek kartu di list card sama di deck lewat indexnya sama atau ngga
        bool isDiffrent = false;
        for (int i = 0; i<cards.Count; i++)
        {
            if (shuffledCards[i] != cards[i])
            {
                isDiffrent = true;
                break;
            }
        }
        Assert.That(isDiffrent, Is.True, "Kartu di deck harusnya sudah di acak!");
  
    }
    //top card bukan action card
    [Test]
    public void startGame_TopCardShouldNotActionCard()
    {
        //ambil topcard nya di awal game
        ICard topCard = _gameController.GetTopCard();
        bool isDiffrent = false;
        //cek kalo top card bukan action card
        if (topCard.CardType != CardType.Draw && topCard.CardType != CardType.Wild &&
            topCard.CardType != CardType.WildDraw && topCard.CardType != CardType.Skip &&
            topCard.CardType != CardType.Reverse)
        {
            isDiffrent = true;
        }
        Assert.That(isDiffrent, Is.True, "Kartu pertama tidak boleh action card!");
    }

    //current player harus pemain pertama 
    [Test]
    public void GetCurrentPlayer_ShouldReturnFirstPlayr_WhenInitialize()
    {
        List <IPlayer> players = _gameController.GetPlayerList();

        IPlayer currentPlayer = _gameController.GetCurrentPlayer();

        Assert.That(currentPlayer, Is.EqualTo(players[0]), "Ketika memulai game, current player harus di index[0]");
    }

  

   




}
