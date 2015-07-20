using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Suits{Hearts,Diamonds,Clubs,Spikes};

public class PlayerScript : MonoBehaviour
{
	public GameObject Card;
	CardScript leftCard;
	CardScript rightCard;

	public PlayerMoveController playerMoveController;

	public HandController handContoller; 
	//public List<CardScript>[] Combination = new List<CardScript>[13];

	Vector3 leftCardPosition;
	Vector3 rightCardPosition;


	void Awake()
	{
		handContoller = new HandController ();
		playerMoveController = new PlayerMoveController ();
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

	public void MakeMove()
	{
		playerMoveController.Thinking = true;
	}

	public void NextRound()
	{
		playerMoveController.DefaultValues ();
		leftCard.gameObject.SetActive (false);
		rightCard.gameObject.SetActive (false);
		handContoller = new HandController ();
	}
}