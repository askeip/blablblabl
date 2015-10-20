using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayersGameInfo 
{
	protected List<ReadonlyPlayerInfo> readonlyPlayersInfo;
	public List<ReadonlyPlayerInfo> ReadonlyPlayersInfo { get { return readonlyPlayersInfo; } }

	protected float divider;
	public float Divider { get { return divider; }}

    protected bool betsDone;
    public bool BetsDone { get { return betsDone; } }

	//protected int numOfPlayers;
	//public int NumOfPlayers { get { return numOfPlayers; } }

	public float LastRaise { get; set; }

	protected int gamePhase;
	public int GamePhase { get { return gamePhase; } }

	protected float maxBet;
	public float MaxBet{ get { return maxBet; } }

	protected float bigBlind;
	public float BigBlind { get { return bigBlind; } }

	protected int roundsPlayed;
	public int RoundsPlayed { get { return roundsPlayed; } }

	protected float blindDifference;
	public float BlindDifference { get { return blindDifference; } }
}