using NUnit.Framework;
using UNOGame.Logic;
using UNOGame.Models;

namespace UNOGame.Tests;

[TestFixture]
public class PlayerHandCountTest
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
