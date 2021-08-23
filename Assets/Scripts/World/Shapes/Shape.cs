using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Basic Shape", menuName = "Neohanja/Shape Data")]
public class Shape : ScriptableObject
{
    public string shapeName;

    public ShapeFace topFace;
    public ShapeFace bottomFace;
    public ShapeFace northFace;
    public ShapeFace eastFace;
    public ShapeFace southFace;
    public ShapeFace westFace;

    public ShapeFace GetFace(int face)
    {
        switch(face)
        {
            case 0: return topFace;
            case 1: return bottomFace;
            case 2: return northFace;
            case 3: return eastFace;
            case 4: return southFace;
            case 5: return westFace;
        }

        return null;
    }
}
