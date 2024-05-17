using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to handle the ocean effect for a celestial body
public class OceanEffect {

    // Reference to the light source (e.g., the sun)
    Light light;
    // Material to apply the ocean effect
    protected Material material;

    // Method to update the ocean settings based on the generator and shader
    public void UpdateSettings(CelestialBodyGenerator generator, Shader shader) {
        // Create a new material if the current one is null or uses a different shader
        if (material == null || material.shader != shader) {
            material = new Material(shader);
        }

        // Find the light source if it's not already assigned
        if (light == null) {
            light = GameObject.FindObjectOfType<SunShadowCaster>()?.GetComponent<Light>();
        }

        // Get the position of the generator and the ocean radius
        Vector3 centre = generator.transform.position;
        float radius = generator.GetOceanRadius();
        // Set the ocean center and radius in the material
        material.SetVector("oceanCentre", centre);
        material.SetFloat("oceanRadius", radius);

        // Set the planet scale in the material
        material.SetFloat("planetScale", generator.BodyScale);
        // If a light source is found, set the direction to the sun in the material
        if (light) {
            material.SetVector("dirToSun", -light.transform.forward);
        } else {
            // If no light source is found, default to an upward direction and log a warning
            material.SetVector("dirToSun", Vector3.up);
            Debug.Log("No SunShadowCaster found");
        }
        // Update the ocean properties in the material using the generator's settings
        generator.body.shading.SetOceanProperties(material);
    }

    // Method to get the material with the ocean effect applied
    public Material GetMaterial() {
        return material;
    }

}
