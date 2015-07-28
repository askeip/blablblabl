using UnityEngine;
using System.Collections;

using System.Linq;
using System;

public class CardScript : CardBasicScript
{
	public override void ShowCard (string cardName)
	{
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Cards/" + cardName);
		Resources.UnloadUnusedAssets ();
	}
}
