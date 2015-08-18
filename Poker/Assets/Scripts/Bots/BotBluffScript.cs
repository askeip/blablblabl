using UnityEngine;
using System.Collections;
using System.Linq;

public class BotBluffScript : BotBasicScript
{
	//protected override int rnd {get { return 2; } }
	protected override void HighComboDecision ()
	{
		//ChooseHighestHandCard ();
		if (moveController.playerInfo.LastPlayerBet > moveController.playerInfo.Money 
		    || moveController.playerInfo.PlayerBet / 2 > moveController.playerInfo.Money)
			SetBettingAsAction (moveController.playerInfo.Money);
		else if (handController.cardsTaken == 2)
		{
			PreFlopHighCardDecision();
		}
		else if (handController.cardsTaken >= 5)
		{
			if (handController.combo.Item1 >= Combos.TwoPair)
			{
				HighChanceOfWin();
			}
			if (HighChanceOfLuck())
			{
				if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f || rnd.Next(10) <= 6)
					SetBettingAsAction(moveController.gameInfo.BigBlind * 6f);
				else
					botAction = Call;
			}
			else if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f)
			{
				if (rnd.Next(10) <= 6)
					SetBettingAsAction(moveController.gameInfo.BigBlind * 6f);
				else
					botAction = Call;
			}
			else
				CheckFold();
		}
	}

	private void HighChanceOfWin()
	{
		if (highCard.Rank >= 12)
		{
			if (moveController.gameInfo.MaxBet == moveController.playerInfo.PlayerBet)
				SetBettingAsAction(moveController.gameInfo.BigBlind * 9f);
			else
				SetBettingAsAction(moveController.playerInfo.Money);
		}
		else if (moveController.gameInfo.MaxBet <= moveController.gameInfo.BigBlind * highCard.Rank)
			botAction = Call;
	}

	private void PreFlopHighCardDecision()
	{
		var orderedByMoney = moveController.gameInfo.ReadonlyPlayersInfo.Where(z=>!z.Folded)
			.OrderByDescending(z=>z.MoneyAtStartOfRound)
				.ToList();
		if (orderedByMoney[0] == moveController.playerInfo && moveController.playerInfo.MoneyAtStartOfRound / 2 > orderedByMoney[1].MoneyAtStartOfRound &&
		    moveController.gameInfo.MaxBet < moveController.gameInfo.BigBlind * 3.5f) // && Money / 2 > чем у остальных
			SetBettingAsAction(moveController.playerInfo.Money / 8);
		//else if (BluffRound)
		//	SetBettingAsAction(moveController.playerInfo.Money / 6 > moveController.gameInfo.BigBlind * 6f ? moveController.playerInfo.Money / 6 : moveController.gameInfo.BigBlind * 6f);
		else if (highCard.Rank > 8)
		{
			NotLowCardPreFlopDecision();
		}
		else if (leftCard.Card.Suit == rightCard.Card.Suit && Mathf.Abs(leftCard.Card.Rank - rightCard.Card.Rank) <= 2)
		{
			if (moveController.playerInfo.CallSize * 3f >= moveController.playerInfo.Money)
				SetBettingAsAction(moveController.playerInfo.Money);
			else
				botAction = Call;
		}
		else
			CheckFold();
	}

	private void NotLowCardPreFlopDecision()
	{
		int ranksSum = handController.AvailableCards.Sum (z=>z.Value[0].Card.Rank);
		if (ranksSum >= 23)
		{
			if (moveController.gameInfo.MaxBet < moveController.gameInfo.BigBlind * 5f)
				SetBettingAsAction(moveController.gameInfo.BigBlind * 3f);
			else
				botAction = Call;
		}
		else if (highCard.Rank >= 13)
		{
			if (movesDone == 0 && moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f)
				SetBettingAsAction(moveController.gameInfo.BigBlind * 2.5f);
			else
				botAction = Call;
		}
		else if (moveController.playerInfo.CallSize < moveController.gameInfo.BigBlind * 3f)
		{
			if (moveController.playerInfo.CallSize * 3f >= moveController.playerInfo.Money)
				SetBettingAsAction(moveController.playerInfo.Money);
			else
				botAction = Call;
		}
		else if (moveController.playerInfo.CallSize < moveController.playerInfo.Money / 8)
			botAction = Call;
		else if ((highCard.Rank >= 12 && moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 5f)
		         || (leftCard.Card.Suit == rightCard.Card.Suit && Mathf.Abs(leftCard.Card.Rank - rightCard.Card.Rank) <= 2))
		{
			if (moveController.playerInfo.CallSize * 3f >= moveController.playerInfo.Money)
				SetBettingAsAction(moveController.playerInfo.Money);
			else
				botAction = Call;
		}
		else
			CheckFold();
	}

	protected override void PairComboDecision ()
	{
		if (moveController.playerInfo.LastPlayerBet > moveController.playerInfo.Money 
		    || moveController.playerInfo.PlayerBet / 2 > moveController.playerInfo.Money)
			SetBettingAsAction (moveController.playerInfo.Money);
		if (handController.cardsTaken == 2)
		{
			var orderedByMoney = moveController.gameInfo.ReadonlyPlayersInfo.Where(z=>!z.Folded)
				.OrderByDescending(z=>z.MoneyAtStartOfRound)
					.ToList();
			if (orderedByMoney[0] == moveController.playerInfo && moveController.playerInfo.MoneyAtStartOfRound / 2 > orderedByMoney[1].MoneyAtStartOfRound)
				SetBettingAsAction(moveController.playerInfo.Money / 8);
			else if (movesDone == 0)
				SetBettingAsAction(moveController.gameInfo.BigBlind * 6f);
			else
				botAction = Call;
		}
		else if (handController.cardsTaken >= 5)
		{
			var orderedRanks = handController.AvailableCards.Select (z => z.Key)
				.OrderByDescending (z => z)
					.ToList ();
			if (handController.combo.Item2 >= orderedRanks[1] || handController.combo.Item2 >= 10)
			{
				if (rnd.Next(10) <= 6)
					SetBettingAsAction(moveController.gameInfo.BigBlind * 5f);
				else
					botAction = Call;
			}
			else if (handController.combo.Item2 <= orderedRanks[2 + (highCard.Rank == handController.combo.Item2 ? 0 : 1)] &&
			         moveController.playerInfo.CallSize >= moveController.gameInfo.BigBlind * 5f)
				botAction = Fold;
			else //if (HighChanceOfLuck()	&& moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 4f
			     //    || moveController.playerInfo.PlayerBet >= moveController.playerInfo.Money * 5)
			{
				if (rnd.Next(10) > 2)
					SetBettingAsAction(moveController.gameInfo.BigBlind * 5f);
				else
					botAction = Call;
			}
			//else 
			//	botAction = Fold;
		}
	}

	protected override void TwoPairComboDecision ()
	{
		if (movesDone == 0)
			SetBettingAsAction (moveController.gameInfo.BigBlind * 5f);
		else
			botAction = Call;
	}

	protected override void SetComboDecision ()
	{
		SetBettingAsAction (moveController.gameInfo.BigBlind * 7f);
	}

	protected override void StraightDecision ()
	{
		SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
	}

	protected override void FlushDecision ()
	{
		SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
	}

	protected override void FullHouseDecision ()
	{
		SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
	}

	protected override void QuadDecision ()
	{
		SetBettingAsAction (moveController.gameInfo.BigBlind * 6f);
	}

	protected override void StraightFlushDecision ()
	{
		SetBettingAsAction (moveController.gameInfo.BigBlind * 8f);
	}

	protected override void RoyalFlushDecision ()
	{
		SetBettingAsAction (moveController.gameInfo.BigBlind * 10f);
	}
}
