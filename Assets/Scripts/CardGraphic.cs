﻿using System.Collections;
using System.Collections.Generic;
using System;


using UnityEngine;

public class CardGraphic : MonoBehaviour
{
	[SerializeField] private SpriteRenderer cardBack;
	[SerializeField] private SpriteRenderer[] graphics;
	[SerializeField] private CardProperties cardProperties;

	[NonSerialized] public Card card = null;

	private Collider2D boxCollider;

	private void Awake()
	{
		boxCollider = gameObject.GetComponent<BoxCollider2D>();
	}

	public void SetGraphics()
	{
		
		if (card == null)
		{
			return;
		}

		Color setColor = card.colour;
		setColor.a = card.fill;

		for (int s = 0; s < graphics.Length; s++)
		{
			graphics[s].sprite = card.shape;
			graphics[s].color = setColor;
		}

		setColor.a = 0f;
			
		switch (card.count)
		{
			case 1:
				graphics[0].color = setColor;
				graphics[2].color = setColor;
				break;
			case 2:
				graphics[1].color = setColor;
				break;
			case 3:
				break;
			default:
				Debug.Log("Card Missing Case");
				break;
		}

		boxCollider.enabled = true;
		Deselect();
	}

	public void OnClicked()
	{
		GameManager.Instance.OnCardClicked(this);
	}

	public void Select()
	{
		//change color to selected

		cardBack.color = cardProperties.selectedCardColor;
	}

	public void Deselect()
	{
		//change color to deselected	
		cardBack.color = cardProperties.cardColor;
	}

	public void Highlight()
	{
		cardBack.color = cardProperties.setCardColor;
	}

	public void Hide()
	{
		foreach(SpriteRenderer graphic in graphics)
		{
			graphic.color = Color.clear;
		}

		cardBack.color = Color.clear;
		boxCollider.enabled = false;
	}
}
