using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMap : MonoBehaviour
{
	private Dictionary<Vector3Int, VoxelType> _Voxels = new Dictionary<Vector3Int, VoxelType>();

	public VoxelType this[Vector3Int position] => _Voxels.TryGetValue(position, out var state) ? state : VoxelType.Empty;

	public void SetVoxel(Vector3Int position, VoxelType state, bool update = true)
	{
		var currentState = this[position];
		if (currentState == state)
			return;
		_Voxels[position] = state;
		if (update)
			UpdateMap();
	}

	public void SetVoxelBlock(Vector3Int fromPosition, Vector3Int toPosition, VoxelType state)
	{
		for (int x = fromPosition.x; x <= toPosition.x; x++)
		{
			for (int y = fromPosition.y; y <= toPosition.y; y++)
			{
				for (int z = fromPosition.z; z <= toPosition.z; z++)
				{
					SetVoxel(new Vector3Int(x, y, z), state, false);
				}
			}
		}
		UpdateMap();
	}

	public void UpdateMap()
	{
		var mesh = MeshCreator.CreateNewMesh(_Voxels);
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
	}

	private void Start()
	{
		//SetVoxelBlock(new Vector3Int(-10, -10, -10), new Vector3Int(10, 10, 10), true);
	}

}
