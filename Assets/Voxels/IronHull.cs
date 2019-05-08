using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iron hull class
/// The most basic class that can be
/// </summary>
public class IronHull : VoxelBase
{
	#region Properties

	/// <summary>
	/// Override of the type
	/// </summary>
	public override VoxelType Type => VoxelType.IronHull;

	/// <summary>
	/// Override of the texture path
	/// </summary>
	public override IEnumerable<(string,string)> Textures => new[] { ("Default","IronHull.png") };

	#endregion

	#region Constructors

	/// <summary>
	/// Default constructor override 
	/// </summary>
	/// <param name="position"></param>
	public IronHull(Vector3Int position) : base(position)
	{
	}

	#endregion
}