namespace UNOGame.Models;

public class Deck : IDeck
{
    public List <ICard> Cards {get; set;} = new List<ICard>();

    public Deck (List <ICard> cards)
    {
        this.Cards = cards;
    }

}