using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveController	
{
	public float Money;

	public Text MoneyText { get; set; }

	public bool Thinking{ get; set; }
	public bool Folded{ get; set; }
	public bool MadeMove{ get; set; }
	
	public float MaxBet{ get; set; }
	public float LastPlayerBet{ get; set; }
	public float PlayerBet{ get; set; }
	public float CallSize { get; private set; }
	public float Divider = 100f;

	public float LastRaise { get; set; }

	public MoveController()
	{
		Money = 0;
		DefaultValues ();
	}
	public void GetMoney(float money)
	{
		Money += money;
		MoneyText.text = Money.ToString();
	}

	public void MakeMove()
	{
		Thinking = true;
		CallSize = MaxBet - PlayerBet;
	}

	public void Bet(float raise)
	{
		raise = (raise - raise % Divider);
		if (raise < LastRaise && raise != 0)
			raise = LastRaise;
		else if (raise != 0)
			LastRaise = raise;
		float prevBetSize = PlayerBet;
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

	public void CheckFold()
	{
		if (PlayerBet == MaxBet)
			Call ();
		else
			Fold ();
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

	public void NextPhase(float lastRaise)
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
