using UnityEngine;

public static class PlanetVisualUpdater
{
    public static void ApplyMaterialProperties(
        Material mat,
        float radius,
        float minHeight,
        float maxHeight,
        Vector3 center,
        Texture2D biomeTex,
        float biomeShift = 0f)
    {
        if (mat == null) return;
        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_MinHeight", minHeight + biomeShift);
        mat.SetFloat("_MaxHeight", maxHeight);
        mat.SetVector("_PlanetCenter", center);
        if (biomeTex != null)
            mat.SetTexture("_BiomeTexture", biomeTex);
    }

#if UNITY_EDITOR
    public static Texture2D SaveBiomeTexture(Texture2D texture)
    {
        string folderPath = "Assets/_Devkit/Planet/Textures";
        string assetPath = $"{folderPath}/BiomeTexture.asset";

        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Core/Visuals", "Textures");

        Texture2D existing = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (existing != null)
        {
            UnityEditor.EditorUtility.CopySerialized(texture, existing);
            UnityEditor.EditorUtility.SetDirty(existing);
            UnityEditor.AssetDatabase.SaveAssets();
            return existing;
        }
        else
        {
            UnityEditor.AssetDatabase.CreateAsset(texture, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            return texture;
        }
    }
#endif
}