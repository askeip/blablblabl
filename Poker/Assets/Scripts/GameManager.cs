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
	//List<PlayerScript> activePlayers;
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

	PotCounter pot;
	List<PlayerScript> allinPlayers;

	int dealerCheap;
	int lastPlayer;
	int currentPlayer;

	int maxBet;

	int bigBlind;

	void Start()
	{
		winnerChooser = new WinnerChooser ();
		playersFilter = new PlayersFilter ();
		pot = new PotCounter ();
		playerUIScript = Instantiate (playerUI).GetComponent<ButtonCanvasScript> (); 
		playerUIScript.gameObject.SetActive (false);
		cardDeck = new CardDeck();
		cardDeck.Shuffle ();
		dealerCheap = numOfPlayers - 1;
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
			var player = playerScripts[currentPlayer];
			if (player.playerMoveController.Thinking)
				return;
			if (PlayerMadeMove(player.playerMoveController))
		    {
				playerUIScript.gameObject.SetActive(false);
				CheckRaise(player);
				if (player.playerMoveController.Money == 0 && player.playerMoveController.MadeMove)
					allinPlayers = playersFilter.AddAllinPlayer(allinPlayers,player);
				if (player.playerMoveController.Folded && currentPlayer == lastPlayer)
					lastPlayer = SetPrevPlayer(lastPlayer);
				currentPlayer = SetNextPlayer(currentPlayer);
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

	private void NextPhase()
	{
		gamePhase++;
		foreach (var player in playerScripts)
		{
			//if (!player.Folded || player.Money != 0)
			//{
				player.playerMoveController.MadeMove = false;
			//}
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
		for (int i=0;i<playerScripts.Count;i++)
		{
			if (playerScripts[curPlayerNum].playerMoveController.Folded || playerScripts[curPlayerNum].playerMoveController.Money == 0)
			       //(playerScripts[curPlayerNum].MadeMove && playerScripts[curPlayerNum].Money == 0))
				curPlayerNum = (curPlayerNum + playerScripts.Count - 1) % playerScripts.Count;
		}
		return curPlayerNum;
	}

	private int SetNextPlayer(int curPlayerNum)
	{
		/*if (activePlayers [curPlayerNum].Money == 0)
		{
			activePlayers = playersFilter.ActivePlayers(activePlayers);
			return curPlayerNum % activePlayers.Count;
		}*/
		curPlayerNum = (curPlayerNum + 1) % playerScripts.Count;
		for (int i=0;i<playerScripts.Count;i++)
		{
			if (playerScripts[curPlayerNum].playerMoveController.Folded || playerScripts[curPlayerNum].playerMoveController.Money == 0)
		       		//(playerScripts[curPlayerNum].MadeMove && playerScripts[curPlayerNum].Money == 0))	
				curPlayerNum = (curPlayerNum + 1) % playerScripts.Count;
		}
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
		if (allinPlayers.Count > 0)
		{
			pot.CountPots(playerScripts,allinPlayers);
			POTText.text = pot.mainPot.ToString() + "     " + pot.lastPot.ToString();
		}
		if (gamePhase == 0)
		{
			PreFlop();
		}
		else if (gamePhase == 1)
		{
			for (int i =0; i<3; i++)
				PutNewCard(i);
			foreach (var playerScript in playerScripts)
				playerScript.handContoller.CheckSuit ();
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
				if (cardScript.Suit == playerScript.handContoller.FlushPossible.Item1)
				{
					playerScript.handContoller.FlushPossible.Item2 += 1;
				}
			}
		}	
		foreach (var playerScript in playerScripts)
		{
			if (!playerScript.playerMoveController.Folded)
				playerScript.handContoller.UpdateCombo();
		}
		if (gamePhase == 3) 
		{
			foreach (var playerScript in playerScripts)
				if (!playerScript.playerMoveController.Folded)
					playerScript.handContoller.ChooseWinningCards();
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
	}

	private void BetBlinds(int blindPlayer,int bet)
	{
		playerScripts [blindPlayer].playerMoveController.Bet (bet);
		//if (playerScripts[blindPlayer].Money > 0)
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

	public void GiveCards(PlayerScript playerScript,int i)
	{
		playerScript.GetNewHand(cardDeck.cards[i*2],cardDeck.cards[i*2+1]);
	}

	public void DefaultValues()
	{
		pot = new PotCounter ();
		allinPlayers = new List<PlayerScript> ();
		bigBlind = 200;
		maxBet = 0;
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
			if (!playerScripts[i].playerMoveController.Folded)
			{
				GiveCards(playerScripts[i],i);
				playerScripts[i].handContoller.UpdateCombo();
			}
		}
		//activePlayers = playersFilter.ActivePlayers (playerScripts);
		DefaultValues ();
	}

	public void ChooseWinners()
	{
		int minBet = 0;
		for (int i=0;i<pot.pots.Count;i++)
		{
			minBet = pot.pots[i].MinBet;
			var winners = winnerChooser.ChooseWinner (playerScripts
                      .Where(z=>!z.playerMoveController.Folded && z.playerMoveController.PlayerBet >= minBet).ToList());
			pot.GivePOT (winners,pot.pots[i].Pot);		
			WinnersText(winners);
		}
		var winners2 = winnerChooser.ChooseWinner (playerScripts
                   .Where(z=>!z.playerMoveController.Folded && z.playerMoveController.PlayerBet >=minBet + 1).ToList());
		pot.GivePOT (winners2,pot.lastPot);
		WinnersText(winners2);
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

	private void PutNewCard(int numOfCard)
	{
		PutCardOnTable (numOfCard);
		var cardScript = tableCards [numOfCard].GetComponent<CardScript> ();
		var notFoldedPlayers = playersFilter.NotFoldedPlayers (playerScripts);
		foreach (var playerScript in notFoldedPlayers)
			playerScript.handContoller.AddCard (cardScript);
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