using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HandController
{
	public Dictionary<int,List<CardBasicScript>> AvailableCards;

	public Combo combo;

	public int cardsTaken { get; set; }

	public Tuple<Suits,int> FlushPossible;

	public List<CardBasicScript> WinningCards;

	public HandController()
	{
		combo = new Combo (Combos.High, 0);
		AvailableCards = new Dictionary<int, List<CardBasicScript>> ();
		FlushPossible = new Tuple<Suits, int> (Suits.Spikes, 0);
		WinningCards = new List<CardBasicScript> ();
	}

	public void UpdateCombo ()
	{
		combo = new Combo (Combos.High, 0);
		//string combo = "";
		foreach (var e in AvailableCards.Values) {
			//if (e == null)
			//	continue;
			if (combo.Item1 < Combos.Quad)
			{
				if (e.Count == 2)
				{
					combo = combo.TwoItemCombo(e[0].Card.Rank);
				}
				else if (e.Count == 3)
				{
					combo = combo.ThreeItemCombo(e[0].Card.Rank);
				}
			}
			if (e.Count == 4)
			{
				combo = new Combo(Combos.Quad, e[0].Card.Rank);
			}
		}
		if (combo.Item1 < Combos.Straight)
			combo = combo.CheckStraight (AvailableCards);
		if (combo.Item2 == 0)
			combo = new Combo(Combos.High, GetHighestRank());
		if (FlushPossible.Item2 >= 5 && combo.Item1 <= Combos.Straight) 
		{
			combo = combo.CheckStraightFlush(AvailableCards,FlushPossible);
		}
	}

	private int GetHighestRank()
	{
		return AvailableCards.Max(z=>z.Key);
	}

	public void CheckSuit()
	{
		Dictionary<Suits,int> suitCount = new Dictionary<Suits, int>();
		foreach (var listOfCardScript in AvailableCards.Values)
		{
			//if (listOfCardScript == null)
			//	continue;
			foreach (var cardScript in listOfCardScript)
			{
				if (suitCount.ContainsKey(cardScript.Card.Suit))
					suitCount[cardScript.Card.Suit] += 1;
				else
					suitCount.Add(cardScript.Card.Suit,1);
			}
		}
		foreach (var suit in suitCount) 
		{
			if (suit.Value > FlushPossible.Item2)
				FlushPossible = new Tuple<Suits, int>(suit.Key,suit.Value);
		}
	}
	
	public void AddCard(CardBasicScript cScript)
	{
		cardsTaken++;
		if (AvailableCards.ContainsKey (cScript.Card.Rank))
			AvailableCards [cScript.Card.Rank].Add (cScript);
		else
			AvailableCards.Add (cScript.Card.Rank, new List<CardBasicScript>(){cScript});
	}

	public void ChooseWinningCards()
	{
		List<int> ranksForSkip = new List<int> ();
		if (combo.Item1 == Combos.Straight)
		{
			for (int i=0;i<5;i++)
			{
				WinningCards.Add(AvailableCards[combo.Item2-i][0]);
			}
		}
		else
		{
			if(combo.Item2 > 100)
			{
				AddWinningCards(combo.Item2/100);
				ranksForSkip.Add(combo.Item2/100);
			}
			AddWinningCards(combo.Item2%100);
			ranksForSkip.Add(combo.Item2%100);
		}
		var orderedCards = AvailableCards.Select (z => z.Key)
			.OrderByDescending (z => z)
				.ToList ();
		while (WinningCards.Count < 5) 
		{
			if (!ranksForSkip.Contains(orderedCards[0]))
				AddWinningCards(orderedCards[0]);
			orderedCards.RemoveAt(0);
		}
	}
	
	private void AddWinningCards(int rank)
	{
		for (int i=0;i<AvailableCards[rank].Count;i++)
		{
			WinningCards.Add (AvailableCards[rank][i]);
			if (WinningCards.Count == 5)
				break;
		}
	}
}

public class Tuple<T1,T2>
{
	public Tuple(T1 item1, T2 item2)
	{
		Item1 = item1;
		Item2 = item2;
	}
	
	public T1 Item1 { get; set; }
	public T2 Item2 { get; set; }
}