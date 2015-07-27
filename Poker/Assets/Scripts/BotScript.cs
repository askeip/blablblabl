using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class BotScript : PlayerBasicScript
{
	private float timer = 0f;
	private bool finishedMove;
	private bool myMove;
	private int movesDone;
	private float raise;

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
		movesDone++;
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
			FlopDecision();
		}
		timer += Time.deltaTime;
	}

	private void PreFlopDesicion()
	{
		int comboHash = handContoller.combo.GetHashCode ();
		if (comboHash > 20000)
		{
			if (movesDone == 1)
				SetBettingAsAction((moveController.Money - moveController.CallSize) / 4);
			else
				SetBettingAsAction(moveController.Money);
		}
		else if (handContoller.AvailableCards.Sum (z=>z.Value[0].Rank) >= 22)
		{
			if (movesDone == 1)
			{
				SetBettingAsAction((moveController.Money - moveController.CallSize) / 6);
			}
			else
				botAction = Call;
		}
		else if (handContoller.AvailableCards.Sum (z=>z.Value[0].Rank) >= 17 && moveController.PlayerBet + moveController.Money >= moveController.MaxBet * 10)
		{
			botAction = Call;
		}
		else if (moveController.PlayerBet >= moveController.MaxBet / 2.5)
		{
			botAction = Call;
		}
		else 
		{
			botAction = Fold;
		}
	}

	private void FlopDecision()
	{
		int comboHash = handContoller.combo.GetHashCode ();
		if (comboHash > 30000)
		{
			raise = potSize > moveController.PlayerBet * 3 ? moveController.PlayerBet * 2 : potSize;
			botAction = Betting;
		}
		else if (comboHash > 20000)
		{
			if (leftCard.Rank == comboHash % 10000 || rightCard.Rank == comboHash % 10000)
			{
				if (moveController.MaxBet == moveController.PlayerBet)
				{
					SetBettingAsAction(moveController.Divider * 2);
				}
			}
		}
	}

	private void MoveDecision()
	{
		if (handContoller.combo.Item1 == Combos.Straight || handContoller.combo.Item1 == Combos.Flush || handContoller.combo.Item1 == Combos.StraightFlush)
		{
			if (moveController.MaxBet == moveController.PlayerBet)
			{
				SetBettingAsAction(moveController.LastRaise * 5f);				                   
			}
			else 
			{
				SetBettingAsAction(moveController.LastRaise * 1.5f);
			}
		}
		else
		{
			int comboHash = handContoller.combo.GetHashCode ();
			if (leftCard.Rank == comboHash/100 || leftCard.Rank == comboHash%100 || rightCard.Rank == comboHash/100 || rightCard.Rank == comboHash%100)
			{
				ChooseBet();
			}
		}
	}

	private void ChooseBet()
	{
		switch (handContoller.combo.Item1)
		{
		case Combos.High:HighComboDecision();
			break;
		case Combos.Pair:PairComboDecision();
			break;
		}
	}

	private void HighComboDecision()
	{
		if (handContoller.AvailableCards.Count == 2)
		{
			int ranksSum = handContoller.AvailableCards.Sum (z=>z.Value[0].Rank);
			if (ranksSum >= 22)
			{
				if (movesDone == 1)
				{
					SetBettingAsAction((moveController.Money - moveController.CallSize) / 6);
				}
				else
					botAction = Call;
			}
			else if (ranksSum >= 17 && moveController.PlayerBet + moveController.Money >= moveController.MaxBet * 10)
			{
				botAction = Call;
			}
			else 
				CheckFold();
		}
		else
			CheckFold();
	}

	private void PairComboDecision()
	{
		if (movesDone == 1)
			SetBettingAsAction((moveController.Money - moveController.CallSize) / 4);
		else
			SetBettingAsAction(moveController.Money);
	}

	private void CheckFold()
	{
		if (moveController.PlayerBet == moveController.MaxBet)
			Check ();
		else
			Fold ();
	}

	private void SetBettingAsAction(float raise)
	{
		this.raise = raise;
		botAction = Betting;
	}

	public void  Betting()
	{
		if (WaitFinished())
		{
			Bet (raise);
			finishedMove = true;
		}
	}

	public override void Bet (float raise)
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

	public void Check ()
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

	public override void NextRound()
	{
		movesDone = 0;
		base.NextRound ();
	}
}
