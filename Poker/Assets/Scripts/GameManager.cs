﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour 
{
	public GameObject Player;
	List<PlayerScript> playerScripts;
	//List<PlayerScript> activePlayers;
	CardDistributor cardDistributor;

	public GameObject Card;
	public int numOfPlayers;
	Vector3[] positions = new Vector3[]{new Vector3(0,-2,0),new Vector3(-5,0,0),new Vector3(5,0,0)};
	Vector3 deckPosition = new Vector3(0,0,0);

	WinnerChooser winnerChooser;
	PlayersFilter playersFilter;
	OrderController orderController;

	public GameObject playerUI;
	ButtonCanvasScript playerUIScript;

	public Text POTText;
	public Text[] PlayerMoneyText;

	float waitingTime = 0f;
	int gamePhase = 0;

	PotCounter pot;
	List<PlayerScript> allinPlayers;

	int maxBet;

	int bigBlind;

	void Start()
	{
		cardDistributor = new CardDistributor ();
		winnerChooser = new WinnerChooser ();
		playersFilter = new PlayersFilter ();
		orderController = new OrderController (numOfPlayers);
		pot = new PotCounter ();
		playerUIScript = Instantiate (playerUI).GetComponent<ButtonCanvasScript> (); 
		playerUIScript.gameObject.SetActive (false);
		//timer = 0;
		playerScripts = new List<PlayerScript> ();
		for (int i =0; i<numOfPlayers; i++)
		{
			var timePlayer = (GameObject) Instantiate(Player,positions[i],Quaternion.identity);
			timePlayer.name = "Player" + (i + 1).ToString();
			playerScripts.Add(timePlayer.GetComponent<PlayerScript>());
			playerScripts[i].playerMoveController.MoneyText = PlayerMoneyText[i];
			playerScripts[i].playerMoveController.GetMoney(2000);
		}
		NextRound ();
	}

	public void Update()
	{
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

	private void CheckRaise(PlayerScript player)
	{
		if (player.playerMoveController.PlayerBet > maxBet)
		{
			maxBet = player.playerMoveController.PlayerBet;
			SetMaxValues();
		}
	}

	private bool BetsDone()
	{
		var activePlayers = playersFilter.ActivePlayers (playerScripts);
		return (activePlayers.Count == 0 || activePlayers.Count == 1 && activePlayers [0].playerMoveController.PlayerBet >= maxBet);
	}

	private void CheckPlayersCondition()
	{
		var player = playerScripts[orderController.currentPlayer];
		if (player.playerMoveController.Thinking)
			return;
		if (PlayerMadeMove(player.playerMoveController))
		{
			playerUIScript.gameObject.SetActive(false);
			CheckRaise(player);
			if (player.playerMoveController.Money == 0 && !player.playerMoveController.Folded && player.playerMoveController.MadeMove)
				allinPlayers = playersFilter.AddAllinPlayer(allinPlayers,player);
			if (player.playerMoveController.Folded && orderController.CurrentIsLast())
				orderController.SetLastPlayer(playerScripts);
			orderController.SetCurrentPlayer(playerScripts);
			if (!player.playerMoveController.Folded && player.playerMoveController.MadeMove)
				pot.CountPot(player.playerMoveController.LastBet);
			POTText.text = pot.mainPot.ToString() + "     " + pot.lastPot.ToString();
		} 
		else
		{
			player.MakeMove();
			playerUIScript.SetPlayer(player);
		}
	}

	private void NextPhase()
	{
		gamePhase++;
		foreach (var player in playerScripts)
		{
			player.playerMoveController.MadeMove = false;
			player.playerMoveController.Thinking = false;
		}
		allinPlayers = new List<PlayerScript> ();
		//activePlayers = playersFilter.ActivePlayers (activePlayers);
	}

	private bool PlayersMadeMove()
	{
		foreach (var player in playerScripts)
		{
			if (!PlayerMadeMove(player.playerMoveController))
				return false;
		}
		return true;
	}

	private bool PlayerMadeMove(PlayerMoveController player)
	{
		return player.Folded || player.Money == 0 || (player.MadeMove && player.PlayerBet >= maxBet);
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
				if (!playerScript.playerMoveController.Folded)
					playerScript.handContoller.ChooseWinningCards();
			}
			break;
		case 4:ChooseWinners();
			WaitFinish();
			break;
		}
		foreach (var playerScript in playerScripts)
		{
			if (!playerScript.playerMoveController.Folded)
				playerScript.handContoller.UpdateCombo();
		}
		NextPhase ();
	}

	private void PreFlop()
	{
		int blindPlayer = orderController.SetNextPlayer (playerScripts,orderController.dealerCheap);
		BetBlinds (blindPlayer, bigBlind / 2);
		blindPlayer = orderController.SetNextPlayer (playerScripts,blindPlayer);
		BetBlinds (blindPlayer, bigBlind);
		CheckRaise (playerScripts [blindPlayer]);
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
		var cardScript = cardDistributor.TableCards [gamePhase+1].GetComponent<CardScript> ();
		foreach (var playerScript in playerScripts)
		{
			if (cardScript.Suit == playerScript.handContoller.FlushPossible.Item1)
			{
				playerScript.handContoller.FlushPossible.Item2 += 1;
			}
		}
	}

	private void BetBlinds(int blindPlayer,int bet)
	{
		playerScripts [blindPlayer].playerMoveController.Bet (bet);
		playerScripts [blindPlayer].playerMoveController.MadeMove = false;
		pot.CountPot (playerScripts [blindPlayer].playerMoveController.LastBet);
		POTText.text = pot.mainPot.ToString() + "     " + pot.lastPot.ToString();
	}

	private void SetMaxValues()
	{
		foreach (var player in playerScripts)
		{
			player.playerMoveController.MaxBet = maxBet;
		}
	}

	public void DefaultValues()
	{
		pot = new PotCounter ();
		allinPlayers = new List<PlayerScript> ();
		bigBlind = 200;
		maxBet = 0;
		waitingTime = 0f;
		orderController.DefaultValues (playerScripts);
	}

	public void NextRound()
	{
		POTText.text = "";
		gamePhase = 0;
		DestroyCards ();
		cardDistributor.CardDeck.Shuffle ();
		for (int i =0; i<numOfPlayers; i++)
		{
			playerScripts[i].NextRound ();
			if (!playerScripts[i].playerMoveController.Folded)
			{
				cardDistributor.GiveCards(playerScripts[i],i);
				playerScripts[i].handContoller.UpdateCombo();
			}
		}
		DefaultValues ();
	}

	public void ChooseWinners()
	{
		int minBet = 0;
		var notFoldedPlayers = playersFilter.NotFoldedPlayers (playerScripts);
		var activePlayers = playersFilter.ActivePlayers (notFoldedPlayers);
		for (int i=0;i<pot.pots.Count;i++)
		{
			minBet = pot.pots[i].MinBet;
			ChooseWinner(minBet,pot.pots[i].Pot,notFoldedPlayers);
		}
		if (activePlayers.Count != 0)
			ChooseWinner (minBet, pot.lastPot, activePlayers);
		else
			ChooseWinner (minBet, pot.lastPot, playersFilter.HighestBetPlayer(notFoldedPlayers));
	}

	private void ChooseWinner(int minBet,int potSize, List<PlayerScript> possibleWinners)
	{
		var winners = winnerChooser.ChooseWinner (possibleWinners
		      .Where(z=> z.playerMoveController.PlayerBet >= minBet).ToList());
		pot.GivePOT (winners,potSize);		
		WinnersText(winners);
	}

	private void WinnersText(List<PlayerScript> winners)
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
		cardDistributor.TableCards[i] = Card;
		cardDistributor.TableCards[i].GetComponent<CardScript> ().SetCard (cardDistributor.CardDeck.cards[cardDistributor.CardDeck.Length-1-i]);
		cardDistributor.TableCards [i] = (GameObject) Instantiate (cardDistributor.TableCards[i], new Vector3 (this.transform.position.x + i * 3.5f - 7f, this.transform.position.y + 3f,
		                                                                       this.transform.position.z), Quaternion.identity);
		cardDistributor.PutNewCard(playerScripts, i);
	}
	
	public void DestroyCards()
	{
		foreach (var tableCard in cardDistributor.TableCards)
			Destroy (tableCard);
	}
}