using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Custom editor for the TextureViewer component and its derived classes
[CustomEditor(typeof(TextureViewer), true)]
public class TextureEditor : Editor {

    // Editor for the texture generator settings
    Editor generatorEditor;
    // Foldout state for the generator settings in the inspector
    bool generatorFoldout;

    // Override the default inspector GUI
    public override void OnInspectorGUI() {
        // Draw the default inspector GUI
        DrawDefaultInspector();

        // Get a reference to the target TextureViewer instance
        var textureViewer = (TextureViewer)target;

        // Button to generate or update the texture
        if (GUILayout.Button("Generate")) {
            textureViewer.UpdateTexture();
        }

        // Button to save the generated texture to the Resources folder
        if (GUILayout.Button("Save")) {
            string path = Application.dataPath + "/Resources";
            textureViewer.SaveTexture(path);
        }

        // If a generator is assigned, draw its settings editor
        if (textureViewer.generator) {
            bool settingsUpdated = DrawSettingsEditor(textureViewer.generator, ref generatorFoldout, ref generatorEditor);
            // If the generator settings were updated, regenerate the texture
            if (settingsUpdated) {
                textureViewer.UpdateTexture();
            }
        }
    }

    // Method to draw settings editor for the generator
    bool DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor) {
        if (settings != null) {
            // Create a foldout for the generator settings
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            if (foldout) {
                // Check for changes within the settings editor
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();
                    // Return true if any settings were changed
                    if (check.changed) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Method called when the editor is enabled
    private void OnEnable() {
        // Retrieve the foldout state from editor preferences
        generatorFoldout = EditorPrefs.GetBool(nameof(generatorFoldout), false);
    }

    // Method called when the editor is disabled
    void OnDisable() {
        // Save the foldout state to editor preferences
        EditorPrefs.SetBool(nameof(generatorFoldout), generatorFoldout);
    }
}
