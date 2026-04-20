using System;
using UNOGame.Enums;
using UNOGame.Models;

namespace UNOGame.Logic;

class GameController
{
    //dictionary player dan list kartu di hand
    private readonly Dictionary<IPlayer, List<ICard>> _players;
    private bool _isClockWise;
    private IPlayer _currentPlayer;
    private IDeck _gameDeck;
    private IBoard _gameBoard;

   public event Action OnDeckEmpty = delegate { }; 

   public delegate void GameEndHandler(IPlayer winner);
   public event GameEndHandler OnPlayerRunOutCard = delegate { };
   
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
        var playersKeys = _players.Keys;

        List<IPlayer> players = playersKeys.ToList();

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
            OnDeckEmpty?.Invoke();
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
            /*ICard value = deck.Cards[k];
            deck.Cards[k] = deck.Cards[n];
            deck.Cards[n] = value;*/
            (deck.Cards[k], deck.Cards[n]) = (deck.Cards[n], deck.Cards[k]);
        }
    }
    private void RefillDeck(IDeck deck, IBoard board)
    {
        //ambil kartu paling atas dari used card di board biar ngga ke bawa masuk ke card di deck
        ICard topCardAtBoard = GetTopCard(board);

        //hapus topcard dari used card
        board.UsedCards.Remove(topCardAtBoard);
        
        // reset warna wildcard
        foreach (var card in board.UsedCards)
        {
            if (card.CardType == CardType.Wild || card.CardType == CardType.WildDraw)
            {
                // Balikin ke warna hitam
                card.CardColor = CardColor.Black; 
            }
        }
        
        //masukin semua sisa cards yang ada di usedcard ke deck
        deck.Cards.AddRange(board.UsedCards);

        //kosongin cards di usedcard
        board.UsedCards.Clear();

        //simpan topcard di board
        board.UsedCards.Add(topCardAtBoard);

        ShuffleDeck(deck);
    }
    
    public void PlayerTurn(IBoard board)
    {
        //current player
        IPlayer currentPlayer = _currentPlayer;
        //getcurrent hand
        List<ICard> playerHand = _players[currentPlayer];
        GetTopCard(board);
        
        //cek di tangan ada kartu yang valid atau ngga
        bool canPlay = false;
        ICard selectedCard = null;

        foreach (var card in playerHand)
        {
            if (IsPlaceableOnTop(card, board))
            {
                canPlay = true;
                selectedCard = card;
            }
        }

        if (canPlay)
        {
            PlacedCard(selectedCard, board);
        }
        else
        {
            //panggil drawcard untuk ambi 1 kartu ketika kartu ditangan ngga ada yg valid
            ICard newCard = DrawCard(_gameDeck);
            //tambah 1 kartu ke tangan
            playerHand.Add(newCard);
            
            //cek lagi kartu yang udah di ambil di cek lagi valid ngga untuk PlaceCard
            if (IsPlaceableOnTop(newCard, board))
            {
                PlacedCard(newCard, board);
            }
        }
        _currentPlayer = GetNextPlayer();
    }

    public void PlacedCard(ICard card, IBoard board)
    {
        IPlayer currentPlayer = _currentPlayer;
        //hapus card dari hand player
        _players[currentPlayer].Remove(card);
         
        //add card ke board.UsedCards
        board.UsedCards.Add(card);
        
        //ambil warna dari topcard
        ICard topColor = GetTopCard(board);
        topColor.CardColor = card.CardColor;

        //untuk panggil method card behavior 
        if (card.CardType != CardType.WildDraw && card.CardType != CardType.WildDraw)
        {
            //if cardtype (skip, reverese, dan kartu)
            if (card.CardType == CardType.Reverse)
            {
                ReverseDirection();
            }else if (card.CardType == CardType.Skip)
            {
                SkipNextPlayer();
            }else if (card.CardType == CardType.Draw)
            {
                DrawPenalty(2, _gameDeck, GetNextPlayer());
                SkipNextPlayer();
            }
            _currentPlayer = GetNextPlayer();
        }
        //kalo cardtype wild/wilddraw panggil UI untuk pilih color
        else
        {
            //UI pilih warna

        }
        
        //if player handcount udah abis panggil EndGame()
        if (_players[_currentPlayer].Count == 0)
        {
            GameEnd(_currentPlayer);
            return;
           
        }
    }
    public bool IsPlaceableOnTop(ICard card, IBoard board)
    {
        //ambil topcard nya
        ICard topCard = GetTopCard(board);
        
        //Return cardtype nya sama atau ngga 
        return card.CardColor == topCard.CardColor || 
               card.CardType == topCard.CardType ||
               card.CardType == CardType.Wild ||
               card.CardType == CardType.WildDraw;
    }
    public int PlayerHandCount(IPlayer player)
    { 
        return _players[player].Count;
    }
    
   /*ENDGAME*/
   private void GameEnd(IPlayer winner)
   {
       OnPlayerRunOutCard?.Invoke(winner);
   }
}