using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChanceOfWin 
{
	public Dictionary<int,List<CardBasic>> cardsInDeck;
	public WinnerChooser winnerChooser;
	public int cardsLeft;

	public ChanceOfWin()
	{
		DefaultCardsInDeck ();
		winnerChooser = new WinnerChooser ();
	}

	public int CalcChances(List<PlayerBasicScript> notFoldedPlayers)
	{
		var highComboKeepers = winnerChooser.ChooseWinner (notFoldedPlayers);
		return 2;
	}

	public void DefaultCardsInDeck()
	{
		cardsInDeck = new Dictionary<int, List<CardBasic>> ();
		for (int i = 0; i < 13; i++)
		{
			cardsInDeck.Add(i+2,new List<CardBasic>());
			for (int j=0;j<4;j++)
			{
				cardsInDeck[i+2].Add(new CardBasic(i+2,(Suits)j));
			}
		}
		cardsLeft = 52;
	}

	public void RemoveCardFromDeck(CardBasic card)
	{
		int i = 0;
		while (cardsInDeck[card.Rank][(int)card.Suit - i] == null || cardsInDeck[card.Rank][(int)card.Suit - i].Suit != card.Suit)
		{ 
			i++;
			if (i > 4)
				return; //Сделать Exception
		}
		cardsInDeck [card.Rank].RemoveAt ((int)card.Suit - i);
		cardsLeft--;
	}

	public float ChanceOfRepeat(int cardsOnTable,int rank)
	{
		float chance = 0;
		for (int i = cardsOnTable;i < 5; i++)
		{
			chance += cardsInDeck[rank].Count / (cardsLeft - (i - cardsOnTable));
		}
		return chance;
	}

	public void DeleteCards(List<PlayerScript> notFoldedPlayers,List<CardBasic> tableCards)
	{
		foreach (var card in tableCards)
		{
			RemoveCardFromDeck(card);
		}
		foreach (var player in notFoldedPlayers)
		{
			RemoveCardFromDeck(player.leftCard.Card);
			RemoveCardFromDeck(player.rightCard.Card);
		}
	}
}
