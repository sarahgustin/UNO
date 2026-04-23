using UNOGame.Enums;
using UNOGame.Models;

namespace UNOGame.Logic;

public class GameController
{
    //dictionary player dan list kartu di hand
    private readonly Dictionary<IPlayer, List<ICard>> _players;
    private bool _isClockWise;
    private IPlayer _currentPlayer;
    private IDeck _gameDeck;
    private IBoard _gameBoard;
    

    /*Events and Actions */
    //event baru buat pilih warna
   public Func<CardColor> OnRequestColorSelection ;
   
   //event untuk kasih pesan kena pinalty 
   public event Action<string, int, string> OnPlayerPenalty = delegate { }; 
   
   //event untuk kasih draw message
   public event Action<ICard, bool> OnDrawFeedback = delegate { }; 
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

            //masukin kartu ke tangan, 7 kartu untuk mulai permainan
            for (int i = 0; i < 7; i++)
            {
                _players[player].Add(DrawCard(deck));
            }
        }
        _currentPlayer = players[0];
        _isClockWise = true;
        
        //simpan kartu pertama ke board
        _gameBoard.UsedCards.Add(DrawCard(deck));
        
        //condition jika kartu pertama black, player pertama pilih warna
        ICard startCard = GetTopCard(_gameBoard);

        if (startCard.CardType == CardType.Wild || startCard.CardType == CardType.WildDraw)
        {
            if (OnRequestColorSelection != null)
            {
                CardColor chosenColor = OnRequestColorSelection.Invoke();
                startCard.CardColor = chosenColor;
            }
        }
    }

    //method baru untuk tambah kartu ke list card 
    public static List<ICard> GenerateFullDeck(List<ICard> cards)
    {
        List<ICard> fullDeck = new List<ICard>();
        CardColor[] cardColor = { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };

        foreach (var color in cardColor)
        {
            //masukin kartu warna 0, 1 kartu perwarna
            fullDeck.Add(new Card(color, CardType.Zero));
            
            //masukin kartu 1-9 untuk tiap warna
            for (int i = 1; i <= 9; i++)
            {
                // Casting angka i jadi CardType biar dapet One, Two, dst
                CardType numberType = (CardType)i;
                fullDeck.Add(new Card(color, numberType));
                fullDeck.Add(new Card(color, numberType));
            }

            for (int i = 1; i<=2; i++)
            {
                fullDeck.Add(new Card(color, CardType.Skip));
                fullDeck.Add(new Card(color, CardType.Reverse));
                fullDeck.Add(new Card(color, CardType.Draw));
            }
        }

        for(int i =1; i<= 4; i++)
        {
            fullDeck.Add(new Card(CardColor.Black, CardType.Wild));
            fullDeck.Add(new Card(CardColor.Black, CardType.WildDraw));
        }

        return fullDeck;
    }
    
    //untuk ambil data pemain dari list player
    public List<IPlayer> GetPlayerList(){ 
        var playersKeys = _players.Keys;

        List<IPlayer> players = playersKeys.ToList();

        return players;
    }

    public List<ICard> GetCurrentPlayerHand() => _players[_currentPlayer];
    public IPlayer GetCurrentPlayer() => _currentPlayer;
    
    //untuk mendatkan info player selanjutnya
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
        //kalo player cuma 2 balik ke diri sendiri
        if (_players.Count == 2) {
            _currentPlayer = GetNextPlayer(); 
        } else {
            _isClockWise = !_isClockWise;
        }
    }
    private void SkipNextPlayer()
    {
        _currentPlayer = GetNextPlayer();
    }

    public void Wild(CardColor newColor)
    {
        //ambil top card yang ada di board
        ICard topCard = GetTopCard(_gameBoard);

        //ganti warna card di yang ada di topCard
        topCard.CardColor = newColor;

        if(topCard.CardType == CardType.WildDraw)
        {
            IPlayer victim = GetNextPlayer();
            int penaltyAmount = 4;
            DrawPenalty(penaltyAmount, _gameDeck, GetNextPlayer());            
            SkipNextPlayer();
            OnPlayerPenalty.Invoke(victim.Name, penaltyAmount, "Wild Draw +4");
        }
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
   
    //buat validasi ada kartu yang ada di hand ada yang valid ato ngga
    public bool CanCurrentPlayerPlay(IBoard board)
    {
        return _players[_currentPlayer].Any(c => IsPlaceableOnTop(c, board));
    }
    
    public void PlayerTurn(IBoard board, int? choice = null)
    {
        //current player
        IPlayer currentPlayer = _currentPlayer;
        //getcurrent hand
        List<ICard> playerHand = GetCurrentPlayerHand();
        
        if (choice.HasValue)
        {
            int index = choice.Value - 1;
            ICard selectedCard = _players[_currentPlayer][index];
            PlacedCard(selectedCard, board);
        }
        else
        {
           //panggil drawcard untuk ambi 1 kartu ketika kartu ditangan ngga ada yg valid
           ICard newCard = DrawCard(_gameDeck);
        
           //tambah 1 kartu ke tangan
           _players[_currentPlayer].Add(newCard);
           
           bool isMatch = IsPlaceableOnTop(newCard, board);
           
           OnDrawFeedback.Invoke(newCard, isMatch);

           // Cek apakah kartu baru ini bisa dipasang
           if (IsPlaceableOnTop(newCard, board))
           {
               PlacedCard(newCard, board);
           }
           else
           {
               _currentPlayer = GetNextPlayer();
           }
           
        }
    }
    
    public void PlacedCard(ICard selectedCard, IBoard board)
    {
        IPlayer currentPlayer = _currentPlayer;
        //hapus card dari hand player
        _players[currentPlayer].Remove(selectedCard);
         
        //add card ke board.UsedCards
        board.UsedCards.Add(selectedCard);
        
        //ambil warna dari topcard
        ICard topColor = GetTopCard(board);
        topColor.CardColor = selectedCard.CardColor;
        
        //untuk panggil method card behavior 
        if (selectedCard.CardType != CardType.WildDraw && selectedCard.CardType != CardType.Wild)
        {
            //if cardtype (skip, reverese, dan kartu)
            if (selectedCard .CardType == CardType.Reverse)
            {
                ReverseDirection();
            }else if (selectedCard.CardType == CardType.Skip)
            {
                SkipNextPlayer();
            }else if (selectedCard.CardType == CardType.Draw)
            {
                int penaltyAmount = 2;
                DrawPenalty(penaltyAmount, _gameDeck, GetNextPlayer());
                OnPlayerPenalty.Invoke(currentPlayer.Name, penaltyAmount, "Draw +2");
                SkipNextPlayer();
            }
            _currentPlayer = GetNextPlayer();
        }
            //kalo cardtype wild/wilddraw panggil UI untuk pilih color. PAKE EVENT
        else if(selectedCard.CardType == CardType.Wild || selectedCard.CardType == CardType.WildDraw)
        {
            if (OnRequestColorSelection != null)
            {
                CardColor chosenColor = OnRequestColorSelection.Invoke();
                selectedCard.CardColor = chosenColor;
                Wild(chosenColor);
            }
            _currentPlayer = GetNextPlayer();
        }
        
        //if player handcount udah abis panggil EndGame()
        if (_players[currentPlayer].Count == 0)
        {
            GameEnd(currentPlayer);
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