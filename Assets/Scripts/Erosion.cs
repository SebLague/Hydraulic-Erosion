using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion : MonoBehaviour {

    public int seed;
    public int erosionRadius = 3;
    [Range (0, 1)]
    public float inertia = .05f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 
    public float sedimentCapacityFactor = 4; // Multiplier for how much sediment a droplet can carry
    public float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain
    [Range (0, 1)]
    public float erodeSpeed = .1f;
    [Range (0, 1)]
    public float depositSpeed = .1f;
    [Range (0, 1)]
    public float evaporateSpeed = .1f;
    public float gravity = 1;

    // Debug vars
    public List<Vector3> debugPositions;

    public void Erode (float[] nodes, int mapSize) {
        debugPositions = new List<Vector3> ();
        var prng = new System.Random (seed);
        var erosionBrush = new ErosionBrush (erosionRadius);

        // Create water droplet at random point on map
        Vector2 randomPos = new Vector2 (prng.Next (0, mapSize - 1), prng.Next (0, mapSize - 1)) + Vector2.one * .5f; // place in middle of random cell
        WaterDroplet droplet = new WaterDroplet () { position = randomPos, waterVolume = 1, speed = 1 };
        int lifetime = 100;

        for (int i = 0; i < lifetime; i++) {
            Vector2Int dropletCoord = new Vector2Int ((int) droplet.position.x, (int) droplet.position.y);
            // Calculate direction of flow from the height difference of surrounding points
            var point = CalculateHeightAndGradient (nodes, mapSize, droplet.position);
            

            // Update the droplet's direction, speed, position, and apply evaporation
            droplet.direction = (droplet.direction * inertia - point.gradient * (1 - inertia)).normalized;
            // Give droplet random direction if is on flat surface
            if (droplet.direction == Vector2.zero) {
                float randomAngle = (float) prng.NextDouble () * Mathf.PI * 2;
                droplet.direction = new Vector2 (Mathf.Sin (randomAngle), Mathf.Cos (randomAngle));
            }

            Vector2 positionOld = droplet.position;
            droplet.position += droplet.direction;
            debugPositions.Add (new Vector3 (positionOld.x, point.height, positionOld.y));

            // Stop simulating droplet if it has flowed over edge of map
            if (droplet.position.x < 0 || droplet.position.y < 0 || droplet.position.x >= mapSize - 1 || droplet.position.y >= mapSize - 1) {
                break;
            }

            // Calculate new and old height of droplet
            float newHeight = CalculateHeightAndGradient (nodes, mapSize, droplet.position).height;
            float deltaHeight = newHeight - point.height; // negative if moving downwards

            // Droplet is moving out of a pit, so try fill that pit with carried sediment
            if (deltaHeight > 0) {

            }
            // Droplet is moving downwards: calculate its sediment carry capacity and erode/deposit accordingly
            else {
                // Calculate the sediment carry capacity of the droplet. Can carry more when moving fast downhill.
                float sedimentCapacity = Mathf.Max (-deltaHeight * droplet.speed * droplet.waterVolume * sedimentCapacityFactor, minSedimentCapacity);

                if (droplet.sediment > sedimentCapacity) {
                    // Deposit a fraction of the surplus sediment
                    float amountToDeposit = (droplet.sediment - sedimentCapacity) * depositSpeed;

                    // Add the sediment to the four nodes of the current cell using bilinear interpolation
                    // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                    
                    Vector2 offset = positionOld - dropletCoord;
                    int nodeIndexNW = dropletCoord.y * mapSize + dropletCoord.x;
                    nodes[nodeIndexNW] += amountToDeposit * (1 - dropletCoord.x) * (1 - dropletCoord.y);
                    nodes[nodeIndexNW + 1] += amountToDeposit * (dropletCoord.x) * (1 - dropletCoord.y);
                    nodes[nodeIndexNW + mapSize] += amountToDeposit * (1 - dropletCoord.x) * (dropletCoord.y);
                    nodes[nodeIndexNW + mapSize + 1] += amountToDeposit * (dropletCoord.x) * (dropletCoord.y);

                } else {
                    // Erode from the terrain a fraction of the droplet's current carry capacity.
                    // Clamp the erosion to the change in height so that it never digs a hole in the terrain (can at most flatten).
                    float amountToErode = Mathf.Min ((sedimentCapacity - droplet.sediment) * erodeSpeed, -deltaHeight);

                    // Use erosion brush to erode from all nodes inside radius
                    for (int brushPointIndex = 0; brushPointIndex < erosionBrush.offsets.Length; brushPointIndex++)
                    {
                        Vector2Int erodeCoord = dropletCoord + erosionBrush.offsets[brushPointIndex];
                        if (erodeCoord.x >= 0 && erodeCoord.x < mapSize && erodeCoord.y >= 0 && erodeCoord.y < mapSize) {
                            int nodeIndex = erodeCoord.y * mapSize + erodeCoord.x;
                            // Don't erode below zero (to avoid very deep erosion from occuring)
                            float sediment = Mathf.Min(nodes[nodeIndex], amountToErode * erosionBrush.weights[brushPointIndex]);
                            nodes[nodeIndex] -= sediment;
                            droplet.sediment += sediment;
                        }
                    }
                }
            }

            droplet.speed = Mathf.Sqrt (droplet.speed * droplet.speed + deltaHeight * gravity);
            droplet.waterVolume *= (1 - evaporateSpeed);

        }

    }

    HeightAndGradient CalculateHeightAndGradient (float[] nodes, int mapSize, Vector2 pos) {

        Vector2Int coord = new Vector2Int ((int) pos.x, (int) pos.y);
        // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        Vector2 offset = pos - coord;

        // Calculate heights of the four nodes of the droplet's cell
        int nodeIndexNW = coord.y * mapSize + coord.x;
        float heightNW = nodes[nodeIndexNW];
        float heightNE = nodes[nodeIndexNW + 1];
        float heightSW = nodes[nodeIndexNW + mapSize];
        float heightSE = nodes[nodeIndexNW + mapSize + 1];

        // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
        float flowDirectionX = (heightNE - heightNW) * (1 - offset.y) + (heightSE - heightSW) * offset.y;
        float flowDirectionY = (heightSW - heightNW) * (1 - offset.x) + (heightSE - heightNE) * offset.x;
        Vector2 flowDirection = new Vector2 (flowDirectionX, flowDirectionY);

        // Calculate height with bilinear interpolation of the heights of the nodes of the cell
        float height = heightNW * (1 - offset.x) * (1 - offset.y) + heightNE * offset.x * (1 - offset.y) + heightSW * (1 - offset.x) * offset.y + heightSE * offset.x * offset.y;

        return new HeightAndGradient () { height = height, gradient = flowDirection };
    }

    struct HeightAndGradient {
        public float height;
        public Vector2 gradient;
    }

    struct WaterDroplet {
        public Vector2 velocity;
        public Vector2 position;
        public Vector2 direction;
        public float speed;
        public float sediment;
        public float waterVolume;
    }

    struct ErosionBrush {
        // x,y coords of each point inside the radius of the brush
        public readonly Vector2Int[] offsets;
        // weight of the brush at each point (largest at centre; total sums to 1)
        public readonly float[] weights;

        public ErosionBrush (int radius) {
            var weightsList = new List<float> ();
            var offsetsList = new List<Vector2Int> ();
            float weightSum = 0;

            radius += 1; // increase radius by 1 because points on radius have a weight of 0 and so are discarded

            for (int y = -radius; y <= radius; y++) {
                for (int x = -radius; x <= radius; x++) {
                    float sqrDst = x * x + y * y;
                    if (sqrDst < radius * radius) {
                        float weight = 1 - Mathf.Sqrt (sqrDst) / radius;
                        weightSum += weight;

                        offsetsList.Add (new Vector2Int (x, y));
                        weightsList.Add (weight);
                    }
                }
            }

            int numPoints = weightsList.Count;
            weights = new float[numPoints];
            offsets = new Vector2Int[numPoints];

            for (int i = 0; i < numPoints; i++) {
                weights[i] = weightsList[i] / weightSum;
                offsets[i] = offsetsList[i];
            }
        }
    }
}