using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace KSwordKit.Editor.PackageManager
{
    public class KitPackageManagerEditor
    {
        public const string Packages_import_update_uninstall_Assets = "Assets/" + KitConst.KitName + "/PackageManager/Install\\Update\\uninstall";
        public const string Packages_import_update_uninstall = KitConst.KitName + "/PackageManager/Install\\Update\\uninstall _%&I";
        public const string Packages_import_update_uninstall_WindowTitle = "Install\\Update\\uninstall";

        public const string MakeNew_Assets = "Assets/" + KitConst.KitName + "/PackageManager/MakeNewPackage";
        public const string MakeNew = KitConst.KitName + "/PackageManager/MakeNewPackage _%&N";
        public const string MakeNewWindowTitle = "MakeNewPackage";

        public const string About_Assets = "Assets/" + KitConst.KitName + "/About";
        public const string AboutUs = KitConst.KitName + "/About";
        public const string AboutUsWindowTitle = "About";

        public const string CheckUpdate_Assets = "Assets/" + KitConst.KitName + "/PackageManager/CheckUpdate";
        public const string CheckUpdate = KitConst.KitName + "/PackageManager/CheckUpdate _%&M";
        public const string CheckUpdateWindowTitle = "CheckUpdate";

        [MenuItem(Packages_import_update_uninstall_Assets, false, -41)]
        [MenuItem(Packages_import_update_uninstall, false, -41)]
        public static void PackagesFunction()
        {
            KitPackageManagerEditorWindow.Open(Packages_import_update_uninstall_WindowTitle);
        }

        [MenuItem(MakeNew_Assets, false, -20)]
        [MenuItem(MakeNew, false, -20)]
        public static void MakeNewFunction()
        {
            KitPackageManagerEditorMakeNewWindow.Open(MakeNewWindowTitle);

        }

        [MenuItem(About_Assets, false, 4000000)]
        [MenuItem(AboutUs, false, 4000000)]
        public static void AboutFunction()
        {
            Application.OpenURL("https://github.com/keenlovelife");
        }

        [MenuItem(CheckUpdate_Assets, false, 0)]
        [MenuItem(CheckUpdate, false, 0)]
        public static void CheckUpdateFunction()
        {
            KitInitializeEditor.CheckUpdate(KitConst.KitName + ": " + CheckUpdateWindowTitle, true);
        }
    }
}

#endif