using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class BotScript : PlayerBasicScript
{
	private float timer = 0f;
	private bool finishedMove;
	private bool myMove;

	private int raise;

	private Action botAction;

	void Start()
	{
		myMove = false;
		finishedMove = true;
		timer = 0f;
		botAction = null;
	}

	public override void MakeMove ()
	{
		moveController.MakeMove ();
		myMove = true;
		finishedMove = false;
		MakeBet ();
	}

	public override bool PlayerThinking ()
	{
		if (!finishedMove)
		{
			if (botAction != null)
			{
				botAction();
			}
			else if (myMove)
			{
				MakeBet();
			}
			return true;
		}
		else
		{
			botAction = null;
			raise = 0;
			myMove = false;
			return false;
		}
	}

	private void MakeBet()
	{
		if (handContoller.cardsTaken == 2)
		{
			PreFlopDesicion();
		}
		else if(handContoller.cardsTaken == 5)
		{
			return;
		}
		timer += Time.deltaTime;
	}

	private void PreFlopDesicion()
	{
		if (handContoller.combo.GetHashCode() > 200)
		{
			raise = moveController.Money / 4 - moveController.CallSize;
			botAction = Betting;
		}
		else if (handContoller.AvailableCards.Sum (z=>z.Value[0].Rank) >= 19 || moveController.PlayerBet >= moveController.MaxBet / 2)
		{
			botAction = Call;
		}
		else 
		{
			botAction = Fold;
		}
	}

	public void  Betting()
	{
		if (WaitFinished())
		{
			Bet (raise);
			finishedMove = true;
		}
	}

	public override void Bet (int raise)
	{
		base.Bet (raise);
	}

	public override void Call ()
	{
		if (WaitFinished())
		{
			base.Call ();
			finishedMove = true;
		}
	}

	public override void Fold ()
	{
		if (WaitFinished())
		{
			base.Fold ();
			finishedMove = true;
		}
	}

	private bool WaitFinished()
	{
		if (timer > 0 && timer < 4)
		{
			timer += Time.deltaTime;
			return false;
		}
		timer = 0f;
		return true;
	}
}
