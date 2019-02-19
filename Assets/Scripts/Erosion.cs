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
    public float acceleration = 1;
    public float friction = .1f;

    System.Random prng;

    List<Neighbour>[] neighbours;
    List<DropletInfluence>[] dropletInfluenceMap;

    public List<int> debugPoints;

    int size;

    public void Init (int size) {
        this.size = size;

        seed = (randomizeSeed) ? Random.Range (-10000, 10000) : seed;
        prng = new System.Random (seed);

        neighbours = new List<Neighbour>[size * size];
        dropletInfluenceMap = new List<DropletInfluence>[size * size];

        for (int i = 0; i < size * size; i++) {
            int centreX = i % size;
            int centreY = i / size;

            neighbours[i] = new List<Neighbour> ();
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
                            Vector2 dir = new Vector2 (x - centreX, y - centreY).normalized;
                            neighbours[i].Add (new Neighbour (neighbourI, dir));
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
        Vector2 velocity = Vector2.zero;

        for (int lifetime = 0; lifetime < maxLifetime; lifetime++) {
            debugPoints.Add (dropletIndex);
            float dropletHeight = map[dropletIndex];

            int lowestNearbyNeighbourIndex = 0;
            float lowestNearbyHeight = float.MaxValue;
            Vector2 deltaV = Vector2.zero;

            for (int i = 0; i < neighbours[dropletIndex].Count; i++) {
                int neighbourIndex = neighbours[dropletIndex][i].index;
                float neighbourHeight = map[neighbourIndex];
                float delta = dropletHeight - neighbourHeight;
                deltaV +=neighbours[dropletIndex][i].dir * delta;

                if (map[neighbourIndex] < lowestNearbyHeight) {
                    lowestNearbyHeight = map[neighbourIndex];
                    lowestNearbyNeighbourIndex = i;
                }
            }
            float deltaHeight = dropletHeight - lowestNearbyHeight;
  
            //velocity += neighbours[dropletIndex][lowestNearbyNeighbourIndex].dir * deltaHeight * acceleration;
            velocity += deltaV.normalized * acceleration;
            velocity -= velocity * friction;

            for (int i = 0; i < dropletInfluenceMap[dropletIndex].Count; i++) {
                DropletInfluence pointUnderDroplet = dropletInfluenceMap[dropletIndex][i];

                // Water is at local minimum
                if (deltaHeight < 0) {

                }

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

            Vector2 pos = new Vector2 (dropletIndex % size, dropletIndex / size);
            pos += velocity.normalized;
            Vector2Int coord = new Vector2Int (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y));
            if (coord.x >= 0 && coord.x < size && coord.y >= 0 && coord.y < size) {
                dropletIndex = coord.y * size + coord.x;
            } else {
                break;
            }

        }

    }

    struct Neighbour {
        public int index;
        public Vector2 dir;

        public Neighbour (int index, Vector2 dir) {
            this.index = index;
            this.dir = dir;
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