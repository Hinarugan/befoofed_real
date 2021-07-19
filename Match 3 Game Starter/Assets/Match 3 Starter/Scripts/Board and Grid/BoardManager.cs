using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour 
{
	public static BoardManager instance;
	public List<Sprite> characters = new List<Sprite>();
	public GameObject tile; // sus Prefab wird als Standard genutzt
	public int xSize, ySize; 
	private GameObject[,] tiles;

	public bool IsShifting { get; set; } // Wird später benutzt um Matches zu registrieren

	void Start () 
	{
		instance = GetComponent<BoardManager>();

		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

	private void CreateBoard (float xOffset, float yOffset) 
	{

		tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;
		Sprite[] previousleft = new Sprite[ySize];
		Sprite previousbelow = null;
		for (int x = 0; x < xSize; x++) //  der loop um die einzelnen Zeilen und Spalten zu füllen mit Sprites
		{
			for (int y = 0; y < ySize; y++) 
			{
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
				tiles[x, y] = newTile;
				newTile.transform.parent = transform; // durch die verergbung bleibt die Spielhierarchie
				List<Sprite> possibleCharacters = new List<Sprite>();
				possibleCharacters.AddRange(characters); // verhindert wiederholungen
				possibleCharacters.Remove(previousleft[y]);
				possibleCharacters.Remove(previousbelow);
				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
				// rndm pick aus dem Sprite pool, von der Liste die wir erstellt haben 
				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
				previousleft[y] = newSprite;
				previousbelow = newSprite;
			}
        }
    }

	public IEnumerator FindNullTiles()//Diese Methode geht über das Gesamte Board und sucht Tiles, die mit Null Object zugewiesen sind. Wenn so ein Tile gefunden wurde
									  //startet es die ShiftTilesDown-Coroutine
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
				{
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}
	}

	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
	{
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;
		if (renders.Count == 1)
		{
			renders[0].sprite = GetNewSprite(x, ySize - 1);
		}
		for (int k = 0; k < renders.Count - 1; k++)
		{
			renders[k].sprite = renders[k + 1].sprite;
			renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
		}
		for (int y = yStart; y < ySize; y++)
		{  // schleife um heraus zufinden wie viel PLatz benötigt wird, um nach unten zu verschieben
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
			{ // zählt die Plätze in einem Integer 
				nullCount++;
			}
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++)
		{ // eine zweite schleife für das eigentliche verschieben
			GUIManager.instance.Score += 50;
			yield return new WaitForSeconds(shiftDelay);// wartet für ein Delay beim Verschieben
			for (int k = 0; k < renders.Count - 1; k++)
			{ // Geht durch die Liste von "renders" durch
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, ySize -1); //  tauscht jeden sprite mit dem oben drüber durch bis zum ende
			}
		}
		IsShifting = false;
	}
	private Sprite GetNewSprite(int x, int y)
	{
		List<Sprite> possibleCharacters = new List<Sprite>();
		possibleCharacters.AddRange(characters);

		if (x > 0)
		{
			possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < xSize - 1)
		{
			possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0)
		{
			possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}

		return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
	}
}
