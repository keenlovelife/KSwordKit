
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
        const string kitUserSearchDefaultInputString = "搜索包名、作者、描述、版本号等等";

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
            window.richText = new GUIStyle();
            window.richText.richText = true;
            KitInitializeEditor.Request_packages((done, progress) => {
                if (done) Debug.Log(KitConst.KitName + ": 所有可用包已拉取完成！");
            });
        }

        string kitUserSearchInputString = kitUserSearchDefaultInputString;
        Vector2 scorllPos;
        GUIStyle blod;
        GUIStyle richText;
        string idKey = "id";
        string nameKey = "name";
        string versionKey = "version|v";
        string liveWithOtherVersionKey = "livewithotherversion|livewithother";
        string contactKey = "contact";
        string homepageKey = "homepage";
        string dateKey = "date";
        string authorKey = "author";
        string descriptionKey = "description|desc";
        string dependenciesKey = "dependencies|depend|rely|dependency";
        string tagKey = "tag";
        string notagKey = "notag";
        string allKeys
        {
            get
            {
                return idKey + "|" + nameKey + "|" + versionKey + "|" + liveWithOtherVersionKey + "|"
                    + contactKey + "|" + homepageKey + "|" + dateKey + "|" + authorKey + "|" + descriptionKey + "|" 
                    + dependenciesKey + "|" + tagKey;
            }
        }
        string packageCount = "";
        bool seleceted = false;
        bool per_selected = false;
        private void OnGUI()
        {
            GUIEvent();

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
            var selectedPackageCount = 0;
            if (KitInitializeEditor.KitOriginConfig != null &&
                       KitInitializeEditor.KitOriginConfig.PackageCount > 0 &&
                       KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                       KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
            {
                foreach (var c in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                    if (c.selected) selectedPackageCount++;
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

            EditorGUILayout.Space(20);
            // 搜索
            if (!string.IsNullOrEmpty(kitUserSearchInputString) && kitUserSearchInputString != kitUserSearchDefaultInputString)
            {
                var searchResultCount = 0;
                Dictionary<string, string> tagSearchDic;
                var searchResults = Search(kitUserSearchInputString, out tagSearchDic);
                searchResultCount = searchResults.Count;

                if (searchResultCount == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label("无任何搜索结果");
                    GUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    blod.fontSize = 15;
                    EditorGUILayout.LabelField("搜索结果：(" + searchResultCount + ")", blod);
                    EditorGUILayout.EndHorizontal();
                    // 搜索列表内容
                    GUILayout.Space(12);
                    scorllPos = GUILayout.BeginScrollView(scorllPos, false, false);
                    foreach (var opc in searchResults)
                    {
                        if (opc.KitPackageConfig != null && string.IsNullOrEmpty(opc.KitPackageConfig.ImportRootDirectory))
                            opc.KitPackageConfig.ImportRootDirectory = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, opc.ID));
                        var ids = opc.ID.Split('@');
                        if (ids.Length == 2)
                            DrawItemGUI(opc, true, tagSearchDic);
                    }
                    GUILayout.EndScrollView();
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(15);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(30);
                return;
            }

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
            if (seleceted)
                GUILayout.Label("已选择" + selectedPackageCount + "项");
            else
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
        void DrawItemGUI(KitOriginPackageConfig originPackageConfig, bool isSearchResult = false, Dictionary<string, string> tagSearchDic = null)
        {
            var lableWith = 450;

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
            richText.fontSize = 14;
            richText.normal.textColor = new Color(255, 200, 200);
            blod.fontSize = 14;
            blod.normal.textColor = new Color(255, 200, 200);
            if (seleceted)
                originPackageConfig.selected = GUILayout.Toggle(originPackageConfig.selected, "", GUILayout.Width(18));
            var idladel = idname;
            if (isSearchResult)
                idladel = makeRichText(idladel, nameKey, tagSearchDic);
            if (GUILayout.Button(idladel, richText))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("复制文本"), false, () => {
                    UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.Name;
                });
                menu.ShowAsContext();
            }
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
                    if (isSearchResult)
                    {
                        if(dinfo.Name == originPackageConfig.ID)
                        {
                            imported = true;
                            importNames.Add(dinfo.Name);
                        }
                    }
                    else
                    {
                        var dinfoname = dinfo.Name.Split('@')[0];
                        if (dinfoname == ids[0])
                        {
                            imported = true;
                            importNames.Add(dinfo.Name);
                        }
                    }
                }
            }
            
            if (!imported && GUILayout.Button("导入", GUILayout.Width(55), GUILayout.Height(23)))
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
            if (GUILayout.Button("更新", GUILayout.Width(55), GUILayout.Height(23)))
            {
                updateKKP(originPackageConfig, opcs);
            }
            GUI.enabled = true;

            if (!imported) GUI.enabled = false;
            if (haveMoreVersion && !isSearchResult)
            {
                if (GUILayout.Button("卸载所有版本", GUILayout.Width(100), GUILayout.Height(23)))
                {
                    uninstal(opcs);
                }
            }
            else
            {
                if (GUILayout.Button("卸载", GUILayout.Width(55), GUILayout.Height(23)))
                {
                    uninstal(originPackageConfig);
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button("查看", GUILayout.Width(55), GUILayout.Height(23)))
            {
                lookOriginPackageConfigInfo(originPackageConfig.ID);
            }
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();

            if(originPackageConfig.KitPackageConfig != null)
            {
                richText.fontSize = 12;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("ID：", EditorStyles.boldLabel, GUILayout.Width(30));
                GUI.enabled = false;
                if (isSearchResult)
                {
                    var idlabel = originPackageConfig.KitPackageConfig.ID;
                    idlabel = makeRichText(idlabel, idKey, tagSearchDic);
                    GUILayout.Label(idlabel, richText, GUILayout.Width(lableWith));
                }
                else
                    GUILayout.Label(originPackageConfig.KitPackageConfig.ID, GUILayout.Width(lableWith));
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("版本：", EditorStyles.boldLabel, GUILayout.Width(30));
                if (isSearchResult)
                {
                    var versionlabel = originPackageConfig.KitPackageConfig.Version;
                    versionlabel = makeRichText(versionlabel, versionKey, tagSearchDic);
                    GUILayout.Label(versionlabel, richText, GUILayout.Width(lableWith));
                }
                else
                    GUILayout.Label(originPackageConfig.KitPackageConfig.Version, GUILayout.Width(lableWith));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("日期：", EditorStyles.boldLabel, GUILayout.Width(30));
                if (isSearchResult)
                {
                    var datelable = originPackageConfig.KitPackageConfig.Date;
                    datelable = makeRichText(datelable, dateKey, tagSearchDic);
                    GUILayout.Label(datelable, richText, GUILayout.Width(lableWith));
                }
                else
                    GUILayout.Label(originPackageConfig.KitPackageConfig.Date, GUILayout.Width(lableWith));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("作者：", EditorStyles.boldLabel, GUILayout.Width(30));
                var Authorlable = originPackageConfig.KitPackageConfig.Author;
                if (isSearchResult)
                    Authorlable = makeRichText(Authorlable, authorKey, tagSearchDic);
                if (GUILayout.Button(Authorlable, richText, GUILayout.Width(lableWith)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("复制文本"), false, () => {
                        UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.Author;
                    });
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("主页：", EditorStyles.boldLabel, GUILayout.Width(30));
                var HomePagelable = originPackageConfig.KitPackageConfig.HomePage;
                if (isSearchResult)
                    HomePagelable = makeRichText(HomePagelable, homepageKey, tagSearchDic);
                if (GUILayout.Button(HomePagelable, richText, GUILayout.Width(lableWith)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("复制文本"), false, () =>
                    {
                        UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.HomePage;
                    });
                    menu.AddItem(new GUIContent("去浏览器打开"), false, () =>
                    {
                        System.Diagnostics.Process.Start(originPackageConfig.KitPackageConfig.HomePage);
                    });
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("联系：", EditorStyles.boldLabel, GUILayout.Width(30));
                var Contactlable = originPackageConfig.KitPackageConfig.Contact;
                if (isSearchResult)
                    Contactlable = makeRichText(Contactlable, contactKey, tagSearchDic);
                if (GUILayout.Button(Contactlable, richText, GUILayout.Width(lableWith)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("复制文本"), false, () =>
                    {
                        UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.Contact;
                    });
                    menu.AddItem(new GUIContent("发邮件"), false, () =>
                    {
                        System.Uri uri = new System.Uri(string.Format("mailto:{0}", originPackageConfig.KitPackageConfig.Contact));//第二个参数是邮件的标题
                        Application.OpenURL(uri.AbsoluteUri);
                    });
                    menu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("标签：", EditorStyles.boldLabel, GUILayout.Width(30));
                var eachMaxLength = 500;
                var maxHeight = 2;
                if (isSearchResult && (tagSearchDic.ContainsKey(tagKey) || tagSearchDic.ContainsKey(notagKey)))
                {
                    var tags = new List<string>();
                    tags.AddRange(originPackageConfig.KitPackageConfig.Tags);
                    var tagValueList = new List<string>();
                    if (tagSearchDic.ContainsKey(tagKey))
                        tagValueList.AddRange(tagSearchDic[tagKey].Split('|'));
                    if (tagSearchDic.ContainsKey(notagKey))
                        tagValueList.AddRange(tagSearchDic[notagKey].Split('|'));
                    var tagmatchedList = new List<int>();
                    foreach (var value in tagValueList)
                        for (var i = 0; i < tags.Count; i++)
                            if (tags[i].ToLower().Contains(value) && !tagmatchedList.Contains(i)) tagmatchedList.Add(i);
                    GUILayout.BeginVertical();
                    GUI.enabled = false;
                    var _i = 0;
                    var each_length = 0;
                    for (var l = 0; l < maxHeight || tagmatchedList.Count > 0; l++)
                    {
                        each_length = 0;
                        GUILayout.BeginHorizontal();
                        for (; each_length < eachMaxLength && _i < tags.Count; _i++)
                        {
                            if (tagmatchedList.Contains(_i)) tagmatchedList.Remove(_i);
                            var tag = tags[_i];
                            var w = getButtonWidth(tag);
                            tag = makeRichText(tag, tagKey, tagSearchDic);
                            GUILayout.Label(tag, richText, GUILayout.Width(w));
                            each_length += w + 10;
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (_i < tags.Count)
                        GUILayout.Button("...", GUILayout.Width(20));
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
                else
                {
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
                    if (_i < originPackageConfig.KitPackageConfig.Tags.Count)
                        GUILayout.Button("...", GUILayout.Width(20));
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
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
                        if (isSearchResult)
                        {
                            var Dependencieslable = originPackageConfig.KitPackageConfig.Dependencies[l];
                            Dependencieslable = makeRichText(Dependencieslable, dependenciesKey, tagSearchDic);
                            GUILayout.Label(Dependencieslable, richText);
                        }
                        else
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
                var maxLineCount = 4;
                EditorGUILayout.BeginVertical();
                var desckeys = descriptionKey.Split('|');
                var tagSearchDicHave = false;
                var descValue = "";
                if(tagSearchDic != null)
                {
                    foreach (var desc in desckeys)
                        if (tagSearchDic.ContainsKey(desc))
                        {
                            tagSearchDicHave = true;

                            if (string.IsNullOrEmpty(descValue))
                                descValue = tagSearchDic[desc];
                            else
                                descValue += "|" + tagSearchDic[desc];
                        }
                    if(tagSearchDic.ContainsKey(notagKey))
                    {
                        tagSearchDicHave = true;
                        if (string.IsNullOrEmpty(descValue))
                            descValue = tagSearchDic[notagKey];
                        else
                            descValue += "|" + tagSearchDic[notagKey];
                    }
                }

                if (isSearchResult && tagSearchDicHave)
                {
                    var descValues = descValue.Split('|');
                    var descValueList = new List<string>();
                    descValueList.AddRange(descValues);
                    var matchedIndexList = new List<Vector2Int>();
                    var descContent = originPackageConfig.KitPackageConfig.Description.Substring(0);
                    foreach (var value in descValueList)
                    {
                        var dContent = descContent.Substring(0).ToLower();
                        var dindex = dContent.IndexOf(value);
                        var lastStartIndex = 0;
                        while (dindex != -1)
                        {
                            matchedIndexList.Add(new Vector2Int() { 
                                x = lastStartIndex + dindex,
                                y = value.Length
                            });
                            dContent = dContent.Substring(dindex + value.Length);
                            lastStartIndex += dindex + value.Length;
                            dindex = dContent.IndexOf(value);
                        }
                    }
                    var tempLines = new List<string>();
                    var startIndex = 0;
                    var count = 0;
                    var lastMatchIndex = 0;
                    bool haveOver = false;
                    if(descContent.Length <= maxLineLength)
                    {
                        startIndex = 0;
                        count = descContent.Length;
                        haveOver = true;
                    }
                    else
                    {
                        for (var i = 0; i < descContent.Length; i++)
                        {
                            if (descContent[i] == '\n' || count >= maxLineLength)
                            {
                                if (count == 0) tempLines.Add("");
                                else
                                {
                                    if (matchedIndexList.Count > 0)
                                    {
                                        var matched = matchedIndexList[0];
                                        var left = "";
                                        var center = "";
                                        var rigth = "";
                                        if (matched.x >= startIndex && matched.x <= startIndex + count - 1)
                                        {
                                            if (startIndex < matched.x)
                                                left = descContent.Substring(startIndex, matched.x - startIndex);
                                            if (startIndex + count - 1 >= matched.x + matched.y - 1)
                                            {
                                                center = descContent.Substring(matched.x, matched.y);
                                                matchedIndexList.RemoveAt(0);
                                            }
                                            else if (startIndex + count - 1 < matched.x + matched.y - 1)
                                            {
                                                var c = startIndex + count - matched.x;
                                                center = descContent.Substring(matched.x, c);
                                                matchedIndexList[0] = new Vector2Int()
                                                {
                                                    x = startIndex + count,
                                                    y = matched.y - c
                                                };
                                            }
                                            if (startIndex + count - 1 > matched.x + matched.y - 1)
                                                rigth = descContent.Substring(matched.x + matched.y, startIndex + count - matched.x - matched.y);
                                        }
                                        center = "<color=yellow><b>" + center + "</b></color>";
                                        var line = left + center + rigth;
                                        tempLines.Add(line);
                                        lastMatchIndex = tempLines.Count - 1;
                                    }
                                    else
                                        tempLines.Add(descContent.Substring(startIndex, count));
                                }
                                startIndex = i + 1;
                                count = 0;
                                haveOver = false;
                            }
                            else
                            {
                                count++;
                                haveOver = true;
                            }
                        }
                    }
                    if (haveOver && matchedIndexList.Count > 0)
                    {
                        var matchedStr = "";
                        var lastIndex = startIndex;
                        for(var i = 0; i < matchedIndexList.Count; i++)
                        {
                            var matched = matchedIndexList[i];
                            matchedStr += descContent.Substring(lastIndex, matched.x - lastIndex);
                            matchedStr += "<color=yellow><b>" + descContent.Substring(matched.x, matched.y) + "</b></color>";
                            lastIndex = matched.x + matched.y;
                        }
                        if (lastIndex < descContent.Length)
                            matchedStr += descContent.Substring(lastIndex);
                        tempLines.Add(matchedStr);
                        lastMatchIndex = tempLines.Count - 1;
                        matchedIndexList.Clear();
                    }
                    for (var i = 0; i < tempLines.Count && (i <= lastMatchIndex || i < maxLineCount); i++)
                    {
                        if (GUILayout.Button(tempLines[i], richText))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("复制文本"), false, () =>
                            {
                                UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.Description;
                            });
                            menu.ShowAsContext();
                        }
                    }
                    if (tempLines.Count > maxLineCount - 1 && lastMatchIndex < tempLines.Count - 1)
                    {
                        if (GUILayout.Button("...", richText))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("复制文本"), false, () =>
                            {
                                UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.Description;
                            });
                            menu.ShowAsContext();
                        }
                    }
                }
                else
                {
                    var lines = originPackageConfig.KitPackageConfig.Description.Split('\n');
                    var tempLines = new List<string>();
                    for (var i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        if (line.Length <= maxLineLength)
                            tempLines.Add(line);
                        else
                        {
                            while (line.Length > maxLineLength)
                            {
                                tempLines.Add(line.Substring(0, maxLineLength));
                                line = line.Substring(maxLineLength, line.Length - maxLineLength);
                            }
                            if (!string.IsNullOrEmpty(line))
                                tempLines.Add(line);
                        }
                    }
                    for (var i = 0; i < tempLines.Count && i < maxLineCount; i++)
                    {
                        if (GUILayout.Button(tempLines[i], richText))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("复制文本"), false, () =>
                            {
                                UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.Description;
                            });
                            menu.ShowAsContext();
                        }
                    }
                    if (lines.Length > maxLineCount - 1)
                    {
                        if (GUILayout.Button("...", richText))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("复制文本"), false, () =>
                            {
                                UnityEngine.GUIUtility.systemCopyBuffer = originPackageConfig.KitPackageConfig.Description;
                            });
                            menu.ShowAsContext();
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                if (KitInitializeEditor.KitOriginConfig.OriginPackageDic != null)
                {
                    if (KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(ids[0]) && 
                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]] != null &&
                        KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]].Count > 1)
                    {
                        var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]];
                        var _versions = new List<int>();
                        _versions.AddRange(versions);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(35);
                        GUILayout.Label("所有版本：", EditorStyles.boldLabel, GUILayout.Width(60));

                        var maxShowVersionCount = 4;
                        var lastidSearchIndex = 0;
                        for (var i = 0; i < versions.Count; i++)
                        {
                           var _versionID = KitInitializeEditor.KitOriginConfig.PackageList[versions[i]];
                            if (_versionID == originPackageConfig.ID)
                            {
                                if (i > lastidSearchIndex)
                                    lastidSearchIndex = i;
                            }
                            else
                            {
                                if (System.IO.Directory.Exists(System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, _versionID))))
                                {
                                    if (i > lastidSearchIndex)
                                        lastidSearchIndex = i;
                                }
                            }
                        }
                        if (isSearchResult)
                        {
                            var tagValues = new List<string>();
                            var versionKeys = versionKey.Split('|');
                            foreach (var vkey in versionKeys)
                                if (tagSearchDic.ContainsKey(vkey))
                                    tagValues.AddRange(tagSearchDic[vkey].Split('|'));
                            if (tagSearchDic.ContainsKey(notagKey))
                                tagValues.AddRange(tagSearchDic[notagKey].Split('|'));
                            foreach (var v in tagValues)
                            {
                                for (var i = 0; i < versions.Count; i++)
                                    if (KitInitializeEditor.KitOriginConfig.PackageList[versions[i]].Split('@')[1] == v)
                                        if (i > lastidSearchIndex)
                                            lastidSearchIndex = i;
                            }
                        }

                        EditorGUILayout.BeginVertical();

                        var _i = 0;
                        for (; _i < _versions.Count && (_i <= lastidSearchIndex || _i < maxShowVersionCount); _i++)
                        {
                            var versionID = KitInitializeEditor.KitOriginConfig.PackageList[_versions[_i]];
                            var version = versionID.Split('@')[1];
                            var w = GUILayout.Width(getButtonWidth(version));
                            GUILayout.BeginHorizontal();
                            if (isSearchResult)
                                version = makeRichText(version, versionKey, tagSearchDic);
                            GUILayout.Button(version, richText, w);
                            var versionPath = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, versionID));
                            var versionImported = System.IO.Directory.Exists(versionPath);
                            if (versionImported)
                            {
                                GUI.enabled = false;
                                GUILayout.Button("已导入", GUILayout.Width(45), GUILayout.Height(23));
                                GUI.enabled = true;
                            }
                            else if (GUILayout.Button("导入", GUILayout.Width(45), GUILayout.Height(23)))
                            {
                                importKKPFile(KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[_versions[_i]], "导入");
                            }
                            GUI.enabled = versionImported;
                            if (GUILayout.Button("卸载", GUILayout.Width(45), GUILayout.Height(23)))
                            {
                                uninstal(KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[_versions[_i]]);
                            }
                            GUI.enabled = true;
                            if (GUILayout.Button("查看", GUILayout.Width(45), GUILayout.Height(23)))
                            {
                                lookOriginPackageConfigInfo(KitInitializeEditor.KitOriginConfig.PackageList[_versions[_i]]);
                            }
                            GUILayout.EndHorizontal();
                        }
                        if (_i < _versions.Count)
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
        void GUIEvent()
        {
            //if (Event.current.type == EventType.MouseDown)
            //{
            //    Debug.LogError(EventType.MouseDown);//鼠标按下
            //}
            //else if (Event.current.type == EventType.MouseUp)
            //{
            //    Debug.LogError(EventType.MouseUp);//鼠标抬起
            //}
            //else if (Event.current.type == EventType.MouseMove)
            //{
            //    Debug.LogError(EventType.MouseMove);
            //}
            //else if (Event.current.type == EventType.MouseDrag)
            //{
            //    Debug.LogError(EventType.MouseDrag);//鼠标拖动
            //}
            //else if (Event.current.type == EventType.KeyDown)
            //{
            //    Debug.LogError(EventType.KeyDown);//按键按下
            //}
            //else if (Event.current.type == EventType.KeyUp)
            //{
            //    Debug.LogError(EventType.KeyUp);//按键抬起
            //}
            //else if (Event.current.type == EventType.ScrollWheel)
            //{
            //    Debug.LogError(EventType.ScrollWheel);//中轮滚动
            //}
            //else if (Event.current.type == EventType.Repaint)
            //{
            //    Debug.LogError(EventType.Repaint);//每一帧重新渲染会发
            //}
            //else if (Event.current.type == EventType.Layout)
            //{
            //    Debug.LogError(EventType.Layout);
            //}
            //else if (Event.current.type == EventType.DragUpdated)
            //{
            //    Debug.LogError(EventType.DragUpdated);//拖拽的资源进入界面
            //}
            //else if (Event.current.type == EventType.DragPerform)
            //{
            //    Debug.LogError(EventType.DragPerform);//拖拽的资源放到了某个区域里
            //}
            //else if (Event.current.type == EventType.Ignore)
            //{
            //    Debug.LogError(EventType.Ignore);//操作被忽略
            //}
            //else if (Event.current.type == EventType.Used)
            //{
            //    Debug.LogError(EventType.Used);//操作已经被使用过了
            //}
            //else if (Event.current.type == EventType.ValidateCommand)
            //{
            //    Debug.LogError(EventType.ValidateCommand);//有某种操作被触发（例如复制和粘贴）
            //}
            //else if (Event.current.type == EventType.ExecuteCommand)
            //{
            //    Debug.LogError(EventType.ExecuteCommand);//有某种操作被执行（例如复制和粘贴）
            //}
            //else if (Event.current.type == EventType.DragExited)
            //{
            //    Debug.LogError(EventType.DragExited);//松开拖拽的资源
            //}
            //else if (Event.current.type == EventType.ContextClick)
            //{
            //    Debug.LogError(EventType.ContextClick);//右键点击
            //}
            //else if (Event.current.type == EventType.MouseEnterWindow)
            //{
            //    Debug.LogError(EventType.MouseEnterWindow);
            //}
            //else if (Event.current.type == EventType.MouseLeaveWindow)
            //{
            //    Debug.LogError(EventType.MouseLeaveWindow);
            //}

            //if (Event.current.type == EventType.ContextClick)
            //{
            //    GenericMenu menu = new GenericMenu();

            //    menu.AddItem(new GUIContent("1"), false, null, null);

            //    menu.AddSeparator("");

            //    menu.AddItem(new GUIContent("2"), false, null, null);

            //    menu.ShowAsContext();

            //    //设置该事件被使用

            //    Event.current.Use();
            //}
        }
        void RemoveDuplicateElements(Dictionary<string, string> tagSearchDic)
        {
            var temp = new Dictionary<string, string>();
            foreach (var kv in tagSearchDic)
            {
                var values = kv.Value.Split('|');
                var valueList = new List<string>();
                for(var i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    bool canadd = true;
                    for (var j = 0; j < values.Length; j++)
                    {
                        if (i == j) continue;
                        if(values[j].Contains(value))
                        {
                            canadd = false;
                            break;
                        }
                    }
                    if (canadd)
                        valueList.Add(value); 
                }
                var v = "";
                foreach (var _v in valueList)
                    if (string.IsNullOrEmpty(v))
                        v = _v;
                    else
                        v += "|" + _v;
                temp[kv.Key] = v;
            }
            tagSearchDic.Clear();
            foreach (var kv in temp)
                tagSearchDic[kv.Key] = kv.Value;
        }
        void RemoveDuplicateElements(List<string> strs)
        {
            var _strs = new List<string>();
            for (var i = 0; i < strs.Count; i++)
            {
                var value = strs[i];
                bool canadd = true;
                for (var j = 0; j < strs.Count; j++)
                {
                    if (i == j) continue;
                    if (strs[j].Contains(value))
                    {
                        canadd = false;
                        break;
                    }
                }
                if (canadd)
                    _strs.Add(value);
            }
            strs.Clear();
            strs.AddRange(_strs);
        }
        void RemoveDuplicateArea(List<Vector2Int> indexCountList)
        {
            int removeIndex;
            do
            {
                removeIndex = -1;
                for (var i = 0; i < indexCountList.Count; i++)
                {
                    var indexi = indexCountList[i];
                    for (var j = 0; j < indexCountList.Count; j++)
                    {
                        if (i == j) continue;
                        var indexj = indexCountList[j];
                        if (indexi.x >= indexj.x && indexi.x <= indexj.x + indexj.y && indexi.x + indexi.y - 1 > indexj.x + indexj.y - 1)
                        {
                            indexCountList[i] = new Vector2Int()
                            {
                                x = indexj.x,
                                y = indexi.x + indexi.y - indexj.x
                            };
                            removeIndex = j;
                            break;
                        }
                        else if (indexi.x + indexi.y - 1 >= indexj.x && indexi.x + indexi.y - 1 <= indexj.x + indexj.y - 1)
                        {
                            indexCountList[i] = new Vector2Int()
                            {
                                x = indexi.x,
                                y = indexi.y + indexj.x - indexi.x
                            };
                            removeIndex = j;
                            break;
                        }
                    }
                    if (removeIndex != -1)
                        break;
                }
                if (removeIndex != -1)
                    indexCountList.RemoveAt(removeIndex);

            } while (removeIndex != -1);
        }
        Dictionary<int, int> matchStrs(string match, List<string> strs)
        {
            RemoveDuplicateElements(strs);
            var matchedIndexList = new List<Vector2Int>();
            foreach(var value in strs)
            {
                var _match = match.ToLower();
                var index = _match.IndexOf(value);
                while(index != -1)
                {
                    bool canadd = true;
                    for (var i = 0; i < matchedIndexList.Count; i++)
                    {
                        var matched = matchedIndexList[i];
                        if (index >= matched.x && index <= matched.x + matched.y && index + value.Length - 1 > matched.x + matched.y - 1)
                        {
                            matchedIndexList[i] = new Vector2Int()
                            {
                                x = matched.x,
                                y = index + value.Length - matched.x
                            };
                            canadd = false;
                        }
                        else if(index + value.Length - 1 >= matched.x && index + value.Length -1 <= matched.x + matched.y - 1)
                        {
                            matchedIndexList[i] = new Vector2Int()
                            {
                                x = index,
                                y = matched.y + matched.x - index
                            };
                            canadd = false;
                        }
                    }
                    if (canadd)
                        matchedIndexList.Add(new Vector2Int()
                        {
                            x = index,
                            y = value.Length
                        });
                    else
                        RemoveDuplicateArea(matchedIndexList);
                    _match = _match.Substring(index + value.Length);
                    index = _match.IndexOf(value);
                }
            }
            var dic = new Dictionary<int, int>();
            foreach (var ic in matchedIndexList)
                dic[ic.x] = ic.y;
            return dic;
        }
        string makeRichText(string text, string tag, Dictionary<string, string> tagSearchDic)
        {
            if (tag == dateKey && tagSearchDic.ContainsKey(tag))
                return "<color=yellow><b>" + text + "</b></color>";
            var idladel = text.Substring(0);
            var tags = tag.Split('|');
            
            var values = new List<string>();
            foreach(var _tag in tags)
                if (tagSearchDic.ContainsKey(_tag))
                    values.AddRange(tagSearchDic[_tag].Split('|'));
            if (tagSearchDic.ContainsKey(notagKey))
                values.AddRange(tagSearchDic[notagKey].Split('|'));
            var matchedIndexDic = matchStrs(text, values);
            var _idlabel = "";
            var lastIndex = 0;
            foreach(var kv in matchedIndexDic)
            {
                if (kv.Key - lastIndex > 0)
                    _idlabel += idladel.Substring(lastIndex, kv.Key - lastIndex);
                _idlabel += "<color=yellow><b>" + idladel.Substring(kv.Key, kv.Value) + "</b></color>";
                lastIndex = kv.Key + kv.Value;
            }
            if (lastIndex < idladel.Length)
                _idlabel += idladel.Substring(lastIndex);
            return _idlabel;
        }
        List<string> makeTagSearchValueList(string values)
        {
            var vs = values.Split(',');
            var vList = new List<string>();
            foreach(var v in vs)
            {
                var _v = v.Trim().ToLower();
                if (!string.IsNullOrEmpty(_v) && !string.IsNullOrWhiteSpace(_v) && !vList.Contains(_v))
                    vList.Add(_v);
            }
            return vList;
        }
        string makeTagSearchValue(List<string> values)
        {
            var value = "";
            foreach(var v in values)
            {
                if (value == "")
                    value = v;
                else
                    value = value + "|" + v;
            }
            return value;
        }
        string makeTagSearchValue(string values)
        {
            return makeTagSearchValue(makeTagSearchValueList(values));
        }
        List<KitOriginPackageConfig> Search(string searchStr, out Dictionary<string, string> tagSearchDic)
        {
            var _searchStr = searchStr.Substring(0);
            tagSearchDic = new Dictionary<string, string>();
            if (_searchStr[_searchStr.Length - 1] != '|') _searchStr += '|';
            var index = _searchStr.IndexOf('|');
            if(index == -1)
            {
                var sindex = _searchStr.IndexOf(':');
                if(sindex != -1)
                {
                    var key = _searchStr.Substring(0, sindex).Trim().ToLower();
                    var value = "";
                    if (_searchStr.Length > sindex + 1)
                        value = _searchStr.Substring(sindex + 1, _searchStr.Length - sindex - 1).Trim().ToLower();
                    if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) &&
                        !string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
                    {
                        value = makeTagSearchValue(value);
                        if(!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                            tagSearchDic[key] = value;
                    }
                }
                else
                {
                    var value = makeTagSearchValue(_searchStr.Trim().ToLower());
                    if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                        tagSearchDic[notagKey.ToLower()] = value;
                }
            }
            else
            {
                do
                {
                    var s = _searchStr.Substring(0, index).ToLower();
                    var sindex = s.IndexOf(':');
                    var key = "";
                    var value = "";
                    if (sindex != -1)
                    {
                        key = s.Substring(0, sindex);
                        if (s.Length > sindex + 1)
                            value = s.Substring(sindex + 1, s.Length - sindex - 1);
                        key = key.Trim().ToLower();
                        value = value.Trim().ToLower();
                    }
                    else
                    {
                        key = notagKey.ToLower().Trim();
                        value = s.Trim().ToLower();
                    }
                    if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
                        key = notagKey;
                    if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value != "|")
                    {
                        if (!tagSearchDic.ContainsKey(key))
                        {
                            tagSearchDic[key] = makeTagSearchValue(value);
                        }
                        else
                        {
                            List<string> _values = new List<string>();
                            var _valueList = makeTagSearchValueList(value);

                            var _ss = tagSearchDic[key].Split('|');
                            var _ssList = new List<string>();
                            _ssList.AddRange(_ss);

                            foreach (var _v in _valueList)
                                if (!_ssList.Contains(_v))
                                    _values.Add(_v);
                            if(_values.Count > 0)
                            {
                                value = makeTagSearchValue(_values);
                                tagSearchDic[key] = tagSearchDic[key] + "|" + value;
                            }
                        }
                    }
                    if (_searchStr.Length <= index + 1)
                        break;
                    _searchStr = _searchStr.Substring(index + 1, _searchStr.Length - index - 1);
                    index = _searchStr.IndexOf('|');
                } 
                while (index != -1);
            }

            RemoveDuplicateElements(tagSearchDic);

            var searchResults = new List<KitOriginPackageConfig>();
            if (KitInitializeEditor.KitOriginConfig != null &&
                KitInitializeEditor.KitOriginConfig.PackageCount > 0 &&
                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 0)
                foreach (var opc in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                    if (matchOriginPackageConfig(opc, tagSearchDic)) searchResults.Add(opc);

            return searchResults;
        }
        bool matchOriginPackageConfig(KitOriginPackageConfig opc, Dictionary<string, string> tagSearchDic)
        {
            var versionKeys = versionKey.Split('|');
            var versionKeyList = new List<string>();
            versionKeyList.AddRange(versionKeys);

            var liveKeys = liveWithOtherVersionKey.Split('|');
            var liveKeyList = new List<string>();
            liveKeyList.AddRange(liveKeys);

            var descKeys = descriptionKey.Split('|');
            var descKeyList = new List<string>();
            descKeyList.AddRange(descKeys);

            var depdKeys = dependenciesKey.Split('|');
            var depdKeyList = new List<string>();
            depdKeyList.AddRange(depdKeys);

            var _allKeys = allKeys.Split('|');
            var _allkeyList = new List<string>();
            _allkeyList.AddRange(_allKeys);
            foreach (var key in _allkeyList)
            {
                if (!tagSearchDic.ContainsKey(key)) continue;

                var values = tagSearchDic[key].Split('|');
                var content = "";
                if (idKey == key)
                    content = opc.ID;
                else if(opc.KitPackageConfig != null)
                {
                    if (nameKey == key)
                        content = opc.KitPackageConfig.Name;
                    else if (contactKey == key)
                        content = opc.KitPackageConfig.Contact;
                    else if(homepageKey == key)
                        content = opc.KitPackageConfig.HomePage;
                    else if (tagKey == key)
                    {
                        foreach(var tag in opc.KitPackageConfig.Tags)
                            foreach (var value in values)
                                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && tag.ToLower().Contains(value.ToLower())) return true;
                    }
                    else if(dateKey == key)
                    {
                        foreach (var value in values)
                            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                            {
                                System.DateTime vdate;
                                System.DateTime date;
                                var datesuccess = System.DateTime.TryParse(opc.KitPackageConfig.Date, out date);
                                var v = value;
                                if (v.StartsWith("=="))
                                {
                                    v = v.Replace("==", "").Trim();
                                    var vdatesuccess = System.DateTime.TryParse(v, out vdate);
                                    if (vdatesuccess && datesuccess)
                                    {
                                        var vdatestr = vdate.ToString("yyyy-MM-dd HH:mm:ss");
                                        while (vdatestr.EndsWith("00"))
                                        {
                                            if (vdatestr.Length - 3 < 1) break;
                                            vdatestr = vdatestr.Trim();
                                            vdatestr = vdatestr.Substring(0, vdatestr.Length - 3);
                                        }
                                        var datestr = date.ToString("yyyy-MM-dd HH:mm:ss");
                                        if (!string.IsNullOrEmpty(vdatestr) && datestr.StartsWith(vdatestr)) return true;
                                    }
                                }
                                else
                                {
                                    if (v.StartsWith(">="))
                                    {
                                        v = v.Replace(">=", "").Trim();
                                        var vdatesuccess = System.DateTime.TryParse(v, out vdate);
                                        if (vdatesuccess && datesuccess && date >= vdate) return true;
                                    } 
                                    else if (v.StartsWith("<="))
                                    {
                                        v = v.Replace("<=", "").Trim();
                                        var vdatesuccess = System.DateTime.TryParse(v, out vdate);
                                        if (vdatesuccess && datesuccess && date <= vdate) return true;
                                    }
                                    else if (v.StartsWith(">"))
                                    {
                                        v = v.Replace(">", "").Trim();
                                        var vdatesuccess = System.DateTime.TryParse(v, out vdate);
                                        if (vdatesuccess && datesuccess && date > vdate) return true;
                                    }
                                    else if (v.StartsWith("<"))
                                    {
                                        v = v.Replace("<", "").Trim();
                                        var vdatesuccess = System.DateTime.TryParse(v, out vdate);
                                        if (vdatesuccess && datesuccess && date < vdate) return true;
                                    }
                                }
                            }
                    }
                    else
                    {
                        if (versionKeyList.Contains(key))
                            content = opc.KitPackageConfig.Version;
                        else if (liveKeyList.Contains(key))
                            content = opc.KitPackageConfig.liveWithOtherVersion.ToString();
                        else if (descKeyList.Contains(key))
                            content = opc.KitPackageConfig.Description;
                        else if (depdKeyList.Contains(key))
                        {
                            foreach (var value in values)
                                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                                {
                                    foreach(var d in opc.KitPackageConfig.Dependencies)
                                        if (d.ToLower().Contains(value.ToLower())) return true;
                                }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content))
                {
                    content = content.ToLower();
                    foreach (var value in values)
                        if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && content.Contains(value.ToLower())) return true;
                }
            }

            var notagValues = new List<string>();
            foreach(var k in tagSearchDic.Keys)
            {
                if (!_allkeyList.Contains(k))
                {
                    var values = tagSearchDic[k].Split('|');
                    notagValues.AddRange(values);
                }
            }
            foreach (var value in notagValues)
            {
                var searchStr = value;
                if (opc.KitPackageConfig != null)
                {
                    var pc = opc.KitPackageConfig;
                    if (match(pc.Name, searchStr)) return true;
                    if (match(pc.Version, searchStr)) return true;
                    if (match(pc.Author, searchStr)) return true;
                    if (match(pc.Contact, searchStr)) return true;
                    if (match(pc.HomePage, searchStr)) return true;
                    if (match(pc.Date, searchStr)) return true;
                    if (match(pc.Description, searchStr)) return true;
                    foreach (var depd in pc.Dependencies)
                        if (match(depd, searchStr)) return true;
                    foreach (var tag in pc.Tags)
                        if (match(tag, searchStr)) return true;
                }
            }

            return false;
        }
        bool match(string str, string matchStr)
        {
            return str.Trim().ToLower().Contains(matchStr.Trim().ToLower());
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
        void lookOriginPackageConfigInfo(string id)
        {
            KitLookPackageInfoWindow.Open(id);
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
        void DirectoryDelete(string dir)
        {
            if (System.IO.Directory.Exists(dir))
                System.IO.Directory.Delete(dir, true);
            var dirMetaFilePath = dir + ".meta";
            if (System.IO.File.Exists(dirMetaFilePath))
                System.IO.File.Delete(dirMetaFilePath);
        }
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