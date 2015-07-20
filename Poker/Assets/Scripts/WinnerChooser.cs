using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinnerChooser 
{
	public List<PlayerScript> ChooseWinner(List<PlayerScript> possibleWinners)
	{
		List<PlayerScript> winners = GetWinnersByHash (possibleWinners);
		if (winners.Count > 1) 
		{
			winners = GetWinnersByCards(winners);
		}
		return winners;
	}

	private List<PlayerScript> GetWinnersByHash(List<PlayerScript> playerScripts)
	{
		List<PlayerScript> possibleWinners = new List<PlayerScript> ();
		var highestHash = 0;
		foreach (var player in playerScripts)
		{
			if (!player.playerMoveController.Folded)
			{
				var psHash = player.handContoller.combo.GetHashCode();
				if (psHash > highestHash)
				{
					possibleWinners = new List<PlayerScript>(){player};
					highestHash = psHash;
				}
				else if (psHash == highestHash)
					possibleWinners.Add (player);
			}
		}
		return possibleWinners;
	}

	private List<PlayerScript> GetWinnersByCards(List<PlayerScript> possibleWinners)
	{
		var winners = new List<PlayerScript> ();
		for (int i =0; i<5; i++) {
			var highRank = 0;
			foreach (var player in possibleWinners) {
				if (player.handContoller.WinningCards [i].Rank > highRank) {
					winners = new List<PlayerScript> (){player};
					highRank = player.handContoller.WinningCards [i].Rank;
				} else if (player.handContoller.WinningCards [i].Rank == highRank)
					winners.Add (player);
			}
			if (winners.Count == 1)
				break;
			else if (i != 4)
				winners = new List<PlayerScript> ();
		}
		return winners;
	}
}