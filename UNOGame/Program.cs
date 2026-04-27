using System;
using UNOGame.Enums;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.UI;

namespace UNOGame;
class UNOGame
{   
    //tambah kartu
    static List<ICard> GenerateFullDeck()
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
                // Casting angka i jadi CardType enum biar dapet One, Two, dll. 
                CardType numberType = (CardType)i;
                fullDeck.Add(new Card(color, numberType));
                fullDeck.Add(new Card(color, numberType));
            }

            for (int i = 1; i <= 2; i++)
            {
                fullDeck.Add(new Card(color, CardType.Skip));
                fullDeck.Add(new Card(color, CardType.Reverse));
                fullDeck.Add(new Card(color, CardType.Draw));   
            }
        }
        for (int i = 1; i <= 2; i++)
        {
            fullDeck.Add(new Card(CardColor.Black, CardType.Wild));
            fullDeck.Add(new Card(CardColor.Black, CardType.Wild));
        }
        
        return fullDeck;
    }
    static void Main(string[] args)
    {
        int menuChoice =  ConsoleDisplay.ShowMenu();
        if (menuChoice == 1)
        {
            IBoard board = new Board();
            List<ICard> allCard = GenerateFullDeck();
            IDeck deck = new Deck(allCard);
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
                ConsoleDisplay.ShowHeader(gc, deck);
                ConsoleDisplay.ShowPlayerStats(gc);
                ConsoleDisplay.ShowTopCard(gc);
                ConsoleDisplay.ShowHand(gc); 
                           
                if (gc.CanCurrentPlayerPlay()) 
                {
                    int choice = ConsoleDisplay.GetPlayerChoice(gc.GetCurrentPlayerHand().Count);
                    gc.PlayerTurn(choice);
                }
                else 
                {
                    ConsoleDisplay.ShowDrawMessage();
                    Console.ReadLine();
                    gc.PlayerTurn(null); // Kirim null biar masuk ke logic draw di dalem
                }
            }  
        }else{
            Environment.Exit(0);
        }
    }
}