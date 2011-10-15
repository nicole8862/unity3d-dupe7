using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Dupe7AutosaveWriter : AutosaveWriter
{
	public override void Write (Stream stream)
	{
		print ("Writing autosave");
		
		Dupe7.GameStateMachine game = GetComponent<GameplayView> ().Game;
		new BinaryFormatter ().Serialize (stream, game);
	}
}
