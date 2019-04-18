using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshCreator
{
	/// <summary>
	/// X dimension of the image with all textures
	/// </summary>
	private const int TEXTURE_DIMENSION_X = 3;

	/// <summary>
	/// Y dimension of the image with all textures
	/// </summary>
	private const int TEXTURE_DIMENSION_Y = 1;


	/// <summary>
	/// Create a new mesh for the chunk
	/// </summary>
	/// <param name="voxels">Voxels of the chunk</param>
	/// <returns></returns>
	public static Mesh CreateNewMesh(Dictionary<Vector3Int, VoxelType> voxels)
	{
		// Initialize vertices, triangles, normals and UVs

		var vertices = new List<Vector3>();
		var triangles = new List<int>();
		var normals = new List<Vector3>();
		var uvs = new List<Vector2>();

		// Create all the faces

		foreach (var voxel in voxels.Where(x => x.Value != VoxelType.Empty))
		{
			foreach (VoxelFace face in Enum.GetValues(typeof(VoxelFace)))
			{
				var direction = FaceToDirection(face);

				var neighbourState = voxels.TryGetValue(voxel.Key + direction, out var state) ? state : VoxelType.Empty;

				if (neighbourState == VoxelType.Empty)
					CreateFace(voxel.Key, voxel.Value, face, ref vertices, ref triangles, ref normals, ref uvs);
			}
		}

		// Remeber to allways set the vertices first
		// If not if will create errors

		return new Mesh
		{
			vertices = vertices.ToArray(),
			triangles = triangles.ToArray(),
			normals = normals.ToArray(),
			uv = uvs.ToArray(),
		};
	}

	/// <summary>
	/// Creates a single face
	/// </summary>
	/// <param name="position">Position of the voxel</param>
	/// <param name="type">Type of the voxel</param>
	/// <param name="face">Direction of the face</param>
	/// <param name="meshVertices">List of vertecies</param>
	/// <param name="meshTriangles">List of triangles</param>
	/// <param name="meshNormals">List of normals</param>
	/// <param name="meshUV">List of UVs</param>
	private static void CreateFace(Vector3Int position, VoxelType type, VoxelFace face, ref List<Vector3> meshVertices, ref List<int> meshTriangles, ref List<Vector3> meshNormals, ref List<Vector2> meshUV)
	{
		// Offset all the vertecies accorting to position
		var faceVertecies = FaceToVertices(face).Select(vertexPosition => vertexPosition + position);

		var currentIndex = meshVertices.Count;

		// Add our new vertices
		meshVertices.AddRange(faceVertecies);

		// Add normals 
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

		// Calculate the base UV (bottom left correr)
		var baseUV = TypeToUVBase(type);

		// Add the UVs
		meshUV.AddRange(new[]
			{
				baseUV,
				baseUV + Vector2.up/TEXTURE_DIMENSION_Y,
				baseUV + Vector2.up/TEXTURE_DIMENSION_Y + Vector2.right/TEXTURE_DIMENSION_X,
				baseUV + Vector2.right/TEXTURE_DIMENSION_X,
			}
		);
	}

	/// <summary>
	/// Gets the verticies of the face
	/// UPDATED - all verticies start from top left
	/// </summary>
	/// <param name="face"></param>
	/// <returns></returns>
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
					new Vector3(-0.5f,0.5f,-0.5f),
					new Vector3(0.5f,0.5f,-0.5f),
					new Vector3(0.5f,-0.5f,-0.5f),
					new Vector3(-0.5f,0.5f,-0.5f),
				};

			case VoxelFace.East:
				return new[]
				{
					new Vector3(0.5f,0.5f,-0.5f),
					new Vector3(0.5f,0.5f,0.5f),
					new Vector3(0.5f,-0.5f,0.5f),
					new Vector3(0.5f,-0.5f,-0.5f),
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
					new Vector3(-0.5f,0.5f,0.5f),
					new Vector3(0.5f,0.5f,0.5f),
					new Vector3(0.5f,0.5f,-0.5f),
					new Vector3(-0.5f,0.5f,-0.5f),
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
	/// Temporary function for getting the correct texture
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private static Vector2 TypeToUVBase(VoxelType type)
	{
		switch (type)
		{
			case VoxelType.Empty:
				return Vector2.zero;

			case VoxelType.IronHull:
				return new Vector2(2 / 3f, 0);

			case VoxelType.SteelHull:
				return new Vector2(0, 0);

			case VoxelType.CobaltHull:
				return new Vector2(1 / 3f, 0);

			default:
				throw new ArgumentException("You have given me invalid voxel type");
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
}