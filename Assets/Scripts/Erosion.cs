using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion : MonoBehaviour {

    public int seed = 0;
    public bool randomizeSeed;
    public int dropletRadius = 3;
    public float absorptionFactor = .1f;
    public float depositFactor = .1f;
    public float soilCarryCapacity = 1;

    System.Random prng;

    List<int>[] neighbours;
    List<DropletInfluence>[] dropletInfluenceMap;

    public List<int> debugPoints;

    int size;

    public void Init (int size) {
        this.size = size;

        seed = (randomizeSeed) ? Random.Range (-10000, 10000) : seed;
        prng = new System.Random (seed);

        neighbours = new List<int>[size * size];
        dropletInfluenceMap = new List<DropletInfluence>[size * size];

        for (int i = 0; i < size * size; i++) {
            int centreX = i % size;
            int centreY = i / size;

            neighbours[i] = new List<int> ();
            dropletInfluenceMap[i] = new List<DropletInfluence> ();

            for (int xOffset = -dropletRadius; xOffset <= dropletRadius; xOffset++) {
                for (int yOffset = -dropletRadius; yOffset <= dropletRadius; yOffset++) {
                    if (xOffset == 0 && yOffset == 0) {
                        continue;
                    }

                    int x = centreX + xOffset;
                    int y = centreY + yOffset;
                    int neighbourI = y * size + x;

                    if (x >= 0 && x < size && y >= 0 && y < size) {
                        if (Mathf.Abs (xOffset) <= 1 && Mathf.Abs (yOffset) <= 1) {
                            neighbours[i].Add (neighbourI);
                        }

                        if (xOffset * xOffset + yOffset * yOffset <= dropletRadius * dropletRadius) {
                            float weight = 1 - Mathf.Sqrt (xOffset * xOffset + yOffset * yOffset) / dropletRadius;
                            dropletInfluenceMap[i].Add (new DropletInfluence (neighbourI, weight));
                        }
                    }
                }
            }
        }

        print ("init complete");
    }

    public void Erode (float[] map) {
        debugPoints = new List<int> ();
        int dropletIndex = prng.Next (0, map.Length);
        float soilQuantity = 0;

        int maxLifetime = size;

        for (int lifetime = 0; lifetime < maxLifetime; lifetime++) {
            debugPoints.Add (dropletIndex);
            float dropletHeight = map[dropletIndex];

            int lowestNearbyIndex = 0;
            float lowestNearbyHeight = float.MaxValue;

            for (int i = 0; i < neighbours[dropletIndex].Count; i++) {
                int neighbourIndex = neighbours[dropletIndex][i];
                if (map[neighbourIndex] < lowestNearbyHeight) {
                    lowestNearbyHeight = map[neighbourIndex];
                    lowestNearbyIndex = neighbourIndex;
                }
            }

            for (int i = 0; i < dropletInfluenceMap[dropletIndex].Count; i++) {
                DropletInfluence pointUnderDroplet = dropletInfluenceMap[dropletIndex][i];

                if (soilQuantity < 1) {
                    float absorbAmount = absorptionFactor * pointUnderDroplet.weight;
                    soilQuantity += absorbAmount;
                    //map[pointUnderDroplet.index] -= absorbAmount;
                } else {
                    float depositAmount = depositFactor * pointUnderDroplet.weight;
                    soilQuantity -= depositAmount;
                    //map[pointUnderDroplet.index] += depositAmount;
                }
            }

            if (lowestNearbyHeight < dropletHeight) {
                dropletIndex = lowestNearbyIndex;
            } else {
                print ("Exit");
                break;
            }
        }

    }

    struct DropletInfluence {
        public int index;
        public float weight;

        public DropletInfluence (int index, float weight) {
            this.index = index;
            this.weight = weight;
        }
    }
}