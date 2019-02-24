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

        if (GUILayout.Button("Erode (" + m.numErosionIterations + " iterations)")) {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            m.Erode();
            sw.Stop();
            Debug.Log($"Erosion finished ({m.numErosionIterations} iterations; {sw.ElapsedMilliseconds}ms)");
        }


    }

    void OnEnable() {
        m = (MeshGenerator)target;
    }
}
