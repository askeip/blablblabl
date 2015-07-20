using UnityEngine;
using System.Collections;

public class ButtonCanvasScript : MonoBehaviour 
{
	PlayerMoveController playerMoveController;

	public void SetPlayer(PlayerScript player)
	{
		this.playerMoveController = player.playerMoveController;
		this.gameObject.SetActive (true);
	}

	public void Bet(int bet)
	{
		playerMoveController.Bet (bet);
	}

	public void Call()
	{
		playerMoveController.Call ();
	}

	public void Fold()
	{
		playerMoveController.Fold ();
	}
}
