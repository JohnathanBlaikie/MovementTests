using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTriangle 
{
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    int submeshIndex;

    public List<Vector3> Vert { get { return vertices; } set { vertices = value; } }
    public List<Vector3> Norm { get { return normals; } set { normals = value; } }
    public List<Vector2> UV { get { return uvs; } set { uvs = value; } }
    public int SMIndex { get { return submeshIndex; } set { SMIndex = value; } }

    public MeshTriangle(Vector3[] _vertices, Vector3[] _normals, Vector2[] _uvs, int _submeshIndex)
    {
        vertices.AddRange(_vertices);
        normals.AddRange(_normals);
        uvs.AddRange(_uvs);

        submeshIndex = _submeshIndex;
    }

    public void Clear()
    {
        vertices.Clear();
        normals.Clear();
        uvs.Clear();

        submeshIndex = 0;
    }
}
