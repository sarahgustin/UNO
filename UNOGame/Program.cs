
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.UI;


    IBoard board = new Board();
    List<ICard> AllCard = GameController.GenerateFullDeck(new List<ICard>());
    IDeck deck = new Deck(AllCard);

   
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


        //loop game 
        while (!isGameOver)
        {
            Console.Clear();

            //Tampil header, buat info topcard, giliran, sama arah jalan
            ConsoleDisplay.ShowHeader(gc, board);

            //tampil list player dan jumlah kartunya
            ConsoleDisplay.ShowPlayerStats(gc);

            //tampil list kartu currentplayer di tanga
            ConsoleDisplay.ShowHand(gc);

            /*pilih kartu
            List<ICard> playerHand = gc.GetCurrentPlayerHand();
            int choice = ConsoleDisplay.GetPlayerChoice(playerHand.Count);
            gc.PlayerTurn(board, choice);*/

            if (gc.CanCurrentPlayerPlay(board)) 
            {
                // Kalau ada yang cocok, baru minta input nomor
                int choice = ConsoleDisplay.GetPlayerChoice(gc.GetCurrentPlayerHand().Count);
                gc.PlayerTurn(board, choice);
            }
            else 
            {
                // Kalau GAK ADA yang cocok, langsung draw tanpa nanya nomor
                Console.WriteLine("\n>>> Gak ada kartu cocok! Tekan ENTER untuk ambil kartu... <<<");
                Console.ReadLine(); 
                gc.PlayerTurn(board, null); // Kirim null biar masuk ke logic draw di dalem
            }
        }
    }else{
        Environment.Exit(0);
    }