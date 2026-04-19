namespace UNOGame.Models;
using UNOGame.Enums;

public interface ICard
{
    CardColor CardColor { get; set; }
    CardType CardType { get; }
}