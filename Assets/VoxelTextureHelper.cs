using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class VoxelTextureHelper
{
	/// <summary>
	/// Size of the texture - x
	/// </summary>
	public const int TEXTURE_SIZE_X = 16;
	
	/// <summary>
	/// Size of the texture - y
	/// </summary>
	public const int TEXTURE_SIZE_Y = 16;

	/// <summary>
	/// Size of a single texture - Y
	/// </summary>
	public static Vector2 UpOffset { get; private set; }
	
	/// <summary>
	/// Size of a single texture - X
	/// </summary>
	public static Vector2 RightOffset { get; private set; }

	/// <summary>
	/// Map of all the base UVs
	/// </summary>
	private static Dictionary<VoxelType, Dictionary<string,Vector2>> _UVs = new Dictionary<VoxelType, Dictionary<string, Vector2>>();

	public static Texture2D TextureMap { get; private set; }

	public static void CreateTextueMap()
	{
		var voxels = VoxelHelper.GetAllVoxels();
		var count = voxels.Sum(x=>x.Textures.Count());
		
		var size = Mathf.CeilToInt(Mathf.Sqrt(count));

		UpOffset = new Vector2(0, 1f / size);
		RightOffset = new Vector2(1f / size, 0);

		TextureMap = new Texture2D(size * TEXTURE_SIZE_X, size * TEXTURE_SIZE_Y)
		{
			filterMode = FilterMode.Point
		};

		var counter = 0;

		foreach (var voxel in voxels)
		{
			_UVs.Add(voxel.Type, new Dictionary<string, Vector2>());
			foreach ((var identifier, var texturePath) in voxel.Textures)
			{
				var path = Path.Combine(Path.GetDirectoryName(Application.dataPath.Replace('/', '\\')), "Textures", texturePath);
				if (!File.Exists(path))
					continue;
				var bytes = File.ReadAllBytes(path);
				var texture = new Texture2D(TEXTURE_SIZE_X, TEXTURE_SIZE_Y);
				texture.LoadImage(bytes);
				texture.Apply();
				var positionX = (counter % size) * TEXTURE_SIZE_X;
				var positionY = (counter / size) * TEXTURE_SIZE_Y;
				TextureMap.SetPixels(positionX, positionY, TEXTURE_SIZE_X, TEXTURE_SIZE_Y, texture.GetPixels());

				_UVs[voxel.Type].Add(identifier, new Vector2(positionX / (float)TextureMap.width, positionY / (float)TextureMap.height));
				counter++;
			}
		}
		TextureMap.Apply();
	}

	public static Vector2 GetBaseUV(VoxelType type,string identifier = "Default")
	{
		return _UVs[type][identifier];
	}
}