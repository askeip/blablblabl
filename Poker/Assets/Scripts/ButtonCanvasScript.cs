using UnityEngine;
using System.Collections;

public class ButtonCanvasScript : MonoBehaviour 
{
	PlayerScript player;

	public void SetPlayer(PlayerScript player)
	{
		this.player = player;
		this.gameObject.SetActive (true);
	}

	public void Bet(int bet)
	{
		player.Bet (bet);
	}

	public void Call()
	{
		player.Call ();
	}

	public void Fold()
	{
		player.Fold ();
	}
}
