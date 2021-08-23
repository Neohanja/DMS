using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomBlueprint))]
public class RoomMaker : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomBlueprint blueprint = (RoomBlueprint)target;

        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 18
        };

        int[,] gridTiles = new int[blueprint.roomWidth, blueprint.roomHeight];

        for(int i = blueprint.roomTiles.Count - 1; i >= 0; --i)
        {
            if (blueprint.roomTiles[i].x < blueprint.roomWidth && blueprint.roomTiles[i].y < blueprint.roomHeight)
                gridTiles[blueprint.roomTiles[i].x, blueprint.roomTiles[i].y] = blueprint.roomTiles[i].editorIndex;
            else
                blueprint.roomTiles.RemoveAt(i);
        }


        GUILayout.Label("\nRoom   Layout\n", style, GUILayout.ExpandWidth(true));

        blueprint.scroll = EditorGUILayout.BeginScrollView(blueprint.scroll);
        for(int x = 0; x < blueprint.roomWidth; ++x)
        {
            GUILayout.BeginHorizontal();
            for(int y = 0; y < blueprint.roomHeight; ++y)
            {
                //gridTiles[x, y] = EditorGUILayout.IntField(gridTiles[x, y]);
                int newTile = EditorGUILayout.IntField(gridTiles[x, y]);

                if(newTile != gridTiles[x,y])
                {
                    int height = 1;
                    byte wallTile = blueprint.mainWall;
                    byte floorTile = blueprint.mainFloor;
                    
                    switch(newTile)
                    {
                        case 1:
                            height = 3;
                            break;
                        case 2:
                            height = 1;
                            break;
                        case 3:
                            wallTile = 5;
                            height = 2;
                            break;
                        case 4:
                            height = 1;
                            if (!blueprint.entryPoints.Contains(new Vector2Int(x, y)))
                                blueprint.entryPoints.Add(new Vector2Int(x, y));
                            break;
                    }
                    
                    if (newTile != 4) 
                    { 
                        if(blueprint.entryPoints.Contains(new Vector2Int(x,y)))
                        {
                            blueprint.entryPoints.Remove(new Vector2Int(x, y));
                        }
                    }

                    BlueprintBlockPlacement newBlock =
                        new BlueprintBlockPlacement(x, y, floorTile, wallTile, height, newTile);

                    if (newTile == 0) blueprint.roomTiles.RemoveAt(blueprint.roomTiles.IndexOf(newBlock));
                    else if (blueprint.roomTiles.Contains(newBlock)) blueprint.roomTiles[blueprint.roomTiles.IndexOf(newBlock)] = newBlock;
                    else blueprint.roomTiles.Add(newBlock);

                    blueprint.roomTiles.Sort();
                }
            }
            
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Label("\n\nKey:", style, GUILayout.ExpandWidth(true));
        GUILayout.Label("\n0: Void (Empty)\t\t\t4: Entry Point" +
                        "\n\n1: Wall\n\n2: Floor\n\n3: Flag Base (Decor)");
    }
}