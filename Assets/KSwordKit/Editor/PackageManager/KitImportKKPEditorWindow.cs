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

        static List<KKPFilepath> kkpFilepaths = new List<KKPFilepath>();
        static List<string> assetPaths = new List<string>();
        float hspaceCount = 20;
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

            if(kkpFilepaths.Count > 0)
            {
                GUILayout.Space(10);
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
            }
        }
        void DrawKKPGUI(KKPFilepath kkpfilepath)
        {
            GUILayout.BeginHorizontal();
            kkpfilepath.selected = EditorGUILayout.Toggle(kkpfilepath.selected, GUILayout.Width(15));
            GUILayout.Label(kkpfilepath.config.ID);
            GUILayout.EndHorizontal();
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
                            kkp.FileIndexs = JsonUtility.FromJson<KitPacker.FileIndexs>(_fileIndexstext);
                            kkpFilepaths.Add(kkp);
                        }
                    }
                }
            }
        }
    }
}
