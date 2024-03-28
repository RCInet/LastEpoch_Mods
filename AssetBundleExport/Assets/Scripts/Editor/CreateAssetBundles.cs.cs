using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Create AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = Application.dataPath + "/../Out/FromScript";
        if (!Directory.Exists(assetBundleDirectory)) { Directory.CreateDirectory(assetBundleDirectory); }
        try
        {            
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        catch (Exception e) { Debug.LogWarning(e); }
        
		
    }
}