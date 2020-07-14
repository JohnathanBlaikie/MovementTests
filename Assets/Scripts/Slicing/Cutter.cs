using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter
{

    public static bool currentCut;
    public static Mesh originalMesh;

    public static void Cut(GameObject oGO, Vector3 _contactPoint,
        Vector3 _direction, Material _cutMaterial = null,
        bool fill = true, bool _addRigidbody = false)
    {
        if(currentCut)
        {
            return;
        }

        currentCut = true;

        Plane plane = new Plane(oGO.transform.InverseTransformDirection(-_direction),
            oGO.transform.InverseTransformPoint(_contactPoint));
        originalMesh = oGO.GetComponent<MeshFilter>().mesh;
        List<Vector3> addedVertices = new List<Vector3>();

        GeneratedMesh leftMesh = new GeneratedMesh();
        GeneratedMesh rightMesh = new GeneratedMesh();

        int[] submeshIndices;
        int triangleIndexA, triangleIndexB, triangleIndexC;


        for (int i = 0; i< originalMesh.subMeshCount; i++)
        {
            submeshIndices = originalMesh.GetTriangles(i);
            for (int j = 0; j < originalMesh.subMeshCount; j+= 3)
            {
                triangleIndexA = submeshIndices[j];
                triangleIndexB = submeshIndices[j + 1];
                triangleIndexC = submeshIndices[j + 2];

                MeshTriangle currentTriangle = GetTriangle(triangleIndexA, triangleIndexB, triangleIndexC, i);
                bool triangleALeftSide = plane.GetSide(originalMesh.vertices[triangleIndexA]);
                bool triangleBLeftSide = plane.GetSide(originalMesh.vertices[triangleIndexB]);
                bool triangleCLeftSide = plane.GetSide(originalMesh.vertices[triangleIndexC]);

                if (triangleALeftSide && triangleBLeftSide && triangleCLeftSide)
                {
                    leftMesh.AddTriangle(currentTriangle);
                }
                else if (!triangleALeftSide && !triangleBLeftSide && !triangleCLeftSide)
                {
                    rightMesh.AddTriangle(currentTriangle);
                }
                else
                {
                    
                }

            }

        }
    }

    private static MeshTriangle GetTriangle(int tIA, int tIB, int tIC, int smIdx)
    {

        Vector3[] verticesToAdd = new Vector3[]
        {
            originalMesh.vertices[tIA],
            originalMesh.vertices[tIB],
            originalMesh.vertices[tIC]
        };
        Vector3[] normalsToAdd = new Vector3[]
        {
            originalMesh.normals[tIA],
            originalMesh.normals[tIB],
            originalMesh.normals[tIC]
        };
        Vector2[] uvsToAdd = new Vector2[]
         {
            originalMesh.uv[tIA],
            originalMesh.uv[tIB],
            originalMesh.uv[tIC]
         };
        return new MeshTriangle(verticesToAdd, normalsToAdd, uvsToAdd, smIdx);
    }

    private static void CutTriangle(Plane _plane, MeshTriangle meshTri, bool tALS, bool TBLS, bool TCLS, 
        GeneratedMesh lSide, GeneratedMesh rSide, List<Vector3> addedVert)
    {
        List<bool> leftSide = new List<bool>();
        leftSide.Add(tALS);
        leftSide.Add(TBLS);
        leftSide.Add(TCLS);

        MeshTriangle leftMeshTriangle = new MeshTriangle(new Vector3[2], new Vector3[2], new Vector2[2], meshTri.SMIndex);
        MeshTriangle rightMeshTriangle = new MeshTriangle(new Vector3[2], new Vector3[2], new Vector2[2], meshTri.SMIndex);

        bool left = false;
        bool right = false;

        for (int i = 0; i <3; i++)
        {
            if(leftSide[i])
            {
                if(!left)
                {
                    left = true;

                    leftMeshTriangle.Vert[0] = meshTri.Vert[i];
                    leftMeshTriangle.Vert[1] = leftMeshTriangle.Vert[0];

                    leftMeshTriangle.UV[0] = meshTri.UV[i];
                    leftMeshTriangle.UV[1] = leftMeshTriangle.UV[0];

                    leftMeshTriangle.Norm[0] = meshTri.Norm[i];
                    leftMeshTriangle.Norm[1] = leftMeshTriangle.Norm[0];

                }
                else
                {
                    leftMeshTriangle.Vert[1] = meshTri.Vert[i];
                    leftMeshTriangle.Norm[1] = meshTri.Vert[i];
                    leftMeshTriangle.UV[1] = meshTri.Vert[i];


                }
            }
            else
            {
                if (!right)
                {
                    right = true;

                    rightMeshTriangle.Vert[0] = meshTri.Vert[i];
                    rightMeshTriangle.Vert[1] = rightMeshTriangle.Vert[0];

                    rightMeshTriangle.UV[0] = meshTri.UV[i];
                    rightMeshTriangle.UV[1] = rightMeshTriangle.UV[0];

                    rightMeshTriangle.Norm[0] = meshTri.Norm[i];
                    rightMeshTriangle.Norm[1] = rightMeshTriangle.Norm[0];

                }
                else
                {
                    rightMeshTriangle.Vert[1] = meshTri.Vert[i];
                    rightMeshTriangle.Norm[1] = meshTri.Vert[i];
                    rightMeshTriangle.UV[1] = meshTri.Vert[i];


                }
            }
        }

        float normalizedDistance;
        float distance;

        _plane.Raycast(new Ray(leftMeshTriangle.Vert[0],
            (rightMeshTriangle.Vert[0] - leftMeshTriangle.Vert[0]).normalized), out distance);

        normalizedDistance = distance / (rightMeshTriangle.Vert[0] - leftMeshTriangle.Vert[0]).magnitude;
        Vector3 vertLeft = Vector3.Lerp(leftMeshTriangle.Vert[0], rightMeshTriangle.Vert[0], normalizedDistance);
        addedVert.Add(vertLeft);

        Vector3 normalLeft = Vector3.Lerp(leftMeshTriangle.Norm[0], rightMeshTriangle.Norm[0], normalizedDistance);
        Vector2 uvLeft = Vector2.Lerp(leftMeshTriangle.UV[0], rightMeshTriangle.UV[0], normalizedDistance);

        _plane.Raycast(new Ray(leftMeshTriangle.Vert[1],
            (rightMeshTriangle.Vert[1] - leftMeshTriangle.Vert[1]).normalized), out distance);

        normalizedDistance = distance / (rightMeshTriangle.Vert[1] - leftMeshTriangle.Vert[1]).magnitude;
        Vector3 vertRight = Vector3.Lerp(leftMeshTriangle.Vert[1], rightMeshTriangle.Vert[1], normalizedDistance);
        addedVert.Add(vertLeft);

        Vector3 normalRight = Vector3.Lerp(leftMeshTriangle.Norm[1], rightMeshTriangle.Norm[1], normalizedDistance);
        Vector2 uvRight = Vector2.Lerp(leftMeshTriangle.UV[1], rightMeshTriangle.UV[1], normalizedDistance);

        MeshTriangle currentTriangle;
        Vector3[] updatedVert = new Vector3[] { leftMeshTriangle.Vert[0], vertLeft, vertRight };
        Vector3[] updatedNorm = new Vector3[] { leftMeshTriangle.Norm[0], normalLeft, normalRight };
        Vector2[] updatedUV = new Vector2[] { leftMeshTriangle.UV[0], uvLeft, uvRight };

        currentTriangle = new MeshTriangle(updatedVert, updatedNorm, updatedUV, meshTri.SMIndex);

        if(updatedVert[0] != updatedVert[1] && updatedVert[0] != updatedVert[2])
        {
            if(Vector3.Dot(Vector3.Cross(updatedVert[1] - updatedVert[0],updatedVert[2] - updatedVert[0]), updatedVert[0]) <0)
            {
                FlipTriangle(currentTriangle);

            }
            lSide.AddTriangle(currentTriangle);

        }

        updatedVert = new Vector3[] { leftMeshTriangle.Vert[0], leftMeshTriangle.Vert[1], vertRight };
        updatedNorm = new Vector3[] { leftMeshTriangle.Norm[0], leftMeshTriangle.Norm[1], normalRight };
        updatedUV = new Vector2[] { leftMeshTriangle.UV[0], uvLeft, uvRight };

        currentTriangle = new MeshTriangle(updatedVert, updatedNorm, updatedUV, meshTri.SMIndex);

        if (updatedVert[0] != updatedVert[1] && updatedVert[0] != updatedVert[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVert[1] - updatedVert[0], updatedVert[2] - updatedVert[0]), updatedVert[0]) < 0)
            {
                FlipTriangle(currentTriangle);
            }
            lSide.AddTriangle(currentTriangle);

        }

        updatedVert = new Vector3[] { rightMeshTriangle.Vert[0], vertLeft, vertRight };
        updatedNorm = new Vector3[] { rightMeshTriangle.Norm[0], normalLeft, normalRight };
        updatedUV = new Vector2[] { rightMeshTriangle.UV[0], uvLeft, uvRight };

        currentTriangle = new MeshTriangle(updatedVert, updatedNorm, updatedUV, meshTri.SMIndex);

        if (updatedVert[0] != updatedVert[1] && updatedVert[0] != updatedVert[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVert[1] - updatedVert[0], updatedVert[2] - updatedVert[0]), updatedVert[0]) < 0)
            {
                FlipTriangle(currentTriangle);

            }
            rSide.AddTriangle(currentTriangle);

        }

        updatedVert = new Vector3[] { rightMeshTriangle.Vert[0], rightMeshTriangle.Vert[1], vertRight };
        updatedNorm = new Vector3[] { rightMeshTriangle.Norm[0], rightMeshTriangle.Norm[1], normalRight };
        updatedUV = new Vector2[] { rightMeshTriangle.UV[0], uvLeft, uvRight };

        currentTriangle = new MeshTriangle(updatedVert, updatedNorm, updatedUV, meshTri.SMIndex);

        if (updatedVert[0] != updatedVert[1] && updatedVert[0] != updatedVert[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVert[1] - updatedVert[0], updatedVert[2] - updatedVert[0]), updatedVert[0]) < 0)
            {
                FlipTriangle(currentTriangle);
            }
            rSide.AddTriangle(currentTriangle);

        }
    }

    private static void FlipTriangle(MeshTriangle meshTri)
    {
        Vector3 temp = meshTri.Vert[2];
        meshTri.Vert[2] = meshTri.Vert[0];
        meshTri.Vert[0] = temp;

        temp = meshTri.Norm[2];
        meshTri.Norm[2] = meshTri.Norm[0];
        meshTri.Norm[0] = temp;

        Vector2 temp2 = meshTri.UV[2];
        meshTri.UV[2] = meshTri.UV[0];
        meshTri.UV[0] = temp2;
    }

}
