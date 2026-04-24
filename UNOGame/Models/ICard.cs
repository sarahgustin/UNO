using UNOGame.Enums;
namespace UNOGame.Models;


public interface ICard
{
    CardColor CardColor { get; set; }
    CardType CardType { get; }
}