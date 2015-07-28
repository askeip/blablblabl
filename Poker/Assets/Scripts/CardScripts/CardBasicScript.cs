using UnityEngine;
using System.Collections;

using System.Linq;
using System;

public class CardBasicScript : MonoBehaviour 
{
	public int Rank;
	
	public Suits Suit;
	
	public void SetCard(string cardName)
	{
		Rank = RankToInt(cardName[0]);
		Suit = SuitToSuits(cardName[1]);
		ShowCard (cardName);
		//string name = "5S";
	}

	public virtual void ShowCard(string cardName)
	{
	}
	
	private Suits SuitToSuits(char suit)
	{
		switch (suit) 
		{
		case 'S':return Suits.Spikes;
		case 'H':return Suits.Hearts;
		case 'D':return Suits.Diamonds;
		default:return Suits.Clubs;
		}
	}
	private int RankToInt(char rank)
	{
		switch (rank) 
		{
		case 'T': return 10;
		case 'J': return 11;
		case 'Q': return 12;
		case 'K': return 13;
		case 'A': return 14;
		default: return int.Parse(rank.ToString());
		}
	}
}
