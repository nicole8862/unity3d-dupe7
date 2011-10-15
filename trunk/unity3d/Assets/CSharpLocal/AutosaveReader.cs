using UnityEngine;
using System.Collections;
using System;
using System.IO;

public abstract class AutosaveReader : MonoBehaviour
{
	public abstract void Read(Stream stream);
	
	public void Load()
	{
		if(PlayerPrefs.HasKey("autosave"))
			Read(new MemoryStream(Convert.FromBase64String(PlayerPrefs.GetString("autosave"))));
	}
	
	void Start()
	{
		Load();
	}
}
