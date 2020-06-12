using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (Arena))]
public class ArenaEditor : Editor {

    public override void OnInspectorGUI () {
        Arena spawner = (Arena) target;
        DrawDefaultInspector ();

        GUILayout.TextArea ("Only press during play mode!!!");

        if (GUILayout.Button ("Spawn Vorpal")) {
            spawner.Spawn (0, 0);
        }
        if (GUILayout.Button ("Spawn Ranger")) {
            spawner.Spawn (1, 0);
        }
        if (GUILayout.Button ("Spawn Wraith")) {
            spawner.Spawn (2, 0);
        }
        if (GUILayout.Button ("Spawn Shell")) {
            spawner.Spawn (3, 0);
        }
        if (GUILayout.Button ("Spawn Warden")) {
            spawner.Spawn (4, 0);
        }
        if (GUILayout.Button ("Spawn Drone")) {
            spawner.Spawn (5, 0);
        }
    }
}