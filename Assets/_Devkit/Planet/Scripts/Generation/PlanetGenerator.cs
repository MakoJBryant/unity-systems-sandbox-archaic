using UnityEngine;
using MakoJBryant.SolarSystem.Generation;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class PlanetGenerator : MonoBehaviour
{
    [Header("Subsystem References")]
    public ShapeGenerator terrain;
    public OceanGenerator ocean;
    public AtmosphereGenerator atmosphere;

    [Header("Biome Settings")]
    public BiomeSettings biomeSettings;

    [Header("Planet Properties")]
    public float rotationSpeed = 10f;
    public float axialTilt = 23.5f;

    [Header("Gravity")]
    public float gravityStrength = 9.8f;

    private MeshRenderer meshRenderer;
    private Texture2D biomeTexture;

    void Awake()
    {
        transform.rotation = Quaternion.Euler(axialTilt, 0f, 0f);
    }

    [ContextMenu("Generate Planet")]
    public void GeneratePlanet()
    {
        if (terrain == null) return;

        // 1. Generate terrain shape
        terrain.GenerateShape();

        float radius = terrain.radius;

        // Normalized elevations for shader (values around 1.0)
        float minElevNormalized = terrain.MinElevation;
        float maxElevNormalized = terrain.MaxElevation;

        // World scale elevations for ocean/atmosphere
        float minElevation = terrain.MinElevation * radius;
        float maxElevation = terrain.MaxElevation * radius;

        // 2. Generate biome texture
        biomeTexture = BiomeGenerator.GenerateBiomeTexture(biomeSettings);

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            biomeTexture = PlanetVisualUpdater.SaveBiomeTexture(biomeTexture);
#endif

        // 3. Apply material properties to terrain using normalized elevations
        meshRenderer = terrain.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            PlanetVisualUpdater.ApplyMaterialProperties(
                meshRenderer.sharedMaterial,
                radius,
                minElevNormalized,
                maxElevNormalized,
                transform.position,
                biomeTexture
            );
        }

        // 4. Generate ocean using world scale elevations
        if (ocean != null)
            ocean.Generate(terrain.resolution, minElevation, maxElevation);

        // 5. Generate atmosphere using world scale radius
        if (atmosphere != null)
            atmosphere.Generate(terrain.resolution, radius, maxElevation);
    }

    public void GenerateAndSave()
    {
        GeneratePlanet();
        if (terrain != null)
            terrain.GenerateAndSave();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

}