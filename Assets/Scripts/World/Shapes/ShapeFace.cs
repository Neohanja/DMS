using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeFace
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvMap;

    public bool skipFace;
    public bool fullCoverage;

    public ShapeFace(List<Vector3> verts, List<int> tris, List<Vector2> uvs, bool invisible, bool blockView)
    {
        vertices = verts.ToArray();
        triangles = tris.ToArray();
        uvMap = uvs.ToArray();
        skipFace = invisible;
        fullCoverage = blockView;
    }

    public ShapeFace()
    {
        vertices = new Vector3[0];
        triangles = new int[0];
        uvMap = new Vector2[0];
        fullCoverage = false;
        skipFace = false;
    }
}
