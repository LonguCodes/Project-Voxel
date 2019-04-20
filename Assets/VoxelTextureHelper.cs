using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class VoxelTextureHelper
{
	public const int TEXTURE_SIZE_X = 16;
	public const int TEXTURE_SIZE_Y = 16;

	public static Vector2 UpOffset { get; private set; }
	public static Vector2 RightOffset { get; private set; }

	private static Dictionary<VoxelType, Vector2> _UVs = new Dictionary<VoxelType, Vector2>();

	public static Texture2D TextureMap { get; private set; }

	public static void CreateTextueMap()
	{
		var voxels = VoxelHelper.GetAllVoxels();
		var count = voxels.Count();
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
			var path = Path.Combine(Path.GetDirectoryName(Application.dataPath.Replace('/', '\\')), "Textures", voxel.TexturePath);
			if (!File.Exists(path))
				continue;

			var bytes = File.ReadAllBytes(path);
			var texture = new Texture2D(TEXTURE_SIZE_X, TEXTURE_SIZE_Y);
			texture.LoadImage(bytes);
			texture.Apply();
			var positionX = (counter % size) * TEXTURE_SIZE_X;
			var positionY = (counter / size) * TEXTURE_SIZE_Y;
			TextureMap.SetPixels(positionX, positionY, TEXTURE_SIZE_X, TEXTURE_SIZE_Y, texture.GetPixels());

			_UVs.Add(voxel.Type, new Vector2(positionX / (float)TextureMap.width, positionY / (float)TextureMap.height));
			counter++;
		}
		TextureMap.Apply();
	}

	public static Vector2 GetBaseUV(VoxelType type)
	{
		return _UVs[type];
	}
}