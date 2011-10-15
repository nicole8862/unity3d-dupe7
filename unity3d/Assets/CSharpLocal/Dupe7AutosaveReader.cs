using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Dupe7AutosaveReader : AutosaveReader
{
	public override void Read (Stream stream)
	{
		print ("Reading autosave");
#if false		
		try {
			Dupe7.GameStateMachine game = (Dupe7.GameStateMachine)(new BinaryFormatter ().Deserialize (stream));
			((GameplayView)Instantiate (GetComponent<MainMenuView> ().gameplayPrefab)).Game = game;
			Destroy (gameObject);
		} catch (Exception e) {
			print (e);
		}
#endif
	}
}
