using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;
	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	private SpriteRenderer render;
	private bool isSelected = false;
	private bool matchFound = false;



	void Awake()
	{
		render = GetComponent<SpriteRenderer>();
    }

	private void Select()
	{
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		
	}

	private void Deselect()
	{
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

	void OnMouseDown()
	{

		if (isSelected)
		{ 
			Deselect();
		}
		else
		{
			if (previousSelected == null)
			{ 
				Select();
			}
			else
			{
				if(GetAllAdjacentTiles().Contains(previousSelected.gameObject))
					//Blickt die AdjacentTile Liste ein und überprüft ob das zuvor ausgewählte Tile in der AdjacentTile Liste ist.
				{
					SwapSprite(previousSelected.render);
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();//Tauscht die ausgewählte Sprites miteinander.
					ClearAllMatches();
					GUIManager.instance.MoveCounter--;
				}
				else
				{
					previousSelected.GetComponent<Tile>().Deselect();
					Select();//Die zweite gewählte Tile ist nicht neben der Ersten. Die erste Tile wird deselected, die Zweite wird selected.
				}
			}
		}
	}
	public void SwapSprite(SpriteRenderer render2)
	{

		Sprite tempSprite = render2.sprite; 
		render2.sprite = render.sprite;
		render.sprite = tempSprite; 
		
	}
	private GameObject GetAdjacent(Vector2 castDir)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		if(hit.collider != null)
		{
			return hit.collider.gameObject;
		}
		return null;
	}//Die Methode nutzt den Raycast um die umliegenden Tiles zu identifizieren.

	private List<GameObject> GetAllAdjacentTiles()
	{
		List<GameObject> AdjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
		{
			AdjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return AdjacentTiles;
	}
	/*Diese Methode benutzt GetAdjacent() um eine Liste an umliegenden Tiles zu erzeugen. Loopt in alle Richtungen
	und fügt alle angrenzenden Tiles zu adjacentDirections hinzu. Diese Methode verhindert, dass Tiles across the map geswappt
	werden können.*/

	private List<GameObject> FindMatch(Vector2 castDir)
	{ 
		List<GameObject> matchingTiles = new List<GameObject>(); // Eine Liste wird erstellt, welche die MatchingTiles speichert.
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // Sendet Ray in die Richtung der CastDir
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
		{ 
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir); // Sendet weitere Raycasts bis entweder nichts getroffen wird oder
		}
		return matchingTiles; // der Sprite vom returnten Object Sprite abweicht. Wenn beide Voraussetzungen zutreffen wird es als Match registriert und der Liste hinzugefügt
	}

	private void ClearMatch(Vector2[] paths) // Legt die Vector2 CastDir's als Array an
	{
		List<GameObject> matchingTiles = new List<GameObject>(); // Liste, um Matching Tiles zu speichern.
		for (int i = 0; i < paths.Length; i++) // Geht durch die Liste der CastDir's und fügt Matching Tiles der Liste hinzu.
		{
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2) // Überprüft, ob eine Dreierreihe besteht.
		{
			for (int i = 0; i < matchingTiles.Count; i++) // Geht durch die matchingTiles und nullifizieren die Sprites.
			{
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			}
			matchFound = true; 
		}
	}
	public void ClearAllMatches()
	{
		if (render.sprite == null)
			return;

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
		{
			render.sprite = null;
			matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			SFXManager.instance.PlaySFX(Clip.Clear);
		}
	}//Checkt, ob das bestehende Match von rechts nach links oder von oben nach unten verläuft und löscht die entsprechenden Sprites.
}