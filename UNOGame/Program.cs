
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.UI;


    IBoard board = new Board();
    List<ICard> allCard = GameController.GenerateFullDeck(new List<ICard>());
    IDeck deck = new Deck(allCard);

   
    int menuChoice =  ConsoleDisplay.ShowMenu();
    if (menuChoice == 1)
    {
        List<IPlayer> players = ConsoleDisplay.InitPlayer();
    
        GameController gc =  new GameController(players, deck, board);

        bool isGameOver = false;
        
        //subscribe event buat kalo player menang dan game selesai
        gc.OnPlayerRunOutCard += (winner) =>
        {
            ConsoleDisplay.ShowWinnerAnnouncement(winner);
            isGameOver = true;
        };
        
        /*subscribe event */
        gc.OnRequestColorSelection = ConsoleDisplay.ShowColorPick;
        gc.OnDeckEmpty += ConsoleDisplay.ShowDeckEmptyMessage;
        gc.OnDrawFeedback += ConsoleDisplay.ShowDrawResult;
        gc.OnPlayerPenalty += ConsoleDisplay.ShowPenaltyMessage;


        //loop game 
        while (!isGameOver)
        {
            Console.Clear();

            //Tampil header, buat info topcard, giliran, sama arah jalan
            ConsoleDisplay.ShowHeader(gc, board, deck);

            //tampil list player dan jumlah kartunya
            ConsoleDisplay.ShowPlayerStats(gc);

            //tampil list kartu currentplayer di tanga
            ConsoleDisplay.ShowHand(gc);
            
            if (gc.CanCurrentPlayerPlay(board)) 
            {
                // Kalau ada yang cocok, baru minta input nomor
                int choice = ConsoleDisplay.GetPlayerChoice(gc.GetCurrentPlayerHand().Count);
                gc.PlayerTurn(board, choice);
            }
            else 
            {
                ConsoleDisplay.ShowDrawMessage();
                Console.ReadLine();
                gc.PlayerTurn(board, null); // Kirim null biar masuk ke logic draw di dalem
                //abis ini langsun ke skip 
            }
        }
    }else{
        Environment.Exit(0);
    }