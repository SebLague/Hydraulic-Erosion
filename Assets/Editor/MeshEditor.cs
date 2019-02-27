using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MeshGenerator))]
public class MeshEditor : Editor {

    MeshGenerator m;

    public override void OnInspectorGUI () {
        DrawDefaultInspector ();

        if (GUILayout.Button ("Generate Mesh")) {
            m.StartMeshGeneration ();
        }

        if (GUILayout.Button ("Erode (" + m.numErosionIterations + " iterations)")) {
            var sw = new System.Diagnostics.Stopwatch ();
            sw.Start ();
            m.Erode ();
            sw.Stop ();
            Debug.Log ($"Erosion finished ({m.numErosionIterations} iterations; {sw.ElapsedMilliseconds}ms)");
        }
    }

    void OnSceneGUI () {
        if (m.showNumIterations) {
            Handles.BeginGUI ();
            GUIStyle s = new GUIStyle (EditorStyles.boldLabel);
            s.fontSize = 40;

            string label = "Erosion iterations: " + m.numAnimatedErosionIterations;
            Vector2 labelSize = s.CalcSize (new GUIContent (label));

            Rect p = SceneView.currentDrawingSceneView.position;
            GUI.Label (new Rect (p.width / 2 - labelSize.x / 2, p.height - labelSize.y * 2.5f, labelSize.x, labelSize.y), label, s);
            Handles.EndGUI ();
        }
    }
    void OnEnable () {
        m = (MeshGenerator) target;
        Tools.hidden = true;
    }

    void OnDisable () {
        Tools.hidden = false;
    }
}