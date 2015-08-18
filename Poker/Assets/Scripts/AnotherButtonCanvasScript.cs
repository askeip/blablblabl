using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnotherButtonCanvasScript : MonoBehaviour
{
	public Slider BetSlider;
	public Button BetButton;
	public Button CallButton;
	public Button FoldButton;
	public Transform ParrentCanvas;

	PlayerScript player;

	public float bet { get; set; }

	void Awake()
	{
		transform.SetParent (GameObject.Find ("TextUI").transform, false);
		BetButton.onClick.AddListener(() => Bet());
		CallButton.onClick.AddListener (() => Call ());
		FoldButton.onClick.AddListener (() => Fold ());
	}

	public void Start()
	{
		Instantiate (BetButton);
		Instantiate (CallButton);
		Instantiate (FoldButton);
		Instantiate (BetSlider);
		BetSlider.wholeNumbers = true;
		if (player.moveController.gameInfo.LastRaise < player.moveController.playerInfo.Money - player.moveController.playerInfo.CallSize)
		{
			BetSlider.minValue = player.moveController.gameInfo.LastRaise;
			BetSlider.maxValue = player.moveController.playerInfo.Money - player.moveController.playerInfo.CallSize;
			BetSlider.value = BetSlider.minValue;
			BetSlider.interactable = true;
		}
		else
		{
			BetSlider.minValue = 0f;
			BetSlider.maxValue = player.moveController.playerInfo.Money;
			BetSlider.value = BetSlider.maxValue;
			BetSlider.interactable = false;
		}
		bet = BetSlider.value;
	}

	public void Update()
	{
		bet = BetSlider.value;
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
}
