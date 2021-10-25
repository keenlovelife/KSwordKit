using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
namespace KSwordKit.Editor.PackageManager
{
    /// <summary>
    /// KSwordKit‘¥≈‰÷√–≈œ¢
    /// </summary>
    [Serializable]
    public class KitOriginConfig
    {
        public string Version;
        public int PackageCount;
        public List<KitOriginPackageConfig> PackageList;
    }

    [Serializable]
    public class KitOriginPackageConfig
    {
        public string ID;
        public string Description;
        public string URL;
        public string Extension;
    }

}
#endif