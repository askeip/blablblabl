using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDistributor
{
	public GameObject[] TableCards;
	 
	public CardDeck CardDeck { get; set; }

	PlayersFilter playersFilter;

	public CardDistributor()
	{
		TableCards = new GameObject[5];
		CardDeck = new CardDeck();
		playersFilter = new PlayersFilter ();
	}

	public void GiveCards(PlayerBasicScript playerScript,int i)
	{
		playerScript.GetNewHand(CardDeck.cards[i*2],CardDeck.cards[i*2+1]);
	}

	public void PutNewCard(List<PlayerBasicScript> playerScripts, int numOfCard)
	{
		var cardScript = TableCards[numOfCard].GetComponent<CardBasicScript> ();
		var notFoldedPlayers = playersFilter.NotFoldedPlayers (playerScripts);
		foreach (var playerScript in notFoldedPlayers)
			playerScript.handController.AddCard (cardScript);
	}
}