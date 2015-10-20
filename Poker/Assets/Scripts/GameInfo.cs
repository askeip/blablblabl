using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInfo :PlayersGameInfo
{
	public void AddPlayerInfo(ReadonlyPlayerInfo readonlyPlayerInfo)
	{
		readonlyPlayersInfo.Add (readonlyPlayerInfo);
	}

	public void InsertPlayerInfo(int index,ReadonlyPlayerInfo readonlyPlayerInfo)
	{
		readonlyPlayersInfo.Insert (index, readonlyPlayerInfo);
	}

	public void ChangePlayerInfoItem(int index,ReadonlyPlayerInfo readonlyPlayerInfo)
	{
		readonlyPlayersInfo [index] = readonlyPlayerInfo;
	}

	public GameInfo(float blindDifference, float divider,float bigBlind = 0f)//,int numOfPlayers)
	{
		readonlyPlayersInfo = new List<ReadonlyPlayerInfo> ();
		this.blindDifference = blindDifference;
		this.bigBlind = bigBlind == 0 ? 2 * blindDifference : bigBlind;
		roundsPlayed = -1;
		this.divider = divider;
		//this.numOfPlayers = numOfPlayers;
		NextRound ();
	}

	public void SetMaxBet(float maxBet)
	{
		this.maxBet = maxBet;
	}

    public void SetBetsDone(bool betsDone)
    {
        this.betsDone = betsDone;
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
		LastRaise = 0;
	    betsDone = false;
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
