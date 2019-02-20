using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public int mapSize = 256;
    public float scale = 1;
    public float elevationScale = 1;

    public bool animateErosion;
    public bool showGizmos;

    float[] map;

    Erosion erosion;
    public int numErosionIterationsPerFrame = 2;
    public int numErosionIterations = 0;

    Mesh mesh;

    void Start () {
        map = FindObjectOfType<HeightMapGenerator> ().Generate (mapSize);
        erosion = FindObjectOfType<Erosion> ();
        GenerateMesh ();
    }

    public void StartMeshGeneration () {
        map = FindObjectOfType<HeightMapGenerator> ().Generate (mapSize);
        GenerateMesh ();
    }

    public void Erode () {
        erosion = FindObjectOfType<Erosion> ();
        //erosion.Init(mapSize);
        erosion.Erode (map, mapSize);
        GenerateMesh ();
    }

    void Update () {
        if (animateErosion) {
            for (int i = 0; i < numErosionIterationsPerFrame; i++) {
                erosion.Erode (map, mapSize);
            }
            GenerateMesh ();
            numErosionIterations++;
        }
    }

    void GenerateMesh () {

        Vector3[] verts = new Vector3[mapSize * mapSize];
        int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];
        int t = 0;

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                int i = y * mapSize + x;

                Vector2 percent = new Vector2 (x / (mapSize - 1f), y / (mapSize - 1f));
                Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
                pos += Vector3.up * map[i] * elevationScale;
                verts[i] = pos;

                if (x != mapSize - 1 && y != mapSize - 1) {

                    triangles[t + 0] = i + mapSize;
                    triangles[t + 1] = i + mapSize + 1;
                    triangles[t + 2] = i;

                    triangles[t + 3] = i + mapSize + 1;
                    triangles[t + 4] = i + 1;
                    triangles[t + 5] = i;
                    t += 6;
                }
            }
        }

        if (mesh == null) {
            mesh = new Mesh();
        }
        else {
            mesh.Clear();
        }

        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals ();

        GetComponent<MeshFilter> ().mesh = mesh;

    }

    Vector3 PointFromIndex (int i, float h = 0) {
        int y = i / mapSize;
        int x = i - y * mapSize;
        Vector2 percent = new Vector2 (x / (mapSize - 1f), y / (mapSize - 1f));
        Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
        pos += Vector3.up * (map[i] * elevationScale + h);
        return pos;
    }

    Vector3 MeshPointFromMapPoint (Vector3 mapPoint) {
        Vector2 percent = new Vector2 (mapPoint.x / (mapSize - 1f), mapPoint.z / (mapSize - 1f));
        Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
        pos += Vector3.up * mapPoint.y * elevationScale;
        return pos;
    }

    void OnDrawGizmos () {
        if (erosion != null && erosion.debugPositions != null && showGizmos) {
            Gizmos.color = Color.red;
            for (int i = 0; i < erosion.debugPositions.Count; i++) {

                var p1 = MeshPointFromMapPoint (erosion.debugPositions[i]);

                float p = i / (erosion.debugPositions.Count - 1f);
                float s = Mathf.Lerp (.2f, .05f, p);
                Gizmos.DrawSphere (p1, s);

                if (i < erosion.debugPositions.Count - 1) {
                    float h = .1f;
                    Gizmos.DrawLine (p1 + Vector3.up * h, MeshPointFromMapPoint (erosion.debugPositions[i + 1]) + Vector3.up * h);
                }
            }
        }

    }

}