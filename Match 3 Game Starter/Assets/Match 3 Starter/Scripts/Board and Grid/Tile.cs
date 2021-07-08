using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
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
				SwapSprite(previousSelected.render);
				previousSelected.Deselect(); 
			}
		}
	}
	public void SwapSprite(SpriteRenderer render2)
	{
		Sprite tempSprite = render2.sprite; 
		render2.sprite = render.sprite;
		render.sprite = tempSprite; 
		
	}

}