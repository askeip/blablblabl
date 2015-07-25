using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonCanvasScript : MonoBehaviour 
{
	//public Slider kekSlider;
	//public Button betButton;
	PlayerScript player;
	public float bet { get; set; }

	void Start()
	{
		bet = player.moveController.LastRaise;
		//betButton.OnPointerClick += Bet ();
	}

	public void SetPlayer(PlayerScript player)
	{
		this.player = player;
	}

	public void Bet()
	{
		player.Bet ((int)bet);
	}

	public void Call()
	{
		player.Call ();
	}

	public void Fold()
	{
		player.Fold ();
	}

	void OnGUI()
	{
		Rect box = new Rect(Screen.width * 0.8f, Screen.height * 0.2f, Screen.width * 0.2f, Screen.height * 0.2f);
		if (player.moveController.Money > player.moveController.LastRaise + player.moveController.CallSize)
			bet = GUI.HorizontalSlider (box, bet, player.moveController.LastRaise, player.moveController.Money - player.moveController.CallSize);
		else
		{
			GUI.HorizontalSlider (box, 1, 0, 1); 
			bet = player.moveController.Money;
		}
		GUI.Label (box, bet.ToString ());//(new Rect (box.x, box.y - box.width - 20f, box.width, box.height), bet.ToString ());
	}
}
