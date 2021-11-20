using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
namespace KSwordKit.Editor.Timer
{
    public class TimerEditor
    {
        public const string InstalledPackageList_Assets = "Assets/" + KitConst.KitName + "/InstalledPackageList/Timer@v1.0.0 (priority=20)";
        public const string InstalledPackageList = KitConst.KitName + "/InstalledPackageList/Timer@v1.0.0 (priority=20)";
        public const string InstalledPackageListWindowTitle = "InstalledPackageList";

        [MenuItem(InstalledPackageList_Assets, false, 20)]
        [MenuItem(InstalledPackageList, false, 20)]
        public static void LookSelf()
        {
            
        }

    }
}
#endif