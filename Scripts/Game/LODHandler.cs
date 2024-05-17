using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LODHandler : MonoBehaviour {
    // LOD screen heights
    [Header("LOD screen heights")]
    // LOD level is determined by body's screen height (1 = taking up entire screen, 0 = tiny speck) 
    public float lod1Threshold = .5f;
    public float lod2Threshold = .2f;

    // Debug options
    [Header("Debug")]
    public bool debug;
    public CelestialBody debugBody;

    // Reference to the main camera and its transform
    Camera cam;
    Transform camT;
    // Arrays to hold all celestial bodies and their generators
    CelestialBody[] bodies;
    CelestialBodyGenerator[] generators;

    void Start() {
        // In play mode, find all celestial bodies and their generators
        if (Application.isPlaying) {
            bodies = FindObjectsOfType<CelestialBody>();
            generators = new CelestialBodyGenerator[bodies.Length];
            for (int i = 0; i < generators.Length; i++) {
                generators[i] = bodies[i].GetComponentInChildren<CelestialBodyGenerator>();
            }
        }
    }

    void Update() {
        // Display debug information if debug mode is enabled
        DebugLODInfo();

        // Handle LODs during play mode
        if (Application.isPlaying) {
            HandleLODs();
        }

    }

    // Method to handle LODs for all celestial bodies
    void HandleLODs() {
        for (int i = 0; i < bodies.Length; i++) {
            if (generators[i] != null) {
                // Calculate the screen height of the celestial body and set its LOD
                float screenHeight = CalculateScreenHeight(bodies[i]);
                int lodIndex = CalculateLODIndex(screenHeight);
                generators[i].SetLOD(lodIndex);
            }

        }
    }

    // Method to calculate the LOD index based on the screen height
    int CalculateLODIndex(float screenHeight) {
        if (screenHeight > lod1Threshold) {
            return 0;
        } else if (screenHeight > lod2Threshold) {
            return 1;
        }
        return 2;
    }

    // Method to display debug information about LODs
    void DebugLODInfo() {
        if (debugBody && debug) {
            float h = CalculateScreenHeight(debugBody);
            int index = CalculateLODIndex(h);
            Debug.Log($"Screen height of {debugBody.name}: {h} (lod = {index})");
        }
    }

    // Method to calculate the screen height of a celestial body
    float CalculateScreenHeight(CelestialBody body) {
        if (cam == null) {
            cam = Camera.main;
            camT = cam.transform;
        }
        Quaternion originalRot = camT.rotation;
        Vector3 bodyCentre = body.transform.position;
        camT.LookAt(bodyCentre);

        Vector3 viewA = cam.WorldToViewportPoint(bodyCentre - camT.up * body.radius);
        Vector3 viewB = cam.WorldToViewportPoint(bodyCentre + camT.up * body.radius);
        float screenHeight = Mathf.Abs(viewA.y - viewB.y);
        camT.rotation = originalRot;

        return screenHeight;
    }
}
