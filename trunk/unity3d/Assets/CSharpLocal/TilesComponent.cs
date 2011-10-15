using UnityEngine;
using System.Collections;

public class TilesComponent : MonoBehaviour
{
	public Renderer tilePrefab;

	Material offMaterial;
	Material highlightColumnMaterial;
	Material blinkMaterial;
	Material wipeMaterial;

	Renderer[,] tiles = new Renderer[7, 7];
	bool wink;

	IEnumerator Start ()
	{
		// Mesh
		Mesh mesh = new Mesh ();
		{
			float pos = 0.5f * (42.0f / 43.0f);
			{
				mesh.vertices = new Vector3[] { new Vector3 { x = -pos, y = -pos, z = 0.0f }, new Vector3 { x = -pos, y = pos, z = 0.0f }, new Vector3 { x = pos, y = pos, z = 0.0f }, new Vector3 { x = pos, y = -pos, z = 0.0f } };
				
				mesh.uv = new Vector2[] { new Vector2 { x = 0.0f, y = 0.0f }, new Vector2 { x = 0.0f, y = 1.0f }, new Vector2 { x = 1.0f, y = 1.0f }, new Vector2 { x = 1.0f, y = 0.0f } };
				
				mesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
			}
		}
		
		// Materials
		offMaterial = new Material (tilePrefab.renderer.sharedMaterial);
		{
			offMaterial.mainTextureScale = new Vector2 (42.0f / 128.0f, 42.0f / 128.0f);
			offMaterial.mainTextureOffset = new Vector2 (86.0f / 128.0f, 86.0f / 128.0f);
		}
		
		highlightColumnMaterial = new Material (tilePrefab.renderer.sharedMaterial);
		{
			highlightColumnMaterial.mainTextureScale = new Vector2 (42.0f / 128.0f, 42.0f / 128.0f);
		}
		
		blinkMaterial = new Material (tilePrefab.renderer.sharedMaterial);
		{
			blinkMaterial.mainTextureScale = new Vector2 (42.0f / 128.0f, 42.0f / 128.0f);
			blinkMaterial.mainTextureOffset = new Vector2 (43.0f / 128.0f, 86.0f / 128.0f);
		}
		
		wipeMaterial = new Material (tilePrefab.renderer.sharedMaterial);
		{
			wipeMaterial.mainTextureScale = new Vector2 (42.0f / 128.0f, 42.0f / 128.0f);
			wipeMaterial.mainTextureOffset = new Vector2 (43.0f / 128.0f, 43.0f / 128.0f);
		}
		
		// Objects
		for (int i = 0; i < 49; i++) {
			int c = i / 7, r = i % 7;
			Renderer renderer = (Renderer)Instantiate (tilePrefab);
			{
				renderer.gameObject.name = "tile" + c + r;
				
				renderer.transform.parent = transform;
				renderer.transform.localPosition = new Vector3 (c, r, 1.0f);
				renderer.transform.localScale = Vector3.one;
				
				renderer.GetComponent<MeshFilter> ().mesh = mesh;
				renderer.material = offMaterial;
			}
			tiles[c, r] = renderer;
		}
		
		// Coroutine
		float wipeDuration = 0.25f;
		
		for (;;) {
			while (!wink)
				yield return null;
			
			float winkTime = 0.0f;
			
			// Wipe in
			while (winkTime < wipeDuration) {
				float t = winkTime / wipeDuration;
				wipeMaterial.mainTextureOffset = new Vector2 (43.0f / 128.0f, (86.0f - 43.0f * t) / 128.0f);
				winkTime += Time.deltaTime;
				yield return null;
			}
			
			blinkMaterial.mainTextureOffset = new Vector2 (86.0f / 128.0f, 86.0f / 128.0f);
			
			// Wipe out
			while (winkTime < wipeDuration + wipeDuration) {
				float t = (winkTime - wipeDuration) / wipeDuration;
				wipeMaterial.mainTextureOffset = new Vector2 (86.0f / 128.0f, (43.0f + 43.0f * t) / 128.0f);
				winkTime += Time.deltaTime;
				yield return null;
			}
			
			blinkMaterial.mainTextureOffset = new Vector2 (43.0f / 128.0f, 86.0f / 128.0f);
			
			Reset ();
			
			wink = false;
		}
	}

	public void Reset ()
	{
		foreach (var tile in tiles)
			tile.material = offMaterial;
	}

	public void HighlightColumn (int col)
	{
		for (int c = 0; c < 7; c++)
			for (int r = 0; r < 7; r++)
				tiles[c, r].material = (col == c) ? highlightColumnMaterial : offMaterial;
	}

	public void Wink (Dupe7.DiscMarkedState[,] markedStates)
	{
		for (int c = 0; c < 7; c++) {
			for (int r = 0; r < 7; r++) {
				if (Dupe7.DiscMarkedState.Unmarked != markedStates[c, r]) {
					bool marked = (Dupe7.DiscMarkedState.DiscMarked == markedStates[c, r]);
					tiles[c, r].material = marked ? wipeMaterial : blinkMaterial;
				}
			}
		}
		
		wink = true;
	}
}
