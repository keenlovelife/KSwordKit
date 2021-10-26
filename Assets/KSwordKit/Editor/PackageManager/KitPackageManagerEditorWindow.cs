
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace KSwordKit.Editor.PackageManager
{
    public class KitPackageManagerEditorWindow : EditorWindow
    {
        static KitPackageManagerEditorWindow window;
        const string kitUserSearchDefaultInputString = "搜索包名、包作者、包描述等等";
        static string kitUserSearchInputString = kitUserSearchDefaultInputString;
        const string kitTempFileName = KitConst.KitName + ".tempfile";
        const int kitItemViewHeight = 25;
        const int kitSubTitleHeight = 20;

        /// <summary>
        /// 窗口打开显示函数
        /// </summary>
        /// <param name="data">窗口数据</param>
        public static void Open(string subtitle)
        {
            var windowTitle = KitConst.KitName + "：" + subtitle;
            window = GetWindow<KitPackageManagerEditorWindow>(true, windowTitle);
            window.minSize = new Vector2(600, 500);
            window.blod = new GUIStyle();
        }

        Vector2 scorllPos;
        bool isLoading;
        GUIStyle blod;
        Texture loading;

        bool showDisplayProgressBar = false;
        string displayProgressBarTitle;
        string displayProgressBarInfo;
        float progress;
        bool clear;

        private void OnGUI()
        {
            if (showDisplayProgressBar)
            {
                EditorUtility.DisplayProgressBar(displayProgressBarTitle, displayProgressBarInfo, progress);
            }

            if(clear)
            {
                EditorUtility.ClearProgressBar();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            blod.fontSize = 15;
            blod.normal.textColor = new Color(255, 200, 200);
            EditorGUILayout.LabelField("搜索：", blod, GUILayout.Width(40));
            kitUserSearchInputString = EditorGUILayout.TextField(kitUserSearchInputString);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            blod.fontSize = 20;
            EditorGUILayout.LabelField("包列表：", blod);
            if (isLoading)
            {
                if (loading == null)
                    loading = Resources.Load<Texture>("loading");
                GUILayout.Label(loading, GUILayout.Width(20), GUILayout.Height(20));
            }
            if (GUILayout.Button("检查更新", GUILayout.Width(100), GUILayout.Height(24)))
            {

            }
            if (GUILayout.Button("全部更新", GUILayout.Width(100), GUILayout.Height(24)))
            {

            }
            if (GUILayout.Button("全部卸载", GUILayout.Width(100), GUILayout.Height(24)))
            {

            }
            EditorGUILayout.EndHorizontal();

            // 包列表内容
            GUILayout.Space(12);
            if (KitInitializeEditor.KitOriginConfig != null)
            {
                if (KitInitializeEditor.KitOriginConfig.PackageCount <= 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label("无任何可用包");
                    GUILayout.EndHorizontal();
                }
                else
                {
                    scorllPos = GUILayout.BeginScrollView(scorllPos, false, false);
                    if (KitInitializeEditor.KitOriginConfig != null)
                    {
                        if(KitInitializeEditor.KitOriginConfig.PackageCount > 0 && KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null)
                        {
                            for (var i = 0; i < KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count; i++)
                                DrawItemGUI(i + 1, KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[i]);
                        }

                    }
                    else
                    {

                    }
                    GUILayout.EndScrollView();
                }
            }

            GUILayout.Space(30);
        }

        void DrawItemGUI(int index, KitOriginPackageConfig originPackageConfig)
        {
            GUILayout.Button("", GUILayout.Height(2));
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            blod.fontSize = 14;
            blod.normal.textColor = new Color(255, 200, 200);
            GUILayout.Label(index.ToString() + ",", blod, GUILayout.Width(15), GUILayout.Height(22));
            GUILayout.Label(originPackageConfig.ID, blod, GUILayout.Height(22));

            if(string.IsNullOrEmpty(originPackageConfig.kkpfilepath))
                originPackageConfig.kkpfilepath = System.IO.Path.Combine(KitConst.KitPackagesRootDirectory, originPackageConfig.ID + ".kkp");

            if (GUILayout.Button("导入", GUILayout.Width(50), GUILayout.Height(23)))
            {
                EditorUtility.DisplayProgressBar("导入: " + originPackageConfig.ID, "正在准备数据...", 0);

                System.Action unpackAction = () => {
                    KitPacker.Unpack(originPackageConfig.kkpfilepath,
                    System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, originPackageConfig.ID)),
                    (filename, progress) =>
                    {
                        KitToolEditor.WaitNextFrame(() => {
                            EditorUtility.DisplayProgressBar("导入: " + originPackageConfig.ID, "包下载完毕！正在导入: " + filename, 0.5f + progress * 0.5f);
                        });
                    });
                };

                if (System.IO.File.Exists(originPackageConfig.kkpfilepath))
                {
                    unpackAction();
                }
                else
                {
                    var kkpurl = originPackageConfig.kkpurl;
                    var _www = new UnityEngine.Networking.UnityWebRequest(kkpurl);
                    _www.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(originPackageConfig.kkpfilepath);
                    _www.disposeDownloadHandlerOnDispose = true;

                    KitToolEditor.AddWebRequest(new KitToolEditor.WebRequest()
                    {
                        www = _www,
                        waitAction = (uwq) =>
                        {
                            KitToolEditor.WaitNextFrame(() => {
                                EditorUtility.DisplayProgressBar("导入: " + originPackageConfig.ID, "正在下载包 " + uwq.downloadProgress * 100, uwq.downloadProgress * 100 / 2);
                            });
                        },
                        ResultAction = (uwq) =>
                        {
                            if (uwq.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                            {
                                KitToolEditor.WaitNextFrame(() => {
                                    EditorUtility.DisplayProgressBar("导入: " + originPackageConfig.ID, "包下载完毕！正在导入... ", uwq.downloadProgress * 100 / 2);
                                });

                                unpackAction();
                                KitToolEditor.WaitNextFrame(() => {
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                    EditorUtility.ClearProgressBar();
                                    Debug.Log(KitConst.KitName + ": 导入 " + originPackageConfig.ID + " 成功！");
                                });
                            }
                            else
                            {
                                KitToolEditor.WaitNextFrame(() => {
                                    EditorUtility.DisplayDialog("导入: " + originPackageConfig.ID, "下载包失败：" + uwq.error, "确定");
                                    EditorUtility.ClearProgressBar();
                                });
                            }
                        }
                    });
                }
            }
            if (GUILayout.Button("更新", GUILayout.Width(50), GUILayout.Height(23)))
            {

            }
            if (GUILayout.Button("卸载", GUILayout.Width(50), GUILayout.Height(23)))
            {

            }
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();


            if(originPackageConfig.KitPackageConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);

                GUILayout.Label("描述：", EditorStyles.boldLabel, GUILayout.Width(30));
                GUILayout.Label(originPackageConfig.KitPackageConfig.Description);

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
        }

        /// <summary>
        /// 遍历得到本地所有组件所在目录
        /// </summary>
        /// <param name="kitLocalResourceRootDir">套件本地资源所在得根目录</param>
        /// <param name="kitAllComponentsRootPathList">套件所有所在的组件的根目录列表，每个目录内都可能包含多个组件资源。</param>
        /// <param name="targetComponentDirectoryCallback">遍历得到的目标组件所在的目录位置回调</param>
        static void EachKitLocalResourceComponentRootDirectory(string kitLocalResourceRootDir, List<string> kitAllComponentsRootPathList, System.Action<string, string> targetComponentDirectoryCallback)
        {
            if (!string.IsNullOrEmpty(kitLocalResourceRootDir) && kitAllComponentsRootPathList != null && targetComponentDirectoryCallback != null)
            {
                foreach (var componentRootPath in kitAllComponentsRootPathList)
                {
                    if (!string.IsNullOrEmpty(componentRootPath))
                    {
                        var rootPath = System.IO.Path.Combine(kitLocalResourceRootDir, componentRootPath);
                        var rootDirInfo = new System.IO.DirectoryInfo(rootPath);
                        foreach (var rootDir in rootDirInfo.GetDirectories())
                        {
                            targetComponentDirectoryCallback(componentRootPath, rootDir.FullName);
                        }
                    }
                }
            }
        }

        ///// <summary> 
        ///// 窗口更新多次调用
        ///// </summary> 
        //private void Update()
        //{
        //    if (window == null || windowData == null)
        //    {
        //        var tempfilePath = System.IO.Path.Combine(Application.temporaryCachePath, kitTempFileName);
        //        if (System.IO.File.Exists(tempfilePath))
        //        {
        //            var tempfilelines = System.IO.File.ReadAllLines(tempfilePath);
        //            var windowtitle = tempfilelines[0];
        //            GetWindow<KitManagementEditorWindow>(true, windowtitle).Close();
        //            var tempfilecontent = tempfilelines[1];
        //            if (tempfilecontent == KitManagementEditor.InstallComponentWindowTitle) KitManagementEditor.InstallComponentFunction();
        //            else if (tempfilecontent == KitManagementEditor.UninstallComponentWindowTitle) KitManagementEditor.UninstallComponentFunction();
        //        }
        //        return;
        //    }
        //}

        ///// <summary>
        ///// 窗口GUI更新时触发
        ///// </summary>
        //private void OnGUI()
        //{
        //    if (window == null || windowData == null)
        //        return;

        //    EditorGUILayout.BeginHorizontal();
        //    GUILayout.Space(10);
        //    kitUserSearchInputString = EditorGUILayout.TextField("所有组件：", kitUserSearchInputString, GUILayout.Height(kitSubTitleHeight));
        //    window.SearchItem(kitUserSearchInputString);
        //    GUILayout.Space(15);
        //    EditorGUILayout.EndHorizontal();

        //    scorllPos = EditorGUILayout.BeginScrollView(scorllPos, false, false, GUILayout.Height(window.position.height - 30), GUILayout.Width(window.position.width));

        //    foreach (var item in windowData.kitShouldShowConfigList)
        //        window.AddItemView(item);

        //    EditorGUILayout.EndScrollView();
        //}
        ///// <summary>
        ///// 根据用户输入的搜索字符串，筛选匹配的子项
        ///// </summary>
        ///// <param name="userSearchInputString">用户输入的搜索字符串</param>
        //void SearchItem(string userSearchInputString)
        //{
        //    if (string.IsNullOrEmpty(userSearchInputString) || userSearchInputString == kitUserSearchDefaultInputString)
        //    {
        //        windowData.kitShouldShowConfigList.Clear();
        //        windowData.kitShouldShowConfigList.AddRange(windowData.KitConfigList);
        //    }
        //    else
        //    {
        //        windowData.kitShouldShowConfigList.Clear();
        //        //var pattern = "[" + userSearchInputString + "]";
        //        foreach (var kitConfig in windowData.KitConfigList)
        //        {
        //            //if (Regex.IsMatch(kitConfig.Name, pattern) 
        //            //    || Regex.IsMatch(kitConfig.Author, pattern) 
        //            //    || Regex.IsMatch(kitConfig.Contact, pattern) 
        //            //    || Regex.IsMatch(kitConfig.Date, pattern)
        //            //    || Regex.IsMatch(kitConfig.HomePage, pattern)
        //            //    || Regex.IsMatch(kitConfig.Version, pattern) )
        //            if (kitConfig.Name.ToLower().StartsWith(userSearchInputString.ToLower())
        //                || kitConfig.Author.ToLower().StartsWith(userSearchInputString.ToLower())
        //                )
        //            {
        //                windowData.kitShouldShowConfigList.Add(kitConfig);
        //            }
        //        }
        //    }
        //}
        ///// <summary>
        ///// 添加子项视图到窗口上
        ///// </summary>
        ///// <param name="config">子项的具体配置数据</param>
        //void AddItemView(KitConfig config)
        //{
        //    EditorGUILayout.BeginHorizontal();
        //    GUILayout.Label(config.DisplayedName, EditorStyles.boldLabel, GUILayout.Height(kitItemViewHeight));
        //    GUILayout.FlexibleSpace();

        //    var buttonName = "安装";
        //    var isComponentInstalled = window.IsComponentInstalled(config);
        //    if (isComponentInstalled)
        //        buttonName = "重新安装";

        //    if (GUILayout.Button(buttonName, GUILayout.Width(120), GUILayout.Height(kitItemViewHeight)))
        //    {
        //        var error = window.InstallComponent(config, buttonName == "重新安装");
        //        AssetDatabase.Refresh();
        //        EditorUtility.DisplayDialog("安装组件 '" + config.DisplayedName + "' ", string.IsNullOrEmpty(error) ? "安装成功！" : "安装失败: \n" + error, "确定");
        //    }

        //    EditorGUI.BeginDisabledGroup(!isComponentInstalled);
        //    if (GUILayout.Button("卸载", GUILayout.Width(120), GUILayout.Height(kitItemViewHeight)))
        //    {
        //        var error = window.UninstallComponent(config);
        //        AssetDatabase.Refresh();
        //        EditorUtility.DisplayDialog("卸载组件 '" + config.DisplayedName + "' ", string.IsNullOrEmpty(error) ? "卸载成功！" : "卸载失败: \n" + error, "确定");
        //    }
        //    EditorGUI.EndDisabledGroup();

        //    EditorGUILayout.EndHorizontal();
        //}
        ///// <summary>
        ///// 组件是否已安装
        ///// </summary>
        ///// <param name="config">组件配置数据</param>
        ///// <returns>返回true，表示该组件已安装；否则，组件未安装。</returns>
        //bool IsComponentInstalled(KitConfig config)
        //{
        //    var installRootDir = System.IO.Path.Combine(windowData.KitComponentInstallRootDirectory, config.Classification);
        //    var componentInstallDir = System.IO.Path.Combine(installRootDir, config.DisplayedName);
        //    if (!System.IO.Directory.Exists(componentInstallDir)) return false;
        //    return true;
        //}
        /// <summary>
        /// 安装组件到Unity项目中
        /// </summary>
        /// <param name="kitConfig">组件具体的配置数据</param>
        /// <returns>返回null或字符串，null表示安装成功，字符串则表示安装失败的描述。</returns>
        //string InstallPackage(KitConfig config, bool isReinstall)
        //{
        //    if (isReinstall)
        //    {
        //        var error = UninstallComponent(config);
        //        if (!string.IsNullOrEmpty(error))
        //            return error;
        //        AssetDatabase.Refresh();
        //    }

        //    var installRootDir = System.IO.Path.Combine(windowData.KitComponentInstallRootDirectory, config.Classification);
        //    var componentInstallDir = System.IO.Path.Combine(installRootDir, config.DisplayedName);
        //    try
        //    {
        //        window.DirectoryCopy(config.LocalResourceDirectory, componentInstallDir, true);
        //        var installedConfigFilePath = System.IO.Path.Combine(componentInstallDir, windowData.KitConfigFileName);
        //        if (System.IO.File.Exists(installedConfigFilePath))
        //            System.IO.File.Delete(installedConfigFilePath);
        //        foreach (var fileSetting in config.FileSettings)
        //        {
        //            var fileSettingSourcePath = System.IO.Path.Combine(componentInstallDir, fileSetting.SourcePath);
        //            if (System.IO.File.Exists(fileSettingSourcePath))
        //            {
        //                System.IO.File.Copy(fileSettingSourcePath, fileSetting.DestPath, true);
        //                System.IO.File.Delete(fileSettingSourcePath);
        //            }
        //            else if (System.IO.Directory.Exists(fileSettingSourcePath))
        //            {
        //                DirectoryCopy(fileSettingSourcePath, fileSetting.DestPath, true);
        //                DirectoryDelete(fileSettingSourcePath);
        //            }
        //        }
        //        config.LocalInstallationResourceDirectory = componentInstallDir;
        //        foreach (var dependency in config.Dependencies)
        //        {
        //            foreach (var _config in windowData.KitConfigList)
        //            {
        //                if (dependency == _config.Name || dependency == _config.DisplayedName)
        //                {
        //                    if (_config.DisplayedName == config.DisplayedName) continue;
        //                    if (!IsComponentInstalled(_config))
        //                        InstallComponent(_config, false);
        //                }
        //            }
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        return e.Message;
        //    }
        //    return null;
        //}
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="sourceDir">源文件夹</param>
        /// <param name="destDir">目标文件夹</param>
        /// <param name="copySubDirs">是否递归拷贝子目录</param>
        void DirectoryCopy(string sourceDir, string destDir, bool copySubDirs)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDir);
            }

            System.IO.DirectoryInfo[] dirs = dir.GetDirectories();
            System.IO.Directory.CreateDirectory(destDir);
            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string tempPath = System.IO.Path.Combine(destDir, file.Name);
                file.CopyTo(tempPath, true);
            }

            if (copySubDirs)
            {
                foreach (System.IO.DirectoryInfo subdir in dirs)
                {
                    string tempPath = System.IO.Path.Combine(destDir, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        /// <summary>
        /// 卸载组件
        /// </summary>
        /// <param name="config">组件配置数据</param>
        /// <returns>返回null或字符串，null表示卸载成功，字符串则表示卸载失败的描述。</returns>
        //string UninstallComponent(KitConfig config)
        //{
        //    try
        //    {
        //        DirectoryDelete(config.LocalInstallationResourceDirectory);
        //        foreach (var filesetting in config.FileSettings)
        //        {
        //            if (System.IO.File.Exists(filesetting.DestPath))
        //            {
        //                FileDelete(filesetting.DestPath);
        //            }
        //            else if (System.IO.Directory.Exists(filesetting.DestPath))
        //            {
        //                DirectoryDelete(filesetting.DestPath);
        //            }
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        return e.Message;
        //    }

        //    return null;
        //}
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir">要删除的目录</param>
        void DirectoryDelete(string dir)
        {
            if (System.IO.Directory.Exists(dir))
                System.IO.Directory.Delete(dir, true);
            if (System.IO.Directory.Exists(dir))
                System.IO.Directory.Delete(dir);
            var dirMetaFilePath = dir + ".meta";
            if (System.IO.File.Exists(dirMetaFilePath))
                System.IO.File.Delete(dirMetaFilePath);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">要删除的文件路径</param>
        void FileDelete(string file)
        {
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);
            var fileMetaPath = file + ".meta";
            if (System.IO.File.Exists(fileMetaPath))
                System.IO.File.Delete(fileMetaPath);
        }
    }
}