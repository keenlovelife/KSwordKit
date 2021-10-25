using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace KSwordKit
{
    public class KSwordKitConfig : ScriptableObject
    {
        public string KitInstallationPath = "";
        public string KitVersion = "";
    }
}
