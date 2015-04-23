using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CSGVoxellizer : MonoBehaviour
{
	[DllImport("BuildingGeneratorCPP")]
	public static extern int TestContains(double x, double y, double z);

	private bool init = false;
	void Update()
	{
		if (init || !Input.GetKeyDown(KeyCode.F1))
		{
			return;
		}
		init = true;

		float voxelSize = 0.25f;
		float csgSize = 5.0f;
		int numIterations = (int)(csgSize / voxelSize);

		for (int i = 0; i < numIterations; ++i)
		{
			float x = i * voxelSize;
			for (int j = 0; j < numIterations; ++j)
			{
				float y = j * voxelSize;
				for (int k = 0; k < numIterations; ++k)
				{
					float z = k * voxelSize;
					if (TestContains(x, y, z) == 1)
					{
						GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
						voxel.transform.position = new Vector3(x - 2.5f, y - 2.5f, z - 2.5f);
						voxel.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);
					}
				}
			}
		}
	}
}
