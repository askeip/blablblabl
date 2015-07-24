using UnityEngine;
using System.Collections;

public class PlayerScript : PlayerBasicScript
{
	public GameObject playerUI;
	ButtonCanvasScript playerUIScript;

	public void Start()
	{
		playerUIScript = Instantiate (playerUI).GetComponent<ButtonCanvasScript> (); 
		playerUIScript.gameObject.SetActive (false);
		playerUIScript.SetPlayer (this.playerMoveController);
	}

	public override void MakeMove ()
	{
		playerUIScript.gameObject.SetActive (true);
		playerUIScript.bet = playerMoveController.LastRaise;
		playerMoveController.MakeMove ();
	}

	public override bool PlayerThinking ()
	{
		if (playerMoveController.Thinking)
			return true;
		else
		{
			playerUIScript.gameObject.SetActive(false);
		}
		return false;
	}
}
