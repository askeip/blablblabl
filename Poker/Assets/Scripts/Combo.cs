using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Combos{High=1,Pair,TwoPair,Set,Straight,Flush,FullHouse,Quad,StraightFlush,RoyalFlush};

public class Combo 
{
	public Combos ComboName;

	public int Strength;

	public Combo(Combos combo,int strength)
	{
		ComboName = combo;
		Strength = strength;
	}

	public override int GetHashCode ()
	{
		return (int)ComboName * 10000 + Strength;
	}

	public override bool Equals (object o)
	{
		if (!(o is Combo))
			return false;
		var combo = o as Combo;
		return ComboName == combo.ComboName && Strength == combo.Strength;
	}

	public Combo CheckStraightFlush(Dictionary<int,List<CardBasicScript>> combination,FlushChecker flushChecker)
	{
		var flush = new Combo(Combos.Flush, HighFlushRank(combination,flushChecker));
		if (ComboName!=Combos.Straight)
			return flush;
		for (int i =0; i<5; i++)
		{
			var sameSuit = false;
			foreach (var cardScript in combination[Strength-i])
			{
				if (cardScript.Card.Suit == flushChecker.Suit)
				{
					sameSuit = true;
					break;
				}
			}
			if (!sameSuit)
				return flush;
		}
		if (Strength == 14)
			return new Combo(Combos.RoyalFlush,Strength);
		return new Combo(Combos.StraightFlush,Strength);
	}

	public int HighFlushRank(Dictionary<int,List<CardBasicScript>> combination,FlushChecker flushChecker)
	{
		return combination.OrderByDescending (z => z.Key)
			.Where (z => z.Value.Where (d => d.Card.Suit == flushChecker.Suit) != null)
				.Select (z => z.Value [0].Card.Rank)
				.ElementAt (0);
	}
	
	public Combo CheckStraight(Dictionary<int,List<CardBasicScript>> combination)
	{
		int straightFromRank = 14;
		while (straightFromRank > 4) 
		{
			if (combination.ContainsKey(straightFromRank))
			{
				int cardsInARow = CardsInARow(combination,straightFromRank - 1);
				if (cardsInARow >= 4)
					return new Combo(Combos.Straight,straightFromRank);
			}
			straightFromRank--;
		}
		return this;
	}

	public static int CardsInARow(Dictionary<int,List<CardBasicScript>> combination,int checkFromRank)
	{
		int cardsInARow = 0;
		while (true)
		{
			if (combination.ContainsKey(checkFromRank))
			{
				cardsInARow++;
				checkFromRank--;
			}
			else
			{
				if (checkFromRank == 1 && combination.ContainsKey(14))
					cardsInARow++;
				return cardsInARow;
			}
		}
	}
	
	public Combo TwoItemCombo(int comboRank)
	{
		if (Strength == 0)
			return new Combo(Combos.Pair,comboRank);
		if (ComboName < Combos.Set) {
			if (ComboName == Combos.Pair)
				return new Combo (Combos.TwoPair, (Strength > comboRank ? Strength * 100 + comboRank : comboRank * 100 + Strength));
			else if (ComboName == Combos.TwoPair) {
				if (comboRank > Strength % 100)
					return new Combo (Combos.TwoPair, (Strength / 100 > comboRank ? Strength - Strength % 100 + comboRank : comboRank * 100 + Strength / 100));
			}
		} else if (ComboName == Combos.Set)
			return new Combo (Combos.FullHouse, Strength * 100 + comboRank);
		else if (ComboName == Combos.FullHouse && comboRank > Strength % 100)
			return new Combo (Combos.FullHouse, Strength - Strength % 100 + comboRank);
		return this;
	}
	
	public Combo ThreeItemCombo(int comboRank)
	{
		if (Strength == 0)
			return new Combo(Combos.Set,comboRank);
		else if (ComboName == Combos.Pair)
			return new Combo(Combos.FullHouse,comboRank*100 + Strength);
		else if (ComboName == Combos.TwoPair)
			return new Combo(Combos.FullHouse,comboRank*100 + Strength/100);
		else if (ComboName == Combos.Set)
		{
			return new Combo(Combos.FullHouse,
			                 comboRank > Strength ? comboRank * 100 + Strength : Strength * 100 + comboRank);
		}
		return this;
	}

	
	
	public override string ToString ()
	{
		switch (ComboName) 
		{
		case Combos.High: return "High " + Strength;
		case Combos.Pair: return "Pair of " + Strength;
		case Combos.TwoPair: return "Two Pair of " + Strength/100 + " and " + Strength%100;
		case Combos.Set: return "Set of " + Strength;
		case Combos.Straight: return "Straight from " + Strength;
		case Combos.Flush : return "Flush";
		case Combos.FullHouse: return "FullHouse of " + Strength/100 + " and " + Strength%100;
		case Combos.Quad: return "Quad of " + Strength;
		case Combos.StraightFlush: return "StraightFlush from " + Strength;
		case Combos.RoyalFlush: return "RoyalFlush";
		default: return "KEKUS 4T0 TUT MOJET BIT";
		}
	}
}
