using UnityEngine;
using MakoJBryant.SolarSystem.Generation;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class PlanetGenerator : MonoBehaviour
{
    // =========================================================
    // SUBSYSTEM REFERENCES
    // =========================================================

    public TerrainGenerator terrain;
    public OceanGenerator ocean;
    public AtmosphereGenerator atmosphere;

    // =========================================================
    // PLANET PROPERTIES
    // =========================================================

    public float rotationSpeed = 10f;
    public float axialTilt = 23.5f;
    public Transform sun;
    public float orbitSpeed = 1f;
    public float gravityStrength = 9.8f;

    // =========================================================
    // INTERNALS
    // =========================================================

    private MeshRenderer meshRenderer;
    private Texture2D terrainColorTexture;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(axialTilt, 0f, 0f);
    }

    // =========================================================
    // GENERATE — preview in editor, no disk writes
    // =========================================================

    [ContextMenu("Generate Planet")]
    public void GeneratePlanet()
    {
        if (terrain == null)
        {
            Debug.LogWarning("[PlanetGenerator] TerrainGenerator is missing.");
            return;
        }

        // -------------------------------------------------
        // 1. GENERATE TERRAIN
        // -------------------------------------------------

        terrain.GenerateTerrain();

        float radius = terrain.radius;

        float minElevNormalized = terrain.MinElevation;
        float maxElevNormalized = terrain.MaxElevation;

        float minElevation = terrain.MinElevation * radius;
        float maxElevation = terrain.MaxElevation * radius;

        // -------------------------------------------------
        // 2. GENERATE TERRAIN COLOR TEXTURE
        //    Pass actual elevation range so texture h values
        //    match what the shader samples
        // -------------------------------------------------

        if (terrain.colorSettings != null)
        {
            terrainColorTexture =
                TerrainColorGenerator.GenerateColorTexture(
                    terrain.colorSettings,
                    terrain.MinElevation,
                    terrain.MaxElevation);

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                terrainColorTexture =
                    PlanetVisualUpdater.SaveTerrainColorTexture(
                        terrainColorTexture);
            }
#endif
        }

        // -------------------------------------------------
        // 3. APPLY TERRAIN MATERIALS
        // -------------------------------------------------

        meshRenderer = terrain.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            PlanetVisualUpdater.ApplyMaterialProperties(
                meshRenderer.sharedMaterial,
                radius,
                minElevNormalized,
                maxElevNormalized,
                transform.position,
                terrainColorTexture
            );
        }

        // -------------------------------------------------
        // 4. GENERATE OCEAN
        // -------------------------------------------------

        if (ocean != null)
        {
            ocean.Generate(
                terrain.resolution,
                minElevation,
                maxElevation);
        }

        // -------------------------------------------------
        // 5. GENERATE ATMOSPHERE
        // -------------------------------------------------

        if (atmosphere != null)
        {
            atmosphere.Generate(
                terrain.resolution,
                radius,
                maxElevation);
        }
    }

    // =========================================================
    // SAVE — writes mesh asset to disk, separate from generate
    // =========================================================

    [ContextMenu("Save Planet")]
    public void SavePlanet()
    {
        if (terrain == null)
        {
            Debug.LogWarning("[PlanetGenerator] TerrainGenerator is missing — nothing to save.");
            return;
        }

#if UNITY_EDITOR
        terrain.Save();
        Debug.Log("[PlanetGenerator] Planet saved.");
#endif
    }

    // =========================================================
    // RUNTIME
    // =========================================================

    private void Update()
    {
        if (Application.isPlaying)
        {
            transform.Rotate(
                Vector3.up,
                rotationSpeed * Time.deltaTime,
                Space.Self);

            if (sun != null)
            {
                transform.RotateAround(
                    sun.position,
                    Vector3.up,
                    orbitSpeed * Time.deltaTime);
            }
        }
    }

    private void OnValidate()
    {
        if (terrain == null)
            terrain = GetComponentInChildren<TerrainGenerator>();

        if (ocean == null)
            ocean = GetComponentInChildren<OceanGenerator>();

        if (atmosphere == null)
            atmosphere = GetComponentInChildren<AtmosphereGenerator>();
    }
}