using UnityEngine;
using System.Collections;

using System.Linq;
using System;

public class CardScript : CardBasicScript
{
	Sprite cardSprite;

	public void Start()
	{
		cardSprite = this.gameObject.GetComponent<SpriteRenderer> ().sprite;
	}

	public override void ShowCard ()
	{
		cardSprite = Resources.Load<Sprite> ("OtherCards/" + ((Card.Rank - 2) + 13 * (int)Card.Suit));
		Resources.UnloadUnusedAssets ();
	}

	public override void HideCard()
	{
		cardSprite = Resources.Load<Sprite> ("OtherCards/" + "flipside");
		Resources.UnloadUnusedAssets ();
	}
}
