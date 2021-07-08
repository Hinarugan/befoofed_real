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

}
