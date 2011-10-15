using UnityEngine;
using System.Collections;

public class BonusComponent : MonoBehaviour
{
	public int discNum;
	public int score;
	
	IEnumerator Start()
	{
		renderer.material.color = ColorForDiscNum (discNum);
		GetComponent<TextMesh>().text = "+" + score;

		yield return new WaitForSeconds(animation.clip.length);
		
		Destroy(transform.parent.gameObject);
	}
	
	Color ColorForDiscNum (int discNum)
	{
		Color color = Color.white;
		
		switch (discNum) {
		case 1:
			color = new Color (0.0f, 128.0f / 255.0f, 0.0f);
			break;
		
		case 2:
			color = new Color (205.0f / 255.0f, 205.0f / 255.0f, 0.0f);
			break;
		
		case 3:
			color = new Color (1.0f, 165.0f / 255.0f, 0.0f);
			break;
		
		case 4:
			color = new Color (1.0f, 0.0f, 0.0f);
			break;
		
		case 5:
			color = new Color (139.0f / 255.0f, 0.0f, 139.0f / 255.0f);
			break;
		
		case 6:
			color = new Color (0.0f, 191.0f / 255.0f, 1.0f);
			break;
		
		case 7:
			color = new Color (72.0f / 255.0f, 61.0f / 255.0f, 139.0f / 255.0f);
			break;
		}
		
		return color;
	}
}
