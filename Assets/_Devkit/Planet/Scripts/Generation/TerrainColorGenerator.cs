using MakoJBryant.SolarSystem.Generation;
using UnityEngine;

public static class TerrainColorGenerator
{
    public static Texture2D GenerateColorTexture(
        TerrainColorSettings colorSettings,
        float minElevation,
        float maxElevation,
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
            name = "TerrainColorMap",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[resolution];

        System.Array.Sort(
            regions,
            (a, b) => a.startHeight.CompareTo(b.startHeight));

        float elevRange = maxElevation - minElevation;

        for (int i = 0; i < resolution; i++)
        {
            // h is 0-1 across the texture, but represents minElevation to maxElevation
            float t = i / (float)(resolution - 1);
            float elevation = Mathf.Lerp(minElevation, maxElevation, t);

            // Normalize elevation back to 0-1 within the actual terrain range
            // so startHeight values in the SO always mean:
            //   0 = sea level (minElevation), 1 = highest peak (maxElevation)
            float h = elevRange > 0f
                ? (elevation - minElevation) / elevRange
                : 0f;

            Color col = regions[0].color;

            for (int r = 0; r < regions.Length; r++)
            {
                if (h < regions[r].startHeight) break;

                col = regions[r].color;

                if (r < regions.Length - 1)
                {
                    float nextStart = regions[r + 1].startHeight;
                    float blendWidth = regions[r].blendAmount;
                    float blendStart = nextStart - blendWidth;
                    float blendEnd = nextStart;

                    if (h >= blendStart)
                    {
                        float bt = Mathf.InverseLerp(blendStart, blendEnd, h);
                        bt = Mathf.SmoothStep(0f, 1f, bt);
                        col = Color.Lerp(regions[r].color, regions[r + 1].color, bt);
                    }
                }
            }

            pixels[i] = new Color(col.r, col.g, col.b, 1f);
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}