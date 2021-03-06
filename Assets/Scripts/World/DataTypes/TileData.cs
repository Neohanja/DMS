using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Tile", menuName = "Neohanja/Tile Data")]
public class TileData : ScriptableObject
{
    public string tileName;

    public Shape baseShape;

    public int topTexture;
    public int bottomTexture;
    public int northTexture;
    public int eastTexture;
    public int southTexture;
    public int westTexture;

    public bool skipDraw;
    public bool solid;
    public bool drawSurrounding;
    public bool drawBottom;

    public int GetTextureIndex(int face)
    {
        switch(face)
        {
            case 0: return topTexture;
            case 1: return bottomTexture;
            case 2: return northTexture;
            case 3: return eastTexture;
            case 4: return southTexture;
            case 5: return westTexture;
        }

        return 0;
    }
}
