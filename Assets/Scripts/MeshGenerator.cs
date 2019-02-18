using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public float scale = 1;
    public float elevationScale = 1;

    public int erosionIterations = 1;

    float[] map;
    int size;

    Erosion erosion;

    [ContextMenu ("Run")]
    public void Run () {
        size = FindObjectOfType<HeightMapGenerator> ().mapSize;
        map = FindObjectOfType<HeightMapGenerator> ().Generate ();
        FindObjectOfType<Erosion> ().Init (size);
        GenerateMesh (map, size);
    }

    [ContextMenu ("Erode")]
    void Erode () {
        erosion = FindObjectOfType<Erosion> ();

        for (int i = 0; i < erosionIterations; i++) {
            erosion.Erode (map);
        }
        GenerateMesh (map, size);
    }

    void Update () {
        //var m = FindObjectOfType<HeightMapGenerator> ();
        //map = m.Generate();
        //GenerateMesh (map,m.mapSize);
    }

    public void GenerateMesh (float[] map, int size) {

        Vector3[] verts = new Vector3[size * size];
        int[] triangles = new int[(size - 1) * (size - 1) * 6];
        int t = 0;

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                int i = y * size + x;

                Vector2 percent = new Vector2 (x / (size - 1f), y / (size - 1f));
                Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
                pos += Vector3.up * map[i] * elevationScale;
                verts[i] = pos;

                if (x != size - 1 && y != size - 1) {

                    triangles[t + 0] = i + size;
                    triangles[t + 1] = i + size + 1;
                    triangles[t + 2] = i;

                    triangles[t + 3] = i + size + 1;
                    triangles[t + 4] = i + 1;
                    triangles[t + 5] = i;
                    t += 6;
                }
            }
        }

        Mesh mesh = new Mesh () {
            vertices = verts,
            triangles = triangles
        };
        mesh.RecalculateNormals ();

        GetComponent<MeshFilter> ().mesh = mesh;

    }

    Vector3 PointFromIndex (int i, float h = 0) {
        int y = i/ size;
        int x = i - y * size;
        Vector2 percent = new Vector2 (x / (size - 1f), y / (size - 1f));
        Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
        pos += Vector3.up * (map[i] * elevationScale + h);
        return pos;
    }

    void OnDrawGizmos () {
        if (erosion != null && erosion.debugPoints != null) {
            Gizmos.color = Color.red;
            for (int erosionIndex = 0; erosionIndex < erosion.debugPoints.Count; erosionIndex++) {
                float h = .1f;
                int i = erosion.debugPoints[erosionIndex];
                var p1 = PointFromIndex(i,h);

                float p = erosionIndex / (erosion.debugPoints.Count - 1f);
                float s = Mathf.Lerp (.2f, .05f, p);
                Gizmos.DrawSphere (p1, s);
                if (erosionIndex < erosion.debugPoints.Count-1) {
                    Gizmos.DrawLine(p1,PointFromIndex(erosion.debugPoints[erosionIndex+1],h));
                }
            }
        }

    }
}