using System.Collections.Generic;

public struct VoxelRenderData
{
	public VoxelBase Voxel { get; }

	/// <summary>
	/// Faces to render
	/// </summary>
	public HashSet<VoxelFace> FacesToRended { get; }

	/// <summary>
	/// Default constructor
	/// </summary>
	/// <param name="position">Position of the voxel</param>
	/// <param name="type">Type of the voxel</param>
	public VoxelRenderData(VoxelBase voxel) : this()
	{
		Voxel = voxel;
		FacesToRended = new HashSet<VoxelFace>();
	}
}