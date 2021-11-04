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
        /// 窗口打开显示函数
        /// </summary>
        /// <param name="data">窗口数据</param>
        public static void Open(string id)
        {
            var windowTitle = KitConst.KitName + "：" + id;
            window = GetWindow<KitLookPackageInfoWindow>(true, windowTitle);
            window.minSize = new Vector2(400, 600);

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
        string id;
        private void OnGUI()
        {
            if (config == null)
                getConfig();

            EditorGUILayout.Space(20);

            if (config == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(spaceCount);
                var label = "配置文件不存在！";
                if (!string.IsNullOrEmpty(id))
                    label = id + "的" + label;
                GUILayout.Label("配置文件不存在！");
                GUILayout.Space(spaceCount);
                GUILayout.EndHorizontal();
                return;
            }

            scorllPos = GUILayout.BeginScrollView(scorllPos, false, false);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spaceCount);
            GUILayout.Label("ID: ", GUILayout.Width(30));
            GUILayout.Label(config.ID);
            GUILayout.Space(spaceCount);
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
            GUILayout.Space(30);
        }
    }
}
