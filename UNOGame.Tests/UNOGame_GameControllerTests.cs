using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

[TestFixture]
public class GameControllerTests
{
    private GameController _gameController;
    private IBoard _board;
    private IDeck _deck;
    private List<IPlayer> _players;

    [SetUp]
    public void Setup()
    {
        List<ICard> cards =TestDataHelper.GenerateCardsForTest();
        _players = new List<IPlayer> { new Player("A"), new Player("B"), new Player ("C"), new Player ("D") };
        _deck = new Deck(cards);
        _board = new Board();
        
        _gameController = new GameController(_players,_deck, _board); 
    }

    //test player dapet 7 kartu di awal
    [Test]
    public void GameController_ShouldGiveSevenCardsToEachPlayer()
    {
        List<ICard> player1Hand = _gameController.GetCurrentPlayerHand();

        Assert.That(player1Hand.Count, Is.EqualTo(7));
    }
    //kartu udah di shuffle
    [Test]
    public void GameController_Cards_DeckShouldBeShuffled()
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
        Assert.That(isDiffrent, Is.True);
  
    }
    //top card bukan action card
    [Test]
    public void GameController_TopCardShouldNotActionCard()
    {
        //ambil topcard nya di awal game
        ICard topCard = _gameController.GetTopCard();
        bool isDiffrent = false;
        if (topCard.CardType != CardType.Draw && topCard.CardType != CardType.Wild &&
            topCard.CardType != CardType.WildDraw && topCard.CardType != CardType.Skip &&
            topCard.CardType != CardType.Reverse)
        {
            isDiffrent = true;
        }
        Assert.That(isDiffrent, Is.True);
    }
    //current player harus pemain pertama 
    [Test]
    public void GetCurrentPlayer_ShouldReturnFirstPlayr_WhenInitialize()
    {
        List <IPlayer> players = _gameController.GetPlayerList();

        IPlayer currentPlayer = _gameController.GetCurrentPlayer();

        Assert.That(currentPlayer, Is.EqualTo(players[0]));
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
    //invalid test
    [Test]
    public void PlacedCard_InvalidCard_ShouldNotBeAllow()
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
        ICard topCard = TestDataHelper.GenerateCardsForTest().First(card => card.CardColor == CardColor.Yellow && card.CardType == CardType.Four);
        _board.UsedCards.Add(topCard);

        bool canPlay = _gameController.CanCurrentPlayerPlay();

        Assert.That(canPlay, Is. False);
        Assert.That(_gameController.GetTopCard, Is.EqualTo(topCard));
        Assert.That(hand.Count, Is.EqualTo(1));

    }
    [Test]
    public void GetNextPlayer_NormalRotaion_ShouldIncrementIndex()
    {
        //ambil dulu list player 
        List<IPlayer> players = _gameController.GetPlayerList();
        //current player
        IPlayer expectedNextPlayer = players[1];  
        //getnextplayer
        IPlayer NextPlayer = _gameController.GetNextPlayer();
        Assert.That(NextPlayer, Is.EqualTo(expectedNextPlayer));
    }
    //cek reverse rotation
    [Test]
    public void GetNextPlayer_ReverseRotaion_ShouldecrementIndex()
    {
        //clock wise is false
        _gameController.IsClockWise = false;
        //list player
        List<IPlayer> players = _gameController.GetPlayerList();
        //expected index 
        IPlayer expectedNextPlayer = players[3];
        //getnext player
        IPlayer nextPlayer = _gameController.GetNextPlayer();

        Assert.That(nextPlayer, Is.EqualTo(expectedNextPlayer));
    }
    [Test]
    public void GetTopCard_WhenBoardIsEmpty_ShouldReturnNull()
    {
        _board.UsedCards.Clear();
        //board kosong bikin pake mock
        ICard topCard = _gameController.GetTopCard();
        Assert.That(topCard, Is.Null);
    }
    [Test]
    public void GetTopCard_WhenBoardHasCard_ShouldReturnThatCard()
    {
        ICard topCard = _gameController.GetTopCard();
        Assert.That(topCard, Is.Not.Null);
    }

    [Test]
    public void CanCurrentPlayerPlay_ReturnTrue()
    {
        //getplayerhand
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

        Assert.That(_deck.Cards.Count, Is.EqualTo(cardCount - 1));
      
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

    [Test]
    public void PlayerHandCount_WhenPlayerHasThreeCards_ReturnsThree()
    {
       //getcurrentplayer
        IPlayer currentPlayer = _gameController.GetCurrentPlayer();

        //gethandplayer
        List<ICard> hand = _gameController.GetCurrentPlayerHand();
        hand.Clear();

        //tambahin kartu yang mau di mainkan
        List<ICard>  card = TestDataHelper.GenerateCardsForTest().Take(3).ToList();
        //masukin kartu ke tangan
        hand.AddRange(card);
        int count = _gameController.PlayerHandCount(currentPlayer);

        Assert.That(count, Is.EqualTo(3));
    }
  
}
