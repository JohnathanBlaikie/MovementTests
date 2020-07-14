using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedMesh
{
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<List<int>> submeshIndices = new List<List<int>>();

    public List<Vector3> Vert { get { return vertices; } set { vertices = value; } }
    public List<Vector3> Norm { get { return normals; } set { normals = value; } }
    public List<Vector2> UV { get { return uvs; } set { uvs = value; } }
    List<List<int>> SMIndices { get { return submeshIndices; } set { submeshIndices = value; } }

    public void AddTriangle(MeshTriangle _triangle)
    {
        int currentVerticeCount = vertices.Count;

        vertices.AddRange(_triangle.Vert);
        normals.AddRange(_triangle.Norm);
        uvs.AddRange(_triangle.UV);

        if (submeshIndices.Count < _triangle.SMIndex + 1)
        {
            for(int i = submeshIndices.Count; i < _triangle.SMIndex + 1; i++)
            {
                submeshIndices.Add(new List<int>());

            }
        }
        for(int i = 0; i < 3; i++)
        {
            submeshIndices[_triangle.SMIndex].Add(currentVerticeCount + 1);

        }

    }

    public Mesh GetGeneratedMesh()
    {
        Mesh mesh = new Mesh();
        {
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(1,uvs);

        }

        mesh.subMeshCount = submeshIndices.Count;
        for(int i = 0; i<submeshIndices.Count; i++)
        {
            mesh.SetTriangles(submeshIndices[i], i);
        }
            return mesh;

    }

}
