using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SidePot 
{
	public int Pot { get; set; }
	public int MinBet { get; set; }

	public SidePot(int pot, int minBet)
	{
		Pot = pot;
		MinBet = minBet;
	}
}

public class PotCounter
{
	public int mainPot { get; set; }
	public int lastPot { get; set; }
	public List<SidePot> pots{ get; private set; }

	public PotCounter()
	{
		mainPot = 0;
		lastPot = 0;
		pots = new List<SidePot> ();
	}

	public void CountPots(List<PlayerBasicScript> players,List<PlayerBasicScript> allinPlayers)
	{
		allinPlayers = allinPlayers.OrderBy (z => z.playerMoveController.PlayerBet)
			.ToList ();
		foreach (var allinPlayer in allinPlayers)
		{ 
			AddPot(players, allinPlayer.playerMoveController.PlayerBet);
		}
	}

	public void AddPot(List<PlayerBasicScript> players,int allinPlayerBet)
	{
		int prevBet = 0;
		if (pots.Count > 0)
			prevBet = pots [pots.Count - 1].MinBet;
		if (prevBet == allinPlayerBet)
			return;
		var folded = players.Where(z=>z.playerMoveController.PlayerBet > prevBet && z.playerMoveController.PlayerBet < allinPlayerBet)
			.ToList();
		var foldedMoney = folded.Sum(z=>z.playerMoveController.PlayerBet) - folded.Count * prevBet;
		players = players.Where(z=>z.playerMoveController.PlayerBet >= allinPlayerBet)
			.ToList();
		int pot = players.Count * allinPlayerBet + foldedMoney - prevBet * players.Count;
		pots.Add (new SidePot (pot, allinPlayerBet));
		RecountLastPot ();
	}
	
	public void RecountLastPot()
	{
		lastPot -= pots [pots.Count - 1].Pot;
	}

	public void CountPot(int lastBet)
	{
		mainPot += lastBet;
		lastPot += lastBet;
	}

	public void GivePOT(List<PlayerBasicScript> winners,int pot)
	{
		for (int i=0;i<winners.Count;i++)
		{
			winners[i].playerMoveController.GetMoney(pot/winners.Count);
		}
	}
}
