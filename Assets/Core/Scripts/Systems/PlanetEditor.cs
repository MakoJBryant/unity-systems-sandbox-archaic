using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    PlanetGenerator planet;
    Editor shapeEditor;

    void OnEnable()
    {
        planet = (PlanetGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        // Generate + Save — only triggered manually via button
        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
            if (planet.shapeGenerator != null)
                planet.shapeGenerator.GenerateAndSave();
        }

        if (planet.shapeGenerator == null)
        {
            EditorGUILayout.HelpBox(
                "ShapeGenerator is not assigned. PlanetGenerator needs a ShapeGenerator component.",
                MessageType.Warning
            );
        }
        else if (planet.shapeGenerator.shapeSettings == null)
        {
            EditorGUILayout.HelpBox(
                "ShapeSettings asset is not assigned on the ShapeGenerator.",
                MessageType.Warning
            );
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shape Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Tweak values below to preview. Click 'Generate Planet' to save the mesh permanently.",
                MessageType.Info
            );

            CreateCachedEditor(
                planet.shapeGenerator.shapeSettings,
                null,
                ref shapeEditor
            );

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                shapeEditor.OnInspectorGUI();
                if (check.changed)
                {
                    EditorUtility.SetDirty(planet.shapeGenerator.shapeSettings);
                    // Only regenerate shape for live preview, do NOT save
                    planet.GeneratePlanet();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}