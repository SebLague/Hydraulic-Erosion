using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour {
    public int seed;
    public bool randomizeSeed;

    public int mapSize = 256;
    public int numOctaves = 4;
    public float heightMultiplier = 1;
    public float persistence = .5f;
    public float lacunarity = 2;
    public float initialScale = 1;

    public float[] Generate () {
        var map = new float[mapSize * mapSize];
        seed = (randomizeSeed) ? Random.Range (-10000, 10000) : seed;
        var prng = new System.Random (seed);

        Vector2[] offsets = new Vector2[numOctaves];
        for (int i = 0; i < numOctaves; i++) {
            offsets[i] = new Vector2 (prng.Next (-1000, 1000), prng.Next (-1000, 1000));
        }

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {

                float noiseValue = 0;
                float scale = initialScale;
                float weight = 1;
                for (int i = 0; i < numOctaves; i++) {
                    Vector2 p = offsets[i] + new Vector2(x/(float)mapSize,y/(float)mapSize) * scale;
                    noiseValue += Mathf.PerlinNoise(p.x,p.y) * weight;
                    weight *= persistence;
                    scale *= lacunarity;
                }
                map[y*mapSize+x] = noiseValue;
            }
        }

        return map;
    }
}