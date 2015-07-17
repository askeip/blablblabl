using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Suits{Hearts,Diamonds,Clubs,Spikes};

public class PlayerScript : MonoBehaviour
{
	public GameObject Card;
	CardScript leftCard;
	CardScript rightCard;

	public Combo combo;

	public Tuple<Suits,int> FlushPossible;

	public int Money;

	public Dictionary<int,List<CardScript>> Combination = new Dictionary<int, List<CardScript>>(); 
	//public List<CardScript>[] Combination = new List<CardScript>[13];

	public bool Thinking{ get; set; }
	public bool Folded{ get; set; }
	public bool MadeMove{ get; set; }

	public int MaxBet{ get; set; }
	public int LastRaise{ get; set; }
	public int PlayerBet{ get; set; }


	public Text MoneyText { get; set; }

	Vector3 leftCardPosition;
	Vector3 rightCardPosition;

	public List<CardScript> WinningCards;

	void Awake()
	{
		leftCardPosition = new Vector3(transform.position.x - 0.5f,transform.position.y  - 0.5f,transform.position.z);
		rightCardPosition = new Vector3(transform.position.x + 0.5f,transform.position.y  - 0.5f,transform.position.z);

		var timeCard =(GameObject)Instantiate (Card, leftCardPosition, Quaternion.identity);
		leftCard = timeCard.GetComponent<CardScript> ();
		timeCard = (GameObject)Instantiate (Card, rightCardPosition, Quaternion.identity);
		rightCard = timeCard.GetComponent<CardScript> ();
	}

	public void GetNewHand(string newLeftCard,string newRightCard)
	{
		leftCard.SetCard (newLeftCard);
		leftCard.name = this.gameObject.name + "LeftCard";
		rightCard.SetCard (newRightCard);
		rightCard.name = this.gameObject.name + "RightCard";
		AddCard(leftCard);
		AddCard(rightCard);
		leftCard.gameObject.SetActive (true);
		rightCard.gameObject.SetActive (true);
	}

	public void GetMoney(int money)
	{
		Money += money;
		MoneyText.text = Money.ToString();
	}

	/*public void ChooseFiveCards()
	{
		int count = 5;
		if (combo.Item1 == Combos.TwoPair || combo.Item1 == Combos.FullHouse) {
			return;
		}
	}*/

	public void UpdateCombo ()
	{
		combo = new Combo (Combos.High, 0);
		//string combo = "";
		foreach (var e in Combination.Values) {
			//if (e == null)
			//	continue;
			if (combo.Item1 < Combos.Quad)
			{
				if (e.Count == 2)
				{
					combo = combo.TwoItemCombo(e[0].Rank);
				}
				else if (e.Count == 3)
				{
					combo = combo.ThreeItemCombo(e[0].Rank);
				}
			}
			if (e.Count == 4)
			{
				combo = new Combo(Combos.Quad, e[0].Rank);
			}
		}
		if (combo.Item1 < Combos.Straight)
			combo = combo.CheckStraight (Combination);
		if (combo.Item2 == 0)
			combo = new Combo(Combos.High, (leftCard.Rank > rightCard.Rank ? leftCard.Rank : rightCard.Rank));
		if (FlushPossible.Item2 >= 5 && combo.Item1 <= Combos.Straight) 
		{
			combo = combo.CheckStraightFlush(Combination,FlushPossible);
		}
	}

	public void CheckSuit()
	{
		Dictionary<Suits,int> suitCount = new Dictionary<Suits, int>();
		foreach (var listOfCardScript in Combination.Values)
		{
			//if (listOfCardScript == null)
			//	continue;
			foreach (var cardScript in listOfCardScript)
			{
				if (suitCount.ContainsKey(cardScript.Suit))
					suitCount[cardScript.Suit] += 1;
				else
					suitCount.Add(cardScript.Suit,1);
			}
		}
		foreach (var suit in suitCount) 
		{
			if (suit.Value > FlushPossible.Item2)
				FlushPossible = new Tuple<Suits, int>(suit.Key,suit.Value);
		}
	}

	public void MakeMove()
	{
		Thinking = true;
	}

	public void Bet(int raise)
	{
		int callSize = MaxBet - PlayerBet;
		if (Money >= callSize + raise)
		{
			PlayerBet += callSize + raise;
			Money -= (callSize + raise);
		}
		else
		{
			PlayerBet += Money;
			Money = 0;
		}
		Done ();
	}

	public void Call()
	{
		Bet (0);
	}

	public void Fold()
	{
		Folded = true;
		Done ();
	}

	private void Done()
	{
		MoneyText.text = Money.ToString ();
		Thinking = false;
		MadeMove = true;
	}

	public void NextRound()
	{
		DefaultValues ();
		leftCard.gameObject.SetActive (false);
		rightCard.gameObject.SetActive (false);
		Combination = new Dictionary<int, List<CardScript>>();
		FlushPossible = new Tuple<Suits, int> (Suits.Spikes, 0);
		WinningCards = new List<CardScript> ();
	}

	private void DefaultValues()
	{
		Thinking = false;
		if (Money > 0)
			Folded = false;
		else
			Folded = true;
		MadeMove = false;
		MaxBet = 0;
		LastRaise = 0;
		PlayerBet = 0;
	}

	public void AddCard(CardScript cScript)
	{
		if (Combination.ContainsKey (cScript.Rank))
			Combination [cScript.Rank].Add (cScript);
		else
			Combination.Add (cScript.Rank, new List<CardScript>(){cScript});
	}

	public void ChooseWinningCards()
	{
		List<int> ranksForSkip = new List<int> ();
		if (combo.Item1 == Combos.Straight)
		{
			for (int i=0;i<5;i++)
			{
				WinningCards.Add(Combination[combo.Item2-i][0]);
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
		var orderedCards = Combination.Select (z => z.Key)
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
		for (int i=0;i<Combination[rank].Count;i++)
		{
			WinningCards.Add (Combination[rank][i]);
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