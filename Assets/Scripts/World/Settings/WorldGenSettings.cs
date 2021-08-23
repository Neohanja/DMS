using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WorldGenSettings", menuName = "Neohanja/World Generation Settings")]
public class WorldGenSettings : ScriptableObject
{
    [Header("Cell Automata")]
    [Range(0, 10)]
    public int smoothing;
    [Range(0, 8)]
    public int underPopulation;
    [Range(0, 8)]
    public int rebirth;

    [Header("Noise Settings")]
    public float noiseScale;

    [Header("Data Files")]
    public TileData[] tiles;
    public RoomBlueprint[] pregenRooms;
    public Biome[] underworld;
    public Biome[] overworld;
    public Biome defaultBiome;
}
