using UnityEngine;
using System.Collections;

public class CardDeck
{
    public CardBasic[] Cards { get; set; }

    public CardDeck()
    {
        Cards = new CardBasic[52];
        for (int i = 0; i < 13; i++)
            for (int j = 0; j < 4; j++)
            {
                Cards[i * 4 + j] = new CardBasic(i + 2, (Suits)j);
            }
    }

    public int Length { get { return Cards.Length; } }

    public void Shuffle()
    {
        var rnd = new System.Random();
        for (var i = 0; i < Cards.Length; i++)
        {
            var card = Cards[i];
            var j = rnd.Next(i, Cards.Length);
            Cards[i] = Cards[j];
            Cards[j] = card;
        }
    }
}
