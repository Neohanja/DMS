using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceStats", menuName = "Neohanja/Racial Stats")]
public class Racial : ScriptableObject
{
    public string raceName;
    public RaceType raceType;

    public float baseMoveSpeed;
    public float height;
    public float width;

    public Mesh baseMesh;
    public Texture[] skins;

    public string[] genericNames;

    public float sight;
    public float hearing;
    public float smell;

    public enum RaceType
    {
        Normal,
        Undead,
        Demon,
        Fairy
    }

    
}
