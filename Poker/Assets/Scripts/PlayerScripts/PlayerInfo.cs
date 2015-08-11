using UnityEngine;
using System.Collections;

public class PlayerInfo : ReadonlyPlayerInfo
{
	public PlayerInfo()
	{
		money = 0;
		DefaultValues ();
	}

	public void AddOrDeductMoney(float money)
	{
		this.money += money;
	}

	public void SetThinking(bool thinking)
	{
		this.thinking = thinking;
	}

	public void SetFolded(bool folded)
	{
		this.folded = folded;
	}

	public void SetMadeMove(bool madeMove)
	{
		this.madeMove = madeMove;
	}
	public void CalculateCallSize(float maxBet)
	{
		callSize = maxBet - PlayerBet;
	}

	public void IncreasePlayerBet(float playerBet)
	{
		lastPlayerBet = playerBet;
		this.playerBet += playerBet;
	}

	public void DefaultValues()
	{
		thinking = false;
		if (money > 0)
		{
			folded = false;
		}else
			folded = true;
		madeMove = false;
		callSize = 0;
		lastPlayerBet = 0;
		playerBet = 0;
	}
}
