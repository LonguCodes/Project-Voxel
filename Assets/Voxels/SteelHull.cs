using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Steel hull class
/// The most basic class that can be
/// </summary>
public class SteelHull : VoxelBase
{
	#region Properties

	/// <summary>
	/// Override of the type
	/// </summary>
	public override VoxelType Type => VoxelType.SteelHull;

	/// <summary>
	/// Override of the texture path
	/// </summary>
	public override IEnumerable<(string,string)> Textures => new[] { ("Default","SteelHull.png") };

	#endregion

	#region Constructors

	/// <summary>
	/// Default constructor override 
	/// </summary>
	/// <param name="position"></param>
	public SteelHull(Vector3Int position) : base(position)
	{
	}

	#endregion
}