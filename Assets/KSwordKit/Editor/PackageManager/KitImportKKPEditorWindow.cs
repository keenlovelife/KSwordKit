using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KSwordKit.Editor.PackageManager
{
    public class KitImportKKPEditorWindow : EditorWindow
    {
        class KKPFilepath
        {
            public KitPackageConfig config;
            public KitPacker.FileIndexs FileIndexs;
            public string filepath;
            public bool selected;
            public bool per_selected;
            public bool foldout;
        }
        static KitImportKKPEditorWindow window;
        /// <summary>
        /// 窗口打开显示函数
        /// </summary>
        /// <param name="data">窗口数据</param>
        public static void Open(string subtitle, string assetpath)
        {
            var isOpen = EditorWindow.HasOpenInstances<KitImportKKPEditorWindow>();
            assetPaths.Add(assetpath);
            if (!isOpen)
            {
                var windowTitle = KitConst.KitName + "：" + subtitle;
                window = GetWindow<KitImportKKPEditorWindow>(true, windowTitle);
                window.minSize = new Vector2(600, 800);
            }
            getKKPFilepaths();
        }
        static Vector2 scorllPos;
        static List<KKPFilepath> kkpFilepaths = new List<KKPFilepath>();
        static List<string> assetPaths = new List<string>();
        float hspaceCount = 20;
        static GUIStyle blod;
        static GUIStyle richText;
        static bool needDrawGUI = false;
        private void OnGUI()
        {
            if (window == null)
                window = GetWindow<KitImportKKPEditorWindow>(true, KitConst.KitName + "：" + KitImportKKP.openWindowSubtitle);

            if (assetPaths.Count > 0)
            {
                foreach (var path in assetPaths)
                    AssetDatabase.DeleteAsset(path);
                assetPaths.Clear();
            }

            if(kkpFilepaths.Count == 0)
                getKKPFilepaths();
            if(blod == null)
            {
                blod = new GUIStyle();
                blod.fontSize = 14;
                blod.normal.textColor = new Color(255, 200, 200);
            }
            if (richText == null)
            {
                richText = new GUIStyle();
                richText.fontSize = 10;
                richText.richText = true;
            }

            if (kkpFilepaths.Count > 0)
            {
                //foreach (var kkp in kkpFilepaths)
                //    initFileIndexsState(kkp);
                
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Space(8);
                GUILayout.Label("包内容列表：", blod);
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.Button("", GUILayout.Height(1));

                scorllPos = GUILayout.BeginScrollView(scorllPos, false, false);
                GUILayout.BeginHorizontal();
                GUILayout.Space(hspaceCount);
                GUILayout.BeginVertical();
                for (var i = 0; i < kkpFilepaths.Count; i++)
                {
                    if (i != 0)
                        GUILayout.Button("", GUILayout.Height(1));
                    DrawKKPGUI(kkpFilepaths[i]);
                }
                GUILayout.EndVertical();
                GUILayout.Space(hspaceCount);
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();

                GUILayout.Space(5);
                GUILayout.Button("", GUILayout.Height(1));
                GUILayout.Space(10);
                var selectedKKPFilepaths = new List<KKPFilepath>();
                for (var i = 0; i < kkpFilepaths.Count; i++)
                {
                    var kkp = kkpFilepaths[i];
                    if (kkp.selected) 
                        selectedKKPFilepaths.Add(kkp);
                }
                if (GUILayout.Button("导入 (" + selectedKKPFilepaths.Count + ")", GUILayout.Height(40)))
                {
                    if (selectedKKPFilepaths.Count > 0)
                        ImportAll(selectedKKPFilepaths);
                }
                GUILayout.Space(10);
            }

            if(needDrawGUI)
            {
                needDrawGUI = false;
                OnGUI();
            }
        }
        void DrawKKPGUI(KKPFilepath kkpfilepath)
        {
            bool allNotSelected = true;
            foreach(var fi in kkpfilepath.FileIndexs.fileIndexList)
                if(fi.selected)
                {
                    allNotSelected = false;
                    break;
                }
            if (allNotSelected)
            {
                kkpfilepath.selected = false;
                kkpfilepath.per_selected = false;
            }

            GUILayout.BeginHorizontal();
            var kkpImported = System.IO.Directory.Exists(System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory + "/" + kkpfilepath.config.ID));
            kkpfilepath.selected = EditorGUILayout.Toggle(kkpfilepath.selected, GUILayout.Width(15));
            if(kkpImported && allNotSelected)
            {
                kkpfilepath.selected = false;
                kkpfilepath.per_selected = false;
            }
            GUI.enabled = kkpfilepath.selected;
            kkpfilepath.foldout = EditorGUILayout.Foldout(kkpfilepath.foldout, kkpfilepath.config.ID, true);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            if (kkpfilepath.selected != kkpfilepath.per_selected)
            {
                kkpfilepath.per_selected = kkpfilepath.selected;
                foreach (var fi in kkpfilepath.FileIndexs.fileIndexList)
                    fi.selected = kkpfilepath.selected;
            }
            if (kkpfilepath.foldout)
            {
                GUILayout.BeginVertical();
                for (var i = 0; i < kkpfilepath.FileIndexs.fileIndexList.Count; i++)
                {
                    var fileindex = kkpfilepath.FileIndexs.fileIndexList[i];
                    bool isin = false;
                    foreach(var fi in kkpfilepath.FileIndexs.fileIndexList)
                    {
                        if(fi.isDir && fi.childFileindexList.Contains(i))
                        {
                            isin = true;
                            break;
                        }
                    }
                    if (isin) continue;

                    var dispalylabel = fileindex.fileName;
                    var fs = getFileSetting(fileindex, kkpfilepath.config);
                    if(fs != null)
                    {
                        dispalylabel += " -> " + fs.TargetPath;
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(hspaceCount);
                    fileindex.selected = EditorGUILayout.Toggle(fileindex.selected, GUILayout.Width(15));
                    switch (fileindex.fileIndexState)
                    {
                        case KitPacker.FileIndexState.NewFile:
                            GUILayout.Label("<color=yellow><b>New</b></color>", richText, GUILayout.Width(22));
                            break;
                        case KitPacker.FileIndexState.CanUpdate:
                            GUILayout.Label("<color=red><b>Update</b></color>", richText, GUILayout.Width(32));
                            break;
                        case KitPacker.FileIndexState.Same:
                            if (!fileindex.isDir)
                            {
                                fileindex.selected = false;
                                fileindex.per_selected = false;
                                GUI.enabled = false;
                            }
                            break;
                    }
                    if (fileindex.isDir)
                    {
                        bool canselected = false;
                        foreach (var index in fileindex.childFileindexList)
                            if (kkpfilepath.FileIndexs.fileIndexList[index].selected)
                            {
                                canselected = true;
                                break;
                            }
                        if (!canselected) fileindex.selected = false;
                        GUI.enabled = canselected;
                        fileindex.foldout = EditorGUILayout.Foldout(fileindex.foldout, dispalylabel, true);
                        GUI.enabled = true;
                    }
                    else
                        GUILayout.Label(dispalylabel);
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    if(fileindex.isDir && fileindex.selected != fileindex.per_selected)
                    {
                        fileindex.per_selected = fileindex.selected;
                        foreach (var index in fileindex.childFileindexList)
                            kkpfilepath.FileIndexs.fileIndexList[index].selected = fileindex.selected;
                    }
                    if (!kkpfilepath.selected && fileindex.selected)
                    {
                        kkpfilepath.selected = true;
                        kkpfilepath.per_selected = true;
                    }
                    if (fileindex.isDir && fileindex.foldout)
                        drawChildFileindex(kkpfilepath, fileindex, kkpfilepath.FileIndexs.fileIndexList, hspaceCount * 2);
                }
                GUILayout.EndVertical();
            }
        }
        void drawChildFileindex(KKPFilepath kkpfilepath, KitPacker.FileIndex fileIndex, List<KitPacker.FileIndex> files, float space)
        {
            foreach(var index in fileIndex.childFileindexList)
            {
                var fileindex = files[index];
                var dispalylabel = fileindex.fileName;
                var fs = getFileSetting(fileindex, kkpfilepath.config);
                if (fs != null)
                {
                    dispalylabel += " -> " + fs.TargetPath;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Space(space);
                fileindex.selected = EditorGUILayout.Toggle(fileindex.selected, GUILayout.Width(15));
                switch (fileindex.fileIndexState)
                {
                    case KitPacker.FileIndexState.NewFile:
                        GUILayout.Label("<color=green><b>New</b></color>", richText, GUILayout.Width(22));
                        break;
                    case KitPacker.FileIndexState.CanUpdate:
                        GUILayout.Label("<color=red><b>Update</b></color>", richText, GUILayout.Width(32));
                        break;
                    case KitPacker.FileIndexState.Same:
                        if (!fileindex.isDir)
                        {
                            fileindex.selected = false;
                            fileindex.per_selected = false;
                            GUI.enabled = false;
                        }
                        break;
                }
                if (fileindex.isDir)
                {
                    bool canselected = false;
                    foreach (var _index in fileindex.childFileindexList)
                        if (kkpfilepath.FileIndexs.fileIndexList[_index].selected)
                        {
                            canselected = true;
                            break;
                        }
                    if (!canselected) fileindex.selected = false;
                    GUI.enabled = canselected;
                    fileindex.foldout = EditorGUILayout.Foldout(fileindex.foldout, dispalylabel, true);
                    GUI.enabled = true;
                }
                else
                    GUILayout.Label(dispalylabel);
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                if (fileindex.isDir && fileindex.selected != fileindex.per_selected)
                {
                    fileindex.per_selected = fileindex.selected;
                    foreach (var _index in fileindex.childFileindexList)
                        files[_index].selected = fileindex.selected;
                }
                if (!kkpfilepath.selected && fileindex.selected)
                {
                    kkpfilepath.selected = true;
                    kkpfilepath.per_selected = true;
                }
                if (fileindex.isDir && fileindex.foldout)
                    drawChildFileindex(kkpfilepath, fileindex, files, space + hspaceCount);
            }
        }
        static void getKKPFilepaths()
        {
            var kkptempfilepath = System.IO.Path.Combine(Application.temporaryCachePath, KitImportKKP.kkpFilepathsTempFilename);
            if (System.IO.File.Exists(kkptempfilepath))
            {
                kkpFilepaths.Clear();
                var lines = System.IO.File.ReadAllLines(kkptempfilepath, System.Text.Encoding.UTF8);

                var kkps = new List<KKPFilepath>();
                foreach (var line in lines)
                {
                    var _configtext = KitPacker.Unpack_getKitPackageConfig_Text(line, out string error);
                    if (string.IsNullOrEmpty(error))
                    {
                        kkps.Add(new KKPFilepath()
                        {
                            config = JsonUtility.FromJson<KitPackageConfig>(_configtext),
                            filepath = line
                        });
                    }
                }
                foreach(var kkp in kkps)
                {
                    bool find = false;
                    foreach(var _kkp in kkpFilepaths)
                        if(_kkp.config.ID == kkp.config.ID)
                        {
                            find = true;
                            break;
                        }
                    if (!find)
                    {
                        var _fileIndexstext = KitPacker.Unpack_getFileIndexs_Text(kkp.filepath, out string error);
                        if (string.IsNullOrEmpty(error))
                        {
                            kkp.foldout = true;
                            kkp.selected = true;
                            kkp.per_selected = true;
                            kkp.FileIndexs = JsonUtility.FromJson<KitPacker.FileIndexs>(_fileIndexstext);
                            initFileIndexs(kkp);
                            kkpFilepaths.Add(kkp);
                        }
                    }
                }

                needDrawGUI = true;
                if (window)
                    window.Focus();
            }
        }
        static KitPackageConfigFileSetting getFileSetting(KitPacker.FileIndex fi, KitPackageConfig kitPackageConfig)
        {
            if (kitPackageConfig == null || kitPackageConfig.FileSettings == null || kitPackageConfig.FileSettings.Count == 0) return null;
            foreach (var fs in kitPackageConfig.FileSettings)
                if (fs.SourcePath == fi.relativeFilePath) return fs;
            return null;
        }
        static void initFileIndexs(KKPFilepath kkp)
        {
            var fileIndexs = kkp.FileIndexs;
            var importdir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory);
            var outdir = System.IO.Path.Combine(importdir, kkp.config.ID);
            var haveState = System.IO.Directory.Exists(outdir);

            List<KitPacker.FileIndex> tacks = new List<KitPacker.FileIndex>();
            for(var i = 0; i < fileIndexs.fileIndexList.Count; i++)
            {
                var fi = fileIndexs.fileIndexList[i];
                fi.selected = true;
                fi.per_selected = true;
                fi.foldout = true;
                if (haveState)
                {
                    var fs = getFileSetting(fi, kkp.config);
                    var isdir = false;
                    var importFilepath = "";
                    if (fs != null)
                    {
                        isdir = fs.isDir;
                        importFilepath = KitPackageConfigFileSetting.TargetPathToRealPath(fs.TargetPath);
                    }
                    else
                    {
                        isdir = fi.isDir;
                        importFilepath = System.IO.Path.Combine(outdir, fi.relativeFilePath);
                    }
                    if (isdir)
                    {
                        if (System.IO.Directory.Exists(importFilepath))
                            fi.fileIndexState = KitPacker.FileIndexState.Same;
                        else fi.fileIndexState = KitPacker.FileIndexState.NewFile;
                    }
                    else
                    {
                        if (System.IO.File.Exists(importFilepath))
                        {
                            var md5 = KitPacker.CheckMD5(importFilepath);
                            if (fi.MD5Value == md5)
                                fi.fileIndexState = KitPacker.FileIndexState.Same;
                            else
                                fi.fileIndexState = KitPacker.FileIndexState.CanUpdate;
                        }
                        else fi.fileIndexState = KitPacker.FileIndexState.NewFile;
                    }
                }
                else fi.fileIndexState = KitPacker.FileIndexState.None;

                for (var j = tacks.Count - 1; j >= 0; j--)
                {
                    var _fi = tacks[j];
                    if (fi.relativeFilePath.StartsWith(_fi.relativeFilePath))
                    {
                        _fi.childFileindexList.Add(i);
                        if (j != tacks.Count - 1)
                            tacks.RemoveAt(tacks.Count - 1);
                        break;
                    }
                }

                if (fi.isDir)
                {
                    fi.childFileindexList = new List<int>();
                    tacks.Add(fi);
                }
            }
        }
        static void initFileIndexsState(KKPFilepath kkp)
        {
            var fileIndexs = kkp.FileIndexs;
            var importdir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory); ;
            var outdir = System.IO.Path.Combine(importdir, kkp.config.ID);
            var haveState = System.IO.Directory.Exists(outdir);
            for (var i = 0; i < fileIndexs.fileIndexList.Count; i++)
            {
                var fi = fileIndexs.fileIndexList[i];
                if (haveState)
                {
                    var fs = getFileSetting(fi, kkp.config);
                    var isdir = false;
                    var importFilepath = "";
                    if (fs != null)
                    {
                        isdir = fs.isDir;
                        importFilepath = KitPackageConfigFileSetting.TargetPathToRealPath(fs.TargetPath);
                    }
                    else
                    {
                        isdir = fi.isDir;
                        importFilepath = System.IO.Path.Combine(outdir, fi.relativeFilePath);
                    }
                    if (isdir)
                    {
                        if (System.IO.Directory.Exists(importFilepath))
                            fi.fileIndexState = KitPacker.FileIndexState.Same;
                        else fi.fileIndexState = KitPacker.FileIndexState.NewFile;
                    }
                    else
                    {
                        if (System.IO.File.Exists(importFilepath))
                        {
                            var md5 = KitPacker.CheckMD5(importFilepath);
                            if (fi.MD5Value == md5)
                                fi.fileIndexState = KitPacker.FileIndexState.Same;
                            else
                                fi.fileIndexState = KitPacker.FileIndexState.CanUpdate;
                        }
                        else fi.fileIndexState = KitPacker.FileIndexState.NewFile;
                    }
                }
                else fi.fileIndexState = KitPacker.FileIndexState.None;
            }
        }
        static void ImportAll(List<KKPFilepath> selectedKKPFilepaths)
        {
            int doneCount = 0;
            EditorUtility.DisplayProgressBar(window.titleContent.text, "正在准备...", 0);
            foreach (var kkp in selectedKKPFilepaths)
            {
                Import(kkp.filepath, kkp.config.MD5Value, kkp.config.ID, (info, progress, error, done) => {
                    EditorUtility.DisplayProgressBar(window.titleContent.text,
                        "正在导入：" + kkp.config.ID + ", " + info,
                        doneCount / (float)selectedKKPFilepaths.Count + (float)1 / selectedKKPFilepaths.Count * progress);
                    if (done)
                    {
                        doneCount++;
                        if (string.IsNullOrEmpty(error))
                        {
                            KitDebug.Log(KitConst.KitName + ": " + kkp.config.ID + " 导入成功！");
                            EditorUtility.DisplayProgressBar(window.titleContent.text, kkp.config.ID + " 导入成功！", doneCount / (float)selectedKKPFilepaths.Count);
                        }
                        else
                        {
                            EditorUtility.DisplayProgressBar(window.titleContent.text, kkp.config.ID + " 导入失败：" + error, doneCount / (float)selectedKKPFilepaths.Count);
                            KitDebug.LogError(KitConst.KitName + ": " + kkp.config.ID + " 导入失败：" + error);
                        }
                        if(doneCount == selectedKKPFilepaths.Count)
                        {
                            EditorUtility.ClearProgressBar();
                            window.Close();
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            KitDebug.Log(KitConst.KitName + ": 导入处理完成！");
                        }
                    }
                }, kkp.FileIndexs.fileIndexList);
            }
        }
        static void Import(string kkpfilepath, string kkpMD5Value, string id, System.Action<string, float, string, bool> action = null, List<KitPacker.FileIndex> files = null)
        {
            System.Action doneAction = () => {
                var importdir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory);
                var outdir = System.IO.Path.Combine(importdir, id);
                KitPacker.Unpack(kkpfilepath, outdir, (filename, progress, done, error, depds) => {
                    if (action != null) action(filename, progress * 0.3f + 0.4f, null, false);
                    if (done)
                    {
                        if (string.IsNullOrEmpty(error))
                        {
                            if (depds == null || depds.Count == 0)
                            {
                                if (action != null) action("所有文件处理完成！", 1, null, true);
                                KitDebug.Log(KitConst.KitName + ": 所有文件处理完成！");
                            }
                            else
                            {
                                if (action != null) action("正在处理依赖...", 0.7f, null, false);
                                KitDebug.Log(KitConst.KitName + ": " + id + " 正在处理依赖 ..");
                                int doneDepdCount = 0;
                                string allerror = "";
                                foreach (var depd in depds)
                                {
                                    var _kkpfilepath = System.IO.Path.Combine(KitConst.KitPackagesRootDirectory, depd + "." + KitPacker.FileFormat);
                                    Import(_kkpfilepath, "", depd, (info, _progress, _error, isdone) =>
                                    {
                                        if (action != null)
                                            action("处理依赖" + depd + "->" + info,
                                                0.7f + 0.3f * _progress * (doneDepdCount + 1) / (float)depds.Count,
                                                null,
                                                false);
                                        if (isdone)
                                        {
                                            doneDepdCount++;
                                            if (string.IsNullOrEmpty(_error))
                                            {
                                                if (action != null) action("依赖" + depd + "处理完成！", 0.7f + 0.3f * (float)doneDepdCount / depds.Count, null, false);
                                                KitDebug.Log(KitConst.KitName + ": 依赖" + depd + "处理完成！");
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(allerror)) allerror = _error;
                                                else allerror += "\n" + _error;
                                                if (action != null) action("依赖" + depd + "处理失败！", 0.7f + 0.3f * (float)doneDepdCount / depds.Count, _error, false);
                                                KitDebug.LogError(KitConst.KitName + ": 依赖" + depd + "处理失败！" + _error);
                                            }
                                            if (doneDepdCount == depds.Count)
                                                if (action != null)
                                                {
                                                    action("所有依赖处理完毕！", 1, allerror, true);
                                                    if (string.IsNullOrEmpty(allerror))
                                                        KitDebug.Log(KitConst.KitName + ": 所有依赖处理完毕！");
                                                    else
                                                        KitDebug.LogError(KitConst.KitName + ": 所有依赖处理完毕！" + allerror);
                                                }
                                        }
                                    });
                                }
                            }
                        }
                        else if (action != null)
                        {
                            action("", 1, error, true);
                        }
                    }
                }, true, true, files);
            };

            System.Action<string> kkpConfigFileDoneAction = (md5value) => {
                var needNewkkpfile = false;
                if (!System.IO.File.Exists(kkpfilepath) || KitPacker.CheckMD5(kkpfilepath) != md5value)
                    needNewkkpfile = true;
                if (needNewkkpfile)
                {
                    if (System.IO.File.Exists(kkpfilepath))
                        System.IO.File.Delete(kkpfilepath);
                    var kkpurl = KitConst.KitOriginPackagesURL + "/" + KitInitializeEditor.URL(id) + "." + KitPacker.FileFormat;
                    var _www = new UnityEngine.Networking.UnityWebRequest(kkpurl);
                    _www.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(kkpfilepath);
                    _www.disposeDownloadHandlerOnDispose = true;
                    KitToolEditor.AddWebRequest(new KitToolEditor.WebRequest()
                    {
                        www = _www,
                        waitAction = (uwq) =>
                        {
                            if (action != null) action("正在下载包..", 0.2f + 0.2f * uwq.downloadProgress, null, false);
                        },
                        ResultAction = (uwq) =>
                        {
                            if (uwq.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                            {
                                if (action != null) action("下载包成功！", 0.4f, null, false);
                                KitDebug.Log(KitConst.KitName + ": " + id + " 下载包成功！" + kkpfilepath);
                                doneAction();
                            }
                            else
                            {
                                if (action != null) action("下载包失败：" + uwq.error, 1, "下载包失败\n" + uwq.error, true);
                            }
                        }
                    });
                }
                else doneAction();
            };

            if (string.IsNullOrEmpty(kkpMD5Value))
            {
                var webq = new KitToolEditor.WebRequest();
                var configurl = KitConst.KitOriginPackagesURL + "/" + KitInitializeEditor.URL(id) + "." + KitPacker.FileFormat + "." + KitConst.KitPackageConfigFilename;
                webq.www = UnityEngine.Networking.UnityWebRequest.Get(configurl);
                webq.www.SetRequestHeader("Content-Type", "application/json");
                webq.www.SetRequestHeader("Accept", "application/json");
                webq.waitAction = (uwq) => {
                    if (action != null) action("正在请求配置文件...", 0.2f * uwq.downloadProgress, null, false);
                };
                webq.ResultAction = (uwq) =>
                {
                    if (uwq.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        var configfilepath = KitConst.KitPackagesRootDirectory + "/" + id + "." + KitPacker.FileFormat + "." + KitConst.KitPackageConfigFilename;
                        var Config = JsonUtility.FromJson<KitPackageConfig>(uwq.downloadHandler.text);
                        if (System.IO.File.Exists(configfilepath))
                            System.IO.File.Delete(configfilepath);
                        if (!System.IO.Directory.Exists(KitConst.KitPackagesRootDirectory))
                            System.IO.Directory.CreateDirectory(KitConst.KitPackagesRootDirectory);
                        System.IO.File.WriteAllText(configfilepath, uwq.downloadHandler.text);
                        if (action != null) action("配置文件请求成功！", 0.2f, null, false);
                        KitDebug.Log(KitConst.KitName + ": " + id + " 的配置文件请求成功！" + uwq.downloadHandler.text);
                        kkpConfigFileDoneAction(Config.MD5Value);
                    }
                    else if (action != null) action("配置文件请求失败：" + uwq.error, 1, "配置文件请求失败\n" + uwq.error, true);
                };
                KitToolEditor.AddWebRequest(webq);
            }
            else kkpConfigFileDoneAction(kkpMD5Value);
        }
    }
}
