using System.Collections;
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
            init();
        }

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

            bool initsuccess = true;
            if (KSwordKitConfig == null || KitOriginConfig == null)
            {
                if (initTimes <= 1)
                {
                    initsuccess = false;
                    KitToolEditor.WaitNextFrame(() =>
                    {
                        init();
                    });
                }
                else
                {
                    Debug.Log(KitConst.KitName + ": 初始化失败！项目配置文件意外null");
                }
            }

            if(initsuccess)
            {
                EditorApplication.projectChanged += OnProjectChanged;
                EditorApplication.update += EditorApplication_update;
                DateTime = System.DateTime.Now;

                Debug.Log(KitConst.KitName + ": 初始化完成！");
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
                    originConfig.Version = _originConfig.Version;
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
                            opconfig.configurl = KitConst.KitOriginPackagesURL + "/" + URL(packageID) + ".kitPackageConfig.json";
                        if (string.IsNullOrEmpty(opconfig.kkpurl))
                            opconfig.kkpurl = KitConst.KitOriginPackagesURL + "/" + URL(packageID) + ".kkp";

                        if (string.IsNullOrEmpty(opconfig.kkpfilepath))
                            opconfig.kkpfilepath = KitConst.KitPackagesRootDirectory + "/" + packageID + ".kkp";
                        if (string.IsNullOrEmpty(opconfig.configfilepath))
                            opconfig.configfilepath = KitConst.KitPackagesRootDirectory + "/" + packageID + ".kitPackageConfig.json";

                        if (System.IO.File.Exists(opconfig.configfilepath))
                        {
                            opconfig.KitPackageConfig = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(opconfig.configfilepath, System.Text.Encoding.UTF8));
                            if (string.IsNullOrEmpty(opconfig.KitPackageConfig.ImportRootDirectory))
                                opconfig.KitPackageConfig.ImportRootDirectory = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, packageID));
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
        static string URL(string url)
        {
            url = url.Replace(" ", "%20");
            url = url.Replace("@", "%40");
            return url;
        }
        static string KitConfigURL = KitConst.KitCheckForUpdates;
        static UnityEngine.Networking.UnityWebRequest checkWWW;
        static System.DateTime DateTime;
        static bool isFirstRequst = true;
        static bool isRequestting;
        static void RequestUpdate()
        {
            if (isFirstRequst) isFirstRequst = false;
            isRequestting = true;
            checkWWW = UnityEngine.Networking.UnityWebRequest.Get(KitConfigURL);
            checkWWW.SetRequestHeader("Content-Type", "application/json");
            checkWWW.SetRequestHeader("Accept", "application/json");
            checkWWW.certificateHandler = new KitToolEditor.WebRequestCertificate();
            checkWWW.SendWebRequest();
            EditorApplication.update += Request_update;
        }
        static void Request_update()
        {
            if (checkWWW != null && checkWWW.isDone)
            {
                if (checkWWW.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    Debug.Log(KitConst.KitName + ": 请求结果：" + checkWWW.downloadHandler.text);

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
                            originConfig.Version = _originConfig.Version;
                            originConfig.PackageCount = _originConfig.PackageCount;
                            originConfig.PackageList = _originConfig.PackageList;
                        }
                    }

                    Request_packages();

                    if (originConfig != null && originConfig.Version != config.KitVersion)
                        ShowUpdateKitDialog();
                }
                else
                {
                    Debug.LogWarning(KSwordKit.KitConst.KitName + ": 请求资源更新信息出错：" + checkWWW.error);
                }

                EditorApplication.update -= Request_update;

                DateTime = System.DateTime.Now;
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
                    opconfig.configurl = KitConst.KitOriginPackagesURL + "/" + URL(ID) + ".kitPackageConfig.json";
                if (string.IsNullOrEmpty(opconfig.kkpurl))
                    opconfig.kkpurl = KitConst.KitOriginPackagesURL + "/" + URL(ID) + ".kkp";

                if (string.IsNullOrEmpty(opconfig.kkpfilepath))
                    opconfig.kkpfilepath = KitConst.KitPackagesRootDirectory + "/" + ID + ".kkp";
                if (string.IsNullOrEmpty(opconfig.configfilepath))
                    opconfig.configfilepath = KitConst.KitPackagesRootDirectory + "/" + ID + ".kitPackageConfig.json";

                if (System.IO.File.Exists(opconfig.configfilepath))
                {
                    opconfig.KitPackageConfig = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(opconfig.configfilepath, System.Text.Encoding.UTF8));
                    if (string.IsNullOrEmpty(opconfig.KitPackageConfig.ImportRootDirectory))
                        opconfig.KitPackageConfig.ImportRootDirectory = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, ID));
                }
                var webq = new KitToolEditor.WebRequest();
                webq.www = UnityEngine.Networking.UnityWebRequest.Get(opconfig.configurl);
                webq.www.SetRequestHeader("Content-Type", "application/json");
                webq.www.SetRequestHeader("Accept", "application/json");
                webq.ResultAction = (uwq) =>
                {
                    if (uwq.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        Debug.Log("获取：" + ID + " 配置文件成功！\n" + uwq.downloadHandler.text);
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
                        Debug.LogWarning(KitConst.KitName + ": 获取 " + ID + " 配置文件失败！" + uwq.error);
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
        public static void ShowUpdateKitDialog()
        {
            string temp_show_kitDialog = System.IO.Path.Combine(Application.temporaryCachePath, "temp_showed_kitDialog");
            if (!System.IO.File.Exists(temp_show_kitDialog))
                System.IO.File.CreateText(temp_show_kitDialog);
            else
                return;

            if(EditorUtility.DisplayDialog(KitConst.KitName + ": 新版本通知", KitConst.KitName + ": 有新版本了！", "更新", "取消"))
            {
                Debug.Log("更新新版本");

            }
        }
        static void EditorApplication_update()
        {
            if (isFirstRequst || (!isRequestting && (System.DateTime.Now - DateTime).TotalMinutes > 1))
                RequestUpdate();


            var _configPath = System.IO.Path.Combine(KitInstallationPath, configPath);
            if (System.IO.File.Exists(_configPath))
            {
                if (config == null)
                    config = Resources.Load<KSwordKitConfig>(configName);
                if(config.KitInstallationPath != KitInstallationPath || config.KitVersion != KitVersion)
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
                Debug.Log(KSwordKit.KitConst.KitName + ": " + configName + ".asset 是必要资源，必须存在，目前已被重新创建！");
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