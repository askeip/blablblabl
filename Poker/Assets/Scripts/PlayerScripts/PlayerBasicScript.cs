using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerBasicScript : MonoBehaviour
{
	public GameObject Card;
	public CardBasicScript leftCard { get; private set; }
	public CardBasicScript rightCard { get; private set; }
	public float potSize;

	public MoveController moveController;

	public HandController handController; 
	//public List<CardScript>[] Combination = new List<CardScript>[13];

	Vector3 leftCardPosition;
	Vector3 rightCardPosition;


	public void Awake()
	{
		GeneralStart ();
	}

	public void GeneralStart()
	{
		handController = new HandController ();
		moveController = new MoveController ();
		leftCardPosition = new Vector3(transform.position.x - 0.5f,transform.position.y  - 0.5f,transform.position.z);
		rightCardPosition = new Vector3(transform.position.x + 0.5f,transform.position.y  - 0.5f,transform.position.z);
		
		var timeCard =(GameObject)Instantiate (Card, leftCardPosition, Quaternion.identity);
		leftCard = timeCard.GetComponent<CardBasicScript> ();
		timeCard = (GameObject)Instantiate (Card, rightCardPosition, Quaternion.identity);
		rightCard = timeCard.GetComponent<CardBasicScript> ();
	}

	public void HideCards()
	{
		leftCard.HideCard ();
		rightCard.HideCard ();
	}

	public void ShowCards()
	{
		leftCard.ShowCard ();
		rightCard.ShowCard ();
	}

	public void GetNewHand(CardBasic newLeftCard,CardBasic newRightCard)
	{
		SetNewCard (leftCard, newLeftCard, "LeftCard");
		SetNewCard (rightCard, newRightCard, "RightCard");
	}

	private void SetNewCard(CardBasicScript card,CardBasic newCard,string cardName)
	{
		card.SetCard (newCard);
		card.name = this.gameObject.name + cardName;
		handController.AddCard (card);
		card.gameObject.SetActive (true);
		card.HideCard ();
	}

	public virtual void Bet(float raise)
	{
		moveController.Bet (raise);
	}

	public virtual void Call()
	{
		moveController.Call ();
	}

	public virtual void Fold()
	{
		moveController.Fold ();
	}

	public virtual void NextPhase()
	{
		moveController.NextPhase ();
	}

	public virtual void MakeMove()
	{
		moveController.playerInfo.SetThinking(true);
	}

	public virtual bool PlayerThinking()
	{
		return true;
	}

	public virtual void NextRound()
	{
		moveController.playerInfo.DefaultValues ();
		potSize = 0;
		handController.cardsTaken = 0;
		leftCard.gameObject.SetActive (false);
		rightCard.gameObject.SetActive (false);
		handController = new HandController ();
	}
}