using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayersFilter
{
	public List<PlayerScript> NotFoldedPlayers(List<PlayerScript> playerScripts)
	{
		List<PlayerScript> notFoldedPlayers = new List<PlayerScript>();
		foreach (var player in playerScripts)
		{
			if (!player.playerMoveController.Folded)
				notFoldedPlayers.Add(player);
		}
		return notFoldedPlayers;
	}

	public List<PlayerScript> ActivePlayers(List<PlayerScript> playerScripts)
	{
		var notFoldedPlayers = NotFoldedPlayers (playerScripts);
		List<PlayerScript> activePlayers = new List<PlayerScript>();
		foreach (var player in notFoldedPlayers)
		{
			if (player.playerMoveController.Money > 0)
				activePlayers.Add(player);
		}
		return activePlayers;
	}

	public List<PlayerScript> AddAllinPlayer(List<PlayerScript> allinPlayers,PlayerScript player)
	{
		if (allinPlayers.Count == 0)
			allinPlayers.Add (player);
		else
		{
			for (int i=0;i<allinPlayers.Count;i++)
			{
				if (allinPlayers[i].playerMoveController.PlayerBet > player.playerMoveController.PlayerBet)
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

	public List<PlayerScript> HighestBetPlayer (List<PlayerScript> playerScripts)
	{
		int highestBet = playerScripts.Max (z => z.playerMoveController.PlayerBet);
		return playerScripts.Where (z => z.playerMoveController.PlayerBet == highestBet)
			.Take (1)
				.ToList();
	}

	public List<PlayerScript> Losers(List<PlayerScript> allinPlayers)
	{
		var losers = new List<PlayerScript> ();
		foreach (var allinPlayer in allinPlayers)
		{
			if (allinPlayer.playerMoveController.Money == 0)
				losers.Add(allinPlayer);
		}
		return losers;
	}
}
