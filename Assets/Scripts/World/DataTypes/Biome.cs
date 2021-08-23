using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Biome", menuName = "Neohanja/Biome")]
public class Biome : ScriptableObject
{
    public string biomeName;
    public byte groundTile;
    public byte wallTile;
    public int growth;
    public Clutter[] biomeDecor;
}

[System.Serializable]
public class Clutter
{
    public string clutterName;
    public float clutterScale;
    public int zLoc;
    public byte tile;
    [Range(0f,1f)]
    public float threshold;
}