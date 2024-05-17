using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Atmosphere : CustomImageEffect {

    // Reference to the celestial body generator
    public CelestialBodyGenerator planet;
    // Scale factor for the atmosphere
    [Range(0, 1)]
    public float atmosphereScale = 0.2f;

    // Color of the atmosphere
    public Color color;
    // Parameters for testing
    public Vector4 testParams;
    // Compute buffer for sphere data
    ComputeBuffer buffer;
    // Texture for falloff
    Texture2D falloffTex;
    // Gradient for falloff
    public Gradient falloff;
    // Resolution of the gradient texture
    public int gradientRes = 10;
    // Number of steps for atmosphere rendering
    public int numSteps = 10;
    // Texture for blue noise
    public Texture2D blueNoise;

    // Struct to represent a sphere
    public struct Sphere {
        public Vector3 centre;
        public float radius;
        public float waterRadius;

        public static int Size {
            get {
                return sizeof(float) * 5;
            }
        }
    }

    // Method to get the material with applied atmosphere effect
    public override Material GetMaterial() {

        // Validate inputs
        if (material == null || material.shader != shader) {
            if (shader == null) {
                shader = Shader.Find("Unlit/Texture");
            }
            material = new Material(shader);
        }

        // Set sphere data
        Sphere sphere = new Sphere() {
            centre = planet.transform.position,
            radius = (1 + atmosphereScale) * planet.BodyScale,
            waterRadius = planet.GetOceanRadius()
        };

        // Create and set compute buffer
        buffer = new ComputeBuffer(1, Sphere.Size);
        buffer.SetData(new Sphere[] { sphere });
        material.SetBuffer("spheres", buffer);

        // Set other material properties
        material.SetVector("params", testParams);
        material.SetColor("_Color", color);
        material.SetFloat("planetRadius", planet.BodyScale);
        // Create falloff texture from gradient and set it in material
        CelestialBodyShading.TextureFromGradient(ref falloffTex, gradientRes, falloff);
        material.SetTexture("_Falloff", falloffTex);
        material.SetTexture("_BlueNoise", blueNoise);
        material.SetInt("numSteps", numSteps);
        return material;
    }

    // Method to release resources
    public override void Release() {
        buffer.Release();
    }
}
