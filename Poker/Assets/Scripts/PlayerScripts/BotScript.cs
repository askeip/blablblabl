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
		myMove = true;
		finishedMove = false;
		MakeMoveDecision ();
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
				MakeMoveDecision();
			}
			return true;
		}
		else
		{
			botAction = null;
			HideCards();
			raise = 0;
			myMove = false;
			movesDone++;
			return false;
		}
	}

	private void MakeMoveDecision()
	{
		if (handContoller.combo.Item1 == Combos.Straight || handContoller.combo.Item1 == Combos.Flush
		    || handContoller.combo.Item1 >= Combos.StraightFlush)
		{
			ChooseBotAction();
		}
		else
		{
			int comboHash = handContoller.combo.GetHashCode ();
			if (leftCard.Card.Rank == comboHash/100 || leftCard.Card.Rank == comboHash%100 || rightCard.Card.Rank == comboHash/100 || rightCard.Card.Rank == comboHash%100)
			{
				ChooseBotAction();
			}
			else 
				botAction = CheckFold;
		}
		timer += Time.deltaTime;
	}

	private void ChooseBotAction()
	{
		switch (handContoller.combo.Item1)
		{
		case Combos.High:HighComboDecision();
			break;
		case Combos.Pair:PairComboDecision();
			break;
		case Combos.TwoPair:TwoPairComboDecision();
			break;
		case Combos.Set:SetComboDecision();
			break;
		case Combos.Straight:StraightDecision();
			break;
		case Combos.Flush:FlushDecision();
			break;
		case Combos.FullHouse:FullHouseDecision();
			break;
		case Combos.Quad:QuadDecision();
			break;
		case Combos.StraightFlush:StraightFlushDecision();
			break;
		case Combos.RoyalFlush:RoyalFlushDecision();
			break;
		}
	}

	private void HighComboDecision()
	{
		if (handContoller.cardsTaken == 2)
		{
			int ranksSum = handContoller.AvailableCards.Sum (z=>z.Value[0].Card.Rank);
			if (ranksSum >= 22)
			{
				if (movesDone == 0)
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
				botAction = CheckFold;
		}
		else
			botAction = CheckFold;
	}

	private void PairComboDecision()
	{
		if (handContoller.cardsTaken == 2)
		{
			if (movesDone == 0)
				SetBettingAsAction((moveController.Money - moveController.CallSize) / 4);
			else
				SetBettingAsAction(moveController.MaxBet * 2f);
		}
		else if (handContoller.cardsTaken >= 5)
		{
			if (moveController.CanCheck())
			{
				SetBettingAsAction(moveController.BigBlind * 2.5f);
			}
			else if (MaxBetNotBig(1.5f))
			{
				SetBettingAsAction(moveController.BigBlind * 1.5f);
			}
			else if (MaxBetNotBig(2.2f) || moveController.MaxBet < moveController.PlayerBet + moveController.Money)
			{
				botAction = Call;
			}
			else 
				botAction = Fold;
		}
	}

	private void TwoPairComboDecision()
	{
		SetBettingAsAction (moveController.BigBlind * 3f);
	}

	private void SetComboDecision()
	{
		SetBettingAsAction(moveController.BigBlind * 3.4f);
	}

	private void StraightDecision()
	{
		SetBettingAsAction(moveController.BigBlind * 4.5f);
	}

	private void FlushDecision()
	{
		SetBettingAsAction(moveController.BigBlind * 4.2f);
	}

	private void FullHouseDecision()
	{
		SetBettingAsAction(moveController.BigBlind * 6f);
	}

	private void QuadDecision()
	{
		SetBettingAsAction(moveController.BigBlind * 8f);
	}

	private void StraightFlushDecision()
	{
		SetBettingAsAction(moveController.BigBlind * 10f);
	}

	private void RoyalFlushDecision()
	{
		SetBettingAsAction(moveController.BigBlind * 12f);
	}

	public bool MaxBetNotBig(float multiplier)
	{
		return moveController.MaxBet < moveController.PlayerBet + moveController.BigBlind * multiplier;
	}

	private void CheckFold()
	{
		if (moveController.CanCheck())
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
	public override void NextPhase ()
	{
		movesDone = 0;
		base.NextPhase ();
	}
	public override void NextRound()
	{
		movesDone = 0;
		base.NextRound ();
	}
}
