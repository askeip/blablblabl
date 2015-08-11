using UnityEngine;
using System.Collections;

public class ReadonlyPlayerInfo 
{
	protected float money;
	public float Money { get { return money; } }

	protected float callSize;
	public float CallSize { get { return callSize; } }

	protected bool thinking;
	public bool Thinking{ get { return thinking; } }

	protected bool folded;
	public bool Folded{ get { return folded; } }

	protected bool madeMove;
	public bool MadeMove{ get { return madeMove; } }

	protected float lastPlayerBet;
	public float LastPlayerBet{ get { return lastPlayerBet; } }

	protected float playerBet;
	public float PlayerBet{ get { return playerBet; } }
}
