using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class BotBasicScript : PlayerBasicScript
{
	private float timer = 0f;
	private bool finishedMove;
	private bool myMove;
	protected int movesDone;// { get; private set; }
	private float raise;

	protected int bluffChecked;

	protected Action botAction;

	protected System.Random rnd;

	void Start()
	{
		rnd = new System.Random();
		myMove = false;
		finishedMove = true;
		timer = 0f;
		botAction = null;
	}

	public CardBasic highCard { get; private set; }
	
	protected void ChooseHighestHandCard()
	{
		highCard = leftCard.Card.Rank > rightCard.Card.Rank ? leftCard.Card : rightCard.Card;
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
			return !finishedMove;
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
		var orderedRanks = handController.AvailableCards.Select (z => z.Key)
			.OrderByDescending (z => z)
				.ToList ();
		//int highestCardRank = handContoller.GetHighestRank ();
		//int cardsLeft = orderedRanks.Count;
		int firstRowOfCards = 0;
		int secondRowOfCards = 0;
		for (int i=0;i<orderedRanks.Count;)
		{
			firstRowOfCards = secondRowOfCards == 0 ? Combo.CardsInARow(handController.AvailableCards,orderedRanks[i]) : secondRowOfCards;
			if (i + firstRowOfCards < orderedRanks.Count && orderedRanks[i] - firstRowOfCards - 1 == orderedRanks[i+firstRowOfCards])
				secondRowOfCards = Combo.CardsInARow(handController.AvailableCards,orderedRanks[i+firstRowOfCards]);
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
		ChooseHighestHandCard ();
		if (handController.combo.Item1 == Combos.Straight || handController.combo.Item1 == Combos.Flush
		    || handController.combo.Item1 >= Combos.StraightFlush)
		{
			ChooseBotAction();
		}
		else
		{
			int comboHash = handController.combo.GetHashCode ();
			if (leftCard.Card.Rank == (comboHash/100)%100 || leftCard.Card.Rank == comboHash%100 || rightCard.Card.Rank == (comboHash/100)%100 || rightCard.Card.Rank == comboHash%100)
			{
				ChooseBotAction();
			}
			else 
				HighComboDecision();
		}
		timer += Time.deltaTime;
	}

	protected void ChooseBotAction()
	{
		switch (handController.combo.Item1)
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
		if (handController.cardsTaken == 2) {
			int ranksSum = handController.AvailableCards.Sum (z => z.Value [0].Card.Rank);
			if (ranksSum >= 21) {
				//if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 6.5f)
				botAction = Call;
			} else if (highCard.Rank >= 11 && moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 2.5f) {
				botAction = Call;
			} else if (leftCard.Card.Suit == rightCard.Card.Suit && Mathf.Abs (leftCard.Card.Rank - rightCard.Card.Rank) <= 2) {
				botAction = Call;
			} else 
				CheckFold();
		} else if (highCard.Rank >= handController.GetHighestRank () - 1 && moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f)
			botAction = Call;
		else if (HighChanceOfLuck())//	&& moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 4f
		         //|| moveController.playerInfo.PlayerBet >= moveController.playerInfo.Money * 5)
			botAction = Call;
		else
			CheckFold();
	}

	protected bool HighChanceOfLuck()
	{
		return  handController.cardsTaken != 7 && (StraightPossible () || handController.FlushPossible.Item2 >= 4);
	}

	protected virtual void PairComboDecision()
	{
		if (handController.cardsTaken == 2)
		{
			var orderedByMoney = moveController.gameInfo.ReadonlyPlayersInfo.Where(z=>!z.Folded)
				.OrderByDescending(z=>z.MoneyAtStartOfRound)
					.ToList();
			if ((orderedByMoney[0] == moveController.playerInfo && moveController.playerInfo.MoneyAtStartOfRound / 2 > orderedByMoney[1].MoneyAtStartOfRound) ||
			    moveController.gameInfo.MaxBet < moveController.gameInfo.BigBlind * 3.5f) 
				SetBettingAsAction(moveController.gameInfo.BigBlind * 3f);
			else
				botAction = Call;
		}
		else if (handController.cardsTaken >= 5)
		{
			var orderedRanks = handController.AvailableCards.Select (z => z.Key)
				.OrderByDescending (z => z)
					.ToList ();
			if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f || highCard.Rank >= 13
			    || orderedRanks[1 + (highCard.Rank == handController.combo.Item2 ? 0 : 1)] <= handController.combo.Item2)
			{
				if (handController.combo.Item2 >= 9)
					SetBettingAsAction(moveController.gameInfo.BigBlind * 3f);
				else
					botAction = Call;
			}
			else if (highCard.Rank >= 11 && moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 6f)
				botAction = Call;
			else
				CheckFold();
		}
	}

	protected virtual void TwoPairComboDecision()
	{
		if (moveController.CanCheck())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 4f);
		else
			botAction = Call;
	}

	protected virtual void SetComboDecision()
	{
		if (moveController.CanCheck())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
		else
			botAction = Call;
	}

	protected virtual void StraightDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 8f);
		else
			SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
	}

	protected virtual void FlushDecision()
	{
		SetBettingAsAction(moveController.gameInfo.BigBlind * 8f);
	}

	protected virtual void FullHouseDecision()
	{
		SetBettingAsAction(moveController.gameInfo.BigBlind * 8f);
	}

	protected virtual void QuadDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
		else
			SetBettingAsAction(moveController.gameInfo.BigBlind * 8f);
	}

	protected virtual void StraightFlushDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
		else
			SetBettingAsAction(moveController.gameInfo.BigBlind * 10f);
	}

	protected virtual void RoyalFlushDecision()
	{
		if (moveController.CanCheck ())
			SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
		else
			SetBettingAsAction(moveController.gameInfo.BigBlind * 12f);
	}

	protected void CheckFold()
	{
		if (moveController.CanCheck())
			botAction = Check;
		else
			botAction = Fold;
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
		if (moveController.gameInfo.MaxBet == moveController.playerInfo.PlayerBet)
			Check ();
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
		movesDone = -1;
		base.NextPhase ();
	}

	public override void NextRound()
	{
		movesDone = -1;
		base.NextRound ();
	}
}
