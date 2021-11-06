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
            assetPaths.Add(assetpath);
            getKKPFilepaths();
            if (!EditorWindow.HasOpenInstances<KitImportKKPEditorWindow>())
            {
                var windowTitle = KitConst.KitName + "：" + subtitle;
                window = GetWindow<KitImportKKPEditorWindow>(true, windowTitle);
                window.minSize = new Vector2(600, 800);
            }
        }
        static Vector2 scorllPos;
        static List<KKPFilepath> kkpFilepaths = new List<KKPFilepath>();
        static List<string> assetPaths = new List<string>();
        float hspaceCount = 20;
        static GUIStyle blod;
        static GUIStyle richText;
        private void OnGUI()
        {
            if(assetPaths.Count > 0)
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
                    var importdir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory); ;
                    foreach (var kkp in selectedKKPFilepaths)
                    {
                        var outdir = System.IO.Path.Combine(importdir, kkp.config.ID);
                        KitPacker.Unpack(kkp.filepath, outdir, null, true, true, kkp.FileIndexs.fileIndexList);
                    }
                }
                GUILayout.Space(10);
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
            kkpfilepath.selected = EditorGUILayout.Toggle(kkpfilepath.selected, GUILayout.Width(15));
            kkpfilepath.foldout = EditorGUILayout.Foldout(kkpfilepath.foldout, kkpfilepath.config.ID, true);
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
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(hspaceCount);
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
                        fileindex.foldout = EditorGUILayout.Foldout(fileindex.foldout, dispalylabel, true);
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
                    fileindex.foldout = EditorGUILayout.Foldout(fileindex.foldout, dispalylabel, true);
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
            }
        }
        static void initFileIndexs(KKPFilepath kkp)
        {
            var fileIndexs = kkp.FileIndexs;
            var importdir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory); ;
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
                    var importFilepath = System.IO.Path.Combine(outdir, fi.relativeFilePath);
                    if (fi.isDir)
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

    }
}
