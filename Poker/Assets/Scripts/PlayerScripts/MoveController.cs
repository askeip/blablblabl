using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveController	
{
	public PlayersGameInfo gameInfo;
	public PlayerInfo playerInfo;
	public MoveController()
	{
		playerInfo = new PlayerInfo ();
	}

	public void GetMoney(float money)
	{
		playerInfo.AddOrDeductMoney (money);
	}

	public bool CanCheck()
	{
		return gameInfo.MaxBet == playerInfo.PlayerBet;
	}

	public void MakeMove()
	{
		playerInfo.SetThinking(true);
		playerInfo.CalculateCallSize (gameInfo.MaxBet);		
	}

	public void Bet(float raise)
	{
		raise = (raise - raise % gameInfo.Divider);
		if (raise < gameInfo.LastRaise && raise != 0)
			raise = gameInfo.LastRaise;
		else if (raise != 0)
			gameInfo.LastRaise = raise;
		float prevBetSize = playerInfo.PlayerBet;
		if (playerInfo.Money >= playerInfo.CallSize + raise)
		{
			playerInfo.SetLastPlayerBet(playerInfo.CallSize + raise);
			playerInfo.AddOrDeductMoney(-(playerInfo.CallSize + raise));
		}
		else
		{
			playerInfo.SetLastPlayerBet(playerInfo.Money);
			playerInfo.AddOrDeductMoney(-playerInfo.Money);
		}
		Done ();
	}
	
	public void Call()
	{
		Bet (0);
	}

	public void CheckFold()
	{
		if (playerInfo.PlayerBet == gameInfo.MaxBet)
			Call ();
		else
			Fold ();
	}
	
	public void Fold()
	{
		playerInfo.SetLastPlayerBet (playerInfo.LastPlayerBet);
		playerInfo.SetFolded(true);
		Done ();
	}
	
	private void Done()
	{
		playerInfo.SetThinking(false);
		playerInfo.SetMadeMove(true);
	}

	public void NextPhase()
	{
		playerInfo.SetMadeMove(false);
		playerInfo.SetThinking(false);
	}

	public bool FinishedMove()
	{
		return playerInfo.Folded || playerInfo.Money == 0 || (playerInfo.MadeMove && playerInfo.PlayerBet >= gameInfo.MaxBet);
	}
}
