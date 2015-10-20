using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour 
{
	//public GameObject Player;
	//public GameObject Bot;
	List<PlayerBasicScript> playerScripts;
	//List<PlayerScript> activePlayers;
	CardDistributor cardDistributor;

	//public float Divider;

	public GameObject Card;
	public GameInfo gameInfo;
	//public int numOfPlayers;
	Vector3[] positions = new Vector3[]{new Vector3(0,-2,0),new Vector3(-5,0,0),new Vector3(5,0,0)};
	//Vector3 deckPosition = new Vector3(0,0,0);

	public bool gameOver { get; private set; }
	//public float LastRaise { get; private set; }

	WinnerChooser winnerChooser;
	PlayersFilter playersFilter;
	OrderController orderController;

	public PlayerUIInfo[] playersUIInfo;
	public Text POTText;
	//public Text[] PlayerMoneyText;

	float waitingTime = 0f;
	//int gamePhase = 0;

	PotCounter pot;
	List<PlayerBasicScript> allinPlayers;

	//float maxBet;
	public List<GameObject> players;
	//float bigBlind;
	//int roundsPlayed;
	//float blindDifference;

	void Start()
	{
		//blindDifference = 400f;
		//bigBlind = blindDifference;
		//roundsPlayed = -1;
		//Divider = 100f;
		gameInfo = new GameInfo (400f,100f);
		gameOver = false;
		cardDistributor = new CardDistributor ();
		winnerChooser = new WinnerChooser ();
		playersFilter = new PlayersFilter ();
		orderController = new OrderController (players.Count);//gameInfo.NumOfPlayers);
		pot = new PotCounter ();
		//timer = 0;
		float money = 20000f;
		playerScripts = new List<PlayerBasicScript> ();
		for (int i =0; i<players.Count;i++)//i<gameInfo.NumOfPlayers - 1; i++) //playersUIInfo.Count должен быть равен players.Count 
		{
			SeatThePlayer(i,players[i],money);//Player,money);
		}
		//SeatThePlayer (gameInfo.NumOfPlayers - 1, Bot,money);
		NextRound ();
	}

	private void SeatThePlayer(int placeNum, GameObject player,float money)
	{
		var timePlayer = (GameObject) Instantiate(player,positions[placeNum],Quaternion.identity);
		timePlayer.name = "Player" + (placeNum + 1).ToString();
		playerScripts.Add(timePlayer.GetComponent<PlayerBasicScript>());
		//playerScripts [placeNum].GeneralStart (gameInfo);
		//playerScripts[placeNum].moveController.MoneyText = PlayerMoneyText[placeNum];
		gameInfo.AddPlayerInfo (playerScripts [placeNum].moveController.playerInfo);
		playerScripts[placeNum].moveController.GetMoney(money);
		playerScripts[placeNum].moveController.gameInfo = gameInfo;
		playersUIInfo [placeNum].SetNewPlayer (playerScripts [placeNum]);
		//playerScripts [placeNum].moveController.Divider = gameInfo.Divider; //ПЕРЕДЕЛАТЬ
	}

	public void Update()
	{
		if (gameOver)
			return;
        //if (gameInfo.BetsDone) dopisat
		if (waitingTime > 0)
			WaitFinish ();
		else
			RoundController ();
	}

	private void RoundController()
	{
		if (gameInfo.GamePhase == 0)// || BetsDone())
		{
			PhaseManager();
		}
		else 
		{
			CheckPlayersCondition();
		}
		if (PhaseFinished())//(BetsDone ())
		{
            gameInfo.SetBetsDone(BetsDone());
			var notFoldedPlayers = playersFilter.NotFoldedPlayers(playerScripts);
			if (notFoldedPlayers.Count == 1)
			{
				ChooseWinner (0, pot.lastPot, notFoldedPlayers);
				ChooseWinners();
				WaitFinish();
			}
			else
				PhaseManager ();
		}
	}

	private void CheckRaise(PlayerBasicScript player)
	{
		foreach (var playerScript in playerScripts)
			playerScript.potSize = pot.mainPot;
		if (player.moveController.playerInfo.PlayerBet > gameInfo.MaxBet)
		{
			gameInfo.SetMaxBet(player.moveController.playerInfo.PlayerBet);
			//gameInfo.LastRaise = player.moveController.LastRaise;
			//SetMaxValues();
		}
	}

	private bool BetsDone()
	{
		var activePlayers = playersFilter.ActivePlayers (playerScripts);
		/*if (activePlayers.Count == 1)
			activePlayers = activePlayers;
		else if (activePlayers.Count == 0)
			activePlayers = activePlayers;*/
		return (activePlayers.Count == 0 || (activePlayers.Count == 1 && activePlayers [0].moveController.playerInfo.PlayerBet >= gameInfo.MaxBet));
	}

	private void CheckPlayersCondition()
	{
		var player = playerScripts[orderController.currentPlayer];
		if (player.PlayerThinking())
			return;
		if (player.moveController.FinishedMove())
		{
			playersUIInfo[orderController.currentPlayer].MadeMove();
			//player.HideCards();
			if (!player.moveController.playerInfo.Folded && player.moveController.playerInfo.MadeMove)
				pot.CountPot(player.moveController.playerInfo.LastPlayerBet);
			CheckRaise(player);
			if (player.moveController.playerInfo.Money == 0 && !player.moveController.playerInfo.Folded && player.moveController.playerInfo.MadeMove)
				allinPlayers = playersFilter.AddAllinPlayer(allinPlayers,player);
			if (player.moveController.playerInfo.Folded && orderController.CurrentIsLast())
				orderController.SetLastPlayer(playerScripts);
			orderController.SetCurrentPlayer(playerScripts);
			POTText.text = pot.mainPot.ToString();// + "     " + pot.lastPot.ToString();
			/*if (PhaseFinished ())
			{
				PhaseManager();
			}*/		
		} 
		else if (!BetsDone())
		{
			player.MakeMove();
			//player.ShowCards();
		}
	}

	private void NextPhase()
	{
		gameInfo.LastRaise = gameInfo.BigBlind;
		gameInfo.NextGamePhase ();
		foreach (var player in playerScripts)
		{
			player.NextPhase();
		}
		allinPlayers = new List<PlayerBasicScript> ();
		//activePlayers = playersFilter.ActivePlayers (activePlayers);
	}

	private bool PlayersMadeMove()
	{
		foreach (var player in playerScripts)
		{
			if (!player.moveController.FinishedMove())
				return false;
		}
		return true;
	}

	private bool PhaseFinished()
	{
		return BetsDone() || PlayersMadeMove ();
	}

	private void WaitFinish()
	{
		waitingTime += Time.deltaTime;
		if (waitingTime > 7)
		{
			NextRound();
		}
	}

	public void PhaseManager()
	{
		orderController.SetLastAndCurrentPlayers(playerScripts,gameInfo.GamePhase);
		if (allinPlayers.Count > 0)
		{
			pot.CountPots(playerScripts,allinPlayers);
			POTText.text = pot.mainPot.ToString();// + "     " + pot.lastPot.ToString();
		}
		InitPhase ();
		foreach (var playerScript in playerScripts)
		{
			if (!playerScript.moveController.playerInfo.Folded)
				playerScript.handController.UpdateCombo();
		}
		NextPhase ();
	}

	private void InitPhase()
	{
		switch (gameInfo.GamePhase)
		{
		case 0:PreFlop();
			break;
		case 1:Flop();
			break;
		case 2:TurnOrRiver();
			break;
		case 3:TurnOrRiver();
			foreach (var playerScript in playerScripts)
			{
				if (!playerScript.moveController.playerInfo.Folded)
					playerScript.handController.ChooseWinningCards();
			}
			break;
		case 4:ChooseWinners();
			WaitFinish();
			break;
		}
	}

	private void PreFlop()
	{
		int blindPlayer = orderController.SetNextPlayer (playerScripts,orderController.dealerCheap);
		BetBlinds (blindPlayer, gameInfo.BigBlind / 2);
		blindPlayer = orderController.SetNextPlayer (playerScripts,blindPlayer);
		BetBlinds (blindPlayer, gameInfo.BigBlind);
		CheckRaise (playerScripts [blindPlayer]);
	}

	private void SetBigBlind()
	{
		if (gameInfo.RoundsPlayed % 5 == 0)
		{
			gameInfo.NextBigBlind();
			//gameInfo.BigBlind += gameInfo.BlindDifference;
			//foreach (var player in playerScripts)
			//	player.moveController.BigBlind = gameInfo.BigBlind;
		}
	}

	private void Flop()
	{
		for (int i =0; i<3; i++)
			PutCardOnTable (i);
		foreach (var playerScript in playerScripts)
			playerScript.handController.CheckSuit ();
	}

	private void TurnOrRiver()
	{
		PutCardOnTable(gameInfo.GamePhase+1);
		var cardScript = cardDistributor.TableCards [gameInfo.GamePhase+1].GetComponent<CardBasicScript> ();
		foreach (var playerScript in playerScripts)
		{
			if (cardScript.Card.Suit == playerScript.handController.FlushPossible.Item1)
			{
				playerScript.handController.FlushPossible.Item2 += 1;
			}
		}
	}

	private void BetBlinds(int blindPlayer,float bet)
	{
		playerScripts [blindPlayer].moveController.Bet (bet);
		playerScripts [blindPlayer].moveController.playerInfo.SetMadeMove(false);
		playersUIInfo [blindPlayer].MadeMove ();
		pot.CountPot (playerScripts [blindPlayer].moveController.playerInfo.LastPlayerBet);
		POTText.text = pot.mainPot.ToString();// + "     " + pot.lastPot.ToString();
	}

	/*private void SetMaxValues()
	{
		foreach (var player in playerScripts)
		{
			player.moveController.MaxBet = gameInfo.MaxBet;
			player.moveController.LastRaise = gameInfo.LastRaise;
		}
	}*/

	public void DefaultValues()
	{
		POTText.text = "";
		pot = new PotCounter ();
		allinPlayers = new List<PlayerBasicScript> ();
		waitingTime = 0f;
	}

	public void NextRound()
	{
		gameInfo.NextRound ();
		DefaultValues ();
		SetBigBlind ();
		DestroyCards ();
		cardDistributor.CardDeck.Shuffle ();
		for (int i =0; i<players.Count;i++)//gameInfo.NumOfPlayers; i++)
		{
			playerScripts[i].NextRound ();
			if (!playerScripts[i].moveController.playerInfo.Folded)
			{
				cardDistributor.GiveCards(playerScripts[i],i);
				if (playerScripts[i] is PlayerScript)
					playerScripts[i].ShowCards();
				playerScripts[i].handController.UpdateCombo();
			}
		}
		orderController.DefaultValues (playerScripts);
		if (playersFilter.ActivePlayers(playerScripts).Count == 1)
			GameOver ();
	}

	public void GameOver()
	{
		gameOver = true;
	}

	public void ChooseWinners()
	{
		float minBet = 0;
		var notFoldedPlayers = playersFilter.NotFoldedPlayers (playerScripts);
		if (notFoldedPlayers.Count != 1)
		{
			foreach (var player in notFoldedPlayers)
				player.ShowCards ();
		}
		var activePlayers = playersFilter.ActivePlayers (notFoldedPlayers);
		for (int i=0;i<pot.pots.Count;i++)
		{
			minBet = pot.pots[i].MinBet;
			ChooseWinner(minBet,pot.pots[i].Pot,notFoldedPlayers);
		}
		if (pot.pots.Count == 0)// && pot.lastPot != 0)
		{
			if (activePlayers.Count > 1)
				ChooseWinner (minBet, pot.lastPot, activePlayers);
			//else
			//	ChooseWinner (minBet, pot.lastPot, playersFilter.HighestBetPlayer(notFoldedPlayers));
		}
	}

	private void ChooseWinner(float minBet,float potSize, List<PlayerBasicScript> possibleWinners)
	{
		var winners = winnerChooser.ChooseWinner (possibleWinners
		                             .Where(z=> z.moveController.playerInfo.PlayerBet >= minBet).ToList());
		pot.GivePOT (winners,potSize);		
		WinnersText(winners);
	}

	private void WinnersText(List<PlayerBasicScript> winners)
	{
		if (winners.Count > 1) {
			StringBuilder winText = new StringBuilder ();
			winText.Append ("TIE BETWEEN ");
			for (int i=0; i<winners.Count; i++) {
				winText.Append (winners [i].name);
				if (i < winners.Count - 1)
					winText.Append (" and ");
			}
			POTText.text = winText.ToString ();
		} else 
			POTText.text = winners [0].name + " is a winner!";		
	}

	public void PutCardOnTable(int i)
	{
		cardDistributor.TableCards [i] = (GameObject) Instantiate (Card, new Vector3 (this.transform.position.x + i * 3.5f - 7f, this.transform.position.y + 3f,
		                                                                                                       this.transform.position.z), Quaternion.identity);
		cardDistributor.TableCards[i].GetComponent<CardBasicScript> ().SetCard (cardDistributor.CardDeck.cards[cardDistributor.CardDeck.Length-1-i],true);
		cardDistributor.PutNewCard(playerScripts, i);
	}
	
	public void DestroyCards()
	{
		foreach (var tableCard in cardDistributor.TableCards)
			Destroy (tableCard);
	}
}