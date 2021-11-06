using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KSwordKit.Editor.PackageManager
{
    public class KitLookPackageInfoWindow : EditorWindow
    {
        static KitLookPackageInfoWindow window;
        /// <summary>
        /// ���ڴ���ʾ����
        /// </summary>
        /// <param name="data">��������</param>
        public static void Open(string _id)
        {
            id = _id;
            if (window)
            {
                window.Close();
                config = null;
            }
            var windowTitle = KitConst.KitName + "��" + id;
            window = GetWindow<KitLookPackageInfoWindow>(true, windowTitle);
            window.minSize = new Vector2(400, 600);
            window.richText = new GUIStyle();
            window.richText.richText = true;
            
            var tempIDfilepath = System.IO.Path.Combine(Application.temporaryCachePath, tempFilename);
            if (System.IO.File.Exists(tempIDfilepath))
                System.IO.File.Delete(tempIDfilepath);
            System.IO.File.WriteAllText(tempIDfilepath, id);
        }

        static KitPackageConfig config;
        void getConfig()
        {
            var tempIDfilepath = System.IO.Path.Combine(Application.temporaryCachePath, tempFilename);
            if (System.IO.File.Exists(tempIDfilepath))
            {
                id = System.IO.File.ReadAllText(tempIDfilepath);
                var configpath = System.IO.Path.Combine(KitConst.KitPackagesRootDirectory, id) + "." + KitConst.KitPackageConfigFilename;
                if (System.IO.File.Exists(configpath))
                    config = JsonUtility.FromJson<KitPackageConfig>(System.IO.File.ReadAllText(configpath, System.Text.Encoding.UTF8));
            }
        }
        static  readonly string tempFilename = "lookPackage.info";
        Vector2 scorllPos;
        float spaceCount = 20;
        static string id;
        GUIStyle richText;
        private void OnGUI()
        {
            if(window == null)
                window = GetWindow<KitLookPackageInfoWindow>();
            if (config == null)
                getConfig();

            EditorGUILayout.Space(20);

            if (config == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceCount);
                var label = "�����ļ������ڣ�";
                if (!string.IsNullOrEmpty(id))
                    label = id + "��" + label;
                GUILayout.Label("�����ļ������ڣ�");
                GUILayout.Space(spaceCount);
                GUILayout.EndHorizontal();
                return;
            }
            richText.fontSize = 14;
            richText.normal.textColor = new Color(100, 100, 100);
            var ids = id.Split('@');
            var windowPos = window.position;
            var buttonWidth = windowPos.width - 100;
            scorllPos = GUILayout.BeginScrollView(scorllPos, false, false);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            bool imported = false;
            bool needUpdate = true;
            var haveMoreVersion = false;
            if (KitInitializeEditor.KitOriginConfig.OriginPackageDic != null &&
                KitInitializeEditor.KitOriginConfig.OriginPackageDic.ContainsKey(ids[0]) &&
                KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]] != null &&
                KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]].Count > 1)
                haveMoreVersion = true;
            var opcs = new List<KitOriginPackageConfig>();
            KitOriginPackageConfig updateOpc = null;
            if (haveMoreVersion)
            {
                var versions = KitInitializeEditor.KitOriginConfig.OriginPackageDic[ids[0]];
                updateOpc = KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[versions[0]];
                if (updateOpc.ID == id ||
                    (updateOpc.KitPackageConfig != null && System.IO.Directory.Exists(updateOpc.KitPackageConfig.ImportRootDirectory)))
                    needUpdate = false;
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
                        if (dinfo.Name == id)
                            imported = true;
                        importNames.Add(dinfo.Name);
                    }
                }
            }
            KitOriginPackageConfig originPackageConfig = null;
            if (KitInitializeEditor.KitOriginConfig.OriginPackageConfigList != null &&
                KitInitializeEditor.KitOriginConfig.OriginPackageConfigList.Count > 1)
                foreach (var opc in KitInitializeEditor.KitOriginConfig.OriginPackageConfigList)
                    if (opc.ID == id)
                    {
                        originPackageConfig = opc;
                        break;
                    }
            if (!imported && GUILayout.Button("����", GUILayout.Width(55), GUILayout.Height(23)))
            {
                KitPackageManagerEditorWindow.importKKPFile(originPackageConfig, "����");
            }
            if (imported)
            {
                GUI.enabled = false;
                GUILayout.Button("�ѵ���", GUILayout.Width(60), GUILayout.Height(23));
                GUI.enabled = true;
            }
            if (!imported || !needUpdate) GUI.enabled = false;
            if (GUILayout.Button("����", GUILayout.Width(55), GUILayout.Height(23)))
            {
                KitPackageManagerEditorWindow.updateKKP(updateOpc, opcs);
            }
            GUI.enabled = true;
            if (!imported) GUI.enabled = false;
            if (GUILayout.Button("ж��", GUILayout.Width(55), GUILayout.Height(23)))
            {
                KitPackageManagerEditorWindow.uninstal(originPackageConfig);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.Button("", GUILayout.Height(1));
            GUILayout.Space(3);
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("ID: ", EditorStyles.boldLabel, GUILayout.Width(30));
            if(GUILayout.Button(config.ID, richText, GUILayout.Width(buttonWidth)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("�����ı�"), false, () => {
                    UnityEngine.GUIUtility.systemCopyBuffer = config.ID;
                });
                menu.ShowAsContext();
            }
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("�汾: ", EditorStyles.boldLabel, GUILayout.Width(30));
            GUILayout.Label(config.Version);
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("����: ", EditorStyles.boldLabel, GUILayout.Width(30));
            if (GUILayout.Button(config.Author, richText, GUILayout.Width(buttonWidth)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("�����ı�"), false, () => {
                    UnityEngine.GUIUtility.systemCopyBuffer = config.Author;
                });
                menu.ShowAsContext();
            }
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("����: ", EditorStyles.boldLabel, GUILayout.Width(30));
            GUILayout.Label(config.Date);
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("��ҳ: ", EditorStyles.boldLabel, GUILayout.Width(30));
            if (GUILayout.Button(config.HomePage, richText, GUILayout.Width(buttonWidth)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("�����ı�"), false, () => {
                    UnityEngine.GUIUtility.systemCopyBuffer = config.HomePage;
                });
                menu.AddItem(new GUIContent("ȥ�������"), false, () =>
                {
                    System.Diagnostics.Process.Start(config.HomePage);
                });
                menu.ShowAsContext();
            }
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("��ϵ: ", EditorStyles.boldLabel, GUILayout.Width(30));
            if (GUILayout.Button(config.Contact, richText, GUILayout.Width(buttonWidth)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("�����ı�"), false, () => {
                    UnityEngine.GUIUtility.systemCopyBuffer = config.Contact;
                });
                menu.ShowAsContext();
            }
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("��ǩ: ", EditorStyles.boldLabel, GUILayout.Width(30));
            var eachMaxLength = windowPos.width - 40;
            GUILayout.BeginVertical();
            GUI.enabled = false;
            var _i = 0;
            var each_length = 0;
            for (; _i < config.Tags.Count;)
            {
                each_length = 0;
                GUILayout.BeginHorizontal();
                for (; each_length < eachMaxLength && _i < config.Tags.Count; _i++)
                {
                    var tag = config.Tags[_i];
                    var w = KitPackageManagerEditorWindow.getButtonWidth(tag);
                    each_length += w + 10;
                    if(each_length >= eachMaxLength)
                    {
                        _i--;
                        break;
                    }
                    GUILayout.Button(tag, GUILayout.Width(w));
                }
                GUILayout.EndHorizontal();
            }
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

            if (config.Dependencies != null && config.Dependencies.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(spaceCount);
                GUILayout.Label("������", EditorStyles.boldLabel, GUILayout.Width(30));
                GUILayout.BeginVertical();
                GUI.enabled = false;
                for (var l = 0; l < config.Dependencies.Count; l++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(config.Dependencies[l]);
                    GUILayout.EndHorizontal();
                }
                GUI.enabled = true;
                GUILayout.EndVertical();
                GUILayout.Space(spaceCount);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("������", EditorStyles.boldLabel, GUILayout.Width(30));
            var maxLineLength = 40;
            EditorGUILayout.BeginVertical();
            var lines = config.Description.Split('\n');
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
            for (var i = 0; i < tempLines.Count; i++)
            {
                if (GUILayout.Button(tempLines[i], richText))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("�����ı�"), false, () =>
                    {
                        UnityEngine.GUIUtility.systemCopyBuffer = config.Description;
                    });
                    menu.ShowAsContext();
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(spaceCount);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);

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
                    GUILayout.Space(spaceCount);
                    GUILayout.Label("���а汾��", EditorStyles.boldLabel, GUILayout.Width(60));

                    EditorGUILayout.BeginVertical();

                    _i = 0;
                    for (; _i < _versions.Count; _i++)
                    {
                        var versionID = KitInitializeEditor.KitOriginConfig.PackageList[_versions[_i]];
                        var version = versionID.Split('@')[1];
                        var w = GUILayout.Width(KitPackageManagerEditorWindow.getButtonWidth(version));
                        GUILayout.BeginHorizontal();
                        GUILayout.Button(version, richText, w);
                        var versionPath = System.IO.Path.Combine(KitConst.KitInstallationDirectory, System.IO.Path.Combine(KitConst.KitPackagesImportRootDirectory, versionID));
                        var versionImported = System.IO.Directory.Exists(versionPath);
                        if (versionImported)
                        {
                            GUI.enabled = false;
                            GUILayout.Button("�ѵ���", GUILayout.Width(45), GUILayout.Height(23));
                            GUI.enabled = true;
                        }
                        else if (GUILayout.Button("����", GUILayout.Width(45), GUILayout.Height(23)))
                        {
                            KitPackageManagerEditorWindow.importKKPFile(KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[_versions[_i]], "����");
                        }
                        GUI.enabled = versionImported;
                        if (GUILayout.Button("ж��", GUILayout.Width(45), GUILayout.Height(23)))
                        {
                            KitPackageManagerEditorWindow.uninstal(KitInitializeEditor.KitOriginConfig.OriginPackageConfigList[_versions[_i]]);
                        }
                        GUI.enabled = true;
                        if (GUILayout.Button("�鿴", GUILayout.Width(45), GUILayout.Height(23)))
                        {
                            KitPackageManagerEditorWindow.lookOriginPackageConfigInfo(KitInitializeEditor.KitOriginConfig.PackageList[_versions[_i]]);
                        }
                        GUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(spaceCount);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }
            }

            GUILayout.EndScrollView();
            GUILayout.Space(30);
        }
    }
}
