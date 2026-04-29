using UNOGame.Enums;
using UNOGame.Models;
using Serilog;
using Serilog.Events;

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
        
        Log.Information("Game UNO dimulai dengan {PlayerCount} pemain. Arah awal: {Direction}", players.Count, IsClockWise ? "Clockwise" : "Counter-Clockwise");
        
        ShuffleDeck();

        foreach (var player in players)
        {
            _players.Add(player, new List<ICard>());

            for (int i = 0; i < 7; i++)
            {
                _players[player].Add(DrawCard());
                Log.Debug("Pemain terdaftar : {PlayerName}, Jumlah kartu : {CardCount}", player.Name, _players[player].Count);
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
            OnPlayerPenalty?.Invoke(victim.Name, penaltyAmount, "Wild Draw +4");
            Log.Error("Penalty Alert! {PlayerName} terkena pinalti {PenaltyType} sebanyak {DrawAmount} kartu", victim.Name, CardType.WildDraw, penaltyAmount);
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
            Log.Warning("Kartu di Deck habis. Refill kartu dari tumpukan kartu yang sudah terpakai!");
            OnDeckEmpty?.Invoke();
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
        Log.Information("Giliran pemain: {PlayerName}. Sisa kartu: {HandCount}", _currentPlayer.Name, _players[_currentPlayer].Count);
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
           Log.Information("{PlayerName} mendaptkan kartu draw : {newCardColor}{newCardType}",_currentPlayer.Name, newCard.CardColor, newCard.CardType);
           
           bool isMatch = IsPlaceableOnTop(newCard);
           
           OnDrawFeedback?.Invoke(newCard, isMatch);

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
        Log.Information("{PlayerName} memainkan kartu {CardColor} {CardType}", _currentPlayer.Name, selectedCard.CardColor, selectedCard.CardType);
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

                Log.Warning("Action Card:  {SelectedCardColor} {SelectedCardType}. Giliran main berubah.", selectedCard.CardColor, selectedCard.CardType);
            }else if (selectedCard.CardType == CardType.Skip)
            {
                SkipNextPlayer();

                Log.Warning("Action Card:  {SelectedCardColor} {SelectedCardType}. Pemain selanjutnya di-skip.", selectedCard.CardColor, selectedCard.CardType);
            
            }else if (selectedCard.CardType == CardType.Draw)
            {   
                Log.Warning("Action Card: {SelectedCardColor} {SelectedCardType}. {NextPlayer} akan mendapatkan 2 kartu draw dan giliran akan dilewati.", selectedCard.CardColor, selectedCard.CardType GetNextPlayer().Name);
                int penaltyAmount = 2;
                DrawPenalty(penaltyAmount, GetNextPlayer());
                OnPlayerPenalty?.Invoke(GetNextPlayer().Name, penaltyAmount, "Draw +2");
                SkipNextPlayer();

                Log.Error("Penalty : {PlayerName} terkena pinalti {PenaltyType} sebanyak {DrawAmount} kartu", GetNextPlayer().Name, selectedCard.CardType, penaltyAmount);
                
            }
            _currentPlayer = GetNextPlayer();
        }

        else if(selectedCard.CardType == CardType.Wild || selectedCard.CardType == CardType.WildDraw)
        { 
            Log.Warning("Action Card: {SelectedCardColor} {SelectedCardType}. {CurrentPlayer} memilih warna kartu.", selectedCard.CardColor, selectedCard.CardType, _currentPlayer.Name);
            
            CardColor chosenColor = OnRequestColorSelection.Invoke();
            selectedCard.CardColor = chosenColor;

            Log.Information("Pemain {PlayerName} mengubah warna menjadi {NewColor}", _currentPlayer.Name, chosenColor);        

            HandleWildCard(chosenColor);
            _currentPlayer = GetNextPlayer();
        }
        
        if (_players[currentPlayer].Count == 0)
        {
            GameEnd(currentPlayer);
            Log.Information("PERMAINAN SELESAI Pemenangnya adalah {WinnerName}", currentPlayer.Name);

        }
    }
    
    private bool IsPlaceableOnTop(ICard card) 
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
       OnPlayerRunOutCard?.Invoke(winner);
   }
}