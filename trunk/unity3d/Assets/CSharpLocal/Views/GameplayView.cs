using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayView : MonoBehaviour
{
	Mesh[] meshes = new Mesh[9];
	float[,] meshPositions = new float[7, 8];
	MeshFilter nextDiscMesh;
	LinkedList<MeshFilter>[] meshColumns = new LinkedList<MeshFilter>[7];
	int discCounter = 0;
	float speed = 0.0f;

	public GameOverView gameOverPrefab;
	public MeshFilter discPrefab;
	public Transform bonusPrefab;
	public Transform discs;
	public TilesComponent tiles;
	public TextMesh scoreText;
	public TextMesh levelText;
	public DropsComponent dropGauge;
	public TextMesh chainText;

	public Dupe7.GameStateMachine Game { get; set; }

	IEnumerator Start ()
	{
		// Start with a default game if one isn't set up already
		if (null == Game)
			Game = new Dupe7.GameStateMachine (5, 5, 1337);
		
		InitComponents ();
		InitMeshes ();
		
		for (int c = 0; c < Dupe7.GameStateMachine.COLS; c++)
			for (int r = 0; r < Game.columnCounts[c]; r++)
				meshColumns[c].AddLast (CreateDisc (Game.Discs[c, r].num, Game.Discs[c, r].state, new Vector2 (c, r)));
		
		bool levelUp = false, playerDrop = false, gameOver = false;
		while (!gameOver && Game.MoveNext ()) {
			// Debug dump
			print (Game.Current);
			
			var boardDump = "";
			for (int r = Dupe7.GameStateMachine.ROWS - 1; r >= 0; r--) {
				for (int c = 0; c < Dupe7.GameStateMachine.COLS; c++)
					boardDump += (r < Game.ColumnCounts[c] ? Game.Discs[c, r].num + ", " : "0, ");
				boardDump += ": ";
				for (int c = 0; c < Dupe7.GameStateMachine.COLS; c++)
					boardDump += (int)Game.DiscMarkedStates[c, r] + ", ";
				boardDump += "\n";
			}
			print (boardDump);
			
			// Pause after level up
			if (levelUp) {
				yield return new WaitForSeconds (1.0f);
				levelUp = false;
			}
			
			switch (Game.Current) {
			case Dupe7.TurnState.WaitingForPlayer:
				yield return StartCoroutine (SetDropColumn ());
				break;
			
			case Dupe7.TurnState.TurnStarted:
				playerDrop = true;
				meshColumns[Game.DropColumn].AddLast (nextDiscMesh);
				break;
			
			case Dupe7.TurnState.Dropped:
				yield return StartCoroutine (Drop ());
				if (Game.NumTurnsThisLevel > 1)
					gameOver = Game.TestGameOver ();

				// Update drops just once each turn
				if (playerDrop) {
					dropGauge.SetDrops (Game.NumTurnsThisLevel, Game.MaxTurnsThisLevel);
					playerDrop = false;
				}
				break;
			
			case Dupe7.TurnState.Marked:
				tiles.Wink (Game.DiscMarkedStates);
				UpdateMeshesAndRemoveMarked ();
				SetChain (Game.Chain + 1);
				yield return new WaitForSeconds (1.0f);
				SetScore (Game.Score);
				break;
			
			case Dupe7.TurnState.LevelUp:
				yield return new WaitForSeconds (1.0f);
				HideLevelAndDrops ();
				SetScore (Game.Score);
				LevelUp ();
				levelUp = true;
				gameOver = Game.TestGameOver ();
				break;
			
			case Dupe7.TurnState.TurnEnded:
				nextDiscMesh = CreateDisc (Game.NextDisc.num, Game.NextDisc.state, new Vector2 (3.0f, 7.0f));
				ShowLevelAndDrops (Game.NumTurnsThisLevel, Game.MaxTurnsThisLevel, Game.Level);
				break;
			}
		}
		
		yield return new WaitForSeconds (1.0f);
		
		Instantiate (gameOverPrefab);
		Destroy (gameObject);
	}

	void InitComponents ()
	{
		SetScore (Game.Score);
		ShowLevelAndDrops (Game.NumTurnsThisLevel, Game.MaxTurnsThisLevel, Game.Level);
		chainText.renderer.enabled = false;
	}

	void InitMeshes ()
	{
		float pos = 0.5f * (42.0f / 43.0f);
		
		int[] tris = new int[] { 0, 1, 2, 2, 3, 0 };
		
		for (int i = 0; i < 9; i++) {
			Mesh mesh = new Mesh ();
			{
				mesh.vertices = new Vector3[] { new Vector3 { x = -pos, y = -pos, z = 0.0f }, new Vector3 { x = -pos, y = pos, z = 0.0f }, new Vector3 { x = pos, y = pos, z = 0.0f }, new Vector3 { x = pos, y = -pos, z = 0.0f } };
				
				float xo = ((i % 3) * 43.0f / 128.0f);
				float yo = ((i / 3) * 43.0f / 128.0f);
				
				mesh.uv = new Vector2[] { new Vector2 { x = xo, y = yo }, new Vector2 { x = xo, y = yo + 42.0f / 128.0f }, new Vector2 { x = xo + 42.0f / 128.0f, y = yo + 42.0f / 128.0f }, new Vector2 { x = xo + 42.0f / 128.0f, y = yo } };
				
				mesh.triangles = tris;
			}
			meshes[i] = mesh;
		}
		
		for (int c = 0; c < 7; c++)
			meshColumns[c] = new LinkedList<MeshFilter> ();
	}

	void SetScore (int score)
	{
		scoreText.text = score.ToString ("#,#0", System.Globalization.CultureInfo.InvariantCulture);
	}

	void ShowLevelAndDrops (int numDrops, int numDropsPerLevel, int level)
	{
		levelText.text = "LEVEL " + (level + 1);
		levelText.renderer.enabled = true;
		
		dropGauge.SetDrops (numDrops, numDropsPerLevel);
		dropGauge.renderer.enabled = true;
	}

	void HideLevelAndDrops ()
	{
		levelText.renderer.enabled = false;
		dropGauge.renderer.enabled = false;
	}

	IEnumerator SetDropColumn ()
	{
		int dropColumn = 0;
		
		while (!Input.GetMouseButtonDown (0))
			yield return null;
		
		while (!Input.GetMouseButtonUp (0)) {
			dropColumn = CalculateDropColumn ();
			
			var lp = nextDiscMesh.transform.localPosition;
			nextDiscMesh.transform.localPosition = new Vector3 (dropColumn, lp.y, lp.z);
			tiles.HighlightColumn (dropColumn);
			yield return null;
		}
		
		tiles.Reset ();
		
		Game.DropColumn = dropColumn;
	}

	IEnumerator Drop ()
	{
		speed = 0.0f;
		bool stacked;
		do {
			stacked = UpdatePhysics ();
			yield return null;
		} while (!stacked);
	}

	void UpdateMeshesAndRemoveMarked ()
	{
		for (int c = 0; c < meshColumns.Length; c++) {
			var meshNode = meshColumns[c].First;
			for (int r = 0; null != meshNode; r++,meshNode = meshNode.Next)
				meshNode.Value.mesh = MeshForDisc (Game.Discs[c, r].num, Game.Discs[c, r].state);
		}
		
		// Remove marked
		for (int c = 0; c < meshColumns.Length; c++) {
			var meshNode = meshColumns[c].First;
			for (int r = 0; null != meshNode; r++) {
				var nextMeshNode = meshNode.Next;
				if (Dupe7.DiscMarkedState.DiscMarked == Game.DiscMarkedStates[c, r]) {
					CreateBonus (Game.Discs[c, r].num, Game.ChainScore, new Vector2 (c, r));
					
					StartCoroutine (DestroyDisc (meshNode.Value));
					
					meshNode.List.Remove (meshNode);
				}
				meshNode = nextMeshNode;
			}
		}
	}

	MeshFilter CreateDisc (int num, Dupe7.DiscState state, Vector2 pos)
	{
		MeshFilter meshFilter = (MeshFilter)Instantiate (discPrefab);
		{
			meshFilter.mesh = MeshForDisc (num, state);
			
			meshFilter.transform.parent = discs.transform;
			meshFilter.transform.localPosition = new Vector3 (pos.x, pos.y, 0.0f);
			meshFilter.transform.localScale = Vector3.one;
			
			meshFilter.gameObject.name = "disc" + (++discCounter);
		}
		return meshFilter;
	}

	Mesh MeshForDisc (int num, Dupe7.DiscState state)
	{
		bool isOpen = (Dupe7.DiscState.Open == state);
		bool isShut = (Dupe7.DiscState.Shut == state);
		return meshes[isOpen ? (num - 1) : (isShut ? 7 : 8)];
	}

	int CalculateDropColumn ()
	{
		Vector3 worldPoint = discs.transform.InverseTransformPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		float x = Mathf.Round (worldPoint.x);
		return (int)Mathf.Clamp (x, 0, 6);
	}

	bool UpdatePhysics ()
	{
		int cols = meshPositions.GetUpperBound (0) + 1;
		
		// Copy mesh positions to physics positions
		for (int c = 0; c < cols; c++) {
			var meshNode = meshColumns[c].First;
			for (int r = 0; null != meshNode; r++,meshNode = meshNode.Next)
				meshPositions[c, r] = meshNode.Value.transform.localPosition.y;
		}
		
		float at = -80.0f * Time.deltaTime;
		float dist = speed * Time.deltaTime + 0.5f * at * Time.deltaTime;
		speed += at;
		
		bool stacked = true;
		for (int c = 0; c < 7; c++) {
			int stackCount = 0;
			for (int r = 0, n = meshColumns[c].Count; r < n; r++) {
				float y = meshPositions[c, r];
				y += dist;
				
				if (y < stackCount) {
					y = stackCount;
					stackCount++;
				} else
					stacked = false;
				
				meshPositions[c, r] = y;
			}
		}
		
		// Copy physics positions to mesh positions
		for (int c = 0; c < cols; c++) {
			var meshNode = meshColumns[c].First;
			for (int r = 0; null != meshNode; r++,meshNode = meshNode.Next) {
				var lp = meshNode.Value.transform.localPosition;
				meshNode.Value.transform.localPosition = new Vector3 (lp.x, meshPositions[c, r], lp.z);
			}
		}
		
		return stacked;
	}

	void LevelUp ()
	{
		for (int c = 0; c < meshColumns.Length; c++) {
			foreach (var mesh in meshColumns[c])
				mesh.transform.localPosition += new Vector3 (0.0f, 1.0f, 0.0f);
			
			meshColumns[c].AddFirst (CreateDisc (Game.Discs[c, 0].num, Game.Discs[c, 0].state, new Vector2 (c, 0.0f)));
		}
	}

	void SetChain (int chain)
	{
		if (chain > 1) {
			chainText.renderer.enabled = true;
			chainText.text = "CHAIN x" + chain;
			
			// Calling 'Play' when the animation is playing doesn't stop it
			chainText.animation.Stop ();
			chainText.animation.Play ();
		}
	}

	Transform CreateBonus (int discNum, int score, Vector2 pos)
	{
		var bonus = Instantiate (bonusPrefab) as Transform;
		{
			bonus.transform.parent = discs.transform;
			bonus.transform.localPosition = new Vector3 (pos.x, pos.y, -10.0f);
			
			var bonusComponent = bonus.GetComponentInChildren<BonusComponent> ();
			{
				bonusComponent.discNum = discNum;
				bonusComponent.score = score;
			}
		}
		return bonus;
	}
	
	IEnumerator DestroyDisc (MeshFilter discMesh)
	{
		yield return new WaitForSeconds (1.0f);
		Destroy (discMesh.gameObject);
	}
}
