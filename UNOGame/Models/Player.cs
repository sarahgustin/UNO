namespace UNOGame.Models;

class Player : IPlayer
{
    public string Name {get; set;}
    public Player(string Name)
    {
        this.Name = Name;
    }
}