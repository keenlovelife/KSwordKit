﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace KSwordKit.Editor
{
    [InitializeOnLoad]
    public class KitInitializeEditor : UnityEditor.AssetModificationProcessor
    {
        public static KSwordKit.KSwordKitConfig KSwordKitConfig { get { return config; } }
        public static KSwordKit.Editor.PackageManager.KitOriginConfig KitOriginConfig { get { return originConfig; } }
        static KSwordKit.Editor.PackageManager.KitOriginConfig originConfig;
        static string originConfigFilepath = KitConst.KitPackagesRootDirectory + "/" + KitConst.KitOriginPackageConfigFilename;
        static KSwordKit.KSwordKitConfig config;
        static string KitInstallationPath;
        static string KitVersion = KitConst.KitVersion;
        static readonly string scriptName = "KitInitializeEditor";
        static readonly string scriptPath = KSwordKit.KitConst.KitName + "/Editor/Initialize/KitInitializeEditor.cs";
        static readonly string configPath = "Resources/KSwordKitConfig.asset";
        static readonly string configName = "KSwordKitConfig";

        static KitInitializeEditor()
        {
            KitDebug.logEnabled = false;

            init();
        }

        static bool initsuccess;
        public static bool initSuccess { get { return initsuccess; } }
        static int initTimes = 0;
        static void init()
        {
            initTimes++;
            var paths = AssetDatabase.FindAssets(scriptName);
            foreach (var path in paths)
            {
                var _path = AssetDatabase.GUIDToAssetPath(path);
                if (_path.Contains(scriptPath))
                {
                    // 找到KSwordKit安装目录
                    var projectDir = _path.Replace(scriptPath, KSwordKit.KitConst.KitName);
                    KitInstallationPath = projectDir;
                    // 将路径写入文件中，以备其他地方使用。
                    var _configPath = System.IO.Path.Combine(projectDir, configPath);
                    if (System.IO.File.Exists(_configPath))
                    {
                        if (config == null)
                            config = Resources.Load<KSwordKit.KSwordKitConfig>(configName);
                        if (config != null)
                        {
                            config.KitInstallationPath = KitInstallationPath;
                            config.KitVersion = KitVersion;
                            EditorUtility.SetDirty(config);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }
                    else
                    {
                        config = ScriptableObject.CreateInstance<KSwordKit.KSwordKitConfig>();
                        config.KitInstallationPath = KitInstallationPath;
                        config.KitVersion = KitVersion;
                        AssetDatabase.CreateAsset(config, _configPath);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    break;
                }
            }

            initOriginConfig();
            bool _initsuccess = true;
            if (KSwordKitConfig == null || KitOriginConfig == null)
            {
                if (initTimes <= 1)
                {
                    _initsuccess = false;
                    KitToolEditor.WaitNextFrame(() =>
                    {
                        init();
                    });
                }
                else
                {
                    KitDebug.LogError(KitConst.KitName + ": 初始化失败！项目配置文件意外null");
                }
            }

            if(_initsuccess)
            {
                initsuccess = true;
                EditorApplication.projectChanged += OnProjectChanged;
                EditorApplication.update += EditorApplication_update;
                CheckUpdate();
                KitDebug.Log(KitConst.KitName + ": 初始化完成！");
            }
        }
        static void initOriginConfig()
        {
            if (System.IO.File.Exists(originConfigFilepath))
            {
                var _originConfig = JsonUtility.FromJson<PackageManager.KitOriginConfig>(System.IO.File.ReadAllText(originConfigFilepath, System.Text.Encoding.UTF8));
                if (originConfig == null)
                    originConfig = _originConfig;
                else
                {
                    originConfig.LatestVersion = _originConfig.LatestVersion;
                    originConfig.LatestVersionFileName = _originConfig.LatestVersionFileName;
                    originConfig.LatestVersionURL = _originConfig.LatestVersionURL;
                    originConfig.PackageCount = _originConfig.PackageCount;
                    originConfig.PackageList = _originConfig.PackageList;
                }
                if (originConfig.OriginPackageConfigList == null) 
                    originConfig.OriginPackageConfigList = new List<PackageManager.KitOriginPackageConfig>();
                if(originConfig.PackageList != null)
                {
                    foreach (var packageID in originConfig.PackageList)
                    {
                        PackageManager.KitOriginPackageConfig opconfig = null;
                        foreach (var _config in originConfig.OriginPackageConfigList)
                        {
                            if (_config.ID == packageID)
                            {
                                opconfig = _config;
                                break;
                            }
                        }
                        if (opconfig == null)
                        {
                            opconfig = new PackageManager.KitOriginPackageConfig();
                            originConfig.OriginPackageConfigList.Add(opconfig);
                        }
                        opconfig.ID = packageID;
                        if (string.IsNullOrEmpty(opconfig.configurl))
                            opconfig.configurl = KitConst.KitOriginPackagesURL + "/" + URL(packageID) + "." + KitPacker.FileFormat + ".kitPackageConfig.json";
                        if (string.IsNullOrEmpty(opconfig.kkpurl))
                            opconfig.kkpurl = KitConst.KitOriginPackagesURL + "/" + URL(packageID) + "." + KitPacker.FileFormat;

                        if (string.IsNullOrEmpty(opconfig.kkpfilepath))
                            opconfig.kkpfilepath = KitConst.KitPackagesRootDirectory + "/" + packageID + "." + KitPacker.FileFormat;
                        if (string.IsNullOrEmpty(opconfig.configfilepath))
                            opconfig.configfilepath = KitConst.KitPackagesRootDirectory + "/" + packageID + "." + KitPacker.FileFormat + ".kitPackageConfig.json";

                        if (System.IO.File.Exists(opconfig.configfilepath))
                        {
                            opconfig.KitPackageConfig = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(opconfig.configfilepath, System.Text.Encoding.UTF8));
                            if (string.IsNullOrEmpty(opconfig.KitPackageConfig.ImportRootDirectory))
                                opconfig.KitPackageConfig.ImportRootDirectory = System.IO.Path.Combine(KitInstallationPath, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, packageID));
                        }
                    }

                    if (originConfig.OriginPackageDic == null)
                        originConfig.OriginPackageDic = new Dictionary<string, List<int>>();
                    for (var i = 0; i < originConfig.PackageList.Count; i++)
                    {
                        var packageID = originConfig.PackageList[i];
                        var ids = packageID.Split('@');
                        if (!originConfig.OriginPackageDic.ContainsKey(ids[0]))
                            originConfig.OriginPackageDic[ids[0]] = new List<int>();
                        if (!originConfig.OriginPackageDic[ids[0]].Contains(i))
                            originConfig.OriginPackageDic[ids[0]].Add(i);
                    }
                }
            }
        }
        public static string URL(string url)
        {
            url = url.Replace(" ", "%20");
            url = url.Replace("@", "%40");
            return url;
        }
        static string KitConfigURL = KitConst.KitCheckForUpdates;
        static UnityEngine.Networking.UnityWebRequest checkWWW;
        static bool isRequestting;
        static bool needCheckUpdateClearProgressBar = false;
        static string CheckUpdateTitle = null;
        static bool showCheckUpdateDisplayProgressBar = false;
        public static void CheckUpdate(string title = null, bool showDisplayProgressBar = false)
        {
            if (isRequestting) return;
            isRequestting = true;
            CheckUpdateTitle = title;
            showCheckUpdateDisplayProgressBar = showDisplayProgressBar;
            if (showCheckUpdateDisplayProgressBar && !string.IsNullOrEmpty(CheckUpdateTitle))
                needCheckUpdateClearProgressBar = true;
            if (needCheckUpdateClearProgressBar)
                EditorUtility.DisplayProgressBar(CheckUpdateTitle, "请稍等...", 0.2f);
            checkWWW = UnityEngine.Networking.UnityWebRequest.Get(KitConfigURL);
            checkWWW.SetRequestHeader("Content-Type", "application/json");
            checkWWW.SetRequestHeader("Accept", "application/json");
            checkWWW.certificateHandler = new KitToolEditor.WebRequestCertificate();
            checkWWW.SendWebRequest();
            if (needCheckUpdateClearProgressBar)
                EditorUtility.DisplayProgressBar(CheckUpdateTitle, "已发送请求，正在等待检查结果...", 0.6f);
            EditorApplication.update += Request_update;
        }
        static void Request_update()
        {
            if (checkWWW != null && checkWWW.isDone)
            {
                bool canShow = false;
                string title = CheckUpdateTitle;
                if (needCheckUpdateClearProgressBar)
                {
                    canShow = true;
                    needCheckUpdateClearProgressBar = false;
                    showCheckUpdateDisplayProgressBar = false;
                    CheckUpdateTitle = null;
                    EditorUtility.ClearProgressBar();
                }

                if (checkWWW.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    KitDebug.Log(KitConst.KitName + ": 请求结果：" + checkWWW.downloadHandler.text);

                    if (!string.IsNullOrEmpty(checkWWW.downloadHandler.text))
                    {
                        if (!System.IO.Directory.Exists(KitConst.KitPackagesRootDirectory))
                            System.IO.Directory.CreateDirectory(KitConst.KitPackagesRootDirectory);
                        if (System.IO.File.Exists(originConfigFilepath))
                            System.IO.File.Delete(originConfigFilepath);
                        System.IO.File.WriteAllText(originConfigFilepath, checkWWW.downloadHandler.text);
                        var _originConfig = JsonUtility.FromJson<PackageManager.KitOriginConfig>(checkWWW.downloadHandler.text);
                        if (originConfig == null)
                            originConfig = _originConfig;
                        else
                        {
                            originConfig.LatestVersion = _originConfig.LatestVersion;
                            originConfig.LatestVersionFileName = _originConfig.LatestVersionFileName;
                            originConfig.LatestVersionURL = _originConfig.LatestVersionURL;
                            originConfig.PackageCount = _originConfig.PackageCount;
                            originConfig.PackageList = _originConfig.PackageList;
                        }
                    }

                    Request_packages((done, progress) => {
                        if (done) KitDebug.Log(KitConst.KitName + ": 所有可用包已拉取完成！【定时拉取】");
                    });

                    if (originConfig != null && originConfig.LatestVersion != config.KitVersion)
                        ShowUpdateKitDialog(canShow);
                    else
                    {
                        if (canShow)
                            EditorUtility.DisplayDialog(title, "当前已是最新版本", "确定");
                    }
                }
                else
                {
                    KitDebug.LogWarning(KSwordKit.KitConst.KitName + ": 请求资源更新信息出错：" + checkWWW.error + "\nurl:" + checkWWW.url);
                    if (canShow)
                        EditorUtility.DisplayDialog(title, "请求资源更新信息出错：" + checkWWW.error, "确定");
                }

                EditorApplication.update -= Request_update;
                isRequestting = false;
            }
        }
        public static void Request_packages(System.Action<bool, float> requestAction = null)
        {
            if (originConfig == null || originConfig.PackageCount <= 0 || originConfig.PackageList == null)
            {
                if (requestAction != null) requestAction(true, 1);
                return;
            }
            var count = 0;
            if(requestAction != null)
            {
                if (originConfig.PackageList.Count == 0)
                {
                    requestAction(true, 1);
                    return;
                }
                else requestAction(false, 0);
            }
            foreach (var id in originConfig.PackageList)
            {
                var ID = id;
                PackageManager.KitOriginPackageConfig opconfig = null;
                if (originConfig.OriginPackageConfigList == null) 
                    originConfig.OriginPackageConfigList = new List<PackageManager.KitOriginPackageConfig>();
                foreach (var _config in originConfig.OriginPackageConfigList)
                {
                    if (_config.ID == ID)
                    {
                        opconfig = _config;
                        break;
                    }
                }
                if (opconfig == null)
                {
                    opconfig = new PackageManager.KitOriginPackageConfig();
                    originConfig.OriginPackageConfigList.Add(opconfig);
                }
                opconfig.ID = ID;
                if (string.IsNullOrEmpty(opconfig.configurl))
                    opconfig.configurl = KitConst.KitOriginPackagesURL + "/" + URL(ID) + "." + KitPacker.FileFormat + "." + KitConst.KitPackageConfigFilename;
                if (string.IsNullOrEmpty(opconfig.kkpurl))
                    opconfig.kkpurl = KitConst.KitOriginPackagesURL + "/" + URL(ID) + "." + KitPacker.FileFormat;

                if (string.IsNullOrEmpty(opconfig.kkpfilepath))
                    opconfig.kkpfilepath = KitConst.KitPackagesRootDirectory + "/" + ID + "." + KitPacker.FileFormat;
                if (string.IsNullOrEmpty(opconfig.configfilepath))
                    opconfig.configfilepath = KitConst.KitPackagesRootDirectory + "/" + ID + "." + KitPacker.FileFormat + "." + KitConst.KitPackageConfigFilename;

                if (System.IO.File.Exists(opconfig.configfilepath))
                {
                    opconfig.KitPackageConfig = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(opconfig.configfilepath, System.Text.Encoding.UTF8));
                    if (string.IsNullOrEmpty(opconfig.KitPackageConfig.ImportRootDirectory))
                        opconfig.KitPackageConfig.ImportRootDirectory = System.IO.Path.Combine(config.KitInstallationPath, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, ID));
                }
                var webq = new KitToolEditor.WebRequest();
                webq.www = UnityEngine.Networking.UnityWebRequest.Get(opconfig.configurl);
                webq.www.SetRequestHeader("Content-Type", "application/json");
                webq.www.SetRequestHeader("Accept", "application/json");
                webq.ResultAction = (uwq) =>
                {
                    if (uwq.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        KitDebug.Log("获取：" + ID + " 配置文件成功！\n" + uwq.downloadHandler.text);
                        foreach (var config in originConfig.OriginPackageConfigList)
                        {
                            if (config.ID == ID)
                            {
                                config.KitPackageConfig = JsonUtility.FromJson<KitPackageConfig>(uwq.downloadHandler.text);
                                if (System.IO.File.Exists(opconfig.configfilepath))
                                    System.IO.File.Delete(opconfig.configfilepath);
                                if (!System.IO.Directory.Exists(KitConst.KitPackagesRootDirectory))
                                    System.IO.Directory.CreateDirectory(KitConst.KitPackagesRootDirectory);
                                System.IO.File.WriteAllText(opconfig.configfilepath, uwq.downloadHandler.text);
                                break;
                            }
                        }
                    }
                    else
                    {
                        KitDebug.LogWarning(KitConst.KitName + ": 获取 " + ID + " 配置文件失败！" + uwq.error);
                    }

                    if(requestAction != null)
                    {
                        count++;
                        if (count >= originConfig.PackageList.Count)
                        {
                            var temp = new List<PackageManager.KitOriginPackageConfig>();
                            
                            if (originConfig.OriginPackageDic == null)
                                originConfig.OriginPackageDic = new Dictionary<string, List<int>>();
                            for (var i = 0; i < originConfig.PackageList.Count; i++)
                            {
                                var packageID = originConfig.PackageList[i];
                                foreach(var c in originConfig.OriginPackageConfigList)
                                    if(c.ID == packageID)
                                    {
                                        temp.Add(c);
                                        break;
                                    }
                                var ids = packageID.Split('@');
                                if (!originConfig.OriginPackageDic.ContainsKey(ids[0]))
                                    originConfig.OriginPackageDic[ids[0]] = new List<int>();
                                if (!originConfig.OriginPackageDic[ids[0]].Contains(i))
                                    originConfig.OriginPackageDic[ids[0]].Add(i);
                            }
                            originConfig.OriginPackageConfigList.Clear();
                            originConfig.OriginPackageConfigList.AddRange(temp);
                            temp.Clear();
                            temp = null;
                            requestAction(true, 1);
                        }
                        else
                            requestAction(false, count / (float)originConfig.PackageList.Count);
                    }
                };
                KitToolEditor.AddWebRequest(webq);
            }
        }
        public static void ShowUpdateKitDialog(bool showDialog = false)
        {
            if (!showDialog) return;
            if(EditorUtility.DisplayDialog(KitConst.KitName + ": 新版本通知", KitConst.KitName + ": 有新版本了！", "下载", "取消"))
            {
                Application.OpenURL(KitConst.KitReleaseURL);
            }
        }
        static void EditorApplication_update()
        {

            if (!EditorWindow.HasOpenInstances<PackageManager.KitImportKKPEditorWindow>())
            {
                var kkptempfilepath = System.IO.Path.Combine(Application.temporaryCachePath, PackageManager.KitImportKKP.kkpFilepathsTempFilename);
                if (System.IO.File.Exists(kkptempfilepath))
                    System.IO.File.Delete(kkptempfilepath);
            }

            var _configPath = System.IO.Path.Combine(KitInstallationPath, configPath);
            if (System.IO.File.Exists(_configPath))
            {
                if (config == null)
                    config = Resources.Load<KSwordKitConfig>(configName);
                var importRootDir = System.IO.Path.Combine(KitInstallationPath, KitConst.KitPackagesImportRootDirectory);
                if (System.IO.Directory.Exists(importRootDir))
                {
                    var tempImportList = new List<string>();
                    var importRootDirinfo = new System.IO.DirectoryInfo(importRootDir);
                    foreach (var dirinfo in importRootDirinfo.GetDirectories())
                        tempImportList.Add(dirinfo.Name);
                    if (config.KitImportedPackageList != null)
                    {
                        config.KitImportedPackageList.Clear();
                        config.KitImportedPackageList = null;
                    }
                    config.KitImportedPackageList = tempImportList;
                }
                else
                {
                    if (config.KitImportedPackageList != null && config.KitImportedPackageList.Count > 0)
                        config.KitImportedPackageList.Clear();
                }

                if (config.KitInstallationPath != KitInstallationPath || config.KitVersion != KitVersion)
                {
                    config.KitInstallationPath = KitInstallationPath;
                    config.KitVersion = KitVersion;
                    EditorUtility.DisplayDialog("该文件数据不能修改", "KSwordKitSetting.asset 资源数据内容不能修改！", "知道了");
                }
            }

        }
        static void OnProjectChanged()
        {
            var _configPath = System.IO.Path.Combine(KitInstallationPath, configPath);
            if (!System.IO.File.Exists(_configPath))
            {
                config = ScriptableObject.CreateInstance<KSwordKit.KSwordKitConfig>();
                config.KitInstallationPath = KitInstallationPath;
                config.KitVersion = KitVersion;
                AssetDatabase.CreateAsset(config, _configPath);
                AssetDatabase.SaveAssets();
                KitDebug.Log(KSwordKit.KitConst.KitName + ": " + configName + ".asset 是必要资源，必须存在，目前已被重新创建！");
            }
        }
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (assetPath.Contains(configPath))
            {
                EditorUtility.DisplayDialog("不能删除该文件", configName + ".asset 是必要资源，必须存在，不能删除！", "知道了");
                return AssetDeleteResult.DidDelete;
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }

}

#endif