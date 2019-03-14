using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (TerrainGenerator))]
public class MeshEditor : Editor {

    TerrainGenerator terrainGenerator;

    public override void OnInspectorGUI () {
        DrawDefaultInspector ();

        if (GUILayout.Button ("Generate Mesh")) {
            terrainGenerator.GenerateHeightMap ();
            terrainGenerator.ContructMesh();
        }

        string numIterationsString = terrainGenerator.numErosionIterations.ToString();
        if (terrainGenerator.numErosionIterations >= 1000) {
            numIterationsString = (terrainGenerator.numErosionIterations/1000) + "k";
        }

        if (GUILayout.Button ("Erode (" + numIterationsString + " iterations)")) {
            var sw = new System.Diagnostics.Stopwatch ();

            sw.Start();
            terrainGenerator.GenerateHeightMap();
            int heightMapTimer = (int)sw.ElapsedMilliseconds;
            sw.Reset();

            sw.Start();
            terrainGenerator.Erode ();
            int erosionTimer = (int)sw.ElapsedMilliseconds;
            sw.Reset();

            sw.Start();
            terrainGenerator.ContructMesh();
            int meshTimer = (int)sw.ElapsedMilliseconds;

            if (terrainGenerator.printTimers) {
                Debug.Log($"{terrainGenerator.mapSize}x{terrainGenerator.mapSize} heightmap generated in {heightMapTimer}ms");
                Debug.Log ($"{numIterationsString} erosion iterations completed in {erosionTimer}ms");
                Debug.Log ($"Mesh constructed in {meshTimer}ms");
            }

        }
    }

    void OnEnable () {
        terrainGenerator = (TerrainGenerator) target;
        Tools.hidden = true;
    }

    void OnDisable () {
        Tools.hidden = false;
    }
}