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
    public static List<IPlayer> InitPlayer()
    {
        List<IPlayer> players = new List<IPlayer>();

        int count = 0;
        while (count < 2 || count > 4) // Syarat minimal 2, maksimal 4
        {
            Console.Write("Masukkan jumlah pemain (2-4): ");
            int.TryParse(Console.ReadLine(), out count);
        }

        //masukin player yang di assgin ke list player.
        for (int i = 1; i <= count; i++)
        {
            Console.WriteLine($"Masukan Nama Player ke-{i} : ");
            string name = Console.ReadLine() ?? "";

            players.Add(new Player(name));
        }
        return players;
    }
    
    public static string GetCardLabel(ICard card)
    {
        // Kalau 0-9, return angkanya. Kalau >= 10, return nama aslinya (Skip, Wild, reverse)
        return (int)card.CardType < 10 ? ((int)card.CardType).ToString() : card.CardType.ToString();
    }
    //showheader
    public static void ShowHeader(GameController gc, IBoard board, IDeck deck)
    {
        //gettopcard
        ICard topCard = gc.GetTopCard(board);
        //getcurrentPlayer
        IPlayer currentPlayer = gc.GetCurrentPlayer();
        //GetClockIsWise
        bool isClockwise = gc.GetIsClockwise();
        Console.WriteLine("=====================================================");
        Console.WriteLine("                   UNO CONSOLE GAME");
        Console.WriteLine("=====================================================");
        Console.WriteLine($"KARTU DI DECK : {deck.Cards.Count} kartu");
        Console.WriteLine($"GILIRAN       : [{currentPlayer.Name.ToUpper()}]");
        Console.WriteLine($"ARAH MAIN     : {(isClockwise ? "Searah Jarum Jam (Clockwise)" : "Berlawanan Jarum Jam (Counter-Clockwise)")}");
        Console.WriteLine("=======================================");
        Console.WriteLine($"TOP CARD      : [{topCard.CardColor} {GetCardLabel(topCard)}]");
        Console.WriteLine("=======================================");
    }
    
    //show player stats
    public static void ShowPlayerStats(GameController gc)
    {
        //getplayerlist
        List<IPlayer> players = gc.GetPlayerList();
        Console.WriteLine("PLAYER : ");
        //getplayerhand.count
        foreach (var p in players)
        {
            int cardCount = gc.PlayerHandCount(p);
            
            Console.WriteLine($"{p.Name.PadRight(15).ToUpper()} : {cardCount}");
        }
        Console.WriteLine("=======================================");
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
            Console.WriteLine($"{i+1}. [{currentCard.CardColor} {GetCardLabel(currentCard)} ] ");
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
            else
            {
                Console.WriteLine("Input salah! Masukkan angka sesuai nomor kartu.");
            }
            
        }
    }
    //show penalty message
    public static void ShowPenaltyMessage(string playerName, int amount, string reason)
    {

        Console.WriteLine("\n========================================");
        Console.WriteLine($"[PENALTY] : {playerName} terkena efek {reason}!");
        Console.WriteLine($"[+] Menambah {amount} kartu ke tangan.");
        Console.WriteLine($"[!] Giliran {playerName} dilewati!");
        Console.WriteLine("========================================");
        Console.WriteLine("Tekan ENTER untuk lanjut...");
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
    
    //draw message 
    public static void ShowDrawMessage()
    {
        // Kalau GAK ADA yang cocok, langsung draw
        Console.WriteLine("\n>>> Gak ada kartu cocok! Tekan ENTER untuk ambil kartu... <<<");
    }

    public static void ShowDrawResult(ICard card, bool isMatch)
    {
        string cardLabel = (int)card.CardType < 10 ? ((int)card.CardType).ToString() : card.CardType.ToString();
        Console.WriteLine($"\n[DRAW] Anda mendapatkan: [{cardLabel} {card.CardColor}]");
        
        if (isMatch)
        {
            Console.WriteLine(">>>Kartu COCOK! Tekan ENTER untuk langsung memasang...");
        }
        else
        {
            Console.WriteLine(">>> Kartu TIDAK cocok. Tekan ENTER untuk lanjut ke pemain berikutnya...");
        }
        Console.ReadLine();
    }
    
    //even handler
    public static void ShowDeckEmptyMessage()
    {
        Console.WriteLine("============================");
        Console.WriteLine("Kartu Deck Habis!! Mengocok ulang kartu di meja");
        Console.WriteLine(">>>Tekan Enter untuk lanjut...<<<");
        Console.ReadLine();
    }

    public static void ShowWinnerAnnouncement(IPlayer winner)
    {   
        Console.WriteLine("************************************************");
        Console.WriteLine($"       SELAMAT {winner.Name.ToUpper()} ADALAH PEMENANGNYA!");
        Console.WriteLine("       Semua kartu di tangan sudah habis.");
        Console.WriteLine("************************************************");
        Console.WriteLine("Game Selesai! Tekan Enter untuk keluar.");
        Console.ReadLine();
    }


}