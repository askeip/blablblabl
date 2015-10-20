using UnityEngine;
using System.Collections;
using System.Linq;

public class HardBotScript : BotBasicScript
{
	protected override void HighComboDecision ()
	{
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
				if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f && TimeToBet(4))
					SetBettingAsAction(moveController.gameInfo.BigBlind * 6f);
				else
					botAction = Call;
			}
			else if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f)
			{
				if (rnd.Next(10) <= 4)
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
		if (highCard.Rank >= 11)
		{
			if (moveController.playerInfo.CallSize <= moveController.playerInfo.Money / 4)
				SetBettingAsAction (moveController.gameInfo.BigBlind * 4.5f);
			else
				botAction = Call;
		} else if (moveController.gameInfo.MaxBet <= moveController.gameInfo.BigBlind * highCard.Rank)
			botAction = Call;
		else
			botAction = Fold;
	}
	
	private void PreFlopHighCardDecision()
	{
		var orderedByMoney = moveController.gameInfo.ReadonlyPlayersInfo.Where(z=>!z.Folded)
			.OrderByDescending(z=>z.MoneyAtRoundStart)
				.ToList();
		if (orderedByMoney[0] == moveController.playerInfo && moveController.playerInfo.MoneyAtRoundStart / 2 > orderedByMoney[1].MoneyAtRoundStart &&
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
			if (moveController.playerInfo.CallSize * 3f >= moveController.playerInfo.Money  && TimeToBet(5))
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
			if (moveController.gameInfo.MaxBet < moveController.gameInfo.BigBlind * 5f && TimeToBet(5))
				SetBettingAsAction(moveController.gameInfo.BigBlind * 3f);
			else
				botAction = Call;
		}
		else if (highCard.Rank >= 13)
		{
			if (moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 3f && TimeToBet(5))
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
				.OrderByDescending(z=>z.MoneyAtRoundStart)
					.ToList();
			if (orderedByMoney[0] == moveController.playerInfo && moveController.playerInfo.MoneyAtRoundStart / 2 > orderedByMoney[1].MoneyAtRoundStart)
				SetBettingAsAction(moveController.playerInfo.Money / 8);
			else
				SetBettingAsAction(moveController.gameInfo.BigBlind * 6f);
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
				if (rnd.Next(10) > 3)
					SetBettingAsAction(moveController.gameInfo.BigBlind * 5f);
				else
					botAction = Call;
			}
			//else 
			//	botAction = Fold;
		}
	}
}