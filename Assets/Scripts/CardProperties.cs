using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Properties")]
public class CardProperties : ScriptableObject {
	
	public Color cardColor 			= Color.white;
	public Color setCardColor 		= Color.white;
	public Color selectedCardColor 	= Color.white;

	[Range(1,6)]
	public int[] shapeCount;

	public Color[] shapeColour;

	public float[] fillValue;

	public Sprite[] shapeSprite;

	public List<object> Properties()
	{
		List<object> properties = new List<object>();

		properties.Add(shapeCount);

		properties.Add(shapeColour);

		properties.Add(shapeSprite);

		properties.Add(fillValue);

		return properties;
	}
}
