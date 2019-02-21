using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshEditor : Editor
{
    MeshGenerator m;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Mesh")) {
            m.StartMeshGeneration();
        }

        if (GUILayout.Button("Erode")) {
            m.Erode();
        }

        if (GUILayout.Button("Erode Speed Test")) {
            m.SpeedTest();
        }
    }

    void OnEnable() {
        m = (MeshGenerator)target;
    }
}
