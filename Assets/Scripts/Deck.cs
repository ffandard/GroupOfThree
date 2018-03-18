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

	public List<Card> cards ;

	public void Create()
	{
		cards =
			(from shape in cardProperties.shapeSprite
			     from colour in cardProperties.shapeColour
			     from count in cardProperties.shapeCount //IEnumerable<int> Range(1,cardProperties.MaxShapeCount)
			     from fill in cardProperties.fillValue
			     select new Card(shape, colour, count, fill)).ToList();
	}

	public void Shuffle()
	{
		for(int c = 0; c < cards.Count; c++)
		{
			Card current = cards[c];
			int oldIndex = cards.IndexOf(cards[c]);
			int randomIndex = UnityEngine.Random.Range(oldIndex, cards.Count);
			cards[c] = cards[randomIndex];
			cards[randomIndex] = current;

		}
	}

	public bool IsEmpty()
	{
		return cards.Count <= 0;
	}
}
