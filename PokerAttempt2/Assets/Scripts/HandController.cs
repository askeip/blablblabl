using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HandController
{
    public Dictionary<int, List<CardBasic>> AvailableCards;

    public Combo combo;

    public int cardsTaken { get; set; }

    public FlushChecker FlushCheck;

    public List<CardBasic> WinningCards;

    public HandController()
    {
        combo = new Combo(Combos.High, 0);
        AvailableCards = new Dictionary<int, List<CardBasic>>();
        FlushCheck = new FlushChecker(Suits.Spikes, 0);
        WinningCards = new List<CardBasic>();
    }

    public void UpdateCombo()
    {
        combo = new Combo(Combos.High, 0);
        foreach (var e in AvailableCards.Values)
        {
            if (combo.ComboName < Combos.Quad)
            {
                if (e.Count == 2)
                {
                    combo = combo.TwoItemCombo(e[0].Rank);
                }
                else if (e.Count == 3)
                {
                    combo = combo.ThreeItemCombo(e[0].Rank);
                }
            }
            if (e.Count == 4)
            {
                combo = new Combo(Combos.Quad, e[0].Rank);
            }
        }
        if (combo.ComboName < Combos.Straight)
            combo = combo.CheckStraight(AvailableCards);
        if (combo.Strength == 0)
            combo = new Combo(Combos.High, GetHighestRank());
        if (FlushCheck.Amount >= 5 && combo.ComboName <= Combos.Straight)
        {
            combo = combo.CheckStraightFlush(AvailableCards, FlushCheck);
        }
    }

    public int GetHighestRank()
    {
        return AvailableCards.Max(z => z.Key);
    }

    public void CheckSuit()
    {
        Dictionary<Suits, int> suitCount = new Dictionary<Suits, int>();
        foreach (var listOfCardScript in AvailableCards.Values)
        {
            foreach (var card in listOfCardScript)
            {
                if (suitCount.ContainsKey(card.Suit))
                    suitCount[card.Suit] += 1;
                else
                    suitCount.Add(card.Suit, 1);
            }
        }
        foreach (var suit in suitCount)
        {
            if (suit.Value > FlushCheck.Amount)
                FlushCheck = new FlushChecker(suit.Key, suit.Value);//new Tuple<Suits, int>(suit.Key, suit.Value));
        }
    }

    public void AddCard(CardBasic card)
    {
        cardsTaken++;
        if (AvailableCards.ContainsKey(card.Rank))
            AvailableCards[card.Rank].Add(card);
        else
            AvailableCards.Add(card.Rank, new List<CardBasic>() { card });
    }

    public void ChooseWinningCards()
    {
        List<int> ranksForSkip = new List<int>();
        if (combo.ComboName == Combos.Straight)
        {
            for (int i = 0; i < 5; i++)
            {
                if (combo.Strength - i == 1)
                    WinningCards.Add(AvailableCards[14][0]);
                else
                    WinningCards.Add(AvailableCards[combo.Strength - i][0]);
            }
        }
        else
        {
            if (combo.Strength > 100)
            {
                AddWinningCards(combo.Strength / 100);
                ranksForSkip.Add(combo.Strength / 100);
            }
            AddWinningCards(combo.Strength % 100);
            ranksForSkip.Add(combo.Strength % 100);
        }
        var orderedCards = AvailableCards.Select(z => z.Key)
            .OrderByDescending(z => z)
                .ToList();
        while (WinningCards.Count < 5)
        {
            if (!ranksForSkip.Contains(orderedCards[0]))
                AddWinningCards(orderedCards[0]);
            orderedCards.RemoveAt(0);
        }
    }

    private void AddWinningCards(int rank)
    {
        for (int i = 0; i < AvailableCards[rank].Count; i++)
        {
            WinningCards.Add(AvailableCards[rank][i]);
            if (WinningCards.Count == 5)
                break;
        }
    }
}