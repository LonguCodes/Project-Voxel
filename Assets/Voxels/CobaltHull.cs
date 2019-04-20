using System.Linq;
using UnityEngine;

/// <summary>
/// Cobalt hull class
/// More customized one
/// </summary>
public class CobaltHull : VoxelBase
{
	#region Properties

	/// <summary>
	/// Override of the path
	/// </summary>
	public override string TexturePath => "CobaltHull.png";

	/// <summary>
	/// Override of the type
	/// </summary>
	public override VoxelType Type => VoxelType.CobaltHull;

	#endregion

	#region Constructors

	/// <summary>
	/// Default constructor override
	/// </summary>
	/// <param name="position"></param>
	public CobaltHull(Vector3Int position) : base(position)
	{
	}

	#endregion

	#region Methods

	/// <summary>
	/// We render the cube with half the size
	/// </summary>
	/// <param name="face"></param>
	/// <param name="data"></param>
	public override void CreateFace(VoxelFace face, ref MeshData data)
	{
		// Offset all the vertecies accorting to position
		var faceVertecies = GetCubeVerticies(face).Select(vertexPosition => vertexPosition * 0.5f + Postion);

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

		// Calculate the base UV
		var baseUV = VoxelTextureHelper.GetBaseUV(VoxelType.CobaltHull);

		// Apply the UVs
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

	#endregion

	#region Functions

	/// <summary>
	/// The voxel is not full so we allways render it
	/// </summary>
	/// <param name="face"></param>
	/// <returns></returns>
	public override bool ShouldAllwaysRender(VoxelFace face) => true;

	/// <summary>
	/// The voxel is not full so we allways render the neighbour
	/// </summary>
	/// <param name="face"></param>
	/// <returns></returns>
	public override bool ShouldAllwaysRenderNeighbour(VoxelFace face) => true;

	#endregion
}