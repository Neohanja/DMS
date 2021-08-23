using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeshToShape", menuName = "Neohanja/Mesh Shape Converter")]

public class MeshToBlock : Shape
{
    public Mesh blockMesh;
    public bool recalculateMesh;

    private void OnValidate()
    {
        if (recalculateMesh)
        {
            if (blockMesh == null)
            {
                northFace = new ShapeFace();
                southFace = new ShapeFace();
                eastFace = new ShapeFace();
                westFace = new ShapeFace();
                topFace = new ShapeFace();
                bottomFace = new ShapeFace();
            }
            else
            {
                MeshData north = new MeshData(null);
                MeshData east = new MeshData(null);
                MeshData south = new MeshData(null);
                MeshData west = new MeshData(null);
                MeshData top = new MeshData(null);
                MeshData bottom = new MeshData(null);

                for(int v = 0; v < blockMesh.vertexCount; ++v)
                {
                    int directional = 0;

                    //Get the direction the block is facing.
                    if (blockMesh.normals[v].z > 0) directional = 0;
                    else if (blockMesh.normals[v].z < 0) directional = 1;
                    else if (blockMesh.normals[v].y > 0) directional = 2;
                    else if (blockMesh.normals[v].x < 0) directional = 3;
                    else if (blockMesh.normals[v].y < 0) directional = 4;
                    else if (blockMesh.normals[v].x > 0) directional = 5;

                    Vector3 blenderConvert = new Vector3(-blockMesh.vertices[v].x, blockMesh.vertices[v].z, blockMesh.vertices[v].y);

                    switch(directional)
                    {
                        case 0:
                            top.AddPointSingle(blenderConvert, new Vector2(blenderConvert.x, blenderConvert.z));
                            break;
                        case 1:
                            bottom.AddPointSingle(blenderConvert, new Vector2(blenderConvert.x, 1 - blenderConvert.z));
                            break;
                        case 2:
                            north.AddPointSingle(blenderConvert, new Vector2(1 - blenderConvert.x, blenderConvert.y));
                            break;
                        case 3:
                            east.AddPointSingle(blenderConvert, new Vector2(blenderConvert.z, blenderConvert.y));
                            break;
                        case 4:
                            south.AddPointSingle(blenderConvert, new Vector2(blenderConvert.x, blenderConvert.y));
                            break;
                        case 5:
                            west.AddPointSingle(blenderConvert, new Vector2(1 - blenderConvert.z, blenderConvert.y));
                            break;
                    }
                }
                
                for(int t = 0; t < blockMesh.triangles.Length; ++t)
                {
                    int directional = 0;

                    int tI = blockMesh.triangles[t];

                    //Get the direction the block is facing.
                    if (blockMesh.normals[tI].z > 0) directional = 0;
                    else if (blockMesh.normals[tI].z < 0) directional = 1;
                    else if (blockMesh.normals[tI].y > 0) directional = 2;
                    else if (blockMesh.normals[tI].x < 0) directional = 3;
                    else if (blockMesh.normals[tI].y < 0) directional = 4;
                    else if (blockMesh.normals[tI].x > 0) directional = 5;

                    Vector3 blenderConvert = new Vector3(-blockMesh.vertices[tI].x, blockMesh.vertices[tI].z, blockMesh.vertices[tI].y);

                    switch (directional)
                    {
                        case 0:
                            top.AddTris(top.VertIndex(blenderConvert));
                            break;
                        case 1:
                            bottom.AddTris(bottom.VertIndex(blenderConvert));
                            break;
                        case 2:
                            north.AddTris(north.VertIndex(blenderConvert));
                            break;
                        case 3:
                            east.AddTris(east.VertIndex(blenderConvert));
                            break;
                        case 4:
                            south.AddTris(south.VertIndex(blenderConvert));
                            break;
                        case 5:
                            west.AddTris(west.VertIndex(blenderConvert));
                            break;
                    }
                }
                
                northFace = new ShapeFace(north.GetVerts(), north.GetTris(), north.GetUVMap(), north.TriangleCount <= 0, northFace.fullCoverage);
                eastFace = new ShapeFace(east.GetVerts(), east.GetTris(), east.GetUVMap(), east.TriangleCount <= 0, eastFace.fullCoverage);
                southFace = new ShapeFace(south.GetVerts(), south.GetTris(), south.GetUVMap(), south.TriangleCount <= 0, southFace.fullCoverage);
                westFace = new ShapeFace(west.GetVerts(), west.GetTris(), west.GetUVMap(), west.TriangleCount <= 0, westFace.fullCoverage);
                topFace = new ShapeFace(top.GetVerts(), top.GetTris(), top.GetUVMap(), top.TriangleCount <= 0, topFace.fullCoverage);
                bottomFace = new ShapeFace(bottom.GetVerts(), bottom.GetTris(), bottom.GetUVMap(), bottom.TriangleCount <= 0, bottomFace.fullCoverage);

                //To clean up space, though I'm sure garbage collection does this for us?
                north.ClearData();
                east.ClearData();
                south.ClearData();
                west.ClearData();
                top.ClearData();
                bottom.ClearData();
            }

            Debug.Log(shapeName + " mesh to block data completed.");

            recalculateMesh = false;
        }
    }
}