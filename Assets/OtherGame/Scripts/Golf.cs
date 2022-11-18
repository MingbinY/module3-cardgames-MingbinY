using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Golf : MonoBehaviour
{
    public static Golf S;

	[Header("Set in Inspector")]
	public TextAsset deckXML;
	public TextAsset layoutXML;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;
	public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
	public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);

	public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
	public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
	public float reloadDelay = 2f;// 2 sec delay between rounds
	public Text gameOverText, roundResultText, highScoreText, roundCount;

	public Color grayOutColor = Color.gray;

	[Range(0, 1)]
	public float goldCardChance;

	[Header("Set Dynamically")]
	public Deck deck;
	public Layout layout;
	public List<CardGolf> drawPile;
	public Transform layoutAnchor;
	public CardGolf target;
	public List<CardGolf> tableau;
	public List<CardGolf> discardPile;
	public FloatingScore fsRun;

    private void Awake()
    {
		S = this;
		SetUpUITexts();
    }

	void SetUpUITexts()
	{ 
		ShowResultsUI(false);
	}

	public void ShowResultsUI(bool show)
	{
		gameOverText.gameObject.SetActive(show);
		roundResultText.gameObject.SetActive(show);
	}

	void Start()
	{
		deck = GetComponent<Deck>();
		deck.InitDeck(deckXML.text);
		Deck.Shuffle(ref deck.cards);

		//Card c;
		//for (int cNum = 0; cNum < deck.cards.Count; cNum++)
		//{
		//	c = deck.cards[cNum];
		//	c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
		//}

		layout = GetComponent<Layout>();
		layout.ReadLayout(layoutXML.text);
		drawPile = ConverListCardsToListCardGolf(deck.cards);
		LayoutGame();
		SetTableauFaces();
		GolfScoreManager.S.UpdateScoreBoard();
	}

	List<CardGolf> ConverListCardsToListCardGolf(List<Card> lCD)
	{
		List<CardGolf> lCP = new List<CardGolf>();
		CardGolf tCP;
		foreach (Card tCD in lCD)
		{
			tCP = tCD as CardGolf;
			lCP.Add(tCP);
		}

		return lCP;
	}

	CardGolf Draw()
    {
		CardGolf cg = drawPile[0];
		drawPile.RemoveAt(0);
		return cg;
	}

	void LayoutGame()
    {
		if (layoutAnchor == null)
        {
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
        }

		CardGolf cg;

		foreach (SlotDef tSD in layout.slotDefs)
        {
			cg = Draw();
			cg.faceUp = tSD.faceUp;
			cg.transform.parent = layoutAnchor;
			cg.transform.localPosition = new Vector3(
				layout.multiplier.x * tSD.x, layout.multiplier.y * tSD.y, -tSD.layerID);
			cg.layoutID = tSD.id;
			cg.slotDef = tSD;
			cg.state = golfCardState.tableau;
			cg.SetSortingLayerName(tSD.layerName);
			tableau.Add(cg);
		}

		foreach (CardGolf tCG in tableau)
        {
			foreach (int hid in tCG.slotDef.hiddenBy)
            {
				cg = FindCardByLayoutID(hid);
				tCG.hiddenBy.Add(cg);
            }
        }
		MoveToTarget(Draw());
		UpdateDrawPile();
	}

	CardGolf FindCardByLayoutID(int layoutID)
    {
		foreach (CardGolf tCG in tableau)
        {
			if (tCG.layoutID == layoutID)
            {
				return tCG;
            }
        }

		return null;
    }

	void SetTableauFaces()
    {
		foreach (CardGolf cg in tableau)
        {
			bool faceUp = true;
			foreach (CardGolf cover in cg.hiddenBy)
            {
				if (cover.state == golfCardState.tableau)
					faceUp = false;
            }
			Debug.Log(cg + " Face Up: " + faceUp);
			if (!faceUp)
				cg.GetComponent<SpriteRenderer>().color = grayOutColor;
			else
				cg.GetComponent<SpriteRenderer>().color = Color.white;
			cg.faceUp = faceUp;
		}
    }

	void SetFaceColors(CardGolf cg, Color color)
	{
		foreach (SpriteRenderer sr in cg.spriteRenderers)
        {
			sr.color = color;
        }
    }
	void MoveToDiscard(CardGolf cg)
    {
		cg.state = golfCardState.discard;
		discardPile.Add(cg);
		cg.transform.parent = layoutAnchor;

		cg.transform.localPosition = new Vector3
			(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID + 0.5f);
		cg.faceUp = true;
		cg.SetSortingLayerName(layout.discardPile.layerName);
		cg.SetSortOrder(-100 + discardPile.Count);
	}

	void MoveToTarget(CardGolf cd)
	{
		if (target != null)
		{
			MoveToDiscard(target);
		}
		target = cd;
		cd.state = golfCardState.target;
		cd.transform.parent = layoutAnchor;

		cd.transform.localPosition = new Vector3
			(layout.multiplier.x * layout.discardPile.x, layout.multiplier.y * layout.discardPile.y, -layout.discardPile.layerID);
		cd.faceUp = true;
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
		cd.GetComponent<SpriteRenderer>().sprite = deck.cardFrontGold;
	}

	void UpdateDrawPile()
	{
		CardGolf cd;

		for (int i = 0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;

			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3
				(layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
				layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
				-layout.drawPile.layerID + 0.1f * i
				);
			cd.faceUp = false;
			cd.state = golfCardState.drawpile;
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10 * i);
		}
	}

	public void CardClicked(CardGolf cd)
	{
		switch (cd.state)
		{
			case golfCardState.target:
				break;
			case golfCardState.drawpile:
				MoveToDiscard(target);
				MoveToTarget(Draw());
				UpdateDrawPile();
				GolfScoreManager.EVENT(eScoreEvent.draw);
				
				break;
			case golfCardState.tableau:
				bool validMatch = true;
				if (!cd.faceUp)
				{
					validMatch = false;
				}
				if (!AdjacentRank(cd, target))
				{
					validMatch = false;
				}
				if (!validMatch)
					return;
				tableau.Remove(cd);
				MoveToTarget(cd);
				SetTableauFaces();
				if (cd.isGoldCard)
				{
					GolfScoreManager.EVENT(eScoreEvent.mineGold);
					
				}
				else
				{
					GolfScoreManager.EVENT(eScoreEvent.mine);
					
				}
				break;
		}
		CheckForGameOver();
	}

	public bool AdjacentRank(CardGolf c0, CardGolf c1)
	{
		if (!c0.faceUp || !c1.faceUp)
		{
			return false;
		}

		if (Mathf.Abs(c0.rank - c1.rank) == 1)
		{
			return true;
		}

		return false;
	}

	void CheckForGameOver()
	{
		if (tableau.Count == 0)
		{
			GameOver();
			return;
		}

		if (drawPile.Count > 0)
		{
			return;
		}

		foreach (CardGolf cd in tableau)
		{
			if (AdjacentRank(cd, target))
			{
				return;
			}
		}

		GameOver();
	}

	void GameOver()
	{
		int score = GolfScoreManager.SCORE;
		GolfScoreManager.EVENT(eScoreEvent.gameEnd);

		Invoke("ReloadLevel", reloadDelay);
	}

	void ReloadLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
