using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct VoxelRenderData
{
	/// <summary>
	/// Position of the voxel
	/// </summary>
	public Vector3Int Position { get; }

	/// <summary>
	/// Faces to render
	/// </summary>
	public HashSet<VoxelFace> FacesToRended { get; }

	/// <summary>
	/// Type of the voxel
	/// </summary>
	public VoxelType Type { get; }


	/// <summary>
	/// Default constructor
	/// </summary>
	/// <param name="position">Position of the voxel</param>
	/// <param name="type">Type of the voxel</param>
	public VoxelRenderData(Vector3Int position, VoxelType type) : this()
	{
		Position = position;
		FacesToRended = new HashSet<VoxelFace>();
		Type = type;
	}
}

