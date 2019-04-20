using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshCreator
{
	/// <summary>
	/// Create a new mesh for the chunk
	/// </summary>
	/// <param name="voxels">Voxels of the chunk</param>
	/// <returns></returns>
	public static Mesh CreateNewMesh(IEnumerable<VoxelRenderData> voxels)
	{
		// Initialize vertices, triangles, normals and UVs

		var data = new MeshData();

		// Create all faces

		foreach (var voxel in voxels)
		{
			foreach (var face in voxel.FacesToRended)
			{
				voxel.Voxel.CreateFace(face, ref data);
			}
		}

		// Remeber to allways set the vertices first
		// If not if will create errors

		return new Mesh
		{
			vertices = data.Vertices.ToArray(),
			triangles = data.Triangles.ToArray(),
			normals = data.Normals.ToArray(),
			uv = data.UVs.ToArray(),
		};
	}

	/// <summary>
	/// Return a direction from a face
	/// </summary>
	/// <param name="face"></param>
	/// <returns></returns>
	public static Vector3Int FaceToDirection(VoxelFace face)
	{
		switch (face)
		{
			case VoxelFace.North:
				return new Vector3Int(0, 0, 1);

			case VoxelFace.South:
				return new Vector3Int(0, 0, -1);

			case VoxelFace.East:
				return new Vector3Int(1, 0, 0);

			case VoxelFace.West:
				return new Vector3Int(-1, 0, 0);

			case VoxelFace.Up:
				return new Vector3Int(0, 1, 0);

			case VoxelFace.Down:
				return new Vector3Int(0, -1, 0);

			default:
				throw new ArgumentException("You have given me invalid face");
		}
	}

	/// <summary>
	/// Gets a oposite face to given one
	/// </summary>
	/// <param name="face">Source face</param>
	/// <returns>Oposite face</returns>
	public static VoxelFace GetOpositeFace(VoxelFace face)
	{
		switch (face)
		{
			case VoxelFace.North:
				return VoxelFace.South;

			case VoxelFace.South:
				return VoxelFace.North;

			case VoxelFace.East:
				return VoxelFace.West;

			case VoxelFace.West:
				return VoxelFace.East;

			case VoxelFace.Up:
				return VoxelFace.Down;

			case VoxelFace.Down:
				return VoxelFace.Up;

			default:
				throw new ArgumentException("You have given me invalid face");
		}
	}
}

/// <summary>
/// Faces of the voxel
/// </summary>
public enum VoxelFace
{
	/// <summary>
	/// Z+
	/// </summary>
	North,

	/// <summary>
	/// Z-
	/// </summary>
	South,

	/// <summary>
	/// X+
	/// </summary>
	East,

	/// <summary>
	/// X-
	/// </summary>
	West,

	/// <summary>
	/// Y+
	/// </summary>
	Up,

	/// <summary>
	/// Y-
	/// </summary>
	Down
}