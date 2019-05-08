using System.Collections.Generic;
using UnityEngine;

public class VoxelMap : MonoBehaviour
{
	#region Singleton

	/// <summary>
	/// Globaly avaliable instance of the VoxelMap
	/// </summary>
	public static VoxelMap Instance
	{
		get
		{
			if (_Instance is null)
				new GameObject(nameof(VoxelMap)).AddComponent<VoxelMap>();
			return _Instance;
		}
	}



	/// <summary>
	/// Private instance of the VoxelMap
	/// </summary>
	private static VoxelMap _Instance;

	/// <summary>
	/// Awake function to set up the singleton
	/// </summary>
	private void Awake()
	{
		if (_Instance is null)
			_Instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}
		VoxelHelper.RegisterAllVoxels();
		VoxelTextureHelper.CreateTextueMap();
	}



	#endregion Singleton


	#region Fields

	[SerializeField] private Material _ChunkMaterial;

	#endregion
	/// <summary>
	/// Chunks in the world
	/// </summary>
	private Dictionary<Vector3Int, Chunk> _Chunks = new Dictionary<Vector3Int, Chunk>();

	public Material ChunkMaterial => _ChunkMaterial;

	/// <summary>
	/// Gets the type of the voxel from it's world position
	/// </summary>
	/// <param name="position">World position of the voxel</param>
	/// <returns></returns>
	public VoxelBase this[Vector3Int position] => _Chunks.TryGetValue(GetChunkPosition(position), out var chunk) ? chunk[position] : VoxelHelper.Empty;

	/// <summary>
	/// Calculates the position of the chunk from the world position of the voxel
	/// </summary>
	/// <param name="voxelPosition">World position of the voxel</param>
	/// <returns>Position of the coresponding chunk</returns>
	public static Vector3Int GetChunkPosition(Vector3Int voxelPosition) => new Vector3Int(Mathf.FloorToInt(voxelPosition.x / (float)Chunk.CHUNK_SIZE), Mathf.FloorToInt(voxelPosition.y / (float)Chunk.CHUNK_SIZE), Mathf.FloorToInt(voxelPosition.z / (float)Chunk.CHUNK_SIZE));

	/// <summary>
	/// Check if a chunk with given position exists
	/// If so, sets it to the <paramref name="chunk"/>
	/// Just a wrapper for TryGetValue of Dictionary
	/// </summary>
	/// <param name="position">Position of the chunk</param>
	/// <param name="chunk">The chunk if it exists</param>
	/// <returns>Does the chunk exist</returns>
	public bool TryGetChunk(Vector3Int position, out Chunk chunk) => _Chunks.TryGetValue(position, out chunk);

	/// <summary>
	/// Sets single voxel
	/// </summary>
	/// <param name="position">Position of the voxel</param>
	/// <param name="type">Type of the voxel</param>
	/// <param name="update">Should the map be updated after the set</param>
	/// <param name="context">Context for creating the voxel</param>
	public void SetVoxel(Vector3Int position, VoxelType type, bool update = true, object context = null)
	{
		// Check if chunk that would hold the block exists
		// If not, create one
		if (!TryGetChunk(GetChunkPosition(position), out var chunk))
		{
			chunk = Chunk.CreateChunk(GetChunkPosition(position));
			_Chunks.Add(chunk.Position, chunk);
		}

		// Sets the voxel
		chunk.SetVoxel(position, type, update, context);
	}

	/// <summary>
	/// Sets voxels from <paramref name="fromPosition"/> to <paramref name="toPosition"/> to a given type
	/// For testing purposes
	/// </summary>
	/// <param name="fromPosition">Start position</param>
	/// <param name="toPosition">End position</param>
	/// <param name="gridSize">Spaceing of the voxels</param>
	/// <param name="type">To what it should be change</param>
	public void SetVoxelGrid(Vector3Int fromPosition, Vector3Int toPosition, int gridSize, VoxelType type)
	{
		for (int x = fromPosition.x; x <= toPosition.x; x += gridSize)
		{
			for (int y = fromPosition.y; y <= toPosition.y; y += gridSize)
			{
				for (int z = fromPosition.z; z <= toPosition.z; z += gridSize)
				{
					SetVoxel(new Vector3Int(x, y, z), type, false);
				}
			}
		}
		UpdateMap();
	}

	/// <summary>
	/// Updates the map mesh so the voxels are visible
	/// </summary>
	public void UpdateMap()
	{
		// Update each chunk
		foreach (var chunk in _Chunks.Values)
		{
			chunk.UpdateChunk();
		}
	}

	private void Start()
	{
		SetVoxel(new Vector3Int(0, 0, 1), VoxelType.IronHull);
		SetVoxel(new Vector3Int(0, 0, 0), VoxelType.IronHull);
	}
}