using UNOGame.Models;
using UNOGame.Logic;
using UNOGame.Enums;

namespace UNOGame.UI;

public class ConsoleDisplay
{
    //show menu
    public void ShowMenu()
    {
        Console.WriteLine("");
        Console.WriteLine("==================================");
        Console.WriteLine("   WELCOME TO UNO CONSOLE GAME!");
        Console.WriteLine("==================================");
        Console.WriteLine("1. Start Game");
        Console.WriteLine("2. Exit");
        Console.WriteLine("Masukkan Pilihan : ");

        int pilihan = 0;

        switch (pilihan)
        {
            case 1 :  
                initPlayer();
                break;
            case 2 :
                Environment.Exit(0);
                break;
            default : 
                Console.WriteLine("Pilihan Tidak Valid!");
                break;
        }
    }
    //initialize player
    public void initPlayer()
    {
        //masukin player yang assgin ke list player.

        
    }
    //showheader
    public void ShowHeader()
    {
        //gettopcard
        //getcurrentPlayer
        //GetClockIsWise

    }
    //show player stats
    public void ShowPlayerStats()
    {
        //getplayerlist
        //getplayerhand.count
    }
    //show player hand card
    public void ShowHand()
    {
        //getplayerhand.currentplayer
        //bikin tampilan piliaan kartu pake nomor

    }
    public static int  GetPlayerChoice(int handCount)
    {
        //bikin conditonal dari pilihan player, passing ke playerturn
        while (true)
        {
            Console.Write($"\nPilih kartu (1-{handCount}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= handCount)
            {
                return choice;
            }
            Console.WriteLine("Input salah! Masukkan angka sesuai nomor kartu.");
        }

    }
    //show color pick
    public static void  ShowColorPick (GameController gc)
    {
        Console.WriteLine("Pilih Warna");
        Console.WriteLine("1. Red");
        Console.WriteLine("2. Blue");
        Console.WriteLine("3. Green");
        Console.WriteLine("4. Yellow");
        Console.WriteLine("Masukkan Pilihan : ");
        int input = Convert.ToInt32(Console.ReadLine());

        CardColor  chosenColor;
        switch (input)
        {
            case 1 :
                chosenColor = CardColor.Red;
                break;
            case 2 :
                chosenColor = CardColor.Blue;
                break;  
            case 3 :
                chosenColor = CardColor.Green;
                break;
            case 4 :
                chosenColor = CardColor.Yellow;
                break;  

            default:
                chosenColor = CardColor.Red; // Jaga-jaga kalau input salah
                break;   
        }

        gc.Wild(chosenColor);
    }
    
    //even handler
    public void evenHandler()
    {
        
    }


}