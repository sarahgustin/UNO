namespace UNOGame.Models;

public class Board : IBoard
{
    public List<ICard> UsedCards {get; set;} = new List<ICard>();

    public Board()
    {
    
    }
}