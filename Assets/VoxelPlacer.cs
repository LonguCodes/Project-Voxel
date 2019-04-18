using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VoxelPlacer : MonoBehaviour
{
	/// <summary>
	/// The camera reference
	/// </summary>
	private Camera _Camera;

	/// <summary>
	/// Get the camera reference
	/// </summary>
	private void Start()
	{
		_Camera = GetComponent<Camera>();
	}

	/// <summary>
	/// Place and remove voxels
	/// </summary>
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
			Place(VoxelType.IronHull);
		if (Input.GetMouseButtonDown(1))
			Remove();
	}
	
	/// <summary>
	/// Places the voxel under the mouse cursor
	/// </summary>
	/// <param name="type">Type of the voxel</param>
	public void Place(VoxelType type)
	{
		// If there is nothing under the cursor, return
		if (!TryGetVoxelUnderCursor(out var position, out var direction))
			return;

		// Set the voxel
		VoxelMap.Instance.SetVoxel(position + direction, type);
	}

	/// <summary>
	/// Removes the voxel under the cursor
	/// </summary>
	public void Remove()
	{
		// If there is nothing under the cursor, return 
		if (!TryGetVoxelUnderCursor(out var position, out var direction))
			return;

		// Remove the voxel by setting it to empty
		VoxelMap.Instance.SetVoxel(position, VoxelType.Empty);
	}

	/// <summary>
	/// Checks if there is a voxel under the cursor
	/// If so, return it's position and the direction the face we struck is faceing
	/// </summary>
	/// <param name="position">Returned position of the voxel</param>
	/// <param name="direction">Returend direction of the face under the cursor</param>
	/// <returns>If there is any voxel under the cursor</returns>
	public bool TryGetVoxelUnderCursor(out Vector3Int position, out Vector3Int direction)
	{
		// Get the ray from under the cursor
		var ray = _Camera.ScreenPointToRay(Input.mousePosition);

		// Check if there is the ray will hit something
		if (!Physics.Raycast(ray, out var hit, 1000))
		{
			// If not, set position and direction to dummy values and return false
			position = Vector3Int.zero;
			direction = Vector3Int.zero;
			return false;
		}

		// If there is something bellow the cursor, cast the direction to ints (we know there are int but are stored as floats)
		direction = new Vector3Int((int)hit.normal.x, (int)hit.normal.y, (int)hit.normal.z);

		// Calculate the raw position (in floats)
		var rawPosition = hit.point - hit.normal * 0.5f;

		// Calculate the voxel positon (use rounding not casting to get corret result)
		position = new Vector3Int(Mathf.RoundToInt(rawPosition.x), Mathf.RoundToInt(rawPosition.y), Mathf.RoundToInt(rawPosition.z));

		// Return true becuase there was a voxel under the cursor
		return true;
	}
}