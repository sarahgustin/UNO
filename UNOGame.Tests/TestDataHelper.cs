using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.Enums;

namespace UNOGame.Tests;

public static class TestDataHelper
{
    
    //HELPER METHOD BUAT INSTANTIATE LIST CARD
    public static List<ICard> GenerateCardsForTest()
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
}