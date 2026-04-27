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
   public Func<CardColor> OnRequestColorSelection;
   
   //event untuk kasih pesan kena pinalty 
   public event Action<string, int, string> OnPlayerPenalty;
   
   //event untuk kasih draw message
   public event Action<ICard, bool> OnDrawFeedback;
   
   //event untuk ketika kartu di deck habis 
   public event Action OnDeckEmpty;
   
   //event untuk game selesai
   public event Action<IPlayer> OnPlayerRunOutCard;
   
    //Constructor
    public GameController(List<IPlayer> players , IDeck deck, IBoard board)
    {
        //initialize property
        _gameDeck = deck;
        _gameBoard = board;
        _players = new Dictionary<IPlayer, List<ICard>>();
        
        //kocok kartu untuk awal permainan
        ShuffleDeck();

        //pindahin list player ke dictionary biar punya slot hand
        foreach (var player in players)
        {
            _players.Add(player, new List<ICard>());

            //masukin kartu ke tangan, 7 kartu untuk mulai permainan
            for (int i = 0; i < 7; i++)
            {
                _players[player].Add(DrawCard());
            }
        }
        
        _currentPlayer = players[0];
        _isClockWise = true;
        
        //simpan kartu pertama ke board
        _gameBoard.UsedCards.Add(DrawCard());

        //revisi : kalo kartunya draw, skip, reverse, wild kartunya di kocok ulang. 
        ICard startCard = GetTopCard();
        while (startCard.CardType == CardType.Wild || startCard.CardType == CardType.WildDraw ||
            startCard.CardType == CardType.Draw || startCard.CardType == CardType.Reverse || 
            startCard.CardType == CardType.Skip)
        {
            //hapus card dari top card 
            _gameBoard.UsedCards.Remove(startCard);

            //masukin lagi ke deck
            deck.Cards.Add(startCard);
            //shuffle lagi
            ShuffleDeck();

            //simpan lagi di board
            startCard = DrawCard();
            _gameBoard.UsedCards.Add(startCard);
        }
    }
    
    //untuk data pemain dari list player
    public List<IPlayer> GetPlayerList(){ 
        Dictionary<IPlayer, List<ICard>>.KeyCollection playersKeys = _players.Keys;
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

    public bool GetIsClockwise() => _isClockWise;
    
    /*Card Behavior*/
    private bool ReverseDirection() //RETURN 
    {
        //kalo player cuma 2 balik ke diri sendiri
        if (_players.Count == 2) {
            _currentPlayer = GetNextPlayer(); 
        } else {
            _isClockWise = !_isClockWise;
        }

        return _isClockWise;
    }
    private void SkipNextPlayer()
    {
        _currentPlayer = GetNextPlayer();
    }

    private void HandleWildCard(CardColor newColor) //Revisi : ganti nama biar lebih jelas
    {
        //ambil top card yang ada di board
        ICard topCard = GetTopCard();

        //ganti warna card di yang ada di topCard
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
    /*Draw Penalty*/
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
        //ambil kartu dari list card di deck yang paling atas card[0]
        ICard drawnCard = _gameDeck.Cards [0];

        //hapus kartu dari deck
        _gameDeck.Cards.RemoveAt(0);

        return drawnCard;
    }
    
    /*Deck*/
    private void ShuffleDeck() //penamaan benerin 
    {
        Random rng = new Random();
        int currentIndex = _gameDeck.Cards.Count;

        while ( currentIndex > 1)
        {
            currentIndex--;
            //pilih angka n sampai 0 secara random 
            int randomIndex = rng.Next (currentIndex+1);

            //tukar posisi card
            (_gameDeck.Cards[randomIndex], _gameDeck.Cards[currentIndex]) = (_gameDeck.Cards[currentIndex], _gameDeck.Cards[randomIndex]);
        }
    }
    //Revisi : board sama deck ngga perlu jadi parameter
    private void RefillDeck()
    {
        //ambil kartu paling atas dari used card di board biar ngga ke bawa masuk ke card di deck
         ICard topCardAtBoard = GetTopCard();

        //hapus topcard dari used card
        _gameBoard.UsedCards.Remove(topCardAtBoard);
        
        // reset warna wildcard
        foreach (var card in  _gameBoard.UsedCards)
        {
            if (card.CardType == CardType.Wild || card.CardType == CardType.WildDraw)
            {
                // Balikin ke warna hitam
                card.CardColor = CardColor.Black; 
            }
        }
        
        //masukin semua sisa cards yang ada di usedcard ke deck
        _gameDeck.Cards.AddRange(_gameBoard.UsedCards);

        //kosongin cards di usedcard
        _gameBoard.UsedCards.Clear();

        //simpan topcard di board
        _gameBoard.UsedCards.Add(topCardAtBoard);

        ShuffleDeck();
    }
   
    //buat validasi ada kartu yang ada di hand ada yang valid ato ngga
    public bool CanCurrentPlayerPlay() //revisi akses board bisa dar private board
    {
        return _players[_currentPlayer].Any(card => IsPlaceableOnTop(card));
    }
    
    public void PlayerTurn(int? choice = null) //revisi : board parameter bisa dari private board
    {
        if (choice.HasValue)
        {
            //ambil kartu pilihan player 
            int index = choice.Value - 1;
            ICard selectedCard = _players[_currentPlayer][index];
            PlacedCard(selectedCard);
        }
        else
        {
           //panggil drawcard untuk ambi 1 kartu ketika kartu ditangan ngga ada yg valid
           ICard newCard = DrawCard();
        
           //tambah 1 kartu ke tangan
           _players[_currentPlayer].Add(newCard);
           
           bool isMatch = IsPlaceableOnTop(newCard);
           
           OnDrawFeedback.Invoke(newCard, isMatch);

           // Cek apakah kartu baru ini bisa dipasang
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
    
    public void PlacedCard(ICard selectedCard) //revisi : board lewat private
    {
        IPlayer currentPlayer = _currentPlayer;
        //hapus card dari hand player
        _players[currentPlayer].Remove(selectedCard);
         
        //add card ke board.UsedCards
        _gameBoard.UsedCards.Add(selectedCard);
        
        //ambil warna dari topcard
        ICard topColor = GetTopCard();
        topColor.CardColor = selectedCard.CardColor;
        
        //untuk panggil method 1card behavior 
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
                DrawPenalty(penaltyAmount, GetNextPlayer());
                OnPlayerPenalty.Invoke(GetNextPlayer().Name, penaltyAmount, "Draw +2");
                SkipNextPlayer();
            }
            _currentPlayer = GetNextPlayer();
        }
        //kalo cardtype wild/wilddraw panggil UI untuk pilih color. PAKE EVENT
        else if(selectedCard.CardType == CardType.Wild || selectedCard.CardType == CardType.WildDraw)
        { 
            CardColor chosenColor = OnRequestColorSelection.Invoke();
            selectedCard.CardColor = chosenColor;
            HandleWildCard(chosenColor);
            _currentPlayer = GetNextPlayer();
        }
        
        //if player handcount udah abis panggil EndGame()
        if (_players[currentPlayer].Count == 0)
        {
            GameEnd(currentPlayer);
        }
    }
    
    public bool IsPlaceableOnTop(ICard card) //board bisa lewat private
    {
        //ambil topcard nya
        ICard topCard = GetTopCard();
        
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
       OnPlayerRunOutCard.Invoke(winner);
   }
}