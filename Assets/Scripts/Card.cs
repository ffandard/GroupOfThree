using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
	public Sprite shape;
	public Color colour;
	public int count;
	public float fill;

	public Card(Sprite Shape, Color Colour, int Count, float Fill)
	{
		shape = Shape;
		colour = Colour;
		count = Count;
		fill = Fill;
	}
}