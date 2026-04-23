using UNOGame.Models;
using UNOGame.Logic;
using UNOGame.Enums;

namespace UNOGame.UI;

public class ConsoleDisplay
{
    //show menu

    
    public static int ShowMenu()
    {
        Console.WriteLine("");
        Console.WriteLine("==================================");
        Console.WriteLine("   WELCOME TO UNO CONSOLE GAME!");
        Console.WriteLine("==================================");
        Console.WriteLine("1. Start Game");
        Console.WriteLine("2. Exit");
        Console.WriteLine("Masukkan Pilihan : ");        
        return Convert.ToInt32(Console.ReadLine());
        
    }
    //initialize player
    
    public static List <IPlayer> InitPlayer()
    {
        List<IPlayer> players = new List<IPlayer>();
        
        int count = 0;
        while (count < 2 || count > 4) // Syarat minimal 2, maksimal 4
        {
            Console.Write("Masukkan jumlah pemain (2-4): ");
            int.TryParse(Console.ReadLine(), out count);
        }
        
        //masukin player yang assgin ke list player.
        for (int i = 1; i <= count; i++)
        {
            Console.WriteLine($"Masukan Nama Player ke{i} : ");
            string name = Console.ReadLine()??"";
            
            players.Add(new Player(name));
        }
        return players;
    

/*
    public static List<IPlayer> InitPlayer()
    {
        
        int count = 0;
        // LOOP UTAMA: Akan terus bertanya selama input bukan 2, 3, atau 4
        while (true) 
        {
            Console.Write("Masukkan jumlah pemain (2-4): ");
            string input = Console.ReadLine()??"";

            if (int.TryParse(input, out count) && count >= 2 && count <= 4)
            {
                // Jika input beneran angka dan antara 2-4, keluar dari loop
                break; 
            }

            Console.WriteLine("Input Salah! Pemain hanya bisa 2, 3, atau 4.");
        }

        List<IPlayer> players = new List<IPlayer>();
        for (int i = 1; i <= count; i++)
        {
            Console.Write($"Masukkan nama pemain {i}: ");
            string name = Console.ReadLine()??"";
            players.Add(new Player(name));
        }

        return players;
    }*/
    
    //showheader
    public static void ShowHeader(GameController gc, IBoard board)
    {
        //gettopcard
        ICard topCard = gc.GetTopCard(board);
        //getcurrentPlayer
        IPlayer currentPlayer = gc.GetCurrentPlayer();
        //GetClockIsWise
        bool isClockwise = gc.GetIsClockwise();
        
        Console.WriteLine("============================");
        Console.WriteLine("       UNO CONSOLE GAME");
        Console.WriteLine("=============================");
        Console.WriteLine($"TOP CARD : [{topCard.CardColor}{topCard.CardType}]");
        Console.WriteLine("=============================");
        Console.WriteLine($"GILIRAN : [{currentPlayer.Name}]");
        Console.WriteLine($" ARAH MAIN: {(isClockwise ? "Searah Jarum Jam (Clockwise)" : "Berlawanan Jarum Jam (Counter-Clockwise)")}");
    }
    
    //show player stats
    public static void ShowPlayerStats(GameController gc)
    {
        //getplayerlist
        List<IPlayer> players = gc.GetPlayerList();
        
        //getplayerhand.count
        foreach (var p in players)
        {
            int cardCount = gc.PlayerHandCount(p);
            
            Console.WriteLine($"{p.Name.PadRight(15)} : {cardCount}");
        }
    }
    
    //show player hand card
    public static void ShowHand(GameController gc)
    {
        //getplayerhand.currentplayer
        List<ICard> playerCards = gc.GetCurrentPlayerHand();
        
        Console.WriteLine("Kartu Kamu : ");
        //bikin tampilan piliaan kartu pake nomor
        for (int i = 0; i < playerCards.Count; i++)
        {
            ICard currentCard = playerCards[i];
            Console.WriteLine($"{i+1}. [{currentCard.CardType} {currentCard.CardColor}] ");
        }
    }
    public static int GetPlayerChoice(int handCount)
    {
       
        //bikin conditonal dari pilihan player, passing ke playerturn
        while (true)
        {
            Console.Write($"\nPilih nomor kartu (1-{handCount}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= handCount)
            {
                return choice;
            }
            Console.WriteLine("Input salah! Masukkan angka sesuai nomor kartu.");
        }
    }
    //show message
    public static void ShowMessage(string message)
    {
        // Biar keliatan beda sama tulisan kartu, kita kasih hiasan dikit
        Console.WriteLine("\n==========================================");
        Console.WriteLine($"   [!] INFO: {message}");
        Console.WriteLine("==========================================");
        Console.WriteLine("Tekan [Enter] untuk lanjut...");
        Console.ReadLine(); 
    }
    //show color pick
    public static CardColor ShowColorPick ()
    {
        Console.WriteLine("Pilih Warna");
        Console.WriteLine("1. Red");
        Console.WriteLine("2. Blue");
        Console.WriteLine("3. Green");
        Console.WriteLine("4. Yellow");
        Console.WriteLine("Masukkan Pilihan : ");
        int input = Convert.ToInt32(Console.ReadLine());

        return input switch
        {
            1 => CardColor.Red,
            2 => CardColor.Blue,
            3 => CardColor.Green,
            4 => CardColor.Yellow,
            _ => CardColor.Red
        };
    }
    
    //even handler
    public static void ShowDeckEmptyMessage()
    {
        Console.WriteLine("============================");
        Console.WriteLine("Kartu Deck Habis!! Mengocok ulang kartu di meja");
        Console.WriteLine("Tekan Enter untuk lanjut...");
        Console.ReadLine();
    }

    public static void ShowWinnerAnnouncement(IPlayer winner)
    {   
        Console.WriteLine("************************************************");
        Console.WriteLine($"\nSELAMAT {winner.Name} ADALAH PEMENANGNYA!");
        Console.WriteLine("       Semua kartu di tangan sudah habis.");
        Console.WriteLine("************************************************");
        Console.WriteLine("Game Selesai! Tekan Enter untuk keluar.");
        Console.ReadLine();
    }


}