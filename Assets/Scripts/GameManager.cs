﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	private static GameManager instance = null;

	public static GameManager Instance
	{
		get
		{
			return instance;
		}
	}

	public Deck deck;
	public List<CardGraphic> cardGraphics = new List<CardGraphic>();
	private List<CardGraphic> selectedCards = new List<CardGraphic>();
	private List<List<CardGraphic>> sets = new List<List<CardGraphic>>();

	public Text setCountText;
	public string setCountFormat;
	private int setCount;

	private int debugSet = 0;

	private const int CARDS_IN_A_SET = 3;


	#region Public Interface


	public void OnCardClicked(CardGraphic cardGraphic)
	{
		GameManager gameManager = GameManager.Instance;
		List<CardGraphic> selectedCards = gameManager.selectedCards;

		if (!selectedCards.Contains(cardGraphic) && selectedCards.Count < CARDS_IN_A_SET)
		{
			selectedCards.Add(cardGraphic);
			cardGraphic.Select();
		}
		else
		{
			selectedCards.Remove(cardGraphic);
			cardGraphic.Deselect();
		}

		if (selectedCards.Count == CARDS_IN_A_SET && gameManager.IsValidSet(selectedCards))
		{
			Debug.Log("set!");

			setCount++;
			gameManager.UpdateSetCount();
			gameManager.PlaceNewCards();
		}
	}

		
	#endregion

	#region Private Methodes

	private void Awake()
	{
		// Instantiate the GameManager singleton
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Start()
	{
		StartNewGame();
	}


	private void StartNewGame()
	{
		cardGraphics = transform.GetComponentsInChildren<CardGraphic>(true).ToList();

		deck.Create();
		deck.Shuffle();

		Debug.Log("There are " + deck.GetCardCount() + " cards in the deck");

		DisplayCards();

		setCount = 0;
		UpdateSetCount();
		SetsOnTable();
	}

	//#if UNITY_EDITOR

	private void Update()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			Debug.Log(deck.GetCardCount());
		}

		if (Input.GetKeyDown(KeyCode.Q) || (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began))
		{
			HighlightSet();
		}

		if (Input.GetKeyDown(KeyCode.W) || (Input.touchCount == 3 && Input.GetTouch(2).phase == TouchPhase.Began))
		{
			HighlightSet(true);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			StartNewGame();
		}
	}

	//#endif

	private void UpdateSetCount()
	{
		setCountText.text = String.Format(setCountFormat, setCount);
	}



	private void DisplayCards()
	{
		foreach (CardGraphic cardGraphic in cardGraphics)
		{
			Card card = deck.GetCardByIndex(0);
			cardGraphic.card = card;
			deck.RemoveCard(card);
			cardGraphic.SetGraphics();
		}
	}


	private void PlaceNewCards()
	{
		foreach (CardGraphic cardGraphic in cardGraphics)
		{
			if (cardGraphic != null)
			{
				cardGraphic.Deselect();
			}
		}

		foreach (CardGraphic cardGraphic in selectedCards)
		{
			cardGraphic.Deselect();
			if (!deck.IsEmpty())
			{
				Card card = deck.GetCardByIndex(0);
				cardGraphic.card = card;
				deck.RemoveCard(card);
				cardGraphic.SetGraphics();
			}
			else
			{
				cardGraphic.Hide();
				cardGraphics.Remove(cardGraphic);
			}
		}

		selectedCards.Clear();
		SetsOnTable();

		if (IsGameOver())
		{
			StartNewGame();
		}
	}

	private bool IsGameOver()
	{
		return (sets.Count == 0 || deck.IsEmpty());
	}

	private bool IsValidSet(List<CardGraphic> selectedCards)
	{
		int shapeValid = 0;
		int colourValid = 0;
		int fillValid = 0;
		int countValid = 0;

		List<object> properties = new List<object>();


		foreach (CardGraphic graphic in selectedCards)
		{
			if (properties.Contains(graphic.card.colour))
			{
				colourValid++;
			}
			if (properties.Contains(graphic.card.shape))
			{
				shapeValid++;
			}
			if (properties.Contains(graphic.card.fill))
			{
				fillValid++;
			}
			if (properties.Contains(graphic.card.count))
			{
				countValid++;
			}

			properties.Add(graphic.card.colour);
			properties.Add(graphic.card.shape);
			properties.Add(graphic.card.fill);
			properties.Add(graphic.card.count);
		}

		if (shapeValid == 1 || colourValid == 1 || fillValid == 1 || countValid == 1)
		{
			return false;
		}

		return true;
	}

	private void SetsOnTable()
	{
		Debug.Log("Attempting to highlight sets");

		if (cardGraphics.Count < CARDS_IN_A_SET)
		{
			return;
		}

		debugSet = 0;
		sets.Clear();


		for (int c1 = 0; c1 < cardGraphics.Count; c1++)
		{
			for (int c2 = c1 + 1; c2 < cardGraphics.Count; c2++)
			{
				for (int c3 = c2 + 1; c3 < cardGraphics.Count; c3++)
				{
					List<CardGraphic> testSet = new List<CardGraphic>{ cardGraphics[c1], cardGraphics[c2], cardGraphics[c3] };

					if (IsValidSet(testSet))
					{
						sets.Add(testSet);
					}
				}
			}
		}
	}

	private void HighlightSet(bool collect = false)
	{
		if (sets.Count == 0)
		{
			Debug.Log("Couldn't find any sets");
			return;
		}

		if (debugSet > sets.Count - 1)
		{
			debugSet = 0;
			Debug.Log("Found all sets, looping back to first set");
		}

		selectedCards.Clear();

		foreach (CardGraphic cardGraphic in cardGraphics)
		{
			cardGraphic.Deselect();
		}
			
		if (collect)
		{
			foreach(CardGraphic cardGraphic in sets[debugSet])
			{
				OnCardClicked(cardGraphic);
			}
		}
		else
		{
			foreach (CardGraphic cardGraphic in sets[debugSet])
			{
				cardGraphic.Highlight();
			}

			debugSet++;
		}
	}

	#endregion

}
