using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderController
{
	public int dealerCheap;
	public int lastPlayer;
	public int currentPlayer;
	public int blindPlayer;

	public OrderController(int numOfPlayers)
	{
		dealerCheap = numOfPlayers - 1;
		currentPlayer = 0;
		lastPlayer = 0;
		blindPlayer = 0;
	}

	public bool CurrentIsLast()
	{
		return lastPlayer == currentPlayer;
	}

	public void SetLastPlayer(List<PlayerBasicScript> playerScripts)
	{
		lastPlayer = SetPrevPlayer(playerScripts,lastPlayer);
	}

	public void SetCurrentPlayer(List<PlayerBasicScript> playerScripts)
	{
		currentPlayer = SetNextPlayer (playerScripts, currentPlayer);
	}

	public void SetLastAndCurrentPlayers(List<PlayerBasicScript> playerScripts,int gamePhase)
	{
		if (gamePhase == 0)
		{
			lastPlayer = SetNextPlayer (playerScripts,SetNextPlayer (playerScripts, dealerCheap));
		}
		else
			lastPlayer = SetPrevPlayer (playerScripts,dealerCheap + 1);
		currentPlayer = SetNextPlayer (playerScripts,lastPlayer);
	}
	
	public int SetPrevPlayer(List<PlayerBasicScript> playerScripts, int curPlayerNum)
	{
		return SetPlayer (playerScripts, curPlayerNum, playerScripts.Count - 1);
	}
	
	public int SetNextPlayer(List<PlayerBasicScript> playerScripts,int curPlayerNum)
	{
		return SetPlayer (playerScripts, curPlayerNum, 1);
	}

	private int SetPlayer(List<PlayerBasicScript> playerScripts, int curPlayerNum, int sum)
	{
		curPlayerNum = (curPlayerNum + sum) % playerScripts.Count;
		for (int i=0;i<playerScripts.Count;i++)
		{
			if (playerScripts[curPlayerNum].playerMoveController.Folded || playerScripts[curPlayerNum].playerMoveController.Money == 0)
				curPlayerNum = (curPlayerNum + sum) % playerScripts.Count;
		}
		return curPlayerNum;
	}

	public void DefaultValues(List<PlayerBasicScript> playerScripts)
	{
		dealerCheap = SetNextPlayer (playerScripts,dealerCheap);
	}
}
