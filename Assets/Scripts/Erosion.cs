using UnityEngine;

public class Erosion : MonoBehaviour {

    public int seed;
    [Range (2, 8)]
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
    public int maxDropletLifetime = 30;

    public float initialWaterVolume = 1;
    public float initialSpeed = 1;

    // Indices and weights of erosion brush precomputed for every node
    int[][] erosionBrushIndices;
    float[][] erosionBrushWeights;
    System.Random prng;

    int currentSeed;
    int currentErosionRadius;

    void Initialize (int mapSize) {
        if (prng == null || currentSeed != seed) {
            prng = new System.Random (seed);
            currentSeed = seed;
        }

        if (erosionBrushIndices == null || currentErosionRadius != erosionRadius) {
            InitializeBrushIndices (mapSize, erosionRadius);
            currentErosionRadius = erosionRadius;
        }
    }

    public void Erode (float[] nodes, int mapSize, int numIterations = 1) {
        Initialize (mapSize);

        for (int iteration = 0; iteration < numIterations; iteration++) {
            // Create water droplet at random point on map
            Vector2 randomPos = new Vector2 (prng.Next (0, mapSize - 1), prng.Next (0, mapSize - 1));
            WaterDroplet droplet = new WaterDroplet () { position = randomPos, waterVolume = initialWaterVolume, speed = initialSpeed };

            for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++) {
                int dropletCoordX = (int) droplet.position.x;
                int dropletCoordY = (int) droplet.position.y;
                int dropletIndex = dropletCoordY * mapSize + dropletCoordX;

                // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                float offsetX = droplet.position.x - dropletCoordX;
                float offsetY = droplet.position.y - dropletCoordY;

                // Calculate direction of flow from the height difference of surrounding points and update droplet's position accordingly
                HeightAndGradient heightAndGradient = CalculateHeightAndGradient (nodes, mapSize, droplet.position);
                droplet.direction = (droplet.direction * inertia - heightAndGradient.gradient * (1 - inertia)).normalized;
                Vector2 newPos = droplet.position + droplet.direction;
                droplet.position = newPos;
                
                // Stop simulating droplet if it's not moving or has flowed over edge of map
                if (droplet.direction == Vector2.zero || newPos.x < 0 || newPos.y < 0 || newPos.x >= mapSize - 1 || newPos.y >= mapSize - 1) {
                    break;
                }

                // Calculate new and old height of droplet
                float newHeight = CalculateHeightAndGradient (nodes, mapSize, droplet.position).height;
                float deltaHeight = newHeight - heightAndGradient.height; // negative if moving downwards

                // Calculate the sediment carry capacity of the droplet. Can carry more when moving fast downhill.
                float sedimentCapacity = Mathf.Max (-deltaHeight * droplet.speed * droplet.waterVolume * sedimentCapacityFactor, minSedimentCapacity);

                if (droplet.sediment > sedimentCapacity || deltaHeight > 0) {
                    // Deposit a fraction of the surplus sediment
                    float amountToDeposit = (droplet.sediment - sedimentCapacity) * depositSpeed;
                    // If moving uphill, try fill the pit the droplet has just left
                    if (deltaHeight > 0) {
                        amountToDeposit = Mathf.Min (deltaHeight, droplet.sediment);
                    }
                    // Add the sediment to the four nodes of the current cell using bilinear interpolation
                    // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                    nodes[dropletIndex] += amountToDeposit * (1 - offsetX) * (1 - offsetY);
                    nodes[dropletIndex + 1] += amountToDeposit * (offsetX) * (1 - offsetY);
                    nodes[dropletIndex + mapSize] += amountToDeposit * (1 - offsetX) * (offsetY);
                    nodes[dropletIndex + mapSize + 1] += amountToDeposit * (offsetX) * (offsetY);
                    droplet.sediment -= amountToDeposit;
                } else {
                    // Erode from the terrain a fraction of the droplet's current carry capacity.
                    // Clamp the erosion to the change in height so that it never digs a hole in the terrain (can at most flatten).
                    float amountToErode = Mathf.Min ((sedimentCapacity - droplet.sediment) * erodeSpeed, -deltaHeight);

                    // Use erosion brush to erode from all nodes inside radius
                    for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[dropletIndex].Length; brushPointIndex++) {
                        int nodeIndex = erosionBrushIndices[dropletIndex][brushPointIndex];
                        // Don't erode below zero (to avoid very deep erosion from occuring)
                        float sediment = Mathf.Min (nodes[nodeIndex], amountToErode * erosionBrushWeights[dropletIndex][brushPointIndex]);
                        nodes[nodeIndex] -= sediment;
                        droplet.sediment += sediment;
                    }
                }

                // Update droplet's speed and water content
                droplet.speed = Mathf.Sqrt (droplet.speed * droplet.speed + deltaHeight * gravity);
                droplet.waterVolume *= (1 - evaporateSpeed);
            }
        }
    }

    HeightAndGradient CalculateHeightAndGradient (float[] nodes, int mapSize, Vector2 pos) {
        int coordX = (int) pos.x;
        int coordY = (int) pos.y;
        // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        float x = pos.x - coordX;
        float y = pos.y - coordY;

        // Calculate heights of the four nodes of the droplet's cell
        int nodeIndexNW = coordY * mapSize + coordX;
        float heightNW = nodes[nodeIndexNW];
        float heightNE = nodes[nodeIndexNW + 1];
        float heightSW = nodes[nodeIndexNW + mapSize];
        float heightSE = nodes[nodeIndexNW + mapSize + 1];

        // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
        float flowDirectionX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
        float flowDirectionY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;
        Vector2 flowDirection = new Vector2 (flowDirectionX, flowDirectionY);

        // Calculate height with bilinear interpolation of the heights of the nodes of the cell
        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

        return new HeightAndGradient () { height = height, gradient = flowDirection };
    }

    void InitializeBrushIndices (int mapSize, int radius) {
        erosionBrushIndices = new int[mapSize * mapSize][];
        erosionBrushWeights = new float[mapSize * mapSize][];

        int[] indices = new int[radius * radius * 4];
        float[] weights = new float[radius * radius * 4];

        for (int i = 0; i < erosionBrushIndices.GetLength (0); i++) {
            Vector2Int centre = new Vector2Int (i % mapSize, i / mapSize);
            float weightSum = 0;
            int addIndex = 0;

            for (int y = -radius; y <= radius; y++) {
                for (int x = -radius; x <= radius; x++) {
                    float sqrDst = x * x + y * y;
                    if (sqrDst < radius * radius) {
                        Vector2Int coord = new Vector2Int (x, y) + centre;

                        if (coord.x >= 0 && coord.x < mapSize && coord.y >= 0 && coord.y < mapSize) {
                            float weight = 1 - Mathf.Sqrt (sqrDst) / radius;
                            weightSum += weight;
                            weights[addIndex] = weight;
                            indices[addIndex] = coord.y * mapSize + coord.x;
                            addIndex++;
                        }
                    }
                }
            }

            int numEntries = addIndex;
            erosionBrushIndices[i] = new int[numEntries];
            erosionBrushWeights[i] = new float[numEntries];

            for (int j = 0; j < numEntries; j++) {
                erosionBrushIndices[i][j] = indices[j];
                erosionBrushWeights[i][j] = weights[j] / weightSum;
            }
        }
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
}