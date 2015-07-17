using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayersFilter
{
	public List<PlayerScript> NotFoldedPlayers(List<PlayerScript> playerScripts)
	{
		List<PlayerScript> notFoldedPlayers = new List<PlayerScript>();
		foreach (var player in playerScripts)
		{
			if (!player.Folded)
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
			if (player.Money > 0)
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
				if (allinPlayers[i].PlayerBet > player.PlayerBet)
					if (i == 0)
						allinPlayers.Insert(0,player);
				else
					allinPlayers.Insert(i+1,player);
			}
		}
		return allinPlayers;
	}

	public List<PlayerScript> Losers(List<PlayerScript> allinPlayers)
	{
		var losers = new List<PlayerScript> ();
		foreach (var allinPlayer in allinPlayers)
		{
			if (allinPlayer.Money == 0)
				losers.Add(allinPlayer);
		}
		return losers;
	}
}
