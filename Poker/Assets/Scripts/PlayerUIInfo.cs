using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUIInfo : MonoBehaviour
{
	public Text PlayerName;
	public Text PlayerMoneyInfo;
	public Text PlayerMoveInfo;
	public PlayerBasicScript playerBasicScript;  //поменять

	public void Update()
	{
		ChangeFontSize (PlayerName);
		ChangeFontSize (PlayerMoneyInfo);
		ChangeFontSize (PlayerMoveInfo);
	}

	public void ChangeFontSize(Text text)
	{
		text.fontSize = (int)this.gameObject.GetComponent<RectTransform> ().rect.height / 5;
	}

	public void SetNewPlayer(PlayerBasicScript playerBasicScript)
	{
		this.playerBasicScript = playerBasicScript;
		PlayerName.text = playerBasicScript.gameObject.name;
		PlayerMoneyInfo.text = playerBasicScript.moveController.playerInfo.Money.ToString ();
		PlayerMoveInfo.text = null;
	}

	public void MadeMove()
	{
		PlayerMoneyInfo.text = playerBasicScript.moveController.playerInfo.Money.ToString ();
		if (playerBasicScript.moveController.playerInfo.Folded)
			PlayerMoveInfo.text = "Fold";
		else if (playerBasicScript.moveController.playerInfo.CallSize == 0 && playerBasicScript.moveController.playerInfo.LastPlayerBet == 0)
			PlayerMoveInfo.text = "Check";
		else if (playerBasicScript.moveController.playerInfo.LastPlayerBet == playerBasicScript.moveController.playerInfo.CallSize)
			PlayerMoveInfo.text = "Call";
		else if (playerBasicScript.moveController.playerInfo.LastPlayerBet > playerBasicScript.moveController.playerInfo.CallSize)
		{
			if (playerBasicScript.moveController.playerInfo.CallSize == 0)
				PlayerMoveInfo.text = "Bet " + playerBasicScript.moveController.playerInfo.LastPlayerBet;
			else
				PlayerMoveInfo.text = "Raised " + (playerBasicScript.moveController.playerInfo.LastPlayerBet - playerBasicScript.moveController.playerInfo.CallSize);
		}
		else
			PlayerMoveInfo.text = "Something wrong"; // exception
		//PlayerMoveInfo.text ????
	}

	public void ChangeTextSize(RectTransform textRect,RectTransform parentRect)
	{
		textRect.sizeDelta = new Vector2 (parentRect.rect.width, parentRect.rect.height); //ИСПРАВИТЬ
	}

	public void PlayerOut()
	{
		PlayerMoveInfo.text = null;
		PlayerMoveInfo.text = "Busted out";
	}
}
