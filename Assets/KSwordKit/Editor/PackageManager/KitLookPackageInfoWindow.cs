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
        public static void Open(string subtitle)
        {
            var windowTitle = KitConst.KitName + "：" + subtitle;
            window = GetWindow<KitLookPackageInfoWindow>(true, windowTitle);
            window.minSize = new Vector2(400, 600);
        }
    }
}
