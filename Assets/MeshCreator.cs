using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshCreator
{
	public static Mesh CreateNewMesh(Dictionary<Vector3Int, VoxelType> voxels)
	{
		// Initialize vertices and triangles

		var vertices = new List<Vector3>();
		var triangles = new List<int>();
		var normals = new List<Vector3>();

		// Create all the faces

		foreach (var voxel in voxels)
		{
			foreach (VoxelFace face in Enum.GetValues(typeof(VoxelFace)))
			{
				var direction = FaceToDirection(face);

				var neighbourState = voxels.TryGetValue(voxel.Key + direction, out var state) ? state : VoxelType.Empty;

				if (neighbourState == VoxelType.Empty)
					CreateFace(voxel.Key, face, ref vertices, ref triangles, ref normals);
			}
		}

		// Remeber to allways set the vertices first
		// If not if will create errors

		return new Mesh()
		{
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray(),
			normals = normals.ToArray(),
		};
	}


	private static void CreateFace(Vector3Int position, VoxelFace face, ref List<Vector3> meshVertices, ref List<int> meshTriangles, ref List<Vector3> meshNormals)
	{
		// Offset all the vertecies accorting to position
		var faceVertecies = FaceToVertices(face).Select(vertexPosition => vertexPosition + position);

		var currentIndex = meshVertices.Count;

		// Add our new vertices

		meshVertices.AddRange(faceVertecies);

		// Add normlas (which is easy)

		var normal = (Vector3)FaceToDirection(face);


		meshNormals.AddRange(
			new[]{
				normal,
				normal,
				normal,
				normal
			}
		);

		// Create 1st face

		meshTriangles.AddRange(
			new[]
			{
				currentIndex,
				currentIndex+1,
				currentIndex+2
			}
		);

		// Create 2nd face

		meshTriangles.AddRange(
			new[]
			{
				currentIndex,
				currentIndex+2,
				currentIndex+3
			}
		);


	}

	private static IEnumerable<Vector3> FaceToVertices(VoxelFace face)
	{
		switch (face)
		{
			case VoxelFace.North:
				return new[]
				{
					new Vector3(0.5f,0.5f,0.5f),
					new Vector3(-0.5f,0.5f,0.5f),
					new Vector3(-0.5f,-0.5f,0.5f),
					new Vector3(0.5f,-0.5f,0.5f),
				};

			case VoxelFace.South:
				return new[]
				{
					new Vector3(0.5f,0.5f,-0.5f),
					new Vector3(0.5f,-0.5f,-0.5f),
					new Vector3(-0.5f,-0.5f,-0.5f),
					new Vector3(-0.5f,0.5f,-0.5f),
				};

			case VoxelFace.East:
				return new[]
				{
					new Vector3(0.5f,0.5f,0.5f),
					new Vector3(0.5f,-0.5f,0.5f),
					new Vector3(0.5f,-0.5f,-0.5f),
					new Vector3(0.5f,0.5f,-0.5f),
				};

			case VoxelFace.West:
				return new[]
				{
					new Vector3(-0.5f,0.5f,0.5f),
					new Vector3(-0.5f,0.5f,-0.5f),
					new Vector3(-0.5f,-0.5f,-0.5f),
					new Vector3(-0.5f,-0.5f,0.5f),
				};

			case VoxelFace.Up:
				return new[]
				{
					new Vector3(0.5f,0.5f,0.5f),
					new Vector3(0.5f,0.5f,-0.5f),
					new Vector3(-0.5f,0.5f,-0.5f),
					new Vector3(-0.5f,0.5f,0.5f),
				};

			case VoxelFace.Down:
				return new[]
				{
					new Vector3(0.5f,-0.5f,0.5f),
					new Vector3(-0.5f,-0.5f,0.5f),
					new Vector3(-0.5f,-0.5f,-0.5f),
					new Vector3(0.5f,-0.5f,-0.5f),
				};

			default:
				throw new ArgumentException("You have given me invalid face");
		}
	}

	private static Vector3Int FaceToDirection(VoxelFace face)
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

	private enum VoxelFace
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
}