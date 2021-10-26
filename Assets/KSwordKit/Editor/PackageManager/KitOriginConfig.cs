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
        public List<string> PackageList;

        [NonSerialized]
        public List<KitOriginPackageConfig> OriginPackageConfigList;
    }

    public class KitOriginPackageConfig
    {
        public string ID;
        public string kkpurl;
        public string kkpfilepath;
        public string configurl;
        public KitPackageConfig KitPackageConfig;
    }

}
#endif