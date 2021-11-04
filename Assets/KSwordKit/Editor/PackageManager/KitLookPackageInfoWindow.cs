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
        public static void Open(string subtitle)
        {
            var windowTitle = KitConst.KitName + "��" + subtitle;
            window = GetWindow<KitLookPackageInfoWindow>(true, windowTitle);
            window.minSize = new Vector2(400, 600);
        }
    }
}
