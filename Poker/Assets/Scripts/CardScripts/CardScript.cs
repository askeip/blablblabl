/*using UnityEngine;
using System.Collections;

using System.Linq;
using System;

public class CardScript : CardBasicScript
{
	public override void ShowCard ()
	{
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("OtherCards/" + ((Card.Rank - 2) + 13 * (int)Card.Suit));
		Resources.UnloadUnusedAssets ();
	}

	public override void HideCard()
	{
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("OtherCards/" + "flipside");
		Resources.UnloadUnusedAssets ();
	}
}
*/