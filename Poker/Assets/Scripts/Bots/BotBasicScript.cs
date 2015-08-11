using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class BotBasicScript : PlayerBasicScript
{
	private float timer = 0f;
	private bool finishedMove;
	private bool myMove;
	public int movesDone { get; private set; }
	private float raise;

	protected Action botAction;

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

	protected bool StraightPossible()
	{
		var orderedRanks = handContoller.AvailableCards.Select (z => z.Key)
			.OrderByDescending (z => z)
				.ToList ();
		//int highestCardRank = handContoller.GetHighestRank ();
		//int cardsLeft = orderedRanks.Count;
		int firstRowOfCards = 0;
		int secondRowOfCards = 0;
		for (int i=0;i<orderedRanks.Count;)
		{
			firstRowOfCards = secondRowOfCards == 0 ? Combo.CardsInARow(handContoller.AvailableCards,orderedRanks[i]) : secondRowOfCards;
			if (i + firstRowOfCards < orderedRanks.Count && orderedRanks[i] - firstRowOfCards - 1 == orderedRanks[i+firstRowOfCards])
				secondRowOfCards = Combo.CardsInARow(handContoller.AvailableCards,orderedRanks[i+firstRowOfCards]);
			else if (orderedRanks[i] - firstRowOfCards == 2 && orderedRanks[0] == 14)
				secondRowOfCards = 1;
			else
				secondRowOfCards = 0;
			if (firstRowOfCards + secondRowOfCards >= 4)
				return true;
			else
			{
				i+=firstRowOfCards;
			}
		}
		return false;
	}

	public virtual void MakeMoveDecision()
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

	protected void ChooseBotAction()
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

	protected virtual void HighComboDecision()
	{
		if (handContoller.cardsTaken == 2)
		{
			int ranksSum = handContoller.AvailableCards.Sum (z=>z.Value[0].Card.Rank);
			if (ranksSum >= 21)
			{
				if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 6.5f)
					botAction = Call;
			}
			else if (ranksSum >= 17 && moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 2.2f)
			{
				botAction = Call;
			}
			else 
				botAction = CheckFold;
		}
		else if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 2.2f)
			botAction = Call;
		else
			botAction = CheckFold;
	}

	protected virtual void PairComboDecision()
	{
		if (handContoller.cardsTaken == 2)
		{
			if (movesDone == 0)
				SetBettingAsAction(moveController.gameInfo.BigBlind * 2.5f);
			else
				botAction = Call;
		}
		else if (handContoller.cardsTaken >= 5)
		{
			if (moveController.CanCheck())
			{
				SetBettingAsAction(moveController.gameInfo.BigBlind * 2.5f);
			}
			else if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 4f)
			{
				botAction = Call;
			}
			else 
				botAction = Fold;
		}
	}

	protected virtual void TwoPairComboDecision()
	{
		if (moveController.CanCheck())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 3.5f);
		else
			botAction = Call;
	}

	protected virtual void SetComboDecision()
	{
		if (moveController.CanCheck())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 4.5f);
		else
			botAction = Call;
	}

	protected virtual void StraightDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 4.5f);
		else
			SetBettingAsAction (moveController.gameInfo.BigBlind * 3.5f);
	}

	protected virtual void FlushDecision()
	{
		SetBettingAsAction(moveController.gameInfo.BigBlind * 5f);
	}

	protected virtual void FullHouseDecision()
	{
		SetBettingAsAction(moveController.gameInfo.BigBlind * 7f);
	}

	protected virtual void QuadDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 5f);
		else
			SetBettingAsAction(moveController.gameInfo.BigBlind * 8f);
	}

	protected virtual void StraightFlushDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 5f);
		else
			SetBettingAsAction(moveController.gameInfo.BigBlind * 10f);
	}

	protected virtual void RoyalFlushDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 5f);
		else
			SetBettingAsAction(moveController.gameInfo.BigBlind * 12f);
	}

	protected void CheckFold()
	{
		if (moveController.CanCheck())
			Check ();
		else
			Fold ();
	}

	protected void SetBettingAsAction(float raise)
	{
		this.raise = raise;
		botAction = Betting;
	}

	protected void  Betting()
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

	protected void Check ()
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
