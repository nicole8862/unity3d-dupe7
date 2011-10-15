using UnityEngine;
using System.Collections;

public class MainMenuView : MonoBehaviour
{
	Rect startRect = new Rect (-112.0f, -16.0f, 224.0f, 40.0f);
	
	public GameplayView gameplayPrefab;

	void OnGUI ()
	{
		if (GUI.Button (RectFromVirtualRect (startRect), "Start Game")) {
			((GameplayView)Instantiate (gameplayPrefab)).Game = new Dupe7.GameStateMachine (5, 5, 1337);
			Destroy (gameObject);
		}
	}
	
	Rect RectFromVirtualRect (Rect vr)
	{
		float scale = Screen.height / 480.0f;
		float xoffs = Screen.width * 0.5f, yoffs = Screen.height * 0.5f;
		return new Rect(vr.x * scale + xoffs, vr.y * scale + yoffs, vr.width * scale, vr.height * scale);
	}
}
