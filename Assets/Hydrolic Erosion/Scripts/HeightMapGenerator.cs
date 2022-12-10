using System;

using UnityEngine;

using Random = UnityEngine.Random;

public class HeightMapGenerator : MonoBehaviour {
    public int seed;
    public bool randomizeSeed;

    public int numOctaves = 7;
    public float persistence = .5f;
    public float lacunarity = 2;
    public float initialScale = 2;

    public bool useComputeShader = true;
    public ComputeShader heightMapComputeShader;

    public float[] GenerateHeightMap (int mapSize) {
        if (useComputeShader) {
            return GenerateHeightMapGPU (mapSize);
        }
        return GenerateHeightMapCPU (mapSize);
    }

    float[] GenerateHeightMapGPU (int mapSize) {
        // Generates a new random seed if randomizeSeed is true
        seed = (randomizeSeed) ? Random.Range (-10000, 10000) : seed;
        var prng = new System.Random (seed);
        
        Vector2[] offsets = new Vector2[numOctaves];
        for (int i = 0; i < numOctaves; i++) {
            offsets[i] = new Vector2 (prng.Next (-10000, 10000), prng.Next (-10000, 10000));
        }

        ComputeBuffer offsetsBuffer = new ComputeBuffer (offsets.Length, sizeof (float) * 2);
        offsetsBuffer.SetData (offsets);
        heightMapComputeShader.SetBuffer (0, "offsets", offsetsBuffer);

        int floatToIntMultiplier = 1000;
        float[] map = new float[mapSize * mapSize];

        int[] minMaxHeight = { floatToIntMultiplier * numOctaves, 0 };
        ComputeBuffer minMaxBuffer = new ComputeBuffer (minMaxHeight.Length, sizeof (int));
        minMaxBuffer.SetData (minMaxHeight);
        heightMapComputeShader.SetBuffer (0, "minMax", minMaxBuffer);

        heightMapComputeShader.SetInt ("mapSize", mapSize);
        heightMapComputeShader.SetInt ("octaves", numOctaves);
        heightMapComputeShader.SetFloat ("lacunarity", lacunarity);
        heightMapComputeShader.SetFloat ("persistence", persistence);
        heightMapComputeShader.SetFloat ("scaleFactor", initialScale);
        heightMapComputeShader.SetInt ("floatToIntMultiplier", floatToIntMultiplier);

        
        // ----------------------------------ERROR---------------------------------------
        // Does not set appropriate offsets so the result is segments of the same terrain
        
        int placement = 0;
        while(placement < map.Length)
        {
            // Create a map that can be computed
            float[] computeMap = new float[map.Length - placement >= 65535 ? 65535 : map.Length - placement];
            
            // Create a buffer to folder the computed map
            ComputeBuffer mapBuffer = new ComputeBuffer (computeMap.Length, sizeof (int));
            mapBuffer.SetData(computeMap);
            
            // Set the buffer in the shader
            heightMapComputeShader.SetBuffer (0, "heightMap", mapBuffer);
            
            // Dispatch the shader to calculate the segment
            heightMapComputeShader.Dispatch (0, computeMap.Length, 1, 1);
            
            // Get the data then dispose of the buffer
            mapBuffer.GetData(computeMap);
            mapBuffer.Dispose();
            
            // Copy the data to the map
            Array.ConstrainedCopy(computeMap, 0, map, placement, computeMap.Length);
            
            // Move the copy pointer along
            placement += 65535;
        }

        minMaxBuffer.GetData (minMaxHeight);
        minMaxBuffer.Release ();
        offsetsBuffer.Release ();

        float minValue = (float) minMaxHeight[0] / (float) floatToIntMultiplier;
        float maxValue = (float) minMaxHeight[1] / (float) floatToIntMultiplier;

        for (int i = 0; i < map.Length; i++) {
            map[i] = Mathf.InverseLerp (minValue, maxValue, map[i]);
        }

        return map;
    }

    float[] GenerateHeightMapCPU (int mapSize) {
        var map = new float[mapSize * mapSize];
        
        // Generates a new random seed if randomizeSeed is true
        seed = (randomizeSeed) ? Random.Range (-10000, 10000) : seed;
        var prng = new System.Random (seed);

        Vector2[] offsets = new Vector2[numOctaves];
        for (int i = 0; i < numOctaves; i++) {
            offsets[i] = new Vector2 (prng.Next (-1000, 1000), prng.Next (-1000, 1000));
        }

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                float noiseValue = 0;
                float scale = initialScale;
                float weight = 1;
                for (int i = 0; i < numOctaves; i++) {
                    Vector2 p = offsets[i] + new Vector2 (x / (float) mapSize, y / (float) mapSize) * scale;
                    noiseValue += Mathf.PerlinNoise (p.x, p.y) * weight;
                    weight *= persistence;
                    scale *= lacunarity;
                }
                map[y * mapSize + x] = noiseValue;
                minValue = Mathf.Min (noiseValue, minValue);
                maxValue = Mathf.Max (noiseValue, maxValue);
            }
        }

        // Normalize
        if (maxValue != minValue) {
            for (int i = 0; i < map.Length; i++) {
                map[i] = (map[i] - minValue) / (maxValue - minValue);
            }
        }

        return map;
    }
}