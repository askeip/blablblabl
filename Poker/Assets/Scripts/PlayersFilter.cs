using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayersFilter
{
	public List<PlayerBasicScript> NotFoldedPlayers(List<PlayerBasicScript> playerScripts)
	{
		List<PlayerBasicScript> notFoldedPlayers = new List<PlayerBasicScript>();
		foreach (var player in playerScripts)
		{
			if (!player.moveController.Folded)
				notFoldedPlayers.Add(player);
		}
		return notFoldedPlayers;
	}

	public List<PlayerBasicScript> ActivePlayers(List<PlayerBasicScript> playerScripts)
	{
		var notFoldedPlayers = NotFoldedPlayers (playerScripts);
		List<PlayerBasicScript> activePlayers = new List<PlayerBasicScript>();
		foreach (var player in notFoldedPlayers)
		{
			if (player.moveController.Money > 0)
				activePlayers.Add(player);
		}
		return activePlayers;
	}

	public List<PlayerBasicScript> AddAllinPlayer(List<PlayerBasicScript> allinPlayers,PlayerBasicScript player)
	{
		if (allinPlayers.Count == 0)
			allinPlayers.Add (player);
		else
		{
			for (int i=0;i<allinPlayers.Count;i++)
			{
				if (allinPlayers[i].moveController.PlayerBet > player.moveController.PlayerBet)
				{
					allinPlayers.Insert(i,player);
				}
				else if (i == allinPlayers.Count - 1)
					allinPlayers.Add(player);
				else 
					continue;
				break;
			}
		}
		return allinPlayers;
	}

	public List<PlayerBasicScript> HighestBetPlayer (List<PlayerBasicScript> playerScripts)
	{
		float highestBet = playerScripts.Max (z => z.moveController.PlayerBet);
		return playerScripts.Where (z => z.moveController.PlayerBet == highestBet)
			.Take (1)
				.ToList();
	}

	public List<PlayerBasicScript> Losers(List<PlayerBasicScript> allinPlayers)
	{
		var losers = new List<PlayerBasicScript> ();
		foreach (var allinPlayer in allinPlayers)
		{
			if (allinPlayer.moveController.Money == 0)
				losers.Add(allinPlayer);
		}
		return losers;
	}
}
