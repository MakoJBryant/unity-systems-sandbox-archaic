using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    public static class TerrainDeformer
    {
        public static void ApplyTerrainDeformation(
            Vector3[] baseVertices,
            NoiseLayerInstance[] layerInstances,
            out Vector3[] displacedVertices,
            out float minElevation,
            out float maxElevation)
        {
            displacedVertices = new Vector3[baseVertices.Length];

            minElevation = float.MaxValue;
            maxElevation = float.MinValue;

            if (layerInstances == null || layerInstances.Length == 0)
            {
                Debug.LogError("[TerrainDeformer] Missing layer instances.");
                return;
            }

            for (int i = 0; i < baseVertices.Length; i++)
            {
                Vector3 normal = baseVertices[i].normalized;

                float displacement = 0f;
                float firstLayerValue = 0f;
                bool firstCaptured = false;

                for (int l = 0; l < layerInstances.Length; l++)
                {
                    NoiseLayerInstance inst = layerInstances[l];

                    if (inst == null || inst.layer == null)
                        continue;

                    if (!inst.enabledOverride)
                        continue;

                    NoiseLayer layer = inst.layer;

                    float strength = inst.overrideStrength ? inst.strength : layer.strength;
                    float roughness = inst.overrideRoughness ? inst.roughness : layer.roughness;
                    float persistence = inst.overridePersistence ? inst.persistence : layer.persistence;
                    float lacunarity = inst.overrideLacunarity ? inst.lacunarity : layer.lacunarity;
                    int octaves = inst.overrideOctaves ? inst.octaves : layer.octaves;
                    Vector3 offset = inst.overrideOffset ? inst.offset : layer.offset;

                    float noise = 0f;
                    float amplitude = 1f;
                    float frequency = roughness;
                    float totalAmplitude = 0f;

                    for (int o = 0; o < octaves; o++)
                    {
                        Vector3 p = (normal + offset) * frequency;

                        float v = PerlinNoise3D.GenerateNoise(p.x, p.y, p.z);
                        v = v * 2f - 1f;

                        noise += v * amplitude;
                        totalAmplitude += amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    float layerValue =
                        totalAmplitude == 0f ? 0f : noise / totalAmplitude;

                    if (!firstCaptured)
                    {
                        firstLayerValue = layerValue;
                        firstCaptured = true;
                    }

                    if (layer.useFirstLayerAsMask && firstLayerValue <= 0f)
                        continue;

                    displacement += layerValue * strength;
                }

                // IMPORTANT FIX: keep unit sphere space
                Vector3 displaced = normal * (1f + displacement);

                displacedVertices[i] = displaced;

                float h = displaced.magnitude;

                minElevation = Mathf.Min(minElevation, h);
                maxElevation = Mathf.Max(maxElevation, h);
            }
        }
    }
}