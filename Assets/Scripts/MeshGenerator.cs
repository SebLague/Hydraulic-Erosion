using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public int mapSize = 256;
    public float scale = 1;
    public float elevationScale = 1;

    public int erosionIterations = 1;
    public bool showGizmos;

    float[] map;
    

    Erosion erosion;

    public void StartMeshGeneration () {
        map = FindObjectOfType<HeightMapGenerator> ().Generate (mapSize);
        GenerateMesh ();
    }

    public void Erode () {
        erosion = FindObjectOfType<Erosion> ();
        //erosion.Init(mapSize);

        for (int i = 0; i < erosionIterations; i++) {
            //erosion.Erode (map);
        }
        GenerateMesh ();
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

        Mesh mesh = new Mesh () {
            vertices = verts,
            triangles = triangles
        };
        mesh.RecalculateNormals ();

        GetComponent<MeshFilter> ().mesh = mesh;

    }

    Vector3 PointFromIndex (int i, float h = 0) {
        int y = i/ mapSize;
        int x = i - y * mapSize;
        Vector2 percent = new Vector2 (x / (mapSize - 1f), y / (mapSize - 1f));
        Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
        pos += Vector3.up * (map[i] * elevationScale + h);
        return pos;
    }

    /* 
    void OnDrawGizmos () {
        if (erosion != null && erosion.debugPoints != null && showGizmos) {
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
    */
}