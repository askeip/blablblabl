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

	List<SidePot> pots;

	public void CountPots(List<PlayerScript> players,List<PlayerScript> allinPlayers)
	{
		mainPot = CountPot (players);
		allinPlayers = allinPlayers.OrderBy (z => z.PlayerBet)
			.ToList ();
		foreach (var allinPlayer in allinPlayers)
		{ 
			AddPot(players, allinPlayer.PlayerBet);
		}
	}

	public void AddPot(List<PlayerScript> players,int allinPlayerBet)
	{
		int prevBet = 0;
		if (pots.Count > 0)
			prevBet = pots [pots.Count].MinBet;
		if (prevBet == allinPlayerBet)
			return;
		var folded = players.Where(z=>z.PlayerBet > prevBet && z.PlayerBet < allinPlayerBet);
		var foldedMoney = folded.Sum(z=>z.PlayerBet) - folded.Count * prevBet;
		players = players.Where(z=>z.PlayerBet >= allinPlayerBet)
			.ToList();
		int pot = players.Count * allinPlayerBet + foldedMoney - prevBet * players.Count;
		pots.Add (new SidePot (pot, allinPlayerBet));
	}

	public void RecountLastPot(List<PlayerScript> players)
	{
		int allinPlayerBet = pots [pots.Count - 1].MinBet;
		pots.RemoveAt (pots.Count - 1);
		AddPot (players, allinPlayerBet);
	}

	public int CountPot(List<PlayerScript> playerScripts)
	{
		int pot = 0;
		foreach (var player in playerScripts)
		{
			pot+=player.PlayerBet;
		}
		return pot;
	}

	public void CountMainPot(List<PlayerScript> playerScripts)
	{
		mainPot = CountPot (playerScripts);
	}
}
