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

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
            if (planet.terrain != null)
                planet.terrain.GenerateAndSave();
        }

        if (planet.terrain == null)
        {
            EditorGUILayout.HelpBox(
                "Terrain reference is not assigned. Drag the Terrain child GameObject here.",
                MessageType.Warning);
        }
        else if (planet.terrain.shapeSettings == null)
        {
            EditorGUILayout.HelpBox(
                "ShapeSettings asset is not assigned on the Terrain.",
                MessageType.Warning);
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shape Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Tweak values below to preview. Click 'Generate Planet' to save permanently.",
                MessageType.Info);

            CreateCachedEditor(
                planet.terrain.shapeSettings,
                null,
                ref shapeEditor);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                shapeEditor.OnInspectorGUI();
                if (check.changed)
                {
                    EditorUtility.SetDirty(planet.terrain.shapeSettings);
                    planet.GeneratePlanet();
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}