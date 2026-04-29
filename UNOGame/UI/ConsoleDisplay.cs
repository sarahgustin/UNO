using UNOGame.Models;
using UNOGame.Logic;
using UNOGame.Enums;
using Serilog;
namespace UNOGame.UI;

public class ConsoleDisplay
{
    private static int _consoleWidth => Console.WindowWidth;
    private static string centerText (string text)
    {
        int leftPadding = (_consoleWidth - text.Length) / 2; 
        return text.PadLeft(leftPadding + text.Length);
    }

    public static int ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine(new string ('=', _consoleWidth));
        string welcomeText = "WELCOME TO UNO CONSOLE GAME!";
        Console.WriteLine(centerText(welcomeText));
        Console.WriteLine(new string ('=', _consoleWidth));
        Console.WriteLine("1. Start Game");
        Console.WriteLine("2. Exit");
        Console.WriteLine("Masukkan Pilihan : ");        
        return Convert.ToInt32(Console.ReadLine());
    }
    
    public static List<IPlayer> InitPlayer()
    {
        Console.WriteLine();
        Console.WriteLine(new string ('=', _consoleWidth));
        List<IPlayer> players = new List<IPlayer>();

        int count = 0;
        while (count < 2 || count > 4)
        {
            Console.Write("Masukkan jumlah pemain (2-4): ");
            int.TryParse(Console.ReadLine(), out count);
        }

        for (int i = 1; i <= count; i++)
        {
            Console.WriteLine($"Masukan Nama Player ke-{i} : ");
            string name = Console.ReadLine() ?? "";

            players.Add(new Player(name));
        }
        return players;
    }

    private static string GetCardLabel(ICard card)
    {
        return (int)card.CardType < 10 ? ((int)card.CardType).ToString() : card.CardType.ToString();
    }

    private static ConsoleColor GetColor (CardColor color)
    {
        return color switch
        {
            CardColor.Red => ConsoleColor.Red,
            CardColor.Blue => ConsoleColor.Blue,
            CardColor.Green => ConsoleColor.Green,
            CardColor.Yellow => ConsoleColor.Yellow,
            CardColor.Black => ConsoleColor.White,
            _ =>ConsoleColor.White
        };
    }   
    
    public static void ShowHeader(GameController gc, IDeck deck)
    {
        IPlayer currentPlayer = gc.GetCurrentPlayer();
        bool isClockwise = gc.GetIsClockwise();

        string headerText = "UNO CONSOLE GAME";

        Console.WriteLine(new string ('=', _consoleWidth));
        Console.WriteLine(centerText(headerText));
        Console.WriteLine(new string ('=', _consoleWidth));
        Console.WriteLine($"KARTU DI DECK : {deck.Cards.Count} kartu");
        Console.WriteLine($"GILIRAN       : [{currentPlayer.Name.ToUpper()}]");
        Console.WriteLine($"ARAH MAIN     : {(isClockwise ? "Searah Jarum Jam (Clockwise)" : "Berlawanan Jarum Jam (Counter-Clockwise)")}");
        Console.WriteLine(new string ('=', _consoleWidth));
    }

    public static void ShowTopCard(GameController gc)
    {
        int cardwidth = 16;
        int leftPadding = (_consoleWidth - cardwidth) /2;
        string leftPad = new string(' ' , leftPadding);

        ICard topCard = gc.GetTopCard();
        
        Console.WriteLine(leftPad + "┌────────────────┐");

        Console.Write(leftPad + "│");
        Console.ForegroundColor = GetColor(topCard.CardColor);
        Console.Write(topCard.CardColor.ToString().PadRight(cardwidth));
        Console.ResetColor();
        Console.WriteLine("│");

        Console.WriteLine($"{leftPad}│{"".PadRight(cardwidth)}│");
        Console.WriteLine($"{leftPad}│{"".PadRight(cardwidth)}│");
        string value = GetCardLabel(topCard);
        Console.Write(leftPad + "│");
        Console.ForegroundColor = GetColor(topCard.CardColor);
        Console.Write(value.PadLeft((cardwidth + value.Length)/2).PadRight(cardwidth));
        Console.ResetColor();
        Console.WriteLine("│");

        Console.WriteLine($"{leftPad}│{"".PadRight(cardwidth)}│");
        Console.WriteLine($"{leftPad}│{"".PadRight(cardwidth)}│");

        //bawah kartu
        Console.Write(leftPad + "│");
        Console.ForegroundColor = GetColor(topCard.CardColor);
        Console.Write(topCard.CardColor.ToString().PadLeft(cardwidth));
        Console.ResetColor();
        Console.WriteLine("│");
        Console.WriteLine(leftPad + "└────────────────┘");
    }

    public static void ShowPlayerStats(GameController gc)
    {
        List<IPlayer> players = gc.GetPlayerList();
        Console.WriteLine("PLAYER : ");
        
        foreach (var p in players)
        {
            int cardCount = gc.PlayerHandCount(p);
            Console.Write($"{p.Name.PadRight(15).ToUpper()} : {cardCount} Kartu");
            if(cardCount == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [UNO]");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        Console.WriteLine(new string ('=', _consoleWidth));
    }

    public static void ShowHand(GameController gc)
    {
        Console.WriteLine(new string ('=', _consoleWidth));
        
        List<ICard> playerCards = gc.GetCurrentPlayerHand();
        Console.WriteLine("Kartu Kamu : ");

        int cardwidth = 16;
        foreach(var card in playerCards)
        {
            Console.Write("┌────────────────┐ ");
        }
        Console.WriteLine();
        foreach(var card in playerCards)
        {
            Console.Write("│");
            Console.ForegroundColor = GetColor(card.CardColor);
            Console.Write(card.CardColor.ToString().PadRight(cardwidth));
            Console.ResetColor();
            Console.Write("│ ");
        }
        Console.WriteLine();

        for (int i = 1; i<=2; i++)
        {
            foreach(var card in playerCards)
            {
                Console.Write($"│{"".PadRight(cardwidth)}│ ");
            }
            Console.WriteLine();
        }
        

        foreach(var card in playerCards)
        {
            string value = GetCardLabel(card);
            Console.Write("│");
            Console.ForegroundColor = GetColor(card.CardColor);
            Console.Write(value.PadLeft((cardwidth + value.Length)/2).PadRight(cardwidth));
            Console.ResetColor();
            Console.Write("│ ");
        }
        Console.WriteLine();

        for (int i = 1; i<=2; i++)
        {
            foreach(var card in playerCards)
            {
                Console.Write($"│{"".PadRight(cardwidth)}│ ");
            }
            Console.WriteLine();
        }
       

        //bawah kartu 
        foreach(var card in playerCards)
        {    
            Console.Write("│");
            Console.ForegroundColor = GetColor(card.CardColor);
            Console.Write(card.CardColor.ToString().PadLeft(cardwidth));
            Console.ResetColor();
            Console.Write("│ ");
        }
        Console.WriteLine();

        foreach(var card in playerCards)
        {
            Console.Write("└────────────────┘ ");
        }
        Console.WriteLine();

        int number = 1;
        foreach(var card in playerCards)
        {
            string choice = $"[{number++}]";
            Console.Write(choice.PadLeft((18 + choice.Length) / 2).PadRight(18) + " ") ;
        }
        Console.WriteLine();
    }

    public static int GetPlayerChoice(int handCount)
    {
        while (true)
        {
            Console.WriteLine($"Pilih nomor kartu (1-{handCount}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= handCount)
            {
                return choice;
            }
            else
            {
                Console.WriteLine("Input salah! Masukkan angka sesuai nomor kartu.");
                Log.Warning("Input kartu tidak valid. Ulang input kartu!");

            }
            
        }
    }

    public static void ShowPenaltyMessage(string playerName, int amount, string reason)
    {

        Console.WriteLine("========================================");
        Console.WriteLine($"[PENALTY] : {playerName.ToUpper()} terkena {reason}!");
        Console.WriteLine($"[+] Menambah {amount} kartu ke tangan.");
        Console.WriteLine($"[!] Giliran {playerName.ToUpper()} dilewati!");
        
        Console.WriteLine("Tekan ENTER untuk lanjut...");
        Console.ReadLine();
    }
    
    public static CardColor ShowColorPick ()
    {
        Console.WriteLine("========================================");
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
    
    public static void ShowDrawMessage()
    {
        Console.WriteLine(">>> Gak ada kartu cocok! Tekan ENTER untuk ambil kartu... <<<");
    }

    public static void ShowDrawResult(ICard card, bool isMatch)
    {
        string cardLabel = GetCardLabel(card);
        Console.WriteLine($"[DRAW] Anda mendapatkan: [{cardLabel} {card.CardColor}]");
        
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

    public static void ShowDeckEmptyMessage()
    {
        Console.WriteLine("============================");
        Console.WriteLine("Kartu Deck Habis!! Mengocok ulang kartu di meja");
        Console.WriteLine(">>>Tekan Enter untuk lanjut...<<<");
        Console.ReadLine();
    }

    public static void ShowWinnerAnnouncement(IPlayer winner)
    {   
        string winnerText = $"SELAMAT {winner.Name.ToUpper()} ADALAH PEMENANGNYA!";
        string winnerReason ="SEMUA KARTU DITANGAN SUDAH HABIS.";
        string endText = "Game Selesai! Tekan Enter untuk keluar.";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(new string ('*', _consoleWidth));
        Console.WriteLine(centerText(winnerText));
        Console.WriteLine(centerText(winnerReason));
        Console.WriteLine(new string ('*', _consoleWidth));
        Console.WriteLine(centerText(endText));
        Console.ResetColor();
        Console.ReadLine();
    }
}