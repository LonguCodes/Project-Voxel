using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Base class for all voxel classes
/// </summary>
public abstract class VoxelBase
{
	#region Properties

	/// <summary>
	/// Position of the voxel.
	/// Set when creating the voxel
	/// </summary>
	public Vector3Int Position { get; }

	/// <summary>
	/// Relative position of the voxel inside chunk
	/// </summary>
	public Vector3Int RelativePosition { get; }

	/// <summary>
	/// Type of the voxel
	/// Used as a Id
	/// Has to be set from child
	/// </summary>
	public abstract VoxelType Type { get; }

	/// <summary>
	/// Path to the texture
	/// Has to be set from child
	/// </summary>
	public abstract IEnumerable<(string Identifier, string Path)> Textures { get; }

	private string _DefaultUVPath { get;  }

	#endregion

	#region Constructors

	/// <summary>
	/// Default constructor
	/// Not public because the function is abstract
	/// </summary>
	/// <param name="position"></param>
	protected VoxelBase(Vector3Int position)
	{
		Position = position;
		RelativePosition = Chunk.ToRelativePosition(position);
		_DefaultUVPath = Textures.FirstOrDefault().Identifier;
	}

	/// <summary>
	/// Context constructor
	/// Not public because the function is abstract
	/// </summary>
	/// <param name="position"></param>
	protected VoxelBase(Vector3Int position, object context):this(position)
	{
		
	}

	#endregion

	#region Methods

	/// <summary>
	/// Creates a face
	/// Can bo overwriten for custom rendering
	/// </summary>
	/// <param name="face">Face to be rendered</param>
	/// <param name="data">Data holder to put the face render data into</param>
	public virtual void CreateFace(VoxelFace face, ref MeshData data)
	{
		// Offset all the vertecies accorting to position
		var faceVertecies = GetCubeVerticies(face).Select(vertexPosition => vertexPosition + RelativePosition);

		var currentIndex = data.Vertices.Count;

		// Add our new vertices
		data.Vertices.AddRange(faceVertecies);

		// Add normals
		var normal = (Vector3)MeshCreator.FaceToDirection(face);

		data.Normals.AddRange(
			new[]{
				normal,
				normal,
				normal,
				normal
			}
		);

		// Create 1st face
		data.Triangles.AddRange(
			new[]
			{
				currentIndex,
				currentIndex+1,
				currentIndex+2
			}
		);

		// Create 2nd face
		data.Triangles.AddRange(
			new[]
			{
				currentIndex,
				currentIndex+2,
				currentIndex+3
			}
		);

		if (_DefaultUVPath is null)
			return;

		// Calculate the base uv
		var baseUV = VoxelTextureHelper.GetBaseUV(Type,_DefaultUVPath);

		// Apply the uvs
		data.UVs.AddRange(
			new[]
			{
				baseUV,
				baseUV + VoxelTextureHelper.UpOffset,
				baseUV + VoxelTextureHelper.UpOffset + VoxelTextureHelper.RightOffset,
				baseUV + VoxelTextureHelper.RightOffset,
			}
		);
	}

	#endregion Methods

	#region Functions

	/// <summary>
	/// Helper function if you just want to render a cube
	/// </summary>
	/// <param name="face">Face to get the vertices for</param>
	/// <returns></returns>
	protected static IEnumerable<Vector3> GetCubeVerticies(VoxelFace face)
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
					new Vector3(-0.5f,-0.5f,-0.5f),
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
	/// Should given face allways be rendered
	/// </summary>
	/// <param name="face">Face to be potentialy rendered</param>
	/// <returns></returns>
	public virtual bool ShouldAllwaysRender(VoxelFace face)
	{
		return false;
	}

	/// <summary>
	/// Should the neighbours face allways be rendered
	/// </summary>
	/// <param name="face">Face which the neighbour is touching</param>
	/// <returns></returns>
	public virtual bool ShouldAllwaysRenderNeighbour(VoxelFace face)
	{
		return false;
	}

	#endregion Functions
}