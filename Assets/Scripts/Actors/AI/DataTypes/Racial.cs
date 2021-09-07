using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceStats", menuName = "Neohanja/Racial Stats")]
public class Racial : ScriptableObject
{
    public string raceName;
    public Racial baseRace;

    public float baseMoveSpeed;
    public float height;
    public float width;

    public Mesh baseMesh;
    public Texture[] skins;
    public Color colorTone;

    public string[] genericNames;

    public float sight;
    public float hearing;

    public List<CursorIcons.TaskList> availableTasks;
}
