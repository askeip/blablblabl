using UnityEngine;
using System.Collections;

public class FlushChecker
{
    public FlushChecker(Suits suit, int amount)
    {
        Suit = suit;
        Amount = amount;
    }

    public Suits Suit{ get; set; }
    public int Amount{ get; set; }
}
