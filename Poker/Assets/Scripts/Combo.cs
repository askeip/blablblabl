using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Combos{High=1,Pair,TwoPair,Set,Straight,Flush,FullHouse,Quad,StraightFlush,RoyalFlush};

public class Combo 
{
	public Combos Item1;

	public int Item2;

	public Combo(Combos combo,int strength)
	{
		Item1 = combo;
		Item2 = strength;
	}

	public override int GetHashCode ()
	{
		return (int)Item1 * 10000 + Item2;
	}

	public override bool Equals (object o)
	{
		if (!(o is Combo))
			return false;
		var combo = o as Combo;
		return Item1 == combo.Item1 && Item2 == combo.Item2;
	}

	public Combo CheckStraightFlush(Dictionary<int,List<CardBasicScript>> Combination,Tuple<Suits,int> FlushPossible)
	{
		var flush = new Combo(Combos.Flush, HighFlushRank(Combination,FlushPossible));
		if (Item1!=Combos.Straight)
			return flush;
		for (int i =0; i<5; i++)
		{
			var sameSuit = false;
			foreach (var cardScript in Combination[Item2-i])
			{
				if (cardScript.Card.Suit == FlushPossible.Item1)
				{
					sameSuit = true;
					break;
				}
			}
			if (!sameSuit)
				return flush;
		}
		if (Item2 == 14)
			return new Combo(Combos.RoyalFlush,Item2);
		return new Combo(Combos.StraightFlush,Item2);
	}

	public int HighFlushRank(Dictionary<int,List<CardBasicScript>> Combination,Tuple<Suits,int> FlushPossible)
	{
		return Combination.OrderByDescending (z => z.Key)
			.Where (z => z.Value.Where (d => d.Card.Suit == FlushPossible.Item1) != null)
				.Select (z => z.Value [0].Card.Rank)
				.ElementAt (0);
	}
	
	public Combo CheckStraight(Dictionary<int,List<CardBasicScript>> Combination)
	{
		int tries = Combination.Count-4;
		int straightFromRank = 14;
		while (tries>0) 
		{
			if (Combination.ContainsKey(straightFromRank))
			{
				int cardsInARow = CardsInARow(Combination,straightFromRank - 1);
				if (cardsInARow >= 4);
					return new Combo(Combos.Straight,straightFromRank);
				else
					tries-= (cardsInARow + 1);
			}
			straightFromRank--;
		}
		if (Combination.ContainsKey(14))
		{
			int cardsInARow = CardsInARow(Combination,5);
			if (cardsInARow >= 4);
				return new Combo(Combos.Straight,5);
		}
		return this;
	}

	public static int CardsInARow(Dictionary<int,List<CardBasicScript>> Combination,int checkFromRank)
	{
		int cardsInARow = 0;
		while (true)
		{
			if (Combination.ContainsKey(checkFromRank))
			{
				cardsInARow++;
				checkFromRank--;
			}
			else
				return cardsInARow;
		}
	}
	
	public Combo TwoItemCombo(int comboRank)
	{
		if (Item2 == 0)
			return new Combo(Combos.Pair,comboRank);
		if (Item1 < Combos.Set) {
			if (Item1 == Combos.Pair)
				return new Combo (Combos.TwoPair, (Item2 > comboRank ? Item2 * 100 + comboRank : comboRank * 100 + Item2));
			else if (Item1 == Combos.TwoPair) {
				if (comboRank > Item2 % 100)
					return new Combo (Combos.TwoPair, (Item2 / 100 > comboRank ? Item2 - Item2 % 100 + comboRank : comboRank * 100 + Item2 / 100));
			}
		} else if (Item1 == Combos.Set)
			return new Combo (Combos.FullHouse, Item2 * 100 + comboRank);
		else if (Item1 == Combos.FullHouse && comboRank > Item2 % 100)
			return new Combo (Combos.FullHouse, Item2 - Item2 % 100 + comboRank);
		return this;
	}
	
	public Combo ThreeItemCombo(int comboRank)
	{
		if (Item2 == 0)
			return new Combo(Combos.Set,comboRank);
		else if (Item1 == Combos.Pair)
			return new Combo(Combos.FullHouse,comboRank*100 + Item2);
		else if (Item1 == Combos.TwoPair)
			return new Combo(Combos.FullHouse,comboRank*100 + Item2/100);
		else if (Item1 == Combos.Set)
		{
			return new Combo(Combos.FullHouse,
			                 comboRank > Item2 ? comboRank * 100 + Item2 : Item2 * 100 + comboRank);
		}
		return this;
	}

	
	
	public override string ToString ()
	{
		switch (Item1) 
		{
		case Combos.High: return "High " + Item2;
		case Combos.Pair: return "Pair of " + Item2;
		case Combos.TwoPair: return "Two Pair of " + Item2/100 + " and " + Item2%100;
		case Combos.Set: return "Set of " + Item2;
		case Combos.Straight: return "Straight from " + Item2;
		case Combos.Flush : return "Flush";
		case Combos.FullHouse: return "FullHouse of " + Item2/100 + " and " + Item2%100;
		case Combos.Quad: return "Quad of " + Item2;
		case Combos.StraightFlush: return "StraightFlush from " + Item2;
		case Combos.RoyalFlush: return "RoyalFlush";
		default: return "KEKUS 4T0 TUT MOJET BIT";
		}
	}
}
