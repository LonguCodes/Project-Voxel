using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VoxelHelper
{
	public static VoxelBase CreateVoxel(Vector3Int position, VoxelType type, object context = null)
	{
		if (context is null)
			return (VoxelBase)Activator.CreateInstance(_VoxelTypes[type], position);
		return (VoxelBase)Activator.CreateInstance(_VoxelTypes[type], position, context);
	}

	private static VoxelBase _Empty;

	public static VoxelBase Empty
	{
		get
		{
			if (_Empty == null)
				_Empty = CreateVoxel(Vector3Int.zero, VoxelType.Empty);
			return _Empty;
		}
	}

	private static Dictionary<VoxelType, Type> _VoxelTypes = new Dictionary<VoxelType, Type>();

	public static void RegisterVoxelType(VoxelType voxelType, Type type)
	{
		_VoxelTypes.Add(voxelType, type);
	}

	public static void RegisterAllVoxels()
	{

		_VoxelTypes = typeof(VoxelBase).Assembly.GetTypes()
			.Where(x => x.IsSubclassOf(typeof(VoxelBase)) && !x.IsAbstract)
			.ToDictionary(x => ((VoxelBase)Activator.CreateInstance(x, Vector3Int.zero)).Type, x => x);
	}

	public static IEnumerable<VoxelBase> GetAllVoxels()
	{
		foreach (var voxelType in _VoxelTypes)
		{
			yield return (VoxelBase)Activator.CreateInstance(voxelType.Value, Vector3Int.zero);
		}
	}
}