using UnityEngine;
using UnityEditor;

public class VegetationRandomizer : EditorWindow
{
    private Transform parent;
    private Vector3 minRotation = Vector3.zero;
    private Vector3 maxRotation = new Vector3(0, 360, 0);

    private Vector3 minScale = Vector3.one * 0.8f;
    private Vector3 maxScale = Vector3.one * 1.2f;

    [MenuItem("Tools/Vegetation/Randomize Children")]
    public static void ShowWindow()
    {
        GetWindow<VegetationRandomizer>("Vegetation Randomizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Randomize Children Settings", EditorStyles.boldLabel);

        parent = (Transform)EditorGUILayout.ObjectField("Parent Object", parent, typeof(Transform), true);

        GUILayout.Space(10);

        GUILayout.Label("Rotation Range");
        minRotation = EditorGUILayout.Vector3Field("Min Rotation", minRotation);
        maxRotation = EditorGUILayout.Vector3Field("Max Rotation", maxRotation);

        GUILayout.Space(10);

        GUILayout.Label("Scale Range");
        minScale = EditorGUILayout.Vector3Field("Min Scale", minScale);
        maxScale = EditorGUILayout.Vector3Field("Max Scale", maxScale);

        GUILayout.Space(20);

        if (GUILayout.Button("Apply Randomization"))
        {
            if (parent == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a parent object.", "OK");
                return;
            }

            RandomizeChildren(parent);
        }
    }

    private void RandomizeChildren(Transform parent)
    {
        Undo.RecordObject(parent, "Randomize Vegetation");

        foreach (Transform child in parent)
        {
            Undo.RecordObject(child, "Randomize Vegetation");

            // Rotation
            Vector3 newRot = new Vector3(
                Random.Range(minRotation.x, maxRotation.x),
                Random.Range(minRotation.y, maxRotation.y),
                Random.Range(minRotation.z, maxRotation.z)
            );
            child.localRotation = Quaternion.Euler(newRot);

            // Scale
            Vector3 newScale = new Vector3(
                Random.Range(minScale.x, maxScale.x),
                Random.Range(minScale.y, maxScale.y),
                Random.Range(minScale.z, maxScale.z)
            );
            child.localScale = newScale;
        }

        EditorUtility.SetDirty(parent);
        Debug.Log("Vegetation randomization applied!");
    }
}