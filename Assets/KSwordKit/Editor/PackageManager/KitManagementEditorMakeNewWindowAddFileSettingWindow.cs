//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//namespace KSwordKit.Editor.KitManagement
//{

//    public class KitManagementEditorMakeNewWindowAddFileSettingWindow : EditorWindow
//    {
//        static KitManagementEditorMakeNewWindowAddFileSettingWindow window;
//        static System.Action<KitConfigFileSetting> fileSettingAction;
//        static string kitUserSelectedComponentSrcPath = "";
//        static string kitUserSelectedComponentSrcPath2 = "";
//        const string newFileSettingInstallPath = "请给选中的文件指定输入一个安装位置";
//        static string kitUserInputNewFileSettingInstallPath = newFileSettingInstallPath;
//        const string kitTempFileName = "kswordkit.tempfile.addFileSetting.temp";

//        /// <summary>
//        /// 窗口打开显示函数
//        /// </summary>
//        /// <param name="fileSettingAction">窗口关闭时返回的数据</param>
//        public static void Open(string title, System.Action<KitConfigFileSetting> _fileSettingAction = null)
//        {
//            fileSettingAction = _fileSettingAction;
//            window = GetWindow<KitManagementEditorMakeNewWindowAddFileSettingWindow>(false, title);
//            var tempfilePath = System.IO.Path.Combine(Application.temporaryCachePath, kitTempFileName);
//            if (System.IO.File.Exists(tempfilePath))
//                System.IO.File.Delete(tempfilePath);
//            System.IO.File.WriteAllText(tempfilePath, title);
//            window.minSize = new Vector2(400, 200);
//            window.Show();
//        }

//        private void OnGUI()
//        {
//            if (window == null)
//            {
//                var tempfilePath = System.IO.Path.Combine(Application.temporaryCachePath, kitTempFileName);
//                if (System.IO.File.Exists(tempfilePath))
//                {
//                    var tempfilelines = System.IO.File.ReadAllLines(tempfilePath);
//                    var windowtitle = tempfilelines[0];
//                    GetWindow<KitManagementEditorMakeNewWindowAddFileSettingWindow>(false, windowtitle).Close();
//                }
//                return;
//            }

//            GUILayout.Space(10);

//            EditorGUILayout.BeginHorizontal();
//            GUILayout.Space(10);
//            EditorGUILayout.LabelField("新组件文件位置: ", GUILayout.Height(20), GUILayout.Width(100));

//            EditorGUILayout.BeginVertical();
//            var buttonName = "浏览文件夹";
//            var selected = !string.IsNullOrEmpty(kitUserSelectedComponentSrcPath);
//            if (selected)
//                buttonName = "重新选择文件夹";
//            EditorGUILayout.LabelField(kitUserSelectedComponentSrcPath, GUILayout.Height(20));
//            if (GUILayout.Button(buttonName, GUILayout.Width(120), GUILayout.Height(20)))
//            {
//                kitUserSelectedComponentSrcPath = EditorUtility.OpenFolderPanel("选择新组件文件夹", Application.dataPath, "NewComponentFolder");
//                kitUserSelectedComponentSrcPath2 = "";
//            }
//            var buttonName2 = "浏览文件";
//            var selected2 = !string.IsNullOrEmpty(kitUserSelectedComponentSrcPath2);
//            if (selected2)
//                buttonName2 = "重新选择文件夹";
//            EditorGUILayout.LabelField(kitUserSelectedComponentSrcPath2, GUILayout.Height(20));
//            if (GUILayout.Button(buttonName2, GUILayout.Width(120), GUILayout.Height(20)))
//            {
//                kitUserSelectedComponentSrcPath2 = EditorUtility.OpenFilePanel("选择新组件文件", Application.dataPath, "");
//                kitUserSelectedComponentSrcPath = "";
//            }
//            EditorGUILayout.EndVertical();
//            GUILayout.Space(10);
//            EditorGUILayout.EndHorizontal();

//            GUILayout.Space(10);

//            EditorGUILayout.BeginHorizontal(); 
//            GUILayout.Space(10);
//            kitUserInputNewFileSettingInstallPath = EditorGUILayout.TextField("设置安装位置：", kitUserInputNewFileSettingInstallPath, GUILayout.Height(20));
//            GUILayout.Space(10);
//            EditorGUILayout.EndHorizontal();

//            GUILayout.Space(30);
//            EditorGUILayout.BeginHorizontal();
//            if (GUILayout.Button("添加", GUILayout.Width(window.position.size.x - 5), GUILayout.Height(40)))
//            {
//                if (fileSettingAction!= null)
//                {
//                    var fileSetting = new KitConfigFileSetting();
//                    fileSetting.SourcePath = "";
//                    if (!string.IsNullOrEmpty(kitUserSelectedComponentSrcPath))
//                    {
//                        fileSetting.SourcePath = kitUserSelectedComponentSrcPath;
//                    }
//                    else if (!string.IsNullOrEmpty(kitUserSelectedComponentSrcPath2))
//                    {
//                        fileSetting.SourcePath = kitUserSelectedComponentSrcPath2;
//                    }
//                    if (kitUserInputNewFileSettingInstallPath == newFileSettingInstallPath)
//                        kitUserInputNewFileSettingInstallPath = "";
//                    fileSetting.DestPath = kitUserInputNewFileSettingInstallPath;

//                    if(!string.IsNullOrEmpty(fileSetting.SourcePath) && !string.IsNullOrEmpty(fileSetting.DestPath))
//                        fileSettingAction(fileSetting);
//                }
//                window.Close();
//                kitUserInputNewFileSettingInstallPath = newFileSettingInstallPath;
//            }
//            EditorGUILayout.EndHorizontal();
//            GUILayout.Space(10);
//        }
//    }
//}

