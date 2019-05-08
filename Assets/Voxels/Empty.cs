using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dummy class for empty voxels
/// </summary>
public class Empty : VoxelBase
{
	#region Properties

	/// <summary>
	/// Override of the type
	/// </summary>
	public override VoxelType Type => VoxelType.Empty;

	/// <summary>
	/// Override of the texture path
	/// We don't care what's here
	/// </summary>
	public override IEnumerable<(string,string)> Textures => new (string,string)[0];

	#endregion

		#region Constructors

		/// <summary>
		/// Default constructor override
		/// </summary>
		/// <param name="position"></param>
	public Empty(Vector3Int position) : base(position)
	{
	}

	#endregion

	#region Methods

	/// <summary>
	/// We override the function so there will be no rendering of empty voxels
	/// </summary>
	/// <param name="face">Face to be rendered</param>
	/// <param name="data"></param>
	public override void CreateFace(VoxelFace face, ref MeshData data)
	{
	}

	#endregion

	#region Functions

	/// <summary>
	/// Allways return true because the voxel is empty
	/// </summary>
	/// <param name="face"></param>
	/// <returns></returns>
	public override bool ShouldAllwaysRenderNeighbour(VoxelFace face) => true;

	#endregion
}