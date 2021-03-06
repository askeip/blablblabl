﻿using UnityEngine;
using System.Collections;

using System.Linq;
using System;

public enum Suits{Diamonds,Hearts,Spikes,Clubs};

public class CardBasicScript : MonoBehaviour 
{
	public CardBasic Card { get; set; } 

	public void SetCard(CardBasic card,bool showCard = false)
	{
		Card = card;
		if (showCard)
			ShowCard ();
		else
			HideCard ();
	}

	public void ShowCard()
	{
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("OtherCards/" + ((Card.Rank - 2) + 13 * (int)Card.Suit));
		Resources.UnloadUnusedAssets ();
	}

	public void HideCard()
	{
		this.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite> ("OtherCards/" + "flipside");
		Resources.UnloadUnusedAssets ();
	}
}

public class CardBasic
{
	public int Rank;
	
	public Suits Suit;
	
	public CardBasic(int rank, Suits suit)
	{
		Rank = rank;
		Suit = suit;
	}
}
