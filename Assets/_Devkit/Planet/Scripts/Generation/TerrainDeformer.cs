using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    public static class TerrainDeformer
    {
        public static void ApplyTerrainDeformation(
            Vector3[] baseVertices,
            NoiseLayer[] layers,
            float globalHeightOffset,
            out Vector3[] displacedVertices,
            out float minElevation,
            out float maxElevation)
        {
            displacedVertices = new Vector3[baseVertices.Length];

            minElevation = float.MaxValue;
            maxElevation = float.MinValue;

            if (layers == null || layers.Length == 0)
            {
                Debug.LogError("[TerrainDeformer] Missing noise layers.");
                return;
            }

            for (int i = 0; i < baseVertices.Length; i++)
            {
                Vector3 normal = baseVertices[i].normalized;

                float displacement = globalHeightOffset;
                float firstLayerValue = 0f;

                for (int l = 0; l < layers.Length; l++)
                {
                    NoiseLayer layer = layers[l];

                    if (layer == null || !layer.enabled)
                        continue;

                    float noise = 0f;
                    float amplitude = 1f;
                    float frequency = layer.roughness;
                    float totalAmplitude = 0f;

                    for (int o = 0; o < layer.octaves; o++)
                    {
                        Vector3 p = (normal + layer.offset) * frequency;

                        float v = PerlinNoise3D.GenerateNoise(p.x, p.y, p.z);
                        v = v * 2f - 1f;

                        noise += v * amplitude;
                        totalAmplitude += amplitude;

                        amplitude *= layer.persistence;
                        frequency *= layer.lacunarity;
                    }

                    float layerValue =
                        totalAmplitude == 0f ? 0f : noise / totalAmplitude;

                    if (l == 0)
                        firstLayerValue = layerValue;

                    if (layer.useFirstLayerAsMask && firstLayerValue <= 0f)
                        layerValue = 0f;

                    displacement += layerValue * layer.strength;
                }

                float finalRadius = 1f + displacement;

                Vector3 displaced = normal * finalRadius;

                displacedVertices[i] = displaced;

                float height = displaced.magnitude;

                minElevation = Mathf.Min(minElevation, height);
                maxElevation = Mathf.Max(maxElevation, height);
            }
        }
    }
}