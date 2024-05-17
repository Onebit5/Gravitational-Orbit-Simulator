using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

// Creates a menu item to create an instance of AtmosphereSettings in the Unity Editor
[CreateAssetMenu(menuName = "Celestial Body/Atmosphere")]
public class AtmosphereSettings : ScriptableObject {

    // Public fields for configuring the atmosphere
    public bool enabled = true;
    public Shader atmosphereShader;
    public ComputeShader opticalDepthCompute;
    public int textureSize = 256;

    public int inScatteringPoints = 10;
    public int opticalDepthPoints = 10;
    public float densityFalloff = 0.25f;

    public Vector3 wavelengths = new Vector3(700, 530, 460);

    public Vector4 testParams = new Vector4(7, 1.26f, 0.1f, 3);
    public float scatteringStrength = 20;
    public float intensity = 1;

    public float ditherStrength = 0.8f;
    public float ditherScale = 4;
    public Texture2D blueNoise;

    [Range(0, 1)]
    public float atmosphereScale = 0.5f;

    [Header("Test")]
    public float timeOfDay;
    public float sunDst = 1;

    // Texture to store precomputed optical depth values
    RenderTexture opticalDepthTexture;
    // Flag to check if settings are up-to-date
    bool settingsUpToDate;

    // Method to set the properties of the atmosphere material
    public void SetProperties(Material material, float bodyRadius) {
        // Update the sun position based on the time of day
        if (!settingsUpToDate || !Application.isPlaying) {
            var sun = GameObject.Find("Test Sun");
            if (sun) {
                sun.transform.position = new Vector3(Mathf.Cos(timeOfDay), Mathf.Sin(timeOfDay), 0) * sunDst;
                sun.transform.LookAt(Vector3.zero);
            }

            // Calculate the atmosphere radius based on the body radius and atmosphere scale
            float atmosphereRadius = (1 + atmosphereScale) * bodyRadius;

            // Set various parameters for the atmosphere material
            material.SetVector("params", testParams);
            material.SetInt("numInScatteringPoints", inScatteringPoints);
            material.SetInt("numOpticalDepthPoints", opticalDepthPoints);
            material.SetFloat("atmosphereRadius", atmosphereRadius);
            material.SetFloat("planetRadius", bodyRadius);
            material.SetFloat("densityFalloff", densityFalloff);

            // Calculate the scattering coefficients based on wavelengths
            float scatterX = Pow(400 / wavelengths.x, 4);
            float scatterY = Pow(400 / wavelengths.y, 4);
            float scatterZ = Pow(400 / wavelengths.z, 4);
            material.SetVector("scatteringCoefficients", new Vector3(scatterX, scatterY, scatterZ) * scatteringStrength);
            material.SetFloat("intensity", intensity);
            material.SetFloat("ditherStrength", ditherStrength);
            material.SetFloat("ditherScale", ditherScale);
            material.SetTexture("_BlueNoise", blueNoise);

            // Precompute the out-scattering values
            PrecomputeOutScattering();
            material.SetTexture("_BakedOpticalDepth", opticalDepthTexture);

            // Mark settings as up-to-date
            settingsUpToDate = true;
        }
    }

    // Method to precompute out-scattering values
    void PrecomputeOutScattering() {
        if (!settingsUpToDate || opticalDepthTexture == null || !opticalDepthTexture.IsCreated()) {
            // Create the render texture for optical depth
            ComputeHelper.CreateRenderTexture(ref opticalDepthTexture, textureSize, FilterMode.Bilinear);
            opticalDepthCompute.SetTexture(0, "Result", opticalDepthTexture);
            opticalDepthCompute.SetInt("textureSize", textureSize);
            opticalDepthCompute.SetInt("numOutScatteringSteps", opticalDepthPoints);
            opticalDepthCompute.SetFloat("atmosphereRadius", (1 + atmosphereScale));
            opticalDepthCompute.SetFloat("densityFalloff", densityFalloff);
            opticalDepthCompute.SetVector("params", testParams);
            // Run the compute shader to fill the texture
            ComputeHelper.Run(opticalDepthCompute, textureSize, textureSize);
        }
    }

    // Method called when a value in the inspector is changed
    void OnValidate() {
        // Mark settings as outdated
        settingsUpToDate = false;
    }
}
