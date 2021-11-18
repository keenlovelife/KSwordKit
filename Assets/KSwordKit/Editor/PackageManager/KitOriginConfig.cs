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
        public string LatestVersion;
        public string LatestVersionFileName;
        public string LatestVersionURL;
        public int PackageCount;
        public List<string> PackageList;

        [NonSerialized]
        public List<KitOriginPackageConfig> OriginPackageConfigList;
        [NonSerialized]
        public Dictionary<string, List<int>> OriginPackageDic;
    }

    public class KitOriginPackageConfig
    {
        public bool selected;
        public string ID;
        public string kkpurl;
        public string kkpfilepath;
        public string configurl;
        public string configfilepath;
        public KitPackageConfig KitPackageConfig;
    }

}
#endif