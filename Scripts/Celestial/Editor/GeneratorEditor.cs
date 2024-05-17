using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Custom editor for the CelestialBodyGenerator component
[CustomEditor(typeof(CelestialBodyGenerator))]
public class GeneratorEditor : Editor {

    // Reference to the CelestialBodyGenerator instance being edited
    CelestialBodyGenerator generator;
    // Editors for shape and shading settings
    Editor shapeEditor;
    Editor shadingEditor;

    // Foldout states for shape and shading settings in the inspector
    bool shapeFoldout;
    bool shadingFoldout;

    // Override the default inspector GUI
    public override void OnInspectorGUI() {
        // Create a scope to check for changes in the inspector
        using (var check = new EditorGUI.ChangeCheckScope()) {
            DrawDefaultInspector();
            // If any changes are detected, regenerate the celestial body
            if (check.changed) {
                Regenerate();
            }
        }

        // Button to manually trigger generation
        if (GUILayout.Button("Generate")) {
            Regenerate();
        }
        // Button to randomize shading settings
        if (GUILayout.Button("Randomize Shading")) {
            var prng = new System.Random();
            generator.body.shading.randomize = true;
            generator.body.shading.seed = prng.Next(-10000, 10000);
            Regenerate();
        }

        // Button to randomize shape settings
        if (GUILayout.Button("Randomize Shape")) {
            var prng = new System.Random();
            generator.body.shape.randomize = true;
            generator.body.shape.seed = prng.Next(-10000, 10000);
            Regenerate();
        }

        // Button to randomize both shape and shading settings
        if (GUILayout.Button("Randomize Both")) {
            var prng = new System.Random();
            generator.body.shading.randomize = true;
            generator.body.shape.randomize = true;
            generator.body.shape.seed = prng.Next(-10000, 10000);
            generator.body.shading.seed = prng.Next(-10000, 10000);
            Regenerate();
        }

        // Check if any randomization settings are active
        bool randomized = generator.body.shading.randomize || generator.body.shape.randomize;
        randomized |= generator.body.shading.seed != 0 || generator.body.shape.seed != 0;
        // Disable the reset button if no randomization settings are active
        using (new EditorGUI.DisabledGroupScope(!randomized)) {
            // Button to reset randomization settings
            if (GUILayout.Button("Reset Randomization")) {
                var prng = new System.Random();
                generator.body.shading.randomize = false;
                generator.body.shape.randomize = false;
                generator.body.shape.seed = 0;
                generator.body.shading.seed = 0;
                Regenerate();
            }
        }

        // Draw the shape and shading settings editors
        DrawSettingsEditor(generator.body.shape, ref shapeFoldout, ref shapeEditor);
        DrawSettingsEditor(generator.body.shading, ref shadingFoldout, ref shadingEditor);

        // Save the current foldout states
        SaveState();
    }

    // Method to regenerate the celestial body
    void Regenerate() {
        generator.OnShapeSettingChanged();
        generator.OnShadingNoiseSettingChanged();
        EditorApplication.QueuePlayerLoopUpdate();
    }

    // Method to draw settings editor for shape and shading
    void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor) {
        if (settings != null) {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            if (foldout) {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
            }
        }
    }

    // Method called when the editor is enabled
    private void OnEnable() {
        // Retrieve foldout states from editor preferences
        shapeFoldout = EditorPrefs.GetBool(nameof(shapeFoldout), false);
        shadingFoldout = EditorPrefs.GetBool(nameof(shadingFoldout), false);
        // Initialize the generator reference
        generator = (CelestialBodyGenerator)target;
    }

    // Method to save the current foldout states
    void SaveState() {
        EditorPrefs.SetBool(nameof(shapeFoldout), shapeFoldout);
        EditorPrefs.SetBool(nameof(shadingFoldout), shadingFoldout);
    }
}
