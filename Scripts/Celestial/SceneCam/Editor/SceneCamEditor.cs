using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneCamManager))]
public class SceneCamEditor : Editor {

    // Reference to the SceneCamManager
    SceneCamManager manager;

    // Method to draw the custom inspector GUI
    public override void OnInspectorGUI() {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the last active SceneView and all SceneViews
        var activeSceneView = SceneView.lastActiveSceneView;
        var allViews = SceneView.sceneViews;

        // If there are saved views, display them
        if (manager.savedViews.Count > 0) {
            GUILayout.Label($"Saved views: ({manager.savedViews.Count})");
            int deleteIndex = -1;

            // Loop through each saved view and create UI for it
            for (int i = 0; i < manager.savedViews.Count; i++) {
                GUILayout.BeginVertical("GroupBox");
                var savedView = manager.savedViews[i];

                // Text field to edit the name of the saved view
                savedView.name = GUILayout.TextField(savedView.name);

                GUILayout.BeginHorizontal();
                // Button to set the camera to the saved view
                if (GUILayout.Button("Set Camera View")) {
                    Undo.RecordObject(manager, "Set Camera View");
                    activeSceneView.pivot = savedView.pivot;
                    activeSceneView.rotation = savedView.rotation;
                    activeSceneView.size = savedView.size;
                }
                // Button to replace the saved view with the current camera view
                if (GUILayout.Button("Replace")) {
                    Undo.RecordObject(manager, "Replace View");
                    savedView.pivot = activeSceneView.pivot;
                    savedView.rotation = activeSceneView.rotation;
                    savedView.size = activeSceneView.size;
                }
                // Button to delete the saved view
                if (GUILayout.Button("Delete")) {
                    Undo.RecordObject(manager, "Delete View");
                    deleteIndex = i;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            // Remove the view if delete button was clicked
            if (deleteIndex != -1) {
                manager.savedViews.RemoveAt(deleteIndex);
            }

            GUILayout.Space(15);
        }

        // Debugging code for listing all scene views (currently commented out)
        foreach (var v in allViews) {
            // GUILayout.Label(v.ToString());
        }

        // Button to save the current camera view
        if (GUILayout.Button("Save Current View")) {
            Undo.RecordObject(manager, "Save View");
            var newView = new SceneCamManager.SavedView();
            newView.name = $"View ({manager.savedViews.Count})";
            newView.pivot = activeSceneView.pivot;
            newView.rotation = activeSceneView.rotation;
            newView.size = activeSceneView.size;
            manager.savedViews.Add(newView);
        }
    }

    // Method called when the editor is enabled
    void OnEnable() {
        manager = (SceneCamManager)target;

        // Initialize the saved views list if it is null
        if (manager.savedViews == null) {
            manager.savedViews = new List<SceneCamManager.SavedView>();
        }
    }
}
