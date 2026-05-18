using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

public class MapUnityProject : EditorWindow
{
    private string outputPath = "";
    private int maxDepth = 6;
    private bool includeMetaFiles = false;
    private bool scriptsOnly = false;

    [MenuItem("Tools/Map Project Structure")]
    public static void ShowWindow()
    {
        GetWindow<MapUnityProject>("Project Mapper");
    }

    void OnGUI()
    {
        GUILayout.Label("Unity Project Structure Mapper", EditorStyles.boldLabel);
        GUILayout.Space(5);

        maxDepth = EditorGUILayout.IntSlider("Max Folder Depth", maxDepth, 1, 10);
        includeMetaFiles = EditorGUILayout.Toggle("Include .meta files", includeMetaFiles);
        scriptsOnly = EditorGUILayout.Toggle("Scripts & Prefabs Only", scriptsOnly);

        GUILayout.Space(10);

        if (GUILayout.Button("Map Project & Save to Desktop"))
        {
            string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            outputPath = Path.Combine(desktop, "UnityProjectMap.txt");
            GenerateMap();
        }

        if (!string.IsNullOrEmpty(outputPath) && File.Exists(outputPath))
        {
            GUILayout.Space(5);
            EditorGUILayout.HelpBox("Saved to: " + outputPath, MessageType.Info);
        }
    }

    void GenerateMap()
    {
        StringBuilder sb = new StringBuilder();
        string assetsPath = Application.dataPath;
        string projectName = Path.GetFileName(Path.GetDirectoryName(assetsPath));

        sb.AppendLine("===========================================");
        sb.AppendLine($"UNITY PROJECT MAP: {projectName}");
        sb.AppendLine($"Generated: {System.DateTime.Now}");
        sb.AppendLine("===========================================");
        sb.AppendLine();

        // Map folder structure
        sb.AppendLine("FOLDER & FILE STRUCTURE:");
        sb.AppendLine("-------------------------------------------");
        MapDirectory(assetsPath, sb, 0, "Assets");

        // List all scripts separately
        sb.AppendLine();
        sb.AppendLine("===========================================");
        sb.AppendLine("ALL SCRIPTS (.cs files):");
        sb.AppendLine("-------------------------------------------");
        foreach (string file in Directory.GetFiles(assetsPath, "*.cs", SearchOption.AllDirectories))
        {
            string relative = file.Replace(assetsPath, "Assets").Replace("\\", "/");
            sb.AppendLine(relative);
        }

        // List all prefabs
        sb.AppendLine();
        sb.AppendLine("===========================================");
        sb.AppendLine("ALL PREFABS:");
        sb.AppendLine("-------------------------------------------");
        foreach (string file in Directory.GetFiles(assetsPath, "*.prefab", SearchOption.AllDirectories))
        {
            string relative = file.Replace(assetsPath, "Assets").Replace("\\", "/");
            sb.AppendLine(relative);
        }

        // List all ScriptableObjects
        sb.AppendLine();
        sb.AppendLine("===========================================");
        sb.AppendLine("ALL SCRIPTABLE OBJECTS (.asset files):");
        sb.AppendLine("-------------------------------------------");
        foreach (string file in Directory.GetFiles(assetsPath, "*.asset", SearchOption.AllDirectories))
        {
            string relative = file.Replace(assetsPath, "Assets").Replace("\\", "/");
            sb.AppendLine(relative);
        }

        // List all scenes
        sb.AppendLine();
        sb.AppendLine("===========================================");
        sb.AppendLine("ALL SCENES:");
        sb.AppendLine("-------------------------------------------");
        foreach (string file in Directory.GetFiles(assetsPath, "*.unity", SearchOption.AllDirectories))
        {
            string relative = file.Replace(assetsPath, "Assets").Replace("\\", "/");
            sb.AppendLine(relative);
        }

        File.WriteAllText(outputPath, sb.ToString());
        Debug.Log($"Project map saved to: {outputPath}");
        EditorUtility.RevealInFinder(outputPath);
    }

    void MapDirectory(string path, StringBuilder sb, int depth, string displayName)
    {
        if (depth > maxDepth) return;

        string indent = new string(' ', depth * 2);
        sb.AppendLine($"{indent}[{displayName}/]");

        // Files in this directory
        foreach (string file in Directory.GetFiles(path))
        {
            string ext = Path.GetExtension(file).ToLower();
            if (!includeMetaFiles && ext == ".meta") continue;
            if (scriptsOnly && ext != ".cs" && ext != ".prefab" && ext != ".asset" && ext != ".unity") continue;

            string fileName = Path.GetFileName(file);
            sb.AppendLine($"{indent}  {fileName}");
        }

        // Subdirectories
        foreach (string dir in Directory.GetDirectories(path))
        {
            string dirName = Path.GetFileName(dir);
            MapDirectory(dir, sb, depth + 1, dirName);
        }
    }
}
