namespace UNOGame.Models;

public class Player : IPlayer
{
    public string Name {get; set;}
    public Player(string Name)
    {
        this.Name = Name;
    }
}