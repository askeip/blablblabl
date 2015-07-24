using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinnerChooser 
{
	public List<PlayerBasicScript> ChooseWinner(List<PlayerBasicScript> possibleWinners)
	{
		List<PlayerBasicScript> winners = GetWinnersByHash (possibleWinners);
		if (winners.Count > 1) 
		{
			winners = GetWinnersByCards(winners);
		}
		return winners;
	}

	private List<PlayerBasicScript> GetWinnersByHash(List<PlayerBasicScript> playerScripts)
	{
		List<PlayerBasicScript> possibleWinners = new List<PlayerBasicScript> ();
		var highestHash = 0;
		foreach (var player in playerScripts)
		{
			if (!player.playerMoveController.Folded)
			{
				var psHash = player.handContoller.combo.GetHashCode();
				if (psHash > highestHash)
				{
					possibleWinners = new List<PlayerBasicScript>(){player};
					highestHash = psHash;
				}
				else if (psHash == highestHash)
					possibleWinners.Add (player);
			}
		}
		return possibleWinners;
	}

	private List<PlayerBasicScript> GetWinnersByCards(List<PlayerBasicScript> possibleWinners)
	{
		var winners = new List<PlayerBasicScript> ();
		for (int i =0; i<5; i++) {
			var highRank = 0;
			foreach (var player in possibleWinners) {
				if (player.handContoller.WinningCards [i].Rank > highRank) {
					winners = new List<PlayerBasicScript> (){player};
					highRank = player.handContoller.WinningCards [i].Rank;
				} else if (player.handContoller.WinningCards [i].Rank == highRank)
					winners.Add (player);
			}
			if (winners.Count == 1)
				break;
			else if (i != 4)
				winners = new List<PlayerBasicScript> ();
		}
		return winners;
	}
}