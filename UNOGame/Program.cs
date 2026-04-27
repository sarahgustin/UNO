using System;
using UNOGame.Enums;
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.UI;

namespace UNOGame;
class UNOGame
{   
    static List<ICard> GenerateFullDeck()
    {
        List<ICard> fullDeck = new List<ICard>();
        CardColor[] cardColor = {CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };

        foreach (var color in cardColor)
        {
            fullDeck.Add(new Card(color, CardType.Zero));
            
            for (int i = 1; i <= 9; i++)
            {
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
                
            gc.OnPlayerRunOutCard += (winner) =>
            {
                ConsoleDisplay.ShowWinnerAnnouncement(winner);
                isGameOver = true;
            };
                
            gc.OnRequestColorSelection = ConsoleDisplay.ShowColorPick;
            gc.OnDeckEmpty += ConsoleDisplay.ShowDeckEmptyMessage;
            gc.OnDrawFeedback += ConsoleDisplay.ShowDrawResult;
            gc.OnPlayerPenalty += ConsoleDisplay.ShowPenaltyMessage;
        
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
                    gc.PlayerTurn(null);
                }
            }  
        }else{
            Environment.Exit(0);
        }
    }
}