using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion : MonoBehaviour {

    public void Erode(float[] map, int mapSize) {
    {

        // Create water droplet at random point on map

        // Calculate direction of flow from the height difference of surrounding points

        // Update the droplet's direction, speed, position, and apply evaporation

        // Calculate the sediment carry capacity of the droplet

        // Erode or deposit soil
    }

    struct WaterDroplet {
        Vector2 position;
        Vector2 direction;
        float speed;
        float sediment;
        float waterVolume;
    }
   
}