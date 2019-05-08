using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Morphling : VoxelBase
{
	public Morphling(Vector3Int position) : base(position)
	{
		Type = VoxelType.Morphling;
	}

	public Morphling(Vector3Int position, object context) : base(position, context)
	{
		if (context is VoxelType type)
			Type = type;
		else
			Type = VoxelType.Morphling;

	}

	public override VoxelType Type { get; }

	public override IEnumerable<(string Identifier, string Path)> Textures => new[] { ("Default", "Morphling.png") };
}

