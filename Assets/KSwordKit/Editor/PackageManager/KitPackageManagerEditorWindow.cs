﻿
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
        bool seleceted = false;
        bool per_selected = false;
        private void OnGUI()
        {
            EditorGUILayout.Space(20);

            var importdAllNames = new List<string>();
            var packagesImportRootDir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory);
            if (System.IO.Directory.Exists(packagesImportRootDir))
            {
                var dirinfo = new System.IO.DirectoryInfo(packagesImportRootDir);
                foreach (var dinfo in dirinfo.GetDirectories())
                    importdAllNames.Add(dinfo.Name);
            }
            if (KitInitializeEditor.KSwordKitConfig != null)
            {
                bool canSetDiry = false;
                if (KitInitializeEditor.KSwordKitConfig.KitImportedPackageList == null)
                {
                    KitInitializeEditor.KSwordKitConfig.KitImportedPackageList = new List<string>();
                    canSetDiry = true;
                }
                if (importdAllNames.Count != KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Count)
                {
                    KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Clear();
                    KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.AddRange(importdAllNames);
                    canSetDiry = true;
                }
                else
                {
                    bool yes = false;
                    foreach (var id in KitInitializeEditor.KSwordKitConfig.KitImportedPackageList)
                        if (!importdAllNames.Contains(id))
                        {
                            yes = true;
                            break;
                        }
                    if (!yes)
                    {
                        foreach (var id in importdAllNames)
                            if (!KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Contains(id))
                            {
                                yes = true;
                                break;
                            }
                    }
                    if (yes)
                    {
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Clear();
                        KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.AddRange(importdAllNames);
                        canSetDiry = true;
                    }
                }
                if (canSetDiry)
                {
                    EditorUtility.SetDirty(KitInitializeEditor.KSwordKitConfig);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            if (seleceted != per_selected)
            {
                per_selected = seleceted;
                if (KitInitializeEditor.KitOriginConfig != null &&
                       KitInitializeEditor.KitOriginConfig.PackageCount > 0 &&
                       KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                       KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                {
                    foreach (var c in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                        c.selected = false;
                }
            }
            if (KitInitializeEditor.KitOriginConfig == null ||
                KitInitializeEditor.KitOriginConfig.PackageCount <= 0 ||
                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList == null ||
                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count == 0)
                packageCount = "";
            else
                packageCount = KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count.ToString();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            blod.fontSize = 15;
            blod.normal.textColor = new Color(255, 200, 200);
            EditorGUILayout.LabelField("搜索：", blod, GUILayout.Width(40));
            kitUserSearchInputString = EditorGUILayout.TextField(kitUserSearchInputString);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(16);

            EditorGUILayout.BeginHorizontal();

            blod.fontSize = 20;
            EditorGUILayout.LabelField("包列表：(" + packageCount + ")", blod);

            var guienabled = !string.IsNullOrEmpty(packageCount);
            if (guienabled)
            {
                GUI.enabled = seleceted;
                if (GUILayout.Button("导入选中项", GUILayout.Width(80), GUILayout.Height(24)))
                {
                    if (KitInitializeEditor.KitOriginConfig != null &&
                       KitInitializeEditor.KitOriginConfig.PackageCount > 0 &&
                       KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                       KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                    {
                        var opcs = new List<KitOriginPackageConfig>();
                        foreach (var c in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                        {
                            if (c.selected)
                            {
                                if (c.KitPackageConfig != null && System.IO.Directory.Exists(c.KitPackageConfig.ImportRootDirectory))
                                    continue;
                                else
                                    opcs.Add(c);
                            }
                        }
                        if (opcs.Count > 0)
                        {
                            var info = "";
                            foreach (var c in opcs)
                                if (string.IsNullOrEmpty(info))
                                    info = c.ID;
                                else
                                    info += "\n" + c.ID;
                            if (EditorUtility.DisplayDialog("全部导入", "存在一个或多个内容项需要全部导入：\n\n" + info + "\n\n确认全部导入 ？", "确认", "取消"))
                            {
                                importKKPFile(opcs, "全部导入", true, () =>
                                {
                                    Debug.Log(KitConst.KitName + ": 全部导入成功！");
                                    EditorUtility.DisplayDialog("全部导入", info + "\n\n已全部导入成功！", "确定");
                                });
                            }
                        }

                    }
                }
                GUI.enabled = true;

                var buttonName = seleceted ? "更新选中项" : "全部更新";
                if (GUILayout.Button(buttonName, GUILayout.Width(80), GUILayout.Height(24)))
                {
                    if (KitInitializeEditor.KitOriginConfig != null &&
                        KitInitializeEditor.KitOriginConfig.PackageCount > 0 &&
                        KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                        KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                    {
                        var opcs = new List<KitOriginPackageConfig>(); // 已导入
                        var needUpdateOpcs = new List<KitOriginPackageConfig>(); // 需要更新
                        var updateTargetOpcs = new List<KitOriginPackageConfig>(); // 更新包
                        var infoDic = new Dictionary<string, string>();
                        foreach (var c in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                        {
                            if (seleceted && !c.selected)
                            {
                                var cids = c.ID.Split('@');
                                if (KitInitializeEditor.KitOriginConfig.OriginPackageDic != null &&
                                    KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(cids[0]) &&
                                    KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]] != null &&
                                    KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]].Count > 1)
                                {
                                    var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]];
                                    var find = false;
                                    foreach (var v in versions)
                                    {
                                        if (KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[v].selected)
                                        {
                                            find = true;
                                            break;
                                        }
                                    }
                                    if (!find) continue;
                                }
                                else continue;
                            }
                            if (c.KitPackageConfig != null && System.IO.Directory.Exists(c.KitPackageConfig.ImportRootDirectory))
                                opcs.Add(c);
                        }
                        if (opcs.Count > 0 && KitInitializeEditor.KitOriginConfig.OriginPackageDic != null)
                        {
                            foreach (var c in opcs)
                            {
                                var cids = c.ID.Split('@');
                                if (KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(cids[0]) &&
                                    KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]] != null &&
                                    KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]].Count > 1)
                                {
                                    var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]];
                                    var opc = KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[versions[0]];
                                    if (c.ID != opc.ID)
                                    {
                                        if (opc.KitPackageConfig != null && !System.IO.Directory.Exists(opc.KitPackageConfig.ImportRootDirectory))
                                        {
                                            if (infoDic.ContainsKey(cids[0]))
                                                infoDic[cids[0]] = cids[0] + " -> " + opc.ID;
                                            else
                                                infoDic[cids[0]] = c.ID + " -> " + opc.ID;
                                            needUpdateOpcs.Add(c);
                                            bool canadd = true;
                                            foreach (var _c in updateTargetOpcs)
                                                if (_c.ID == opc.ID)
                                                {
                                                    canadd = false;
                                                    break;
                                                }
                                            if (canadd)
                                                updateTargetOpcs.Add(opc);
                                        }
                                    }
                                }
                            }

                            if (needUpdateOpcs.Count > 0)
                            {
                                var info = "";
                                foreach (var kv in infoDic)
                                {
                                    if (string.IsNullOrEmpty(info))
                                        info = kv.Value;
                                    else
                                        info += "\n" + kv.Value;
                                }
                                updateKKP(updateTargetOpcs, needUpdateOpcs, info);
                            }
                        }
                    }
                }
                if (GUILayout.Button(seleceted ? "卸载选中项" : "全部卸载", GUILayout.Width(80), GUILayout.Height(24)))
                {
                    if (seleceted)
                    {
                        if (KitInitializeEditor.KitOriginConfig != null &&
                            KitInitializeEditor.KitOriginConfig.PackageCount > 0 &&
                            KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                            KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                        {
                            var opcs = new List<KitOriginPackageConfig>();
                            foreach (var c in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                            {
                                if (seleceted && !c.selected)
                                {
                                    var cids = c.ID.Split('@');
                                    if (KitInitializeEditor.KitOriginConfig.OriginPackageDic != null &&
                                        KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(cids[0]) &&
                                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]] != null &&
                                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]].Count > 1)
                                    {
                                        var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[cids[0]];
                                        var find = false;
                                        foreach (var v in versions)
                                        {
                                            if (KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[v].selected)
                                            {
                                                find = true;
                                                break;
                                            }
                                        }
                                        if (!find) continue;
                                    }
                                    else continue;
                                }
                                if (c.KitPackageConfig != null && System.IO.Directory.Exists(c.KitPackageConfig.ImportRootDirectory))
                                    opcs.Add(c);
                            }
                            if (opcs.Count > 0)
                                uninstal(opcs);
                        }
                    }
                    else
                    {
                        if (KitInitializeEditor.KitOriginConfig != null &&
                            KitInitializeEditor.KitOriginConfig.PackageCount > 0 &&
                            KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                            KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                        {
                            var opcs = new List<KitOriginPackageConfig>();
                            foreach (var c in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                            {
                                if (c.KitPackageConfig != null && System.IO.Directory.Exists(c.KitPackageConfig.ImportRootDirectory))
                                    opcs.Add(c);
                            }
                            if (opcs.Count > 0)
                                uninstal(opcs);
                        }
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("");
            seleceted = GUILayout.Toggle(seleceted, seleceted ? "关闭多选" : "开启多选", GUILayout.Width(80));
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
            var nameMaxLength_hanzi = 37;
            var nameGoodLength_hanzi = 17;
            var nameMaxLength_zimu = 78;
            var nameGoodLength_zimu = 36;
            var hanzi_to_zimu_c = (float)nameMaxLength_zimu / nameMaxLength_hanzi;
            var zimu_to_hanzi_c = (float)nameMaxLength_hanzi / nameMaxLength_zimu;

            var nameNotGood = false;
            var ids = originPackageConfig.ID.Split('@');

            var idname = ids[0];
            var hanzi_count = 0;
            var zimu_count = 0;
            foreach (var c in idname)
                if (isHanzi(c)) ++hanzi_count;
                else ++zimu_count ;
            if(zimu_count == 0)
            {
                if (idname.Length > nameMaxLength_hanzi)
                    idname = idname.Substring(0, nameMaxLength_hanzi) + "...";
                if(idname.Length > nameGoodLength_hanzi)
                    nameNotGood = true;
            } 
            else if (hanzi_count == 0)
            {
                if (idname.Length > nameMaxLength_zimu)
                    idname = idname.Substring(0, nameMaxLength_zimu) + "...";
                if (idname.Length > nameGoodLength_zimu)
                    nameNotGood = true;
            }
            else
            {
                var idname_hanzi_to_zimuCount = hanzi_count * hanzi_to_zimu_c;
                var idname_length = (int)idname_hanzi_to_zimuCount + zimu_count;
                if (idname_length > nameMaxLength_zimu)
                {
                    if(idname.Length > nameMaxLength_hanzi )
                        idname = idname.Substring(0, nameMaxLength_hanzi) + "...";
                }

                if (idname.Length > nameGoodLength_hanzi)
                    nameNotGood = true;
            }


            GUILayout.Button("", GUILayout.Height(1));
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            blod.fontSize = 14;
            blod.normal.textColor = new Color(255, 200, 200);
            if (seleceted)
                originPackageConfig.selected = GUILayout.Toggle(originPackageConfig.selected, "", GUILayout.Width(18));
            GUILayout.Label(idname, blod, GUILayout.Height(22));

            if (nameNotGood)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("");
            }
            bool imported = false;
            bool needUpdate = true;
            var haveMoreVersion = false;
            if (KitInitializeEditor.KitOriginConfig.OriginPackageDic != null &&
                KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(ids[0]) &&
                KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]] != null &&
                KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]].Count > 1)
                haveMoreVersion = true;
            var opcs = new List<KitOriginPackageConfig>();
            if (haveMoreVersion)
            {
                var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]];
                foreach (var i in versions)
                {
                    var _opc = KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[i];
                    if (_opc.KitPackageConfig != null && System.IO.Directory.Exists(_opc.KitPackageConfig.ImportRootDirectory))
                        opcs.Add(_opc);
                }
            }
            var importNames = new List<string>();
            var packagesImportRootDir = System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory);
            if (System.IO.Directory.Exists(packagesImportRootDir))
            {
                var dirinfo = new System.IO.DirectoryInfo(packagesImportRootDir);
                foreach (var dinfo in dirinfo.GetDirectories())
                {
                    var dinfoname = dinfo.Name.Split('@')[0];
                    if (dinfoname == ids[0])
                    {
                        imported = true;
                        importNames.Add(dinfo.Name);
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

            if (!imported || !needUpdate) GUI.enabled = false;
            if (GUILayout.Button("更新", GUILayout.Width(50), GUILayout.Height(23)))
            {
                updateKKP(originPackageConfig, opcs);
            }
            GUI.enabled = true;

            if (!imported) GUI.enabled = false;
            if (haveMoreVersion)
            {
                if (GUILayout.Button("卸载所有版本", GUILayout.Width(100), GUILayout.Height(23)))
                {
                    uninstal(opcs);
                }
            }
            else
            {
                if (GUILayout.Button("卸载", GUILayout.Width(50), GUILayout.Height(23)))
                {
                    uninstal(originPackageConfig);
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
                GUILayout.Label("最新版本：", EditorStyles.boldLabel, GUILayout.Width(60));
                GUILayout.Label(originPackageConfig.KitPackageConfig.Version);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("作者：", EditorStyles.boldLabel, GUILayout.Width(30));
                GUILayout.Label(originPackageConfig.KitPackageConfig.Author);
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
                        GUILayout.Label("版本列表：", EditorStyles.boldLabel, GUILayout.Width(60));
                        
                        EditorGUILayout.BeginVertical();
                        for (var i = 0; i < versions.Count && i < 4; i++)
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

        void RequestKKPFile(KitOriginPackageConfig originPackageConfig, string title, System.Action successAction = null)
        {
            var _www = new UnityEngine.Networking.UnityWebRequest(originPackageConfig.kkpurl);
            _www.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(originPackageConfig.kkpfilepath);
            _www.disposeDownloadHandlerOnDispose = true;

            KitToolEditor.AddWebRequest(new KitToolEditor.WebRequest()
            {
                www = _www,
                waitAction = (uwq) =>
                {
                    EditorUtility.DisplayProgressBar(title + ": " + originPackageConfig.ID, "正在下载包: " + uwq.downloadProgress * 100 + "%", uwq.downloadProgress * 100 / 2);
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
                        EditorUtility.DisplayDialog(title + ": " + originPackageConfig.ID, "下载包失败：" + uwq.error, "确定");
                    }
                }
            });
        }
        void unpackeKKP(KitOriginPackageConfig originPackageConfig, string title, bool notDisplatDialog = false, System.Action<List<string>> doneAction = null)
        {
            KitPacker.Unpack(
                originPackageConfig.kkpfilepath,
                originPackageConfig.KitPackageConfig.ImportRootDirectory,
                (filename, progress, done, error, dependencies) =>
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
                        if (!string.IsNullOrEmpty(error))
                        {
                            Debug.LogError(KitConst.KitName + ": 导入 " + originPackageConfig.ID + " 失败！" + error);
                        }
                        else
                        {
                            Debug.Log(KitConst.KitName + ": 导入 " + originPackageConfig.ID + " 成功！");
                        }
                        if (!notDisplatDialog)
                        {
                            EditorUtility.DisplayDialog(title + ": " + originPackageConfig.ID, title + "成功！", "确定");
                        }

                        if (doneAction != null) doneAction(dependencies);
                    }
                }
            );
        }
        void importKKPFile(KitOriginPackageConfig originPackageConfig, string title, bool notDisplatDialog = false, System.Action doneAction = null)
        {
            if (notDisplatDialog || EditorUtility.DisplayDialog(title + ": " + originPackageConfig.ID, "确认" + title + " " + originPackageConfig.ID + " ？", "确认", "取消"))
            {
                var opcs = new List<KitOriginPackageConfig>();
                if(KitInitializeEditor.KSwordKitConfig != null &&
                    KitInitializeEditor.KSwordKitConfig.KitImportedPackageList != null &&
                    KitInitializeEditor.KSwordKitConfig.KitImportedPackageList.Count > 0)
                {
                    var idname = originPackageConfig.ID.Split('@')[0];
                    var ids = new List<string>();
                    foreach(var importedID in KitInitializeEditor.KSwordKitConfig.KitImportedPackageList)
                        if (idname == importedID.Split('@')[0])
                            ids.Add(importedID);
                    if(ids.Count > 0)
                    {
                        if(KitInitializeEditor.KitOriginConfig != null &&
                            KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                            KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                        {
                            foreach(var opc in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                            {
                                if (ids.Contains(opc.ID))
                                    opcs.Add(opc);
                            }
                        }
                    }
                }

                System.Action<List<string>> unpackKKPDoneAction = (dependencies) => { 
                    if(dependencies != null && dependencies.Count > 0)
                    {
                        EditorUtility.DisplayProgressBar(title + ": " + originPackageConfig.ID, "包下载完毕, 解析完毕, 正在处理依赖包...", 0);
                        for (var i = 0; i < dependencies.Count; i++)
                        {
                            var depd = dependencies[i];
                            EditorUtility.DisplayProgressBar(title + ": " + originPackageConfig.ID, "包下载完毕, 解析完毕, 正在处理依赖包 " + depd + "(" + (i+1) + "/" + dependencies.Count + ")", (i + 1) / (float)dependencies.Count);
                            if (KitInitializeEditor.KitOriginConfig.PackageList != null &&
                                KitInitializeEditor.KitOriginConfig.PackageList.Contains(depd) &&
                                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                            {
                                foreach (var opc in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                                    if (opc.ID == depd)
                                    {
                                        if (opc.KitPackageConfig != null && System.IO.Directory.Exists(opc.KitPackageConfig.ImportRootDirectory)) continue;
                                        importKKPFile(opc, title, true);
                                    }
                            }
                        }
                        EditorUtility.ClearProgressBar();
                    }
                    if (doneAction != null) doneAction();
                };
                if (!System.IO.File.Exists(originPackageConfig.kkpfilepath))
                {
                    RequestKKPFile(originPackageConfig, title, () =>
                    {
                        if (originPackageConfig.KitPackageConfig != null && !originPackageConfig.KitPackageConfig.liveWithOtherVersion && opcs.Count > 0)
                            uninstal(opcs, true);
                        unpackeKKP(originPackageConfig, title, notDisplatDialog, unpackKKPDoneAction);
                    });
                }
                else 
                {
                    if (originPackageConfig.KitPackageConfig != null && !originPackageConfig.KitPackageConfig.liveWithOtherVersion && opcs.Count > 0)
                        uninstal(opcs, true);
                    unpackeKKP(originPackageConfig, title, notDisplatDialog, unpackKKPDoneAction);
                }
            }
        }
        void importKKPFile(List<KitOriginPackageConfig> kitOriginPackageConfigs, string title, bool notDisplatDialog = false, System.Action doneAction = null)
        {
            var info = "";
            foreach (var c in kitOriginPackageConfigs)
                if (string.IsNullOrEmpty(info))
                    info = c.ID;
                else
                    info += "\n" + c.ID;
            if (notDisplatDialog || EditorUtility.DisplayDialog(title, "存在一个或多个内容项需要" + title + "：\n\n" + info + "\n\n确认" + title + " ？", "确认", "取消"))
            {
                var index = 0;
                foreach(var opc in kitOriginPackageConfigs)
                {
                    importKKPFile(opc, title, notDisplatDialog, ()=> {
                        index++;
                        if (!notDisplatDialog)
                        {
                            Debug.Log(KitConst.KitName + ": " + title + "成功！");
                            EditorUtility.DisplayDialog(title, info + "\n\n已" + title + "成功！", "确定");
                        }
                        if (index >= kitOriginPackageConfigs.Count && doneAction != null)
                            doneAction();
                    });
                }
            }
        }
        void uninstal(KitOriginPackageConfig originPackageConfig, bool notDisplatDialog = false)
        {
            if (notDisplatDialog || EditorUtility.DisplayDialog("卸载：" + originPackageConfig.ID, "确认卸载 " + originPackageConfig.ID + " ？", "确认", "取消"))
            {
                DirectoryDelete(originPackageConfig.KitPackageConfig.ImportRootDirectory);
                if(originPackageConfig.KitPackageConfig.FileSettings!= null &&
                    originPackageConfig.KitPackageConfig.FileSettings.Count > 0)
                {
                    foreach(var fileSetting in originPackageConfig.KitPackageConfig.FileSettings)
                    {
                        if (fileSetting.enableUninstall)
                        {
                            var targetPath = KitPackageConfigFileSetting.TargetPathToRealPath(fileSetting.TargetPath);
                            if (fileSetting.isDir && System.IO.Directory.Exists(targetPath))
                                DirectoryDelete(targetPath);
                            else if (!fileSetting.isDir && System.IO.File.Exists(targetPath))
                                FileDelete(targetPath);
                        }
                    }
                }
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
                Debug.Log(KitConst.KitName + ": " + originPackageConfig.ID + " 已成功卸载！");
                if (!notDisplatDialog)
                    EditorUtility.DisplayDialog("卸载：" + originPackageConfig.ID, "已成功卸载！", "确认");
            }
        }
        void uninstal(List<KitOriginPackageConfig> kitOriginPackageConfigs, bool notDisplatDialog = false)
        {
            var info = "";
            foreach (var c in kitOriginPackageConfigs)
                if (string.IsNullOrEmpty(info))
                    info = c.ID;
                else
                    info += "\n" + c.ID;
            if (notDisplatDialog || EditorUtility.DisplayDialog("全部卸载", "确认全部卸载？\n\n" + info, "确认", "取消"))
            {
                foreach(var originPackageConfig in kitOriginPackageConfigs)
                    uninstal(originPackageConfig, true);
                if (!notDisplatDialog)
                    EditorUtility.DisplayDialog("全部卸载", info + "\n\n已全部卸载完成！", "确认");
            }
        }
        void updateKKP(KitOriginPackageConfig originPackageConfig, List<KitOriginPackageConfig> kitOriginPackageConfigs)
        {
            bool liveWithOtherVersion = false;
            if (originPackageConfig.KitPackageConfig != null)
                liveWithOtherVersion = originPackageConfig.KitPackageConfig.liveWithOtherVersion;

            if (kitOriginPackageConfigs.Count > 1 && !liveWithOtherVersion)
            {
                var info = "";
                foreach (var c in kitOriginPackageConfigs)
                    info += "\n" + c.ID;
                if (EditorUtility.DisplayDialog("更新：" + originPackageConfig.ID.Split('@')[0], "项目中已导入多个版本：\n" + info + "\n\n是否将它们全部更新到最新版本？", "确认", "取消"))
                {
                    uninstal(kitOriginPackageConfigs, true);
                    importKKPFile(originPackageConfig, "更新", true, () => {
                        Debug.Log(KitConst.KitName + ": " + originPackageConfig.ID.Split('@')[0] + " 已成功更新到最新版本！" + originPackageConfig.ID);
                        EditorUtility.DisplayDialog("更新：" + originPackageConfig.ID.Split('@')[0],
                            "已成功更新到最新版本！\n\n" + originPackageConfig.ID,
                            "确定");
                    });
                }
            }
            else if(EditorUtility.DisplayDialog("更新：" + kitOriginPackageConfigs[0].ID, 
                kitOriginPackageConfigs[0].ID + " -> " + originPackageConfig.ID + "\n\n确认更新？", "确认", "取消"))
            {
                if (!liveWithOtherVersion)
                    uninstal(kitOriginPackageConfigs[0], true);
                importKKPFile(originPackageConfig, "更新", true, ()=> {
                    Debug.Log(KitConst.KitName + ": 更新 " + kitOriginPackageConfigs[0].ID + " -> " + originPackageConfig.ID + " 已成功！");
                    EditorUtility.DisplayDialog("更新：" + kitOriginPackageConfigs[0].ID,
                        kitOriginPackageConfigs[0].ID + " -> " + originPackageConfig.ID + "\n\n更新成功！", "确定");
                });
            }
        }
        void updateKKP(List<KitOriginPackageConfig> kitOriginPackageConfigs, List<KitOriginPackageConfig> needUpdateOriginPackageConfigs, string info)
        {
            if (EditorUtility.DisplayDialog("全部更新", "检测到以下内容需要更新：\n\n" + info + "\n\n是否将它们全部更新到最新版本？", "确认", "取消"))
            {
                uninstal(needUpdateOriginPackageConfigs, true);
                var index = 0;
                for (var i = 0; i < kitOriginPackageConfigs.Count; i++)
                {
                    var originPackageConfig = kitOriginPackageConfigs[i];
                    importKKPFile(originPackageConfig, "更新", true, ()=> {
                        index++;
                        if(index == kitOriginPackageConfigs.Count)
                        {
                            Debug.Log(KitConst.KitName + ": 全部更新成功！");
                            EditorUtility.DisplayDialog("全部更新",
                            "已全部更新到了最新版本！",
                            "确定");
                        }
                    });
                }
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
            if (!isHanzi(c))
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
        bool isHanzi(char c)
        {
            string zimu = "0123456789.+-*/\\`~!@#$%^&*()_+[]{}:';\"?><,qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM ";
            return !zimu.Contains(c.ToString());
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