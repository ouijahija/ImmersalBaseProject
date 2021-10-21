using System;
using System.IO;
using UniGLTF;
using UnityEditor;
using UnityEngine;

public class UniGltfCustomImporter : EditorWindow
{
    private string immersalMapPath = ImmersalConstants.ImmersalMapPath;
    private string immersalGlbStoragePath = ImmersalConstants.ImmersalGlbStoragePath;
    
    [MenuItem("Window/ImmersalSetup/UniGltfGlbImport", priority = 1)]
    public static void ShowWindow()
    {
        GetWindow<UniGltfCustomImporter>("ImmersalMapGlbDownloader");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        immersalMapPath = EditorGUILayout.TextField(immersalMapPath);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        immersalGlbStoragePath = EditorGUILayout.TextField(immersalGlbStoragePath);
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Import Glbs"))
        {
            ImportGlbs();
        }
    }
    
    private void ImportGlbs()
    {
     
        if (string.IsNullOrEmpty(immersalMapPath))
        {
            return;
        }

        if (Application.isPlaying)
        {
            return;
        }
       
        //
        // save as asset
        //
        if (immersalMapPath.StartsWithUnityAssetPath())
        {
            Debug.LogWarningFormat("disallow import from folder under the Assets");
            return;
        }
        
        var info = new DirectoryInfo(immersalMapPath);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
           Import(file, immersalGlbStoragePath);
           
        }


    }

    private void Import(FileInfo file, string destinationDir)
    {
        string srcPath = file.FullName;
        var map_id_and_name = file.Name.Split(new[] { "-tex" }, StringSplitOptions.None)[0];
        string destinationPath = destinationDir +  map_id_and_name + "/" + map_id_and_name + ".prefab";
        
        Debug.Log(srcPath);
        Debug.Log(destinationPath);
        Debug.Log(UnityPath.FromFullpath(destinationPath));
        // import as asset
         gltfAssetPostprocessor.ImportAsset(srcPath, Path.GetExtension(srcPath).ToLower(), UnityPath.FromFullpath(destinationPath));
    }
    
  
}
