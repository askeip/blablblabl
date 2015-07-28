using UnityEngine;
using System.Collections;

public class CardDeck
{
	public string[] cards{ get; set; }

	string suits = "HSDC";
	string ranks = "23456789TJQKA";

	public CardDeck()
	{
		cards = new string[52];
		for (int i = 0; i < ranks.Length; i++) {
			for (int j = 0; j < suits.Length; j++) {
				cards [i * suits.Length + j] = ranks [i].ToString () + suits [j].ToString ();
			}
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
