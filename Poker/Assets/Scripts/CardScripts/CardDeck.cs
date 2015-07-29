using UnityEngine;
using System.Collections;

public class CardDeck
{
	public CardBasic[] cards{ get; set; }

	public CardDeck()
	{
		cards = new CardBasic[52];
		for (int i =0; i<13; i++)
			for (int j=0;j<4;j++)
		{
			cards[i * 4 + j] = new CardBasic(i + 2,(Suits)j);
		}
	}

	public int Length { get { return cards.Length; } }
	
	public void Shuffle()
	{
		var rnd = new System.Random();
		for (var i = 0; i < cards.Length; i++)
		{
			var card = cards[i];
			var j = rnd.Next(i,cards.Length);
			cards[i] = cards[j];
			cards[j] = card;
		}
	}
}
