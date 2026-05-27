using MakoJBryant.SolarSystem.Generation;
using UnityEngine;

public static class TerrainColorGenerator
{
    public static Texture2D GenerateColorTexture(
        TerrainColorSettings colorSettings,
        int resolution = 256)
    {
        if (colorSettings == null ||
            colorSettings.regions == null ||
            colorSettings.regions.Length == 0)
            return null;

        var regions = colorSettings.regions;

        Texture2D texture = new Texture2D(
            resolution,
            1,
            TextureFormat.RGBA32,
            false)
        {
            name = "TerrainColorTexture",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[resolution];

        System.Array.Sort(
            regions,
            (a, b) => a.startHeight.CompareTo(b.startHeight));

        for (int i = 0; i < resolution; i++)
        {
            float h = i / (float)(resolution - 1);

            Color col = regions[0].color;
            float bestBlend = 0f;

            foreach (var region in regions)
            {
                float blend =
                    Mathf.Clamp01(
                        (h - region.startHeight) /
                        region.blendAmount);

                if (blend >= bestBlend)
                {
                    bestBlend = blend;
                    col = region.color;
                }
            }

            pixels[i] = col;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }
}