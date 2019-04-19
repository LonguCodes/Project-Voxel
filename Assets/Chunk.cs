using System;
using System.Collections.Generic;
using System.Linq;
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
	/// This holds info which blocks should be calculating when updating the chunk
	/// </summary>
	private List<(Vector3Int Position, VoxelFace Face)> _ToUpdate = new List<(Vector3Int, VoxelFace)>();

	/// <summary>
	/// Faces to render
	/// </summary>
	private Dictionary<Vector3Int, VoxelRenderData> _VoxelRenderData = new Dictionary<Vector3Int, VoxelRenderData>();

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
		chunk.gameObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
		chunk.Position = position;
		chunk.transform.position = position * CHUNK_SIZE;
		return chunk;
	}

	/// <summary>
	/// Check if the block is inside the chunk
	/// </summary>
	/// <param name="positon">Positon of the block, not the chunk</param>
	/// <returns></returns>
	public bool IsInsideChunk(Vector3Int positon)
	{
		var chunkPosition = VoxelMap.GetChunkPosition(positon);
		return chunkPosition == Position;
	}

	/// <summary>
	/// Calculates the releative position from world position
	/// </summary>
	/// <param name="worldPosition">World position of the voxel</param>
	/// <returns>Releative position of the voxel</returns>
	public Vector3Int ToRelativePosition(Vector3Int worldPosition)
	{
		return worldPosition - Position * CHUNK_SIZE;
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
				return _Voxels.TryGetValue(ToRelativePosition(position), out var state) ? state : VoxelType.Empty;
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

		// If it would change nothing, return
		if (this[position] == state)
			return;

		// Create a set of chunks to update
		var chunksToUpdate = new HashSet<Chunk>
		{
			this
		};

		// Go through each face and mak it as to update
		foreach (VoxelFace face in Enum.GetValues(typeof(VoxelFace)))
		{
			// Get the face direction
			var direction = MeshCreator.FaceToDirection(face);

			// Get the neighbour
			var neighbourPosition = position + direction;

			// Set the face to be updated
			SetToUpdate(position, face);

			// Try to get the neighbour. If the neighbour's chunk doesn't exits, skip
			if (!VoxelMap.Instance.TryGetChunk(VoxelMap.GetChunkPosition(neighbourPosition), out var chunk))
				continue;

			// Set the nerrighbout to update
			chunk.SetToUpdate(neighbourPosition, MeshCreator.GetOpositeFace(face));

			// Set the chunk to update if not set yet
			if (!chunksToUpdate.Contains(chunk))
				chunksToUpdate.Add(chunk);
		}

		// Calculate the releative position
		position = ToRelativePosition(position);

		// Set the voxel
		_Voxels[position] = state;


		// If shoudl update, update all marked chunks
		if (update)
			foreach (var chunk in chunksToUpdate)
				chunk.UpdateChunk();
	}

	/// <summary>
	/// Recalculates which faces should be rendered
	/// </summary>
	private void RecalaculateFaces()
	{
		// Go through each face marked as to update
		foreach (var voxelTuple in _ToUpdate)
		{
			// Unpack the tuple
			(var worldPosition, var face) = voxelTuple;

			// Calculate the relative positon
			var position = ToRelativePosition(worldPosition);

			// Check if the voxel exists and is not empty
			if (this[worldPosition] == VoxelType.Empty)
				continue;

			// Get the direction
			var direction = MeshCreator.FaceToDirection(face);

			// Get the neighbour state
			var neighbourState = VoxelMap.Instance[worldPosition + direction];

			// If the neighbour is empty, render the face
			if (neighbourState == VoxelType.Empty)
			{
				// If the render data of the voxel was not created already, create one
				if (!_VoxelRenderData.ContainsKey(position))
					_VoxelRenderData.Add(position, new VoxelRenderData(position, _Voxels[position]));

				// Set the face as rendered
				_VoxelRenderData[position].FacesToRended.Add(face);
			}

		}
		// Clear the to update because we already calculated all of them
		_ToUpdate.Clear();
	}

	/// <summary>
	/// Update the chunk's mech
	/// </summary>
	public void UpdateChunk()
	{
		// Recalculate which faces should be updated
		RecalaculateFaces();

		// Calculate the mesh
		var mesh = MeshCreator.CreateNewMesh(_VoxelRenderData.Values);

		// Set the mesh as rendered one and the collider
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	/// <summary>
	/// Set the face of the voxel to be potentially updated
	/// </summary>
	/// <param name="worldPositon">World postion of the voxel</param>
	/// <param name="face">Face of the voxel</param>
	public void SetToUpdate(Vector3Int worldPositon, VoxelFace face)
	{
		// Create a tuple to be added
		_ToUpdate.Add((worldPositon, face));

		//Calculate the relative positon of the voxel
		var relativePosition = ToRelativePosition(worldPositon);

		// If in the previous updated we maked the face as to render, we remove the mark because we don't know if it will still be rendered
		if (_VoxelRenderData.TryGetValue(relativePosition, out var renderData) && renderData.FacesToRended.Contains(face))
			_VoxelRenderData[relativePosition].FacesToRended.Remove(face);
	}
}