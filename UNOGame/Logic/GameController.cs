using UNOGame.Enums;
using UNOGame.Models;

namespace UNOGame.Logic;

public class GameController
{
    private readonly Dictionary<IPlayer, List<ICard>> _players;
    public bool IsClockWise { get; set; } = true;
    private IPlayer _currentPlayer;
    private IDeck _gameDeck;
    private IBoard _gameBoard;
    
    
    public Func<CardColor> OnRequestColorSelection;
    public event Action<string, int, string> OnPlayerPenalty;
    public event Action<ICard, bool> OnDrawFeedback;
    public event Action OnDeckEmpty;
    public event Action<IPlayer> OnPlayerRunOutCard;
    public GameController(List<IPlayer> players , IDeck deck, IBoard board)
    {
        _gameDeck = deck;
        _gameBoard = board;
        _players = new Dictionary<IPlayer, List<ICard>>();

        ShuffleDeck();

        foreach (var player in players)
        {
            _players.Add(player, new List<ICard>());

            for (int i = 0; i < 7; i++)
            {
                _players[player].Add(DrawCard());
            }
        }
        
        _currentPlayer = players[0];
        IsClockWise = true;

        ICard startCard = DrawCard();
        while (startCard.CardType == CardType.Wild || startCard.CardType == CardType.WildDraw ||
            startCard.CardType == CardType.Draw || startCard.CardType == CardType.Reverse ||
            startCard.CardType == CardType.Skip)
        {
            deck.Cards.Add(startCard);
            ShuffleDeck();
            startCard = DrawCard();
            
        }
        _gameBoard.UsedCards.Add(startCard);
    }

    public IDeck GetDeck => _gameDeck;
    public List<IPlayer> GetPlayerList(){ 
        Dictionary<IPlayer, List<ICard>>.KeyCollection playersKeys = _players.Keys;
        List<IPlayer> players = playersKeys.ToList();
        return players;
    }
    public List<ICard> GetCurrentPlayerHand() => _players[_currentPlayer];
    public IPlayer GetCurrentPlayer() => _currentPlayer;
    public IPlayer GetNextPlayer(){
        List<IPlayer> players = GetPlayerList();
        int currentIndex = players.FindIndex(player => player == _currentPlayer);
        int playerCount = players.Count;
        int nextIndex;

        if(IsClockWise)
        {
            nextIndex = (currentIndex + 1 ) % playerCount;
        }
        else
        {
            nextIndex = (currentIndex - 1 + playerCount) % playerCount;
        }
        return players[nextIndex];
    }
    public ICard GetTopCard () {
        if(_gameBoard.UsedCards.Count > 0)
        {
            ICard topCard = _gameBoard.UsedCards [_gameBoard.UsedCards.Count - 1];
            return topCard;
        }
        return null;        
    }

    public bool GetIsClockwise() => IsClockWise;
    private void ReverseDirection()
    {
        if (_players.Count == 2) {
            _currentPlayer = GetNextPlayer(); 
        } else {
            IsClockWise = !IsClockWise;
        }
    }
    private void SkipNextPlayer()
    {
        _currentPlayer = GetNextPlayer();
    }

    private void HandleWildCard(CardColor newColor) 
    {
        ICard topCard = GetTopCard();
        topCard.CardColor = newColor;

        if(topCard.CardType == CardType.WildDraw)
        {
            IPlayer victim = GetNextPlayer();
            int penaltyAmount = 4;
            DrawPenalty(penaltyAmount, GetNextPlayer());            
            SkipNextPlayer();
            OnPlayerPenalty.Invoke(victim.Name, penaltyAmount, "Wild Draw +4");
        }
    }
    private void DrawPenalty(int amount ,  IPlayer victim) 
    {
        for (int i = 0; i < amount; i++)
        {
            ICard penaltyCard = DrawCard();
            _players[victim].Add(penaltyCard);
        }
    }
    public ICard DrawCard()
    {
        if(_gameDeck.Cards.Count == 0)
        {
            OnDeckEmpty.Invoke();
            RefillDeck(); 
        }
        ICard drawnCard = _gameDeck.Cards [0];
        _gameDeck.Cards.RemoveAt(0);

        return drawnCard;
    }
    
    private void ShuffleDeck()
    {
        Random randonNumberGenerate = new Random();
        int currentIndex = _gameDeck.Cards.Count;

        while ( currentIndex > 1)
        {
            currentIndex--;
            int randomIndex = randonNumberGenerate.Next (currentIndex+1);
            (_gameDeck.Cards[randomIndex], _gameDeck.Cards[currentIndex]) = (_gameDeck.Cards[currentIndex], _gameDeck.Cards[randomIndex]);
        }
    }
    private void RefillDeck()
    {
        ICard topCardAtBoard = GetTopCard();
        _gameBoard.UsedCards.Remove(topCardAtBoard);
 
        foreach (var card in  _gameBoard.UsedCards)
        {
            if (card.CardType == CardType.Wild || card.CardType == CardType.WildDraw)
            {
                card.CardColor = CardColor.Black; 
            }
        }

        _gameDeck.Cards.AddRange(_gameBoard.UsedCards);
        _gameBoard.UsedCards.Clear();
        _gameBoard.UsedCards.Add(topCardAtBoard);

        ShuffleDeck();
    }
   
    public bool CanCurrentPlayerPlay()
    {
        return _players[_currentPlayer].Any(card => IsPlaceableOnTop(card));
    }
    
    public void PlayerTurn(int? choice = null)
    {
        if (choice.HasValue)
        {
            int index = choice.Value - 1;
            ICard selectedCard = _players[_currentPlayer][index];
            PlacedCard(selectedCard);
        }
        else
        {
           ICard newCard = DrawCard();
           _players[_currentPlayer].Add(newCard);
           
           bool isMatch = IsPlaceableOnTop(newCard);
           
           OnDrawFeedback.Invoke(newCard, isMatch);
           if (IsPlaceableOnTop(newCard))
           {
               PlacedCard(newCard);
           }
           else
           {
               _currentPlayer = GetNextPlayer();
           }
           
        }
    }
    
    public void PlacedCard(ICard selectedCard)
    {
        IPlayer currentPlayer = _currentPlayer;
        _players[currentPlayer].Remove(selectedCard);
         
        _gameBoard.UsedCards.Add(selectedCard);
        
        ICard topColor = GetTopCard();
        topColor.CardColor = selectedCard.CardColor;

        if (selectedCard.CardType != CardType.WildDraw && selectedCard.CardType != CardType.Wild)
        {
            if (selectedCard .CardType == CardType.Reverse)
            {
                ReverseDirection();
            }else if (selectedCard.CardType == CardType.Skip)
            {
                SkipNextPlayer();
            }else if (selectedCard.CardType == CardType.Draw)
            {
                int penaltyAmount = 2;
                DrawPenalty(penaltyAmount, GetNextPlayer());
                OnPlayerPenalty.Invoke(GetNextPlayer().Name, penaltyAmount, "Draw +2");
                SkipNextPlayer();
            }
            _currentPlayer = GetNextPlayer();
        }

        else if(selectedCard.CardType == CardType.Wild || selectedCard.CardType == CardType.WildDraw)
        { 
            CardColor chosenColor = OnRequestColorSelection.Invoke();
            selectedCard.CardColor = chosenColor;
            HandleWildCard(chosenColor);
            _currentPlayer = GetNextPlayer();
        }
        
        if (_players[currentPlayer].Count == 0)
        {
            GameEnd(currentPlayer);
        }
    }
    
    public bool IsPlaceableOnTop(ICard card) 
    {
        ICard topCard = GetTopCard();
        
        return card.CardColor == topCard.CardColor || 
               card.CardType == topCard.CardType ||
               card.CardType == CardType.Wild ||
               card.CardType == CardType.WildDraw;
    }
    public int PlayerHandCount(IPlayer player)
    { 
        return _players[player].Count;
    }
   
   private void GameEnd(IPlayer winner)
   {
       OnPlayerRunOutCard.Invoke(winner);
   }
}