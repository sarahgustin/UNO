using UNOGame.Enums;

namespace UNOGame.Models;
class Card : ICard
{
    public CardColor CardColor { get; set; }
    public CardType CardType { get; private set; }

    public Card(CardColor cardColor, CardType cardType)
    {
        CardColor = cardColor;
        CardType = cardType;
    }
}