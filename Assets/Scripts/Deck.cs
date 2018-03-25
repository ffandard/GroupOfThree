using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;

[CreateAssetMenu(fileName = "Deck")]
public class Deck: ScriptableObject
{
	[SerializeField]
	private CardProperties cardProperties;

	public List<Card> Cards { get; set; }

	public void Create()
	{
		Cards =
			(from shape in cardProperties.shapeSprite
		  from colour in cardProperties.shapeColour
		  from count in cardProperties.shapeCount //IEnumerable<int> Range(1,cardProperties.MaxShapeCount)
			     from fill in cardProperties.fillValue
		  select new Card(shape, colour, count, fill)).ToList();
	}

	public void Shuffle()
	{
		for (int c = 0; c < Cards.Count; c++)
		{
			Card current = Cards[c];
			int oldIndex = Cards.IndexOf(Cards[c]);
			int randomIndex = UnityEngine.Random.Range(oldIndex, Cards.Count);
			Cards[c] = Cards[randomIndex];
			Cards[randomIndex] = current;

		}
	}

	public bool IsEmpty()
	{
		return Cards.Count == 0;
	}

	public int GetCardCount()
	{
		return Cards.Count;
	}

	public Card GetCardByIndex(int index)
	{
		return Cards[index];
	}

	public void RemoveCard(Card card)
	{
		Cards.Remove(card);
	}
}
