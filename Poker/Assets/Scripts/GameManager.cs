using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour 
{
	public GameObject Player;
	List<PlayerScript> playerScripts;
	List<PlayerScript> activePlayers;
	CardDeck cardDeck;

	public GameObject Card;
	public int numOfPlayers;
	Vector3[] positions = new Vector3[]{new Vector3(0,-2,0),new Vector3(-5,0,0),new Vector3(5,0,0)};
	Vector3 deckPosition = new Vector3(0,0,0);

	WinnerChooser winnerChooser;
	PlayersFilter playersFilter;

	public GameObject playerUI;
	ButtonCanvasScript playerUIScript;

	GameObject[] tableCards = new GameObject[5];

	public Text POTText;
	public Text[] PlayerMoneyText;

	float waitingTime = 0f;
	int gamePhase = 0;

	int pot;
	List<PlayerScript> allinPlayers;

	int dealerCheap = -1;
	int lastPlayer;
	int currentPlayer;

	int maxBet;
	int lastRaise;

	int bigBlind;

	void Start()
	{
		winnerChooser = new WinnerChooser ();
		playersFilter = new PlayersFilter ();
		playerUIScript = Instantiate (playerUI).GetComponent<ButtonCanvasScript> (); 
		playerUIScript.gameObject.SetActive (false);
		cardDeck = new CardDeck();
		cardDeck.Shuffle ();
		//timer = 0;
		playerScripts = new List<PlayerScript> ();
		for (int i =0; i<numOfPlayers; i++)
		{
			var timePlayer = (GameObject) Instantiate(Player,positions[i],Quaternion.identity);
			timePlayer.name = "Player" + (i + 1).ToString();
			playerScripts.Add(timePlayer.GetComponent<PlayerScript>());
			playerScripts[i].MoneyText = PlayerMoneyText[i];
			playerScripts[i].GetMoney(2000);
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

	private void DeleteAllinLosers()
	{
		
	}

	private void RoundController()
	{
		if (gamePhase == 0)
		{
			PhaseManager();
		}
		else 
		{
			var player = playerScripts[currentPlayer];
			if (PlayerMadeMove(player))
		    {
				if (player.Money == 0)
					allinPlayers = playersFilter.AddAllinPlayer(allinPlayers,player);
				playerUIScript.gameObject.SetActive(false);
				CheckRaise(player);
				if (player.Folded && currentPlayer == lastPlayer)
					lastPlayer = SetNextPlayer(lastPlayer);
				currentPlayer = SetNextPlayer(currentPlayer);
				CountPOT();
			}
			else if (player.Thinking)
				return;
			else
			{
				player.MakeMove();
				playerUIScript.SetPlayer(player);
			}
		}
		if (PhaseFinished ())
		{
			PhaseManager();
		}
	}

	private void CheckRaise(PlayerScript player)
	{
		if (player.PlayerBet > maxBet)
		{
			lastRaise = player.PlayerBet - maxBet;
			maxBet = player.PlayerBet;
			SetMaxValues();
		}
	}

	private bool BetsDone()
	{
		activePlayers = playersFilter.ActivePlayers (playerScripts);
		return (activePlayers.Count == 1 && activePlayers [0].PlayerBet >= maxBet);
	}

	private void NextPhase()
	{
		gamePhase++;
		foreach (var player in playerScripts)
		{
			if (!player.Folded || player.Money != 0)
			{
				player.MadeMove = false;
			}
			player.Thinking = false;
		}
		activePlayers = playersFilter.ActivePlayers (playerScripts);
	}

	private bool PlayersMadeMove()
	{
		foreach (var player in playerScripts)
		{
			if (!PlayerMadeMove(player))
				return false;
		}
		return true;
	}

	private bool PlayerMadeMove(PlayerScript player)
	{
		return player.Folded || player.Money == 0 || (player.MadeMove && player.PlayerBet >= maxBet);
	}

	private bool PhaseFinished()
	{
		return BetsDone() || PlayersMadeMove ();
	}

	private void SetLastAndCurrentPlayers()
	{
		if (gamePhase == 0)
			lastPlayer = SetNextPlayer (SetNextPlayer (dealerCheap));
		else
			lastPlayer = SetPrevPlayer (dealerCheap + 1);
		currentPlayer = SetNextPlayer (lastPlayer);
	}

	private int SetPrevPlayer(int curPlayerNum)
	{
		curPlayerNum = (curPlayerNum + playerScripts.Count - 1) % playerScripts.Count;
		while (playerScripts[curPlayerNum].Folded || 
		       (playerScripts[curPlayerNum].MadeMove && playerScripts[curPlayerNum].Money == 0))
			curPlayerNum = (curPlayerNum + playerScripts.Count - 1) % playerScripts.Count;
		return curPlayerNum;
	}

	private int SetNextPlayer(int curPlayerNum)
	{
		curPlayerNum = (curPlayerNum + 1) % playerScripts.Count;
		/*тут все плохо*/
		while (playerScripts[curPlayerNum].Folded || 
		       (playerScripts[curPlayerNum].MadeMove && playerScripts[curPlayerNum].Money == 0))	
			curPlayerNum = (curPlayerNum + 1) % playerScripts.Count;
		return curPlayerNum;
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
		SetLastAndCurrentPlayers();
		if (gamePhase == 0)
		{
			PreFlop();
		}
		else if (gamePhase == 1)
		{
			for (int i =0; i<3; i++)
				PutNewCard(i);
			foreach (var playerScript in playerScripts)
				playerScript.CheckSuit ();
		} 
		else if (gamePhase == 4)
		{
			ChooseWinners ();
			WaitFinish();
		} 
		else 
		{
			PutNewCard(gamePhase+1);
			var cardScript = tableCards [gamePhase+1].GetComponent<CardScript> ();
			foreach (var playerScript in playerScripts)
			{
				if (cardScript.Suit == playerScript.FlushPossible.Item1)
				{
					playerScript.FlushPossible.Item2 += 1;
				}
			}
		}	
		if (gamePhase == 3) 
		{
			foreach (var playerScript in playerScripts)
				if (!playerScript.Folded)
					playerScript.ChooseWinningCards();
		}
		foreach (var playerScript in playerScripts)
		{
			playerScript.UpdateCombo();
		}
		NextPhase ();
	}

	private void PreFlop()
	{
		int blindPlayer = SetNextPlayer (dealerCheap);
		BetBlinds (blindPlayer, bigBlind / 2);
		blindPlayer = SetNextPlayer (blindPlayer);
		BetBlinds (blindPlayer, bigBlind);
		CheckRaise (playerScripts [blindPlayer]);
		CountPOT ();
	}

	private void BetBlinds(int blindPlayer,int bet)
	{
		playerScripts [blindPlayer].Bet (bet);
		if (playerScripts[blindPlayer].Money > 0)
			playerScripts [blindPlayer].MadeMove = false;
	}

	private void SetMaxValues()
	{
		foreach (var player in playerScripts)
		{
			player.MaxBet = maxBet;
			player.LastRaise = lastRaise;
		}
	}

	public void GiveCards(PlayerScript playerScript,int i)
	{
		playerScript.GetNewHand(cardDeck.cards[i*2],cardDeck.cards[i*2+1]);
	}

	public void DefaultValues()
	{
		allinPlayers = new List<PlayerScript> ();
		bigBlind = 200;
		maxBet = 0;
		lastRaise = 0;
		waitingTime = 0f;
		dealerCheap = SetNextPlayer (dealerCheap);
	}

	public void NextRound()
	{
		POTText.text = "";
		gamePhase = 0;
		DestroyCards ();
		cardDeck.Shuffle ();
		for (int i =0; i<numOfPlayers; i++)
		{
			playerScripts[i].NextRound ();
			if (!playerScripts[i].Folded)
			{
				GiveCards(playerScripts[i],i);
				playerScripts[i].UpdateCombo();
			}
		}
		DefaultValues ();
	}

	public void ChooseWinners()
	{
		var winners = winnerChooser.ChooseWinner (playerScripts);
		GivePOT (winners);		
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

	private void GivePOT(List<PlayerScript> winners)
	{
		for (int i=0;i<winners.Count;i++)
		{
			winners[i].GetMoney(pot/winners.Count);
		}
	}

	private void CountPOT()
	{
		pot = 0;
		foreach (var player in playerScripts)
		{
			pot+=player.PlayerBet;
		}
		POTText.text = pot.ToString ();
	}

	private void PutNewCard(int numOfCard)
	{
		PutCardOnTable (numOfCard);
		var cardScript = tableCards [numOfCard].GetComponent<CardScript> ();
		foreach (var playerScript in playerScripts)
			playerScript.AddCard (cardScript);
	}

	public void PutCardOnTable(int i)
	{
		//Destroy (tableCards[i]);
		tableCards[i] = Card;
		tableCards[i].GetComponent<CardScript> ().SetCard (cardDeck.cards[cardDeck.Length-1-i]);
		tableCards [i] = (GameObject)Instantiate (tableCards[i], new Vector3 (this.transform.position.x + i * 3.5f - 7f, this.transform.position.y + 3f,
		                                                          this.transform.position.z), Quaternion.identity);
	}

	public void DestroyCards()
	{
		foreach (var tableCard in tableCards)
			Destroy (tableCard);
	}
}