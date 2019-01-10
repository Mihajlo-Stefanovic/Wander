using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Planet))]
public class GenerateMapFromEditor:Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate")) {
            Planet.instance.generateMapFor(Planet.instance.graphCenterTile);
        }
    }
}
