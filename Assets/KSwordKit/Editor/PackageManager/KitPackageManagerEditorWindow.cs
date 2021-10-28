
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

        /// <summary>
        /// 窗口打开显示函数
        /// </summary>
        /// <param name="data">窗口数据</param>
        public static void Open(string subtitle)
        {
            var windowTitle = KitConst.KitName + "：" + subtitle;
            window = GetWindow<KitPackageManagerEditorWindow>(true, windowTitle);
            window.minSize = new Vector2(600, 700);
            window.blod = new GUIStyle();

            KitInitializeEditor.Request_packages((done, progress) => {
                if (done) Debug.Log(KitConst.KitName + ": 所有可用包已拉取完成！");
            });

        }

        Vector2 scorllPos;
        GUIStyle blod;
        string packageCount = "";
        private void OnGUI()
        {
            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            blod.fontSize = 15;
            blod.normal.textColor = new Color(255, 200, 200);
            EditorGUILayout.LabelField("搜索：", blod, GUILayout.Width(40));
            kitUserSearchInputString = EditorGUILayout.TextField(kitUserSearchInputString);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            blod.fontSize = 20;
            EditorGUILayout.LabelField("包列表：(" + packageCount + ")", blod);

            if (string.IsNullOrEmpty(packageCount))
                GUI.enabled = false;
            if (GUILayout.Button("全部更新", GUILayout.Width(100), GUILayout.Height(24)))
            {

            }
            if (GUILayout.Button("全部卸载", GUILayout.Width(100), GUILayout.Height(24)))
            {

            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();

            // 包列表内容
            GUILayout.Space(12);
            if (KitInitializeEditor.KitOriginConfig == null || 
                KitInitializeEditor.KitOriginConfig.PackageCount <= 0 || 
                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList == null ||
                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count == 0)
            {
                packageCount = "";
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.Label("无任何可用包");
                GUILayout.EndHorizontal();
            }
            else
            {
                packageCount = KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count.ToString();
                scorllPos = GUILayout.BeginScrollView(scorllPos, false, false);
                if (KitInitializeEditor.KitOriginConfig.PackageCount > 0 && KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null)
                {
                    var idname = "";
                    for (var i = 0; i < KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count; i++)
                    {
                        var opc = KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[i];
                        if (opc.KitPackageConfig != null && string.IsNullOrEmpty(opc.KitPackageConfig.ImportRootDirectory))
                            opc.KitPackageConfig.ImportRootDirectory = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, opc.ID));

                        var ids = opc.ID.Split('@');
                        if (ids.Length == 2 && ids[0] != idname)
                        {
                            idname = ids[0];
                            DrawItemGUI(opc);
                        }
                    }
                    idname = null;
                }
                GUILayout.EndScrollView();
            }

            GUILayout.Space(30);
        }

        void DrawItemGUI(KitOriginPackageConfig originPackageConfig)
        {
            var ids = originPackageConfig.ID.Split('@');

            GUILayout.Button("", GUILayout.Height(2));
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            blod.fontSize = 14;
            blod.normal.textColor = new Color(255, 200, 200);
            GUILayout.Label(ids[0], blod, GUILayout.Height(22));

            bool imported = false;
            bool needUpdate = true;

            var importNames = new List<string>();
            if(string.IsNullOrEmpty(originPackageConfig.KitPackageConfig.ImportRootDirectory))
                originPackageConfig.KitPackageConfig.ImportRootDirectory = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, originPackageConfig.ID));
            var packagesImportRootDir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory);
            if (System.IO.Directory.Exists(packagesImportRootDir))
            {
                var dirinfo = new System.IO.DirectoryInfo(packagesImportRootDir);
                foreach (var dinfo in dirinfo.GetDirectories())
                {
                    var dirs = dinfo.Name.Split('@');
                    if (dirs[0] == ids[0])
                    {
                        imported = true;
                        importNames.Add(dinfo.Name);
                    }
                }
            }
            if (imported)
            {
                if (KitInitializeEditor.KSwordKitConfig != null)
                {
                    bool canSetDiry = false;
                    if (KitInitializeEditor.KSwordKitConfig.KitImportedPackageList == null)
                    {
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList = new List<string>();
                        canSetDiry = true;
                    }
                    foreach (var name in importNames)
                        if (!KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Contains(name))
                            KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Add(name);
                    if (canSetDiry)
                    {
                        EditorUtility.SetDirty(KitInitializeEditor.KSwordKitConfig);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            else
            {
                if (KitInitializeEditor.KSwordKitConfig != null)
                {
                    if (KitInitializeEditor.KSwordKitConfig.KitImportedPackageList != null &&
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Contains(originPackageConfig.ID))
                    {
                        var l = new List<string>();
                        foreach(var id in KitInitializeEditor.KSwordKitConfig.KitImportedPackageList)
                            if (id.StartsWith(ids[0])) l.Add(id);
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Clear();
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.AddRange(l);
                        EditorUtility.SetDirty(KitInitializeEditor.KSwordKitConfig);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
            }
            
            if (!imported && GUILayout.Button("导入", GUILayout.Width(50), GUILayout.Height(23)))
            {
                importKKPFile(originPackageConfig, "导入");
            }
            if (imported)
            {
                GUI.enabled = false;
                GUILayout.Button("已导入", GUILayout.Width(60), GUILayout.Height(23));
                GUI.enabled = true;
            }

            if (imported)
            {
                foreach(var name in importNames)
                    if(name == originPackageConfig.ID)
                    {
                        needUpdate = false;
                        break;
                    }
            }
            if (imported && !needUpdate)
            {
                GUI.enabled = false;
                GUILayout.Button("已是最新版本", GUILayout.Width(100), GUILayout.Height(23));
                GUI.enabled = true;
            }
            else
            {
                if (!imported) GUI.enabled = false;
                if (GUILayout.Button("更新", GUILayout.Width(50), GUILayout.Height(23)))
                {
                    uninstal(originPackageConfig);
                    importKKPFile(originPackageConfig, "更新");
                }
                GUI.enabled = true;
            }

            if (!imported) GUI.enabled = false;
            if (GUILayout.Button("卸载", GUILayout.Width(50), GUILayout.Height(23)))
            {
                if (KitInitializeEditor.KitOriginConfig.OriginPackageDic == null)
                {
                    uninstal(originPackageConfig);
                }
                else if (KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(ids[0]) &&
                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]] != null &&
                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]].Count > 1)
                {
                    if(EditorUtility.DisplayDialog("项目中并存多个版本", "卸载所有版本 or 仅卸载当前版本？", "卸载所有版本", "仅卸载当前版本"))
                    {
                        var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]];
                        var opcs = new List<KitOriginPackageConfig>();
                        foreach (var i in versions)
                        {
                            var _opc = KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[i];
                            if (_opc.KitPackageConfig != null && System.IO.Directory.Exists(_opc.KitPackageConfig.ImportRootDirectory))
                                opcs.Add(_opc);
                        }
                        uninstal(opcs);
                    }
                    else
                    {
                        uninstal(originPackageConfig);
                    }
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button("查看", GUILayout.Width(50), GUILayout.Height(23)))
            {

            }
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();


            if(originPackageConfig.KitPackageConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("作者：", EditorStyles.boldLabel, GUILayout.Width(30));
                GUILayout.Label(originPackageConfig.KitPackageConfig.Author);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("版本：", EditorStyles.boldLabel, GUILayout.Width(30));
                GUILayout.Label(originPackageConfig.KitPackageConfig.Version);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("日期：", EditorStyles.boldLabel, GUILayout.Width(30));
                GUILayout.Label(originPackageConfig.KitPackageConfig.Date);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("标签：", EditorStyles.boldLabel, GUILayout.Width(30));
                var eachMaxLength = 500;
                var maxHeight = 2;
                GUILayout.BeginVertical();
                GUI.enabled = false;
                var _i = 0;
                var each_length = 0;
                for (var l = 0; l < maxHeight; l++)
                {
                    each_length = 0;
                    GUILayout.BeginHorizontal();
                    for (; each_length < eachMaxLength && _i < originPackageConfig.KitPackageConfig.Tags.Count; _i++)
                    {
                        var tag = originPackageConfig.KitPackageConfig.Tags[_i];
                        var w = getButtonWidth(tag);
                        GUILayout.Button(tag, GUILayout.Width(w));
                        each_length += w + 10;
                    }
                    GUILayout.EndHorizontal();
                }
                if(_i < originPackageConfig.KitPackageConfig.Tags.Count)
                    GUILayout.Button("...", GUILayout.Width(20));
                GUI.enabled = true;
                GUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                if(originPackageConfig.KitPackageConfig.Dependencies != null && originPackageConfig.KitPackageConfig.Dependencies.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(35);
                    GUILayout.Label("依赖：", EditorStyles.boldLabel, GUILayout.Width(30));
                    GUILayout.BeginVertical();
                    var dependenciesMaxCount = 3;
                    GUI.enabled = false;
                    for (var l = 0; l <= dependenciesMaxCount && l < originPackageConfig.KitPackageConfig.Dependencies.Count; l++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(originPackageConfig.KitPackageConfig.Dependencies[l]);
                        GUILayout.EndHorizontal();
                    }
                    if (dependenciesMaxCount < originPackageConfig.KitPackageConfig.Dependencies.Count)
                        GUILayout.Label("...", GUILayout.Width(20));
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("描述：", EditorStyles.boldLabel, GUILayout.Width(30));
                var maxLineLength = 40;
                var lines = originPackageConfig.KitPackageConfig.Description.Split('\n');
                var tempLines = new List<string>();
                for(var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line.Length <= maxLineLength)
                        tempLines.Add(line);
                    else
                    {
                        while(line.Length > maxLineLength)
                        {
                            tempLines.Add(line.Substring(0, maxLineLength));
                            line = line.Substring(maxLineLength, line.Length - maxLineLength);
                        }
                        if (!string.IsNullOrEmpty(line))
                            tempLines.Add(line);
                    }
                }
                EditorGUILayout.BeginVertical();
                for(var i = 0; i < tempLines.Count && i < 4; i++)
                    GUILayout.Label(tempLines[i]);
                if (lines.Length > 3)
                    GUILayout.Label("...");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                if (KitInitializeEditor.KitOriginConfig.OriginPackageDic != null)
                {
                    if (KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(ids[0]) && 
                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]] != null &&
                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]].Count > 1)
                    {
                        var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]];
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(35);
                        GUILayout.Label("旧版本：", EditorStyles.boldLabel, GUILayout.Width(40));
                        
                        EditorGUILayout.BeginVertical();
                        for (var i = 1; i < versions.Count && i < 4; i++)
                        {
                            var versionID = KitInitializeEditor.KitOriginConfig.PackageList[versions[i]];
                            var version = versionID.Split('@')[1];
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(version, GUILayout.Width(getButtonWidth(version)));
                            var versionPath = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, versionID));
                            var versionImported = System.IO.Directory.Exists(versionPath);
                            if (versionImported)
                            {
                                GUI.enabled = false;
                                GUILayout.Button("已导入", GUILayout.Width(60), GUILayout.Height(23));
                                GUI.enabled = true;
                            }
                            else if (GUILayout.Button("导入", GUILayout.Width(60), GUILayout.Height(23)))
                            {
                                importKKPFile(KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[versions[i]], "导入");
                            }
                            GUI.enabled = versionImported;
                            if (GUILayout.Button("卸载", GUILayout.Width(50), GUILayout.Height(23)))
                            {
                                uninstal(KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[versions[i]]);
                            }
                            GUI.enabled = true;
                            GUILayout.EndHorizontal();
                        }
                        if(versions.Count > 3)
                        {
                            GUI.enabled = false;
                            GUILayout.Label("more...");
                            GUI.enabled = true;
                        }
                            
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.Space(10);
        }

        void RequestKKPFile(KitOriginPackageConfig originPackageConfig, System.Action successAction = null)
        {
            var _www = new UnityEngine.Networking.UnityWebRequest(originPackageConfig.kkpurl);
            _www.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(originPackageConfig.kkpfilepath);
            _www.disposeDownloadHandlerOnDispose = true;

            KitToolEditor.AddWebRequest(new KitToolEditor.WebRequest()
            {
                www = _www,
                waitAction = (uwq) =>
                {
                    EditorUtility.DisplayProgressBar("导入: " + originPackageConfig.ID, "正在下载包: " + uwq.downloadProgress * 100 + "%", uwq.downloadProgress * 100 / 2);
                },
                ResultAction = (uwq) =>
                {
                    EditorUtility.ClearProgressBar();
                    if (uwq.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        if (successAction != null) successAction();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("导入: " + originPackageConfig.ID, "下载包失败：" + uwq.error, "确定");
                    }
                }
            });
        }
        void unpackeKKP(KitOriginPackageConfig originPackageConfig, string title)
        {
            KitPacker.Unpack(
                originPackageConfig.kkpfilepath,
                originPackageConfig.KitPackageConfig.ImportRootDirectory,
                (filename, progress, done) =>
                {
                    if (!done)
                        EditorUtility.DisplayProgressBar(title + ": " + originPackageConfig.ID, "包下载完毕！正在" + title + ": " + filename, 0.5f + progress * 0.5f);

                    if (done)
                    {
                        EditorUtility.ClearProgressBar();
                        if (KitInitializeEditor.KSwordKitConfig != null)
                        {
                            if (KitInitializeEditor.KSwordKitConfig.KitImportedPackageList == null)
                                KitInitializeEditor.KSwordKitConfig.KitImportedPackageList = new List<string>();
                            KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Add(originPackageConfig.ID);
                            EditorUtility.SetDirty(KitInitializeEditor.KSwordKitConfig);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }

                        Debug.Log(KitConst.KitName + ": " + title + " " + originPackageConfig.ID + " 成功！");
                        EditorUtility.DisplayDialog(title + ": " + originPackageConfig.ID, title + "成功！", "确定");
                    }
                }
            );
        }
        void importKKPFile(KitOriginPackageConfig originPackageConfig, string title)
        {
            EditorUtility.DisplayProgressBar(title +": " + originPackageConfig.ID, "正在准备数据...", 0);
            if (!System.IO.File.Exists(originPackageConfig.kkpfilepath))
            {
                RequestKKPFile(originPackageConfig, () => {
                    unpackeKKP(originPackageConfig, title);
                });
            }
            else unpackeKKP(originPackageConfig, title);
        }
        void uninstal(KitOriginPackageConfig originPackageConfig)
        {
            if (EditorUtility.DisplayDialog("卸载：" + originPackageConfig.ID, "确认卸载？", "确认", "取消"))
            {
                DirectoryDelete(originPackageConfig.KitPackageConfig.ImportRootDirectory);
                if (KitInitializeEditor.KSwordKitConfig != null)
                {
                    if (KitInitializeEditor.KSwordKitConfig.KitImportedPackageList != null &&
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Contains(originPackageConfig.ID))
                    {
                        var l = new List<string>();
                        foreach (var id in KitInitializeEditor.KSwordKitConfig.KitImportedPackageList)
                            if (id != originPackageConfig.ID) l.Add(id);
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Clear();
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.AddRange(l);
                        EditorUtility.SetDirty(KitInitializeEditor.KSwordKitConfig);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
                EditorUtility.DisplayDialog("卸载：" + originPackageConfig.ID, "已成功卸载！", "确认");
            }
        }
        void uninstal(List<KitOriginPackageConfig> kitOriginPackageConfigs)
        {
            var info = "";
            foreach (var c in kitOriginPackageConfigs)
                info += "\n" + c.ID;
            if (EditorUtility.DisplayDialog("卸载所有版本", "确认卸载所有版本？" + info, "确认", "取消"))
            {
                foreach(var originPackageConfig in kitOriginPackageConfigs)
                {
                    DirectoryDelete(originPackageConfig.KitPackageConfig.ImportRootDirectory);
                    if (KitInitializeEditor.KSwordKitConfig != null)
                    {
                        if (KitInitializeEditor.KSwordKitConfig.KitImportedPackageList != null &&
                            KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Contains(originPackageConfig.ID))
                        {
                            var l = new List<string>();
                            foreach (var id in KitInitializeEditor.KSwordKitConfig.KitImportedPackageList)
                                if (id != originPackageConfig.ID) l.Add(id);
                            KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Clear();
                            KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.AddRange(l);
                        }
                    }
                }

                EditorUtility.SetDirty(KitInitializeEditor.KSwordKitConfig);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("全部卸载", "全部完成卸载！", "确认");
            }
        }

        int getButtonWidth(string str)
        {
            int r = 0;
            int hanziCount = 0;
            int zimuCount = 0;
            foreach (var c in str)
            {
                bool ishanzi;
                r += getCharButtonWidth(c, out ishanzi);
                if (ishanzi) hanziCount++;
                else zimuCount++;
            }
            int dw = 0;
            if(hanziCount + zimuCount > 1)
            {
                dw = hanziCount * 5 + zimuCount * 10;
            }
            r -= dw;
            return r;
        }
        int getCharButtonWidth(char c, out bool ishanzi)
        {
            string zimu = "0123456789.+-*/\\`~!@#$%^&*()_+[]{}:';\"?><,qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM ";
            if (zimu.Contains(c.ToString()))
            {
                ishanzi = false;
                return 22;
            }
            else
            {
                ishanzi = true;
                return 25;
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir">要删除的目录</param>
        void DirectoryDelete(string dir)
        {
            if (System.IO.Directory.Exists(dir))
                System.IO.Directory.Delete(dir, true);
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