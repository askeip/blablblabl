using UnityEngine;
using System.Collections;

using System.Linq;
using System;

public class CardScript : MonoBehaviour 
{
	public int Rank;

	public Suits Suit;

	float timer;
	bool startTimer = false;

	/*public void SetCard(int rank, Suits suit)
	{
		Rank = rank;
		Suit = suit;
	}

	private void SetCardSprite (int numOfCard)
	{
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Cards/" + numOfCard);
		Resources.UnloadUnusedAssets ();
	}*/
	public void SetCard(string cardName)
	{
		Rank = RankToInt(cardName[0]);
		Suit = SuitToSuits(cardName[1]);
		//string name = "5S";
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Cards/" + cardName);
		Resources.UnloadUnusedAssets ();
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

	public void ShowCard()
	{
		startTimer = true;
	}
}
