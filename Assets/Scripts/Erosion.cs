using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion : MonoBehaviour {

    public int seed;
    [Range (0, 1)]
    public float inertia = .3f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 

    // Debug vars
    public List<Vector3> debugPositions;

    public void Erode (float[] nodes, int mapSize) {
        debugPositions = new List<Vector3> ();
        var prng = new System.Random (seed);

        // Create water droplet at random point on map
        Vector2 randomPos = new Vector2 (prng.Next (0, mapSize - 1), prng.Next (0, mapSize - 1)) + Vector2.one * .5f; // place in middle of random cell
        WaterDroplet droplet = new WaterDroplet () { position = randomPos, waterVolume = 1 };
        int lifetime = 100;

        for (int i = 0; i < lifetime; i++) {
            // Calculate direction of flow from the height difference of surrounding points
            var point = CalculateHeightAndDirection (nodes, mapSize, droplet.position);

            // Update the droplet's direction, speed, position, and apply evaporation
            droplet.direction = (droplet.direction * inertia - point.direction * (1 - inertia)).normalized;
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
            float newHeight = CalculateHeightAndDirection (nodes, mapSize, droplet.position).height;

            // Calculate the sediment carry capacity of the droplet

            // Erode or deposit soil
        }

    }

    HeightAndDirection CalculateHeightAndDirection (float[] nodes, int mapSize, Vector2 pos) {

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

        return new HeightAndDirection () { height = height, direction = flowDirection };
    }

    struct HeightAndDirection {
        public float height;
        public Vector2 direction;
    }

    struct WaterDroplet {
        public Vector2 position;
        public Vector2 direction;
        public float speed;
        public float sediment;
        public float waterVolume;
    }

}