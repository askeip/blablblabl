using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoveController	
{
	public PlayersGameInfo gameInfo;
	public PlayerInfo playerInfo;
	//public float Money;

	public Text MoneyText { get; set; }

	/*public bool Thinking{ get; set; }
	public bool Folded{ get; set; }
	public bool MadeMove{ get; set; }

	//public float BigBlind { get; set; }
	//public float MaxBet{ get; set; }
	public float LastPlayerBet{ get; set; }
	public float PlayerBet{ get; set; }
	public float CallSize { get; private set; }*/
	//public float Divider = 100f;

	//public float LastRaise { get; set; }

	public MoveController()
	{
		playerInfo = new PlayerInfo ();
	}

	public void GetMoney(float money)
	{
		playerInfo.AddOrDeductMoney (money);
		//Money += money;
		MoneyText.text = playerInfo.Money.ToString();
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
			playerInfo.IncreasePlayerBet(playerInfo.CallSize + raise);
			playerInfo.AddOrDeductMoney(-(playerInfo.CallSize + raise));
		}
		else
		{
			playerInfo.IncreasePlayerBet(playerInfo.Money);
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
		playerInfo.SetFolded(true);
		Done ();
	}
	
	private void Done()
	{
		MoneyText.text = playerInfo.Money.ToString ();
		playerInfo.SetThinking(false);
		playerInfo.SetMadeMove(true);
	}

	public void NextPhase()
	{
		//LastRaise = BigBlind;
		playerInfo.SetMadeMove(false);
		playerInfo.SetThinking(false);
	}

	public bool FinishedMove()
	{
		return playerInfo.Folded || playerInfo.Money == 0 || (playerInfo.MadeMove && playerInfo.PlayerBet >= gameInfo.MaxBet);
	}

	/*public void DefaultValues()
	{
		Thinking = false;
		if (Money > 0)
		{
			Folded = false;
		}else
			Folded = true;
		MadeMove = false;
		CallSize = 0;
		LastPlayerBet = 0;
		PlayerBet = 0;

	}*/
}
