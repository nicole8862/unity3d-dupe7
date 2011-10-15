using UnityEngine;
using System.Collections;

public class DropsComponent : MonoBehaviour
{
	public Material dropMaterial;
	public Material backMaterial;
	
	MeshFilter meshFilter;
	
	void Awake()
	{
		meshFilter = gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>().materials = new []
		{
			dropMaterial, 
			backMaterial
		};
	}
	
	public void SetDrops(int drops, int maxDrops)
	{
		Mesh mesh = new Mesh();
		{
			mesh.vertices = new []
			{
				new Vector3(0, 0, 0),
				new Vector3(0, 10, 0),
				new Vector3(drops * 10, 10, 0),
				new Vector3(drops * 10, 0, 0),
				new Vector3(drops * 10, 0, 0),
				new Vector3(drops * 10, 10, 0),
				new Vector3(maxDrops * 10, 10, 0),
				new Vector3(maxDrops * 10, 0, 0)
			};

			mesh.uv = new []
			{
				new Vector2 { x = 0, y = 0 },
				new Vector2 { x = 0, y = 1 },
				new Vector2 { x = drops, y = 1 },
				new Vector2 { x = drops, y = 0 },
				new Vector2 { x = 0, y = 0.0f },
				new Vector2 { x = 0, y = 1.0f },
				new Vector2 { x = maxDrops - drops, y = 1 },
				new Vector2 { x = maxDrops - drops, y = 0 },
			};
			
			mesh.subMeshCount = 2;
			mesh.SetTriangles(new [] { 0, 1, 2, 2, 3, 0 }, 0);
			mesh.SetTriangles(new [] { 4, 5, 6, 6, 7, 4 }, 1);
		}
		
		meshFilter.mesh = mesh;
	}
}
