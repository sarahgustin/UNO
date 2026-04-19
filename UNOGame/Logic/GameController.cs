using UNOGame.Enums;
using UNOGame.Models;

namespace UNOGame.Logic;

class GameController
{
    //dictionary player dan list kartu di hand
    private readonly Dictionary<IPlayer, List<ICard>> _players;
    private bool _isClockWise = true;
    private IPlayer _currentPlayer;
    private IDeck _gameDeck;
    private IBoard _gameBoard;

   // public event EventHandler OnDeckEmpty;
   // public event EventHandler OnPlayerRunOurCard;

    //Constructor
    public GameController(List<IPlayer> players , IDeck deck, IBoard board)
    {
        _gameDeck = deck;
        _gameBoard = board;
        _players = new Dictionary<IPlayer, List<ICard>>(); //isi dictionary

        ShuffleDeck(deck);

        //pindahin list player ke dictionary biar punya slot hand
        foreach (var player in players)
        {
            _players.Add(player, new List<ICard>());
        }

        _currentPlayer = players[0];

        _isClockWise = true;
    }

    public List<IPlayer> GetPlayerList(){//untuk ambil data pemain dari list player, key pemain untuk dapat mengakses kartu 
        var PlayersKeys = _players.Keys;

        List<IPlayer> players = PlayersKeys.ToList();

        return players;

    }//mengembalikan value List<Player

    public List<ICard> GetCurrentPlayerHand() => _players[_currentPlayer];
    public IPlayer GetCurrentPlayer() => _currentPlayer;
    
    public IPlayer GetNextPlayer(){

        var players = GetPlayerList();
        int currentIndex = players.IndexOf(_currentPlayer);
        int playerCount = players.Count;
        int nextIndex;
        
        //cek clockwise dulu buat tau arah urutan pemain kalo ngga reverse + 1 kalo reverse -1
        if(_isClockWise)
        {
            nextIndex = (currentIndex + 1 ) % playerCount;
        }
        else
        {
            nextIndex = (currentIndex - 1 + playerCount) % playerCount;
        }
        //retrun next index dari currentPlayer
        return players[nextIndex];
    }

    public ICard GetTopCard (IBoard board) {
        if(board.UsedCards.Count > 0)
        {
            ICard topCard = board.UsedCards [board.UsedCards.Count - 1];
            return topCard;
        }
        return null;        
    }

    public IDeck GetDeck() =>_gameDeck;
    public IBoard GetBoard() =>_gameBoard;
    public bool GetIsClockwise() => _isClockWise;
    

    /*Card Behavior*/
    private void ReverseDirection()
    {
        _isClockWise = !_isClockWise;
    }
    private void SkipNextPlayer()
    {
        _currentPlayer = GetNextPlayer();
    }

    private void Wild(CardColor newColor)
    {
        //ambil top card yang ada di board
        ICard topCard = GetTopCard(_gameBoard);

        //ganti warna card di yang ada di topCard
        topCard.CardColor = newColor;

        if(topCard.CardType == CardType.WildDraw)
        {
            DrawPenalty(4, _gameDeck, GetNextPlayer());

            SkipNextPlayer();
        }

        _currentPlayer = GetNextPlayer();
        
    }

    /*Draw Penalty*/
    private void DrawPenalty(int amount , IDeck deck, IPlayer victim) 
    {
        for (int i = 0; i < amount; i++)
        {
            ICard penaltyCard = DrawCard(deck);
            _players[victim].Add(penaltyCard);
        }
    }
    public ICard DrawCard(IDeck deck)
    {
        if(deck.Cards.Count == 0)
        {
            RefillDeck(_gameDeck, _gameBoard);
        }
        //ambil kartu dari list card di deck yang paling atas card[0]
        ICard drawnCard = deck.Cards [0];

        //hapus kartu dari deck
        deck.Cards.RemoveAt(0);

        return drawnCard;
    }

    /*Deck*/
    private void ShuffleDeck(IDeck deck)
    {
        Random rng = new Random();
        int n = deck.Cards.Count;

        while ( n > 1)
        {
            n--;
            //pilih angka n sampai 0 secara random 
            int k = rng.Next (n+1);

            //tukar posisi card
            ICard value = deck.Cards[k];
            deck.Cards[k] = deck.Cards[n];
            deck.Cards[n] = value;
        }
    }
    private void RefillDeck(IDeck deck, IBoard board)
    {
        //ambil kartu paling atas dari used card di board biar ngga ke bawa masuk ke card di deck
        ICard TopCardAtBoard = GetTopCard(board);

        //hapus topcard dari used card
        board.UsedCards.Remove(TopCardAtBoard);

        //masukin semua sisa cards yang ada di usedcard ke deck
        deck.Cards.AddRange(board.UsedCards);

        //kosongin cards di usedcard
        board.UsedCards.Clear();

        //simpan topcard di board
        board.UsedCards.Add(TopCardAtBoard);

        ShuffleDeck(deck);
    }

    
    public void PlayerTurn(IBoard board){}
    public void PlacedCard(ICard card, IBoard board){}
    public bool IsPlaceableOnTop(ICard card, IBoard board) => true;
    public int PlayerHandCount(IPlayer player) => 0;


   /*ENDGAME*/
    private void GameEnd(IPlayer winner){}
}