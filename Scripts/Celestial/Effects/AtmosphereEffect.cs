using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to handle the atmosphere effect for a celestial body
public class AtmosphereEffect {

    // Reference to the light source (e.g., the sun)
    Light light;
    // Material to apply the atmosphere effect
    protected Material material;

    // Method to update the atmosphere settings based on the generator
    public void UpdateSettings(CelestialBodyGenerator generator) {

        // Get the atmosphere shader from the generator's shading settings
        Shader shader = generator.body.shading.atmosphereSettings.atmosphereShader;

        // Create a new material if the current one is null or uses a different shader
        if (material == null || material.shader != shader) {
            material = new Material(shader);
        }

        // Find the light source if it's not already assigned
        if (light == null) {
            light = GameObject.FindObjectOfType<SunShadowCaster>()?.GetComponent<Light>();
        }

        // Update the atmosphere properties in the material using the generator's settings
        generator.body.shading.atmosphereSettings.SetProperties(material, generator.BodyScale);

        // Set the position of the planet's center in the material
        material.SetVector("planetCentre", generator.transform.position);

        // Set the ocean radius in the material
        material.SetFloat("oceanRadius", generator.GetOceanRadius());

        // If a light source is found, set the direction to the sun in the material
        if (light) {
            Vector3 dirFromPlanetToSun = (light.transform.position - generator.transform.position).normalized;
            material.SetVector("dirToSun", dirFromPlanetToSun);
        } else {
            // If no light source is found, default to an upward direction and log a warning
            material.SetVector("dirToSun", Vector3.up);
            Debug.Log("No SunShadowCaster found");
        }
    }

    // Method to get the material with the atmosphere effect applied
    public Material GetMaterial() {
        return material;
    }
}
