using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class TerrainGenerator : MonoBehaviour
    {
        [Header("Mesh Settings")]
        [Range(2, 256)]
        public int resolution = 64;

        public float radius = 2000f;

        [Header("Terrain Settings")]
        public TerrainShapeSettings shapeSettings;
        public TerrainColorSettings colorSettings;

        [Range(-1f, 1f)]
        public float globalHeightOffset = 0f;

        public NoiseLayerInstance[] layerInstances;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private Mesh mesh;

        public float MinElevation { get; private set; }
        public float MaxElevation { get; private set; }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        private void EnsureInitialized()
        {
            if (shapeSettings == null || shapeSettings.noiseLayers == null)
            {
                Debug.LogError("[TerrainGenerator] Missing ShapeSettings.");
                return;
            }

            if (layerInstances != null &&
                layerInstances.Length == shapeSettings.noiseLayers.Length)
                return;

            layerInstances = new NoiseLayerInstance[shapeSettings.noiseLayers.Length];

            for (int i = 0; i < layerInstances.Length; i++)
            {
                layerInstances[i] = new NoiseLayerInstance
                {
                    layer = shapeSettings.noiseLayers[i],
                    enabledOverride = true
                };
            }
        }

        public void GenerateTerrain()
        {
            Debug.Log("[TerrainGenerator] GenerateTerrain CALLED");

            EnsureInitialized();

            SphereCreator.CreateSphereMesh(
                resolution,
                1f,
                out Vector3[] vertices,
                out int[] triangles,
                out Vector2[] uvs);

            TerrainDeformer.ApplyTerrainDeformation(
                vertices,
                layerInstances,
                out Vector3[] displaced,
                out float min,
                out float max);

            MinElevation = min;
            MaxElevation = max;

            // FINAL SCALE ONLY (important fix)
            for (int i = 0; i < displaced.Length; i++)
            {
                displaced[i] *= radius;
            }

            mesh = new Mesh { name = "Planet Terrain Mesh" };

            mesh.vertices = displaced;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void GenerateAndSave()
        {
            GenerateTerrain();

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                SaveMeshAsset();
#endif
        }

#if UNITY_EDITOR
        private void SaveMeshAsset()
        {
            string folderPath = "Assets/_Devkit/Planet/Meshes";
            string assetPath = $"{folderPath}/PlanetMesh.asset";

            if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
            {
                UnityEditor.AssetDatabase.CreateFolder(
                    "Assets/_Devkit/Planet",
                    "Meshes");
            }

            Vector3[] verts = mesh.vertices;
            int[] tris = mesh.triangles;
            Vector2[] uvs = mesh.uv;

            Mesh existingMesh =
                UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

            if (existingMesh != null)
            {
                existingMesh.Clear();
                existingMesh.vertices = verts;
                existingMesh.triangles = tris;
                existingMesh.uv = uvs;

                existingMesh.RecalculateNormals();
                existingMesh.RecalculateBounds();

                meshFilter.sharedMesh = existingMesh;
                meshCollider.sharedMesh = existingMesh;

                UnityEditor.EditorUtility.SetDirty(existingMesh);
            }
            else
            {
                Mesh savedMesh = new Mesh { name = "PlanetTerrainMesh" };

                savedMesh.vertices = verts;
                savedMesh.triangles = tris;
                savedMesh.uv = uvs;

                savedMesh.RecalculateNormals();
                savedMesh.RecalculateBounds();

                UnityEditor.AssetDatabase.CreateAsset(savedMesh, assetPath);

                meshFilter.sharedMesh = savedMesh;
                meshCollider.sharedMesh = savedMesh;
            }

            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}