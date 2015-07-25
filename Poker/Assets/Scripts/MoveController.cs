using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveController	
{
	public int Money;

	public Text MoneyText { get; set; }

	public bool Thinking{ get; set; }
	public bool Folded{ get; set; }
	public bool MadeMove{ get; set; }
	
	public int MaxBet{ get; set; }
	public int LastPlayerBet{ get; set; }
	public int PlayerBet{ get; set; }
	public int CallSize { get; private set; }

	public int LastRaise { get; set; }

	public MoveController()
	{
		Money = 0;
		DefaultValues ();
	}
	public void GetMoney(int money)
	{
		Money += money;
		MoneyText.text = Money.ToString();
	}

	public void MakeMove()
	{
		Thinking = true;
		CallSize = MaxBet - PlayerBet;
	}

	public void Bet(int raise)
	{
		if (raise < LastRaise && raise != 0)
			raise = LastRaise;
		else if (raise != 0)
			LastRaise = raise;
		int prevBetSize = PlayerBet;
		if (Money >= CallSize + raise)
		{
			PlayerBet += CallSize + raise;
			Money -= (CallSize + raise);
		}
		else
		{
			PlayerBet += Money;
			Money = 0;
		}
		LastPlayerBet = PlayerBet - prevBetSize;
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

	public void NextPhase(int lastRaise)
	{
		LastRaise = lastRaise;
		MadeMove = false;
		Thinking = false;
	}

	public bool FinishedMove()
	{
		return Folded || Money == 0 || (MadeMove && PlayerBet >= MaxBet);
	}

	public void DefaultValues()
	{
		Thinking = false;
		if (Money > 0)
		{
			Folded = false;
		}else
			Folded = true;
		MadeMove = false;
		MaxBet = 0;
		LastPlayerBet = 0;
		LastRaise = 0;
		PlayerBet = 0;

	}
}
