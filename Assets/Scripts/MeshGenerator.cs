using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public float scale = 1;
    public float elevationScale = 1;

    [ContextMenu ("Run")]
    public void Run () {
        GenerateMesh (FindObjectOfType<HeightMapGenerator> ().Generate ());
    }

    void Update () {
        GenerateMesh (FindObjectOfType<HeightMapGenerator> ().Generate ());
    }

    public void GenerateMesh (float[] map) {
        int width = map.GetLength (0);
        int height = map.GetLength (1);

        Vector3[] verts = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        int t = 0;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int i = y * width + x;

                Vector2 percent = new Vector2 (x / (width - 1f), y / (height - 1f));
                Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
                pos += Vector3.up * map[i] * elevationScale;
                verts[i] = pos;

                if (x != width - 1 && y != height - 1) {

                    triangles[t + 0] = i + width;
                    triangles[t + 1] = i + width + 1;
                    triangles[t + 2] = i;

                    triangles[t + 3] = i + width + 1;
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
}