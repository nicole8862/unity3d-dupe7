using UnityEngine;
using System.Collections;

public class GameOverView : MonoBehaviour
{
	public MainMenuView mainMenuViewPrefab;

	void Update ()
	{
		if (Input.GetMouseButtonUp (0)) {
			Instantiate (mainMenuViewPrefab);
			Destroy (gameObject);
		}
	}
}
