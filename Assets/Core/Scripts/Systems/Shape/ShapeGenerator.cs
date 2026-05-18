using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class ShapeGenerator : MonoBehaviour
    {
        [Range(2, 256)]
        public int resolution = 64;
        public float radius = 1000f;

        [Header("Settings")]
        public ShapeSettings shapeSettings;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private Mesh mesh;

        public float MinElevation { get; private set; }
        public float MaxElevation { get; private set; }

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

#if UNITY_EDITOR
            Mesh savedMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(
                "Assets/Core/Visuals/Meshes/PlanetMesh.asset");
            if (savedMesh != null)
            {
                mesh = savedMesh;
                meshFilter.sharedMesh = mesh;
                meshCollider.sharedMesh = mesh;
                return;
            }
#endif
            if (mesh == null)
            {
                mesh = new Mesh { name = "Planet Shape Mesh" };
                meshFilter.sharedMesh = mesh;
            }
        }

        public void GenerateShape()
        {
            if (shapeSettings == null ||
                shapeSettings.noiseLayers == null ||
                shapeSettings.noiseLayers.Length == 0)
                return;

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            // Always use a fresh in-memory mesh, never write directly to saved asset
            mesh = new Mesh { name = "Planet Shape Mesh" };

            // 1. Create unit sphere
            SphereCreator.CreateSphereMesh(
                resolution,
                1f,
                out Vector3[] vertices,
                out int[] triangles,
                out Vector2[] uvs
            );

            // 2. Apply terrain deformation
            TerrainGenerator.ApplyTerrainDeformation(
                vertices,
                shapeSettings,
                out Vector3[] displaced,
                out float min,
                out float max
            );

            MinElevation = min;
            MaxElevation = max;

            // 3. Scale to radius
            for (int i = 0; i < displaced.Length; i++)
            {
                displaced[i] = displaced[i].normalized * displaced[i].magnitude * radius;
            }

            // 4. Build mesh
            mesh.Clear();
            mesh.vertices = displaced;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }

        // Only called from editor button, never automatically
        public void GenerateAndSave()
        {
            GenerateShape();
#if UNITY_EDITOR
            // Never save during or after play mode — causes reimport freeze
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                SaveMeshAsset();
#endif
        }

#if UNITY_EDITOR
        private void SaveMeshAsset()
        {
            string folderPath = "Assets/Core/Visuals/Meshes";
            string assetPath = $"{folderPath}/PlanetMesh.asset";

            if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
                UnityEditor.AssetDatabase.CreateFolder("Assets/Core/Visuals", "Meshes");

            // Snapshot data before touching existing asset
            Vector3[] verts = mesh.vertices;
            int[] tris = mesh.triangles;
            Vector2[] uvs = mesh.uv;

            Mesh existingMesh = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            if (existingMesh != null)
            {
                existingMesh.Clear();
                existingMesh.vertices = verts;
                existingMesh.triangles = tris;
                existingMesh.uv = uvs;
                existingMesh.RecalculateNormals();
                existingMesh.RecalculateBounds();

                meshFilter.sharedMesh = existingMesh;
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = existingMesh;

                UnityEditor.EditorUtility.SetDirty(existingMesh);
            }
            else
            {
                Mesh savedMesh = new Mesh { name = "PlanetMesh" };
                savedMesh.vertices = verts;
                savedMesh.triangles = tris;
                savedMesh.uv = uvs;
                savedMesh.RecalculateNormals();
                savedMesh.RecalculateBounds();

                UnityEditor.AssetDatabase.CreateAsset(savedMesh, assetPath);

                meshFilter.sharedMesh = savedMesh;
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = savedMesh;
            }

            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}