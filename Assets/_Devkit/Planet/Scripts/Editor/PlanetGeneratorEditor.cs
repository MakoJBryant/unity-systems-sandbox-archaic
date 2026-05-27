using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    private PlanetGenerator planet;

    private SerializedProperty terrainProp;
    private SerializedProperty oceanProp;
    private SerializedProperty atmosphereProp;

    private SerializedProperty rotationSpeedProp;
    private SerializedProperty axialTiltProp;
    private SerializedProperty sunProp;
    private SerializedProperty orbitSpeedProp;
    private SerializedProperty gravityStrengthProp;

    private void OnEnable()
    {
        planet = (PlanetGenerator)target;

        terrainProp =
            serializedObject.FindProperty("terrain");

        oceanProp =
            serializedObject.FindProperty("ocean");

        atmosphereProp =
            serializedObject.FindProperty("atmosphere");

        rotationSpeedProp =
            serializedObject.FindProperty("rotationSpeed");

        axialTiltProp =
            serializedObject.FindProperty("axialTilt");

        sunProp =
            serializedObject.FindProperty("sun");

        orbitSpeedProp =
            serializedObject.FindProperty("orbitSpeed");

        gravityStrengthProp =
            serializedObject.FindProperty("gravityStrength");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawActionsSection();

        EditorGUILayout.Space(10);

        DrawSubsystemReferencesSection();

        EditorGUILayout.Space(10);

        DrawPlanetPropertiesSection();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawActionsSection()
    {
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();

            if (planet.terrain != null)
            {
                planet.terrain.GenerateAndSave();
            }
        }
    }

    private void DrawSubsystemReferencesSection()
    {
        EditorGUILayout.LabelField("Subsystem References", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(terrainProp);
        EditorGUILayout.PropertyField(oceanProp);
        EditorGUILayout.PropertyField(atmosphereProp);

        if (planet.terrain == null)
        {
            EditorGUILayout.HelpBox(
                "TerrainGenerator reference is not assigned.",
                MessageType.Warning);
        }
    }

    private void DrawPlanetPropertiesSection()
    {
        EditorGUILayout.LabelField("Planet Properties", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(sunProp);

        EditorGUILayout.Space(4);

        EditorGUILayout.PropertyField(rotationSpeedProp);
        EditorGUILayout.PropertyField(orbitSpeedProp);
        EditorGUILayout.PropertyField(axialTiltProp);

        EditorGUILayout.Space(4);

        EditorGUILayout.PropertyField(gravityStrengthProp);
    }
}