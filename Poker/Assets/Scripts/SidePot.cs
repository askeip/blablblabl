using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SidePot 
{
	public float Pot { get; set; }
	public float MinBet { get; set; }

	public SidePot(float pot, float minBet)
	{
		Pot = pot;
		MinBet = minBet;
	}
}

public class PotCounter
{
	public float mainPot { get; set; }
	public float lastPot { get; set; }
	public List<SidePot> pots{ get; private set; }

	public PotCounter()
	{
		mainPot = 0;
		lastPot = 0;
		pots = new List<SidePot> ();
	}

	public void CountPots(List<PlayerBasicScript> players,List<PlayerBasicScript> allinPlayers)
	{
		allinPlayers = allinPlayers.OrderBy (z => z.moveController.playerInfo.PlayerBet)
			.ToList ();
		foreach (var allinPlayer in allinPlayers)
		{ 
			AddPot(players, allinPlayer.moveController.playerInfo.PlayerBet);
		}
	}

	public void AddPot(List<PlayerBasicScript> players,float allinPlayerBet)
	{
		float prevBet = pots.Count > 0 ? pots [pots.Count - 1].MinBet : 0f;
		if (prevBet == allinPlayerBet)
			return;
		var folded = players.Where(z=>z.moveController.playerInfo.PlayerBet > prevBet && z.moveController.playerInfo.PlayerBet < allinPlayerBet)
			.ToList();
		var foldedMoney = folded.Sum(z=>z.moveController.playerInfo.PlayerBet) - folded.Count * prevBet;
		players = players.Where(z=>z.moveController.playerInfo.PlayerBet >= allinPlayerBet)
			.ToList();
		float pot = players.Count * allinPlayerBet + foldedMoney - prevBet * players.Count;
		pots.Add (new SidePot (pot, allinPlayerBet));
		RecountLastPot ();
	}
	
	public void RecountLastPot()
	{
		lastPot -= pots [pots.Count - 1].Pot;
	}

	public void CountPot(float lastBet)
	{
		mainPot += lastBet;
		lastPot += lastBet;
	}

	public void GivePOT(List<PlayerBasicScript> winners,float pot)
	{
		for (int i=0;i<winners.Count;i++)
		{
			winners[i].moveController.GetMoney(pot/winners.Count);
		}
	}
}
