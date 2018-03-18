using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

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
	public CardGraphic[] cardGraphics;

	private List<CardGraphic> selectedCards = new List<CardGraphic>();
	private List<List<CardGraphic>> sets = new List<List<CardGraphic>>();

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

		if (selectedCards.Count == CARDS_IN_A_SET && gameManager.CheckIfSet(selectedCards))
		{
			Debug.Log("set!");
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
		//get cardGraphics in children
		cardGraphics = transform.GetComponentsInChildren<CardGraphic>(true);

		deck = new Deck(null);
		deck.Shuffle();

		Debug.Log("There are " + deck.cards.Count + " cards in the deck");

		DisplayCards();
		SetsOnTable();
	}

	#if UNITY_EDITOR

	private void Update()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			Debug.Log(deck.cards.Count);
		}

		if (Input.GetKeyDown(KeyCode.Q) || Input.touchCount == 2)
		{
			HighlightSet();
		}

		if (Input.GetKeyDown(KeyCode.W) || Input.touchCount >= 3)
		{
			HighlightSet(true);
		}
	}

	#endif

	private void DisplayCards()
	{
		foreach (CardGraphic graphic in cardGraphics)
		{
			graphic.card = deck.cards[0];
			deck.cards.Remove(deck.cards[0]);

			graphic.SetGraphics();
		}
	}


	private void PlaceNewCards()
	{
		foreach (CardGraphic graphic in cardGraphics)
		{
			if (graphic != null)
			{
				graphic.Deselect();
			}
		}

		foreach (CardGraphic graphic in selectedCards)
		{
			graphic.Deselect();
			if (!deck.IsEmpty())
			{
				graphic.card = deck.cards[0];
				deck.cards.Remove(deck.cards[0]);
				graphic.SetGraphics();
			}
			else
			{
				graphic.transform.gameObject.SetActive(false);
			}
		}

		selectedCards.Clear();
		SetsOnTable();
	}

	private bool CheckIfSet(List<CardGraphic> selectedCards)
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

		if (cardGraphics.Length < 3)
		{
			return;
		}

		debugSet = 0;
		sets.Clear();

		for (int c1 = 0; c1 < cardGraphics.Length; c1++)
		{
			if (!cardGraphics[c1].gameObject.activeSelf)
			{
				continue;
			}
			for (int c2 = 0; c2 < cardGraphics.Length; c2++)
			{
				if (!cardGraphics[c2].gameObject.activeSelf)
				{
					continue;
				}
				for (int c3 = 0; c3 < cardGraphics.Length; c3++)
				{				
					if (!cardGraphics[c3].gameObject.activeSelf)
					{
						continue;
					}

					if (c1 != c2 && c2 != c3 && c3 != c1)
					{
						List<CardGraphic> testSet = new List<CardGraphic>{ cardGraphics[c1], cardGraphics[c2], cardGraphics[c3] };

						if (CheckIfSet(testSet))
						{
							bool duplicate = false;
							foreach (List<CardGraphic> set in sets)
							{
								if (set.Contains(cardGraphics[c1]) && set.Contains(cardGraphics[c2]) && set.Contains(cardGraphics[c3]))
								{
									duplicate = true;
								}
							}

							if (!duplicate)
							{
								sets.Add(testSet);
							}

						}
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

		if (collect)
		{
			selectedCards = sets[debugSet];
			Debug.Log("set!");
			PlaceNewCards();
		}
		else
		{
			foreach (CardGraphic graphic in cardGraphics)
			{
				graphic.Deselect();
			}

			foreach (CardGraphic graphic in sets[debugSet])
			{
				graphic.Highlight();
			}

			debugSet++;
		}
	}

	#endregion

}
