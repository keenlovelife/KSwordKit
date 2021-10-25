using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace KSwordKit.Editor.PackageManager
{
    public class KitPackageManagerEditor
    {
        public const string Packages_import_update_uninstall_Assets = "Assets/" + KitConst.KitName + "/包管理/导入 - 更新 - 卸载";
        public const string Packages_import_update_uninstall = KitConst.KitName + "/包管理/导入 - 更新 - 卸载 _%&I";
        public const string Packages_import_update_uninstall_WindowTitle = "安装 - 更新 - 卸载";

        public const string MakeNew_Assets = "Assets/" + KitConst.KitName + "/包管理/导出新包";
        public const string MakeNew = KitConst.KitName + "/包管理/导出新包 _%&N";
        public const string MakeNewWindowTitle = "导出新包";

        public const string About_Assets = "Assets/" + KitConst.KitName + "/包管理/关于作者";
        public const string AboutUs = KitConst.KitName + "/包管理/关于作者 _%&M";
        public const string AboutUsWindowTitle = "关于作者";


        [MenuItem(Packages_import_update_uninstall_Assets, false, 0)]
        [MenuItem(Packages_import_update_uninstall, false, 0)]
        public static void PackagesFunction()
        {
            KitPackageManagerEditorWindow.Open(Packages_import_update_uninstall_WindowTitle);
        }

        [MenuItem(MakeNew_Assets, false, 20)]
        [MenuItem(MakeNew, false, 20)]
        public static void MakeNewFunction()
        {
            //KitPacker.Pack(".KSwordKitLocalResource/Basic/Timer@v1.0.0", "Assets/KSwordKit/Packages/Timer@v1.0.0.package", (filename, progress)=> {
            //    EditorUtility.DisplayProgressBar("导出包：", "正在导出：" + filename, progress);
            //});

            //KitPacker.Pack(".KSwordKitLocalResource/Basic/Enhanced Coroutine@v1.0.0", "Assets/KSwordKit/Packages/Enhanced Coroutine@v1.0.0.package", (filename, progress) => {
            //    EditorUtility.DisplayProgressBar("导出包：", "正在导出：" + filename, progress);
            //});
            //EditorUtility.ClearProgressBar();
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
            KitPackageManagerEditorMakeNewWindow.Open(MakeNewWindowTitle);

        }

        [MenuItem(About_Assets, false, 40)]
        [MenuItem(AboutUs, false, 40)]
        public static void AboutFunction()
        {
            Application.OpenURL("https://github.com/keenlovelife");

        }

    }
}

#endif