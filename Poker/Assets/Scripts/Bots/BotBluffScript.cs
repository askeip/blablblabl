using UnityEngine;
using System.Collections;
using System.Linq;

public class BotBluffScript : BotBasicScript
{
	public bool BluffRound = false;

	public CardBasic highCard;

	protected void ChooseHighestHandCard()
	{
		highCard = leftCard.Card.Rank > rightCard.Card.Rank ? leftCard.Card : rightCard.Card;
	}

	protected override void HighComboDecision ()
	{
		ChooseHighestHandCard ();
		if (handContoller.cardsTaken == 2)
		{
			int ranksSum = handContoller.AvailableCards.Sum (z=>z.Value[0].Card.Rank);
			if (moveController.gameInfo.MaxBet < moveController.gameInfo.BigBlind * 3.5f) // && Money / 2 > чем у остальных
				SetBettingAsAction(moveController.playerInfo.Money / 8);
			else if (BluffRound)
				SetBettingAsAction(moveController.playerInfo.Money / 6 > moveController.gameInfo.BigBlind * 6f ? moveController.playerInfo.Money / 6 : moveController.gameInfo.BigBlind * 6f);
			else if (highCard.Rank > 8)
			{
				if (moveController.playerInfo.CallSize < moveController.gameInfo.BigBlind * 3f)
				{
					if (moveController.playerInfo.CallSize * 3f >= moveController.playerInfo.Money)
						SetBettingAsAction(moveController.playerInfo.Money);
					else
						botAction = Call;
				}
				else if (moveController.playerInfo.CallSize < moveController.playerInfo.Money / 8)
					botAction = Call;
				else if (highCard.Rank >= 12 && moveController.playerInfo.CallSize <= moveController.gameInfo.BigBlind * 5f)
				{
					if (moveController.playerInfo.CallSize * 3f >= moveController.playerInfo.Money)
						SetBettingAsAction(moveController.playerInfo.Money);
					else
						botAction = Call;
				}
				else
					botAction = Fold;
			}
			else if (leftCard.Card.Suit == rightCard.Card.Suit && Mathf.Abs(leftCard.Card.Rank - rightCard.Card.Rank) <= 2)
			{
				if (moveController.playerInfo.CallSize * 3f >= moveController.playerInfo.Money)
					SetBettingAsAction(moveController.playerInfo.Money);
				else
					botAction = Call;
			}
			else
				botAction = Fold;
		}
		else if (handContoller.cardsTaken >= 5)
		{

		}
	}
}
