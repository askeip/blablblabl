using UnityEngine;
using System.Linq;
using System.Collections;

public class BotScript : PlayerBasicScript
{
	private float timer = 0f;
	private bool FinishedMove;

	void Start()
	{
		FinishedMove = false;
		timer = 0f;
	}
	public override void MakeMove ()
	{
		MakeBet ();
	}

	public override bool PlayerThinking ()
	{
		if (!FinishedMove)
		{
			MakeBet();
			return true;
		}
		else
		{
			FinishedMove = false;
			return false;
		}
	}

	private void MakeBet()
	{
		if (timer > 0 && timer < 4)
		{
			timer += Time.deltaTime;
			return;
		}
		else if (timer >= 4)
		{
			timer = 0f;
			FinishedMove = true;
			return;
		}
		if (handContoller.cardsTaken == 2)
		{
			if (handContoller.combo.GetHashCode() > 200)
			{
				playerMoveController.Bet(playerMoveController.Money / 4);
			}
			else if (handContoller.AvailableCards.Sum (z=>z.Value[0].Rank) >= 19)
			{
				playerMoveController.Call();
			}
			else if (playerMoveController.PlayerBet >= playerMoveController.MaxBet / 2)
			{
				playerMoveController.Call();
			}
			else 
			{
				playerMoveController.Fold();
			}
		}
		timer += Time.deltaTime;
	}
}
