using UnityEngine;
using System.Collections;

public class PixelOffsetOrthoComponent : MonoBehaviour
{
	void OnPreCull ()
	{
		Camera.main.orthographicSize = Camera.main.pixelHeight * 0.5f;		
	}
	
	void OnPreRender ()
	{
		float w = Camera.main.pixelWidth, h = Camera.main.pixelHeight;
		GL.LoadPixelMatrix (-w * 0.5f, w * 0.5f, -h * 0.5f, h * 0.5f);
	}
}
