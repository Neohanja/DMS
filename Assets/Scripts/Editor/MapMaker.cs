using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class MapMaker : Editor
{
    Editor genSettings;

    public override void OnInspectorGUI()
    {
        World mapMaker = (World)target;
        base.OnInspectorGUI();

        DrawSettingsEditor(mapMaker.worldGen, mapMaker.UpdateGenSettings, ref mapMaker.genOpen, ref genSettings);        
    }

    void DrawSettingsEditor(Object settings, System.Action onUpdate, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onUpdate != null)
                        {
                            onUpdate();
                        }
                    }
                }
            }
        }
    }
}
