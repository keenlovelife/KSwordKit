/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: OpenPathInFinderEditor.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-7-1
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace KSwordKit.Contents.ResourcesManagement.Editor
{
    public class OpenPathInFinderEditor
    {
        [MenuItem("Assets/KSwordKit/资源管理/打开资源包所在目录（默认位置）（当前编译平台）", false, 2000)]
        [MenuItem("KSwordKit/资源管理/打开资源包所在目录（默认位置）（当前编译平台）", false, 2000)]
        public static void OpenResourcePackagePathInFinder()
        {
            try
            {
                var outputPath = BuildAssetBundlesEditor.assetBundleOutputDirectory();
                EditorUtility.RevealInFinder(outputPath);
                UnityEngine.Debug.Log(BuildAssetBundlesEditor.KSwordKitName + ": 打开资源包所在目录（默认位置）（当前编译平台）-> 完成! ");
            }
            catch (System.Exception e)
            {
                Debug.LogError(BuildAssetBundlesEditor.KSwordKitName + ": 执行 `打开资源包所在目录（默认位置）（当前编译平台）` 时，发生错误 -> " + e.Message);
            }
        }
        [MenuItem("Assets/KSwordKit/资源管理/打开资源包所在目录（StreamingAssets）（当前编译平台）", false, 2000)]
        [MenuItem("KSwordKit/资源管理/打开资源包所在目录（StreamingAssets）（当前编译平台）", false, 2000)]
        public static void OpenResourcePackageStreamingAssetsPathInFinder()
        {
            try
            {
                var outputPath = System.IO.Path.Combine(Application.streamingAssetsPath, BuildAssetBundlesEditor.AssetBundles);
                outputPath = System.IO.Path.Combine(outputPath, EditorUserBuildSettings.activeBuildTarget.ToString());
                outputPath = System.IO.Path.Combine(outputPath, BuildAssetBundlesEditor.ResourceRootDirectoryName);

                EditorUtility.RevealInFinder(outputPath);
                UnityEngine.Debug.Log(BuildAssetBundlesEditor.KSwordKitName + ": 打开资源包所在目录（StreamingAssets）（当前编译平台）-> 完成! ");
            }
            catch (System.Exception e)
            {
                Debug.LogError(BuildAssetBundlesEditor.KSwordKitName + ": 执行 `打开资源包所在目录（StreamingAssets）（当前编译平台）` 时，发生错误 -> " + e.Message);
            }
        }
        [MenuItem("Assets/KSwordKit/资源管理/打开资源包所在目录（PersistentDataPath）（当前编译平台）", false, 2000)]
        [MenuItem("KSwordKit/资源管理/打开资源包所在目录（PersistentDataPath）（当前编译平台）", false, 2000)]
        public static void OpenResourcePackagePersistentDataPathPathInFinder()
        {
            try
            {
                var outputPath = System.IO.Path.Combine(Application.persistentDataPath, BuildAssetBundlesEditor.AssetBundles);
                outputPath = System.IO.Path.Combine(outputPath, EditorUserBuildSettings.activeBuildTarget.ToString());
                outputPath = System.IO.Path.Combine(outputPath, BuildAssetBundlesEditor.ResourceRootDirectoryName);

                EditorUtility.RevealInFinder(outputPath);
                UnityEngine.Debug.Log(BuildAssetBundlesEditor.KSwordKitName + ": 打开资源包所在目录（PersistentDataPath）（当前编译平台）-> 完成! ");
            }
            catch (System.Exception e)
            {
                Debug.LogError(BuildAssetBundlesEditor.KSwordKitName + ": 执行 `打开资源包所在目录（PersistentDataPath）（当前编译平台）` 时，发生错误 -> " + e.Message);
            }
        }
    }
}

