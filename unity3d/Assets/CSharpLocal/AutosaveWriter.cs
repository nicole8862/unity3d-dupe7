using UnityEngine;
using System.Collections;
using System;
using System.IO;

public abstract class AutosaveWriter : MonoBehaviour
{
	public abstract void Write (Stream stream);

	public void Save ()
	{
		MemoryStream ms = new MemoryStream ();
		
		Write (ms);
		byte[] saveBuffer = ms.ToArray ();
		
		PlayerPrefs.SetString ("autosave", Convert.ToBase64String (saveBuffer));
		print ("Wrote " + saveBuffer.Length + " bytes of save data");
	}

	void OnApplicationQuit ()
	{
		Save ();
	}
}
