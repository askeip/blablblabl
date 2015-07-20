using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMoveController	
{
	public int Money;

	public Text MoneyText { get; set; }

	public bool Thinking{ get; set; }
	public bool Folded{ get; set; }
	public bool MadeMove{ get; set; }
	
	public int MaxBet{ get; set; }
	public int LastBet{ get; set; }
	public int PlayerBet{ get; set; }

	public PlayerMoveController()
	{
		Money = 0;
		DefaultValues ();
	}
	public void GetMoney(int money)
	{
		Money += money;
		MoneyText.text = Money.ToString();
	}

	public void Bet(int raise)
	{
		int prevBetSize = PlayerBet;
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
		LastBet = PlayerBet - prevBetSize;
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

	public void DefaultValues()
	{
		Thinking = false;
		if (Money > 0)
			Folded = false;
		else
			Folded = true;
		MadeMove = false;
		MaxBet = 0;
		LastBet = 0;
		PlayerBet = 0;
	}
}
