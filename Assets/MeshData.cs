using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
	public List<int> Triangles { get; private set; } = new List<int>();
	public List<Vector3> Normals { get; private set; } = new List<Vector3>();
	public List<Vector2> UVs { get; private set; } = new List<Vector2>();
}