using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    List<Vector3> verts;
    List<int> tris;
    List<Vector2> uvMap;
    List<Color> colorMap;
    int vertCount;

    Mesh connectedMesh;

    public MeshData(Mesh mesh)
    {
        connectedMesh = mesh;

        verts = new List<Vector3>();
        tris = new List<int>();
        uvMap = new List<Vector2>();
        colorMap = new List<Color>();
        vertCount = 0;
    }

    public void AddFace(Vector3 location, ShapeFace face, int textureID, float lightLevel)
    {
        if (face == null || face.skipFace) return;

        float xUVMod = textureID % GameData.tRows;
        float yUVMod = textureID / GameData.tRows;

        xUVMod *= GameData.tSize;
        yUVMod = 1f - (yUVMod * GameData.tSize) - GameData.tSize;

        for(int v = 0; v < face.vertices.Length; ++v)
        {
            verts.Add(location + face.vertices[v]);
            uvMap.Add(new Vector2(xUVMod, yUVMod) + (face.uvMap[v] * GameData.tSize));
            colorMap.Add(new Color(0, 0, 0, lightLevel));
        }

        for(int t = 0; t < face.triangles.Length; ++t)
        {
            tris.Add(vertCount + face.triangles[t]);
        }

        vertCount += face.vertices.Length;
    }

    public void ClearData()
    {
        verts.Clear();
        tris.Clear();
        uvMap.Clear();
        colorMap.Clear();
        vertCount = 0;
    }

    public void ApplyMesh()
    {
        connectedMesh.Clear();

        connectedMesh.vertices = verts.ToArray();
        connectedMesh.uv = uvMap.ToArray();
        connectedMesh.triangles = tris.ToArray();
        connectedMesh.colors = colorMap.ToArray();
        connectedMesh.RecalculateNormals();

        ClearData();
    }

    public void AddPointSingle(Vector3 loc, Vector2 map)
    {
        if (verts.Contains(loc)) return;

        verts.Add(loc);
        uvMap.Add(map);
        vertCount++;
    }
    public void AddTris(int a)
    {
        tris.Add(a);
    }

    public int VertIndex(Vector3 point)
    {
        return verts.IndexOf(point);
    }

    public List<Vector3> GetVerts()
    {
        return verts;
    }

    public List<int> GetTris()
    {
        return tris;
    }

    public List<Vector2> GetUVMap()
    {
        return uvMap;
    }

    public int TriangleCount
    {
        get
        {
            return tris.Count;
        }
    }

    protected static World GameData { get { return World.WorldMap; } }
}
