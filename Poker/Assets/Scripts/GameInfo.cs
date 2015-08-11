using UnityEngine;
using System.Collections;

public class GameInfo :PlayersGameInfo
{
	public GameInfo(float blindDifference, float divider,int numOfPlayers)
	{
		this.blindDifference = blindDifference;
		bigBlind = blindDifference;
		roundsPlayed = -1;
		this.divider = divider;
		this.numOfPlayers = numOfPlayers;
		NextRound ();
	}

	public void SetMaxBet(float maxBet)
	{
		this.maxBet = maxBet;
	}
	/*public void SetNumOfPlayers(int numOfPlayers)
	{
		this.numOfPlayers = numOfPlayers;
	}

	public void SetDivider(float divider)
	{
		this.divider = divider;
	}*/

	public void NextGamePhase()
	{
		gamePhase++;
	}

	/*public void SetBigBlind(float bigBlind)
	{
		this.bigBlind = bigBlind;
	}*/
	public void NextBigBlind ()
	{
		bigBlind += blindDifference;
	}

	public void NextRound()
	{
		maxBet = 0;
		gamePhase = 0;
		roundsPlayed++;
	}

	/*public void SetRoundsPlayed(int roundsPlayed)
	{
		this.roundsPlayed = roundsPlayed;
	}*/

	/*public void SetBlindDifference(float blindDifference)
	{
		this.blindDifference = blindDifference;
	}*/
}
