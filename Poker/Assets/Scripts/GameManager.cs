using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour 
{
	public GameObject Player;
	public GameObject Bot;
	List<PlayerBasicScript> playerScripts;
	//List<PlayerScript> activePlayers;
	CardDistributor cardDistributor;

	public float Divider;

	public GameObject Card;
	public int numOfPlayers;
	Vector3[] positions = new Vector3[]{new Vector3(0,-2,0),new Vector3(-5,0,0),new Vector3(5,0,0)};
	//Vector3 deckPosition = new Vector3(0,0,0);

	public bool gameOver { get; private set; }
	public float LastRaise { get; private set; }

	WinnerChooser winnerChooser;
	PlayersFilter playersFilter;
	OrderController orderController;

	public Text POTText;
	public Text[] PlayerMoneyText;

	float waitingTime = 0f;
	int gamePhase = 0;

	PotCounter pot;
	List<PlayerBasicScript> allinPlayers;

	float maxBet;

	float bigBlind;
	int roundsPlayed;
	float blindDifference;

	void Start()
	{
		blindDifference = 400f;
		bigBlind = blindDifference;
		roundsPlayed = -1;
		Divider = 100f;
		gameOver = false;
		cardDistributor = new CardDistributor ();
		winnerChooser = new WinnerChooser ();
		playersFilter = new PlayersFilter ();
		orderController = new OrderController (numOfPlayers);
		pot = new PotCounter ();
		//timer = 0;
		playerScripts = new List<PlayerBasicScript> ();
		for (int i =0; i<numOfPlayers - 1; i++)
		{
			SeatThePlayer(i,Player);
		}
		SeatThePlayer (numOfPlayers - 1, Bot);
		NextRound ();
	}

	private void SeatThePlayer(int placeNum, GameObject player)
	{
		var timePlayer = (GameObject) Instantiate(player,positions[placeNum],Quaternion.identity);
		timePlayer.name = "Player" + (placeNum + 1).ToString();
		playerScripts.Add(timePlayer.GetComponent<PlayerBasicScript>());
		playerScripts[placeNum].moveController.MoneyText = PlayerMoneyText[placeNum];
		playerScripts[placeNum].moveController.GetMoney(20000f);
		playerScripts [placeNum].moveController.Divider = Divider;
	}

	public void Update()
	{
		if (gameOver)
			return;
		if (waitingTime > 0)
			WaitFinish ();
		else
			RoundController ();
	}

	private void RoundController()
	{
		if (gamePhase == 0)
		{
			PhaseManager();
		}
		else 
		{
			CheckPlayersCondition();
		}
		if (PhaseFinished ())
		{
			PhaseManager();
		}
	}

	private void CheckRaise(PlayerBasicScript player)
	{
		foreach (var playerScript in playerScripts)
			playerScript.potSize = pot.mainPot;
		if (player.moveController.PlayerBet > maxBet)
		{
			maxBet = player.moveController.PlayerBet;
			LastRaise = player.moveController.LastRaise;
			SetMaxValues();
		}
	}

	private bool BetsDone()
	{
		var activePlayers = playersFilter.ActivePlayers (playerScripts);
		return (activePlayers.Count == 0 || activePlayers.Count == 1 && activePlayers [0].moveController.PlayerBet >= maxBet);
	}

	private void CheckPlayersCondition()
	{
		var player = playerScripts[orderController.currentPlayer];
		if (player.PlayerThinking())
			return;
		if (player.moveController.FinishedMove())
		{
			player.HideCards();
			if (!player.moveController.Folded && player.moveController.MadeMove)
				pot.CountPot(player.moveController.LastPlayerBet);
			CheckRaise(player);
			if (player.moveController.Money == 0 && !player.moveController.Folded && player.moveController.MadeMove)
				allinPlayers = playersFilter.AddAllinPlayer(allinPlayers,player);
			if (player.moveController.Folded && orderController.CurrentIsLast())
				orderController.SetLastPlayer(playerScripts);
			orderController.SetCurrentPlayer(playerScripts);
			POTText.text = pot.mainPot.ToString() + "     " + pot.lastPot.ToString();
		} 
		else if (!BetsDone())
		{
			player.MakeMove();
			player.leftCard.ShowCard();
			player.rightCard.ShowCard();
		}
	}

	private void NextPhase()
	{
		LastRaise = bigBlind;
		gamePhase++;
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
		orderController.SetLastAndCurrentPlayers(playerScripts,gamePhase);
		if (allinPlayers.Count > 0)
		{
			pot.CountPots(playerScripts,allinPlayers);
			POTText.text = pot.mainPot.ToString() + "     " + pot.lastPot.ToString();
		}
		InitPhase ();
		foreach (var playerScript in playerScripts)
		{
			if (!playerScript.moveController.Folded)
				playerScript.handContoller.UpdateCombo();
		}
		NextPhase ();
	}

	private void InitPhase()
	{
		switch (gamePhase)
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
				if (!playerScript.moveController.Folded)
					playerScript.handContoller.ChooseWinningCards();
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
		BetBlinds (blindPlayer, bigBlind / 2);
		blindPlayer = orderController.SetNextPlayer (playerScripts,blindPlayer);
		BetBlinds (blindPlayer, bigBlind);
		CheckRaise (playerScripts [blindPlayer]);
	}

	private void SetBigBlind()
	{
		if (roundsPlayed % 5 == 0)
		{
			bigBlind += blindDifference;
			foreach (var player in playerScripts)
				player.moveController.BigBlind = bigBlind;
		}
	}

	private void Flop()
	{
		for (int i =0; i<3; i++)
			PutCardOnTable (i);
		foreach (var playerScript in playerScripts)
			playerScript.handContoller.CheckSuit ();
	}

	private void TurnOrRiver()
	{
		PutCardOnTable(gamePhase+1);
		var cardScript = cardDistributor.TableCards [gamePhase+1].GetComponent<CardBasicScript> ();
		foreach (var playerScript in playerScripts)
		{
			if (cardScript.Card.Suit == playerScript.handContoller.FlushPossible.Item1)
			{
				playerScript.handContoller.FlushPossible.Item2 += 1;
			}
		}
	}

	private void BetBlinds(int blindPlayer,float bet)
	{
		playerScripts [blindPlayer].moveController.Bet (bet);
		playerScripts [blindPlayer].moveController.MadeMove = false;
		pot.CountPot (playerScripts [blindPlayer].moveController.LastPlayerBet);
		POTText.text = pot.mainPot.ToString() + "     " + pot.lastPot.ToString();
	}

	private void SetMaxValues()
	{
		foreach (var player in playerScripts)
		{
			player.moveController.MaxBet = maxBet;
			player.moveController.LastRaise = LastRaise;
		}
	}

	public void DefaultValues()
	{
		POTText.text = "";
		pot = new PotCounter ();
		allinPlayers = new List<PlayerBasicScript> ();
		maxBet = 0;
		gamePhase = 0;
		waitingTime = 0f;
		orderController.DefaultValues (playerScripts);
	}

	public void NextRound()
	{
		DefaultValues ();
		roundsPlayed++;
		SetBigBlind ();
		DestroyCards ();
		cardDistributor.CardDeck.Shuffle ();
		for (int i =0; i<numOfPlayers; i++)
		{
			playerScripts[i].NextRound ();
			if (!playerScripts[i].moveController.Folded)
			{
				cardDistributor.GiveCards(playerScripts[i],i);
				playerScripts[i].handContoller.UpdateCombo();
			}
		}
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
		var activePlayers = playersFilter.ActivePlayers (notFoldedPlayers);
		for (int i=0;i<pot.pots.Count;i++)
		{
			minBet = pot.pots[i].MinBet;
			ChooseWinner(minBet,pot.pots[i].Pot,notFoldedPlayers);
		}
		if (pot.lastPot != 0)
		{
			if (activePlayers.Count != 0)
				ChooseWinner (minBet, pot.lastPot, activePlayers);
			else
				ChooseWinner (minBet, pot.lastPot, playersFilter.HighestBetPlayer(notFoldedPlayers));
		}
	}

	private void ChooseWinner(float minBet,float potSize, List<PlayerBasicScript> possibleWinners)
	{
		var winners = winnerChooser.ChooseWinner (possibleWinners
		      .Where(z=> z.moveController.PlayerBet >= minBet).ToList());
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
		/*cardDistributor.TableCards[i] = Card;
		cardDistributor.TableCards[i].GetComponent<CardBasicScript> ().SetCard (cardDistributor.CardDeck.cards[cardDistributor.CardDeck.Length-1-i],true);
		cardDistributor.TableCards [i] = (GameObject) Instantiate (cardDistributor.TableCards[i], new Vector3 (this.transform.position.x + i * 3.5f - 7f, this.transform.position.y + 3f,
		                                                                       this.transform.position.z), Quaternion.identity);*/
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