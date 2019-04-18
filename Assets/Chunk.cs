using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	/// <summary>
	/// The size of the chunk in voxels
	/// </summary>
	public const int CHUNK_SIZE = 13;

	/// <summary>
	/// Voxels in the chunk
	/// </summary>
	private Dictionary<Vector3Int, VoxelType> _Voxels = new Dictionary<Vector3Int, VoxelType>();

	/// <summary>
	/// Position of the chunk
	/// </summary>
	public Vector3Int Position { get; private set; }

	/// <summary>
	/// Creates a new chunk
	/// </summary>
	/// <param name="position">Position of the chunk</param>
	/// <returns>Newly createch chunk</returns>
	public static Chunk CreateChunk(Vector3Int position)
	{
		var chunk = new GameObject($"Chunk : {position}").AddComponent<Chunk>();
		chunk.gameObject.AddComponent<MeshFilter>();
		chunk.gameObject.AddComponent<MeshRenderer>();
		chunk.gameObject.AddComponent<MeshCollider>();
		chunk.Position = position;
		return chunk;
	}

	/// <summary>
	/// Check if the block is inside the chunk
	/// </summary>
	/// <param name="positon">Positon of the block, not the chunk</param>
	/// <returns></returns>
	public bool IsInsideChunk(Vector3Int positon)
	{
		var chunkPosition = new Vector3Int(Mathf.FloorToInt(positon.x / (float)CHUNK_SIZE), Mathf.FloorToInt(positon.y / (float)CHUNK_SIZE), Mathf.FloorToInt(positon.z / (float)CHUNK_SIZE));
		return chunkPosition == Position;
	}

	/// <summary>
	/// Calculates the releative position from world position
	/// </summary>
	/// <param name="worldPosition">World position of the voxel</param>
	/// <returns>Releative position of the voxel</returns>
	public Vector3Int ToReleativePosition(Vector3Int worldPosition)
	{
		return new Vector3Int(worldPosition.x % CHUNK_SIZE, worldPosition.y % CHUNK_SIZE, worldPosition.z % CHUNK_SIZE);
	}

	/// <summary>
	/// Calculates the world position from releative position
	/// </summary>
	/// <param name="releativePosition">Releative position of the voxel</param>
	/// <returns>World position of the voxel</returns>
	public Vector3Int ToWorldPositon(Vector3Int releativePosition)
	{
		return Position * CHUNK_SIZE + releativePosition;
	}

	/// <summary>
	/// Indexer for getting voxles in the chunk
	/// </summary>
	/// <param name="position">World position of the voxel</param>
	/// <returns></returns>
	public VoxelType this[Vector3Int position]
	{
		get
		{
			// If the voxel is in the chunk, return it
			if (IsInsideChunk(position))
				return _Voxels.TryGetValue(ToReleativePosition(position), out var state) ? state : VoxelType.Empty;
			// If not, redirect it to the VoxelMap
			return VoxelMap.Instance[position];
		}
	}

	/// <summary>
	/// Sets the voxel in the chunk
	/// </summary>
	/// <param name="position"></param>
	/// <param name="state"></param>
	/// <param name="update"></param>
	public void SetVoxel(Vector3Int position, VoxelType state, bool update = true)
	{
		// If the voxels wouldn't be in the chunk, redirect to VoxelMap
		if (!IsInsideChunk(position))
		{
			VoxelMap.Instance.SetVoxel(position, state, update);
			return;
		}
		
		// Calculate the releative position
		position = ToReleativePosition(position); // CHANGED THAT!!!

		// If it would change nothing, return
		if (this[position] == state)
			return;

		// Set the voxel
		_Voxels[position] = state;

		// If should update the chunk, update it
		if (update)
			UpdateChunk();
	}

	/// <summary>
	/// Update the chunk's mech
	/// </summary>
	public void UpdateChunk()
	{
		// Calculate the mesh
		var mesh = MeshCreator.CreateNewMesh(_Voxels);

		// Set the mesh as rendered one and the collider
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
	}
}