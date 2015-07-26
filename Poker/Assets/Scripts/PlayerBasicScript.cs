using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Suits{Hearts,Diamonds,Clubs,Spikes};

public class PlayerBasicScript : MonoBehaviour
{
	public GameObject Card;
	public CardScript leftCard { get; private set; }
	public CardScript rightCard { get; private set; }
	public float potSize;

	public MoveController moveController;

	public HandController handContoller; 
	//public List<CardScript>[] Combination = new List<CardScript>[13];

	Vector3 leftCardPosition;
	Vector3 rightCardPosition;


	void Awake()
	{
		handContoller = new HandController ();
		moveController = new MoveController ();
		leftCardPosition = new Vector3(transform.position.x - 0.5f,transform.position.y  - 0.5f,transform.position.z);
		rightCardPosition = new Vector3(transform.position.x + 0.5f,transform.position.y  - 0.5f,transform.position.z);

		var timeCard =(GameObject)Instantiate (Card, leftCardPosition, Quaternion.identity);
		leftCard = timeCard.GetComponent<CardScript> ();
		timeCard = (GameObject)Instantiate (Card, rightCardPosition, Quaternion.identity);
		rightCard = timeCard.GetComponent<CardScript> ();
	}


	public void GetNewHand(string newLeftCard,string newRightCard)
	{
		SetNewCard (leftCard, newLeftCard, "LeftCard");
		SetNewCard (rightCard, newRightCard, "RightCard");
	}

	private void SetNewCard(CardScript card,string newCard,string cardName)
	{
		card.SetCard (newCard);
		card.name = this.gameObject.name + cardName;
		handContoller.AddCard (card);
		card.gameObject.SetActive (true);
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

	public virtual void MakeMove()
	{
		moveController.Thinking = true;
	}

	public virtual bool PlayerThinking()
	{
		return true;
	}

	public virtual void NextRound()
	{
		moveController.DefaultValues ();
		handContoller.cardsTaken = 0;
		leftCard.gameObject.SetActive (false);
		rightCard.gameObject.SetActive (false);
		handContoller = new HandController ();
	}
}