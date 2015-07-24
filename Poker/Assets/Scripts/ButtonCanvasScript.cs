using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonCanvasScript : MonoBehaviour 
{
	//public Slider kekSlider;
	//public Button betButton;
	PlayerMoveController playerMoveController;
	public float bet { get; set; }

	void Start()
	{
		bet = playerMoveController.LastRaise;
		//betButton.OnPointerClick += Bet ();
	}

	public void SetPlayer(PlayerMoveController playerMoveController)
	{
		this.playerMoveController = playerMoveController;
	}

	public void Bet()
	{
		playerMoveController.Bet ((int)bet);
	}

	public void Call()
	{
		playerMoveController.Call ();
	}

	public void Fold()
	{
		playerMoveController.Fold ();
	}

	void OnGUI()
	{
		Rect box = new Rect(Screen.width * 0.8f, Screen.height * 0.2f, Screen.width * 0.2f, Screen.height * 0.2f);
		if (playerMoveController.Money > playerMoveController.LastRaise + playerMoveController.CallSize)
			bet = GUI.HorizontalSlider (box, bet, playerMoveController.LastRaise, playerMoveController.Money - playerMoveController.CallSize);
		else
		{
			GUI.HorizontalSlider (box, 1, 0, 1); 
			bet = playerMoveController.Money;
		}
		GUI.Label (box, bet.ToString ());//(new Rect (box.x, box.y - box.width - 20f, box.width, box.height), bet.ToString ());
	}
}
