using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
namespace KSwordKit.Editor.PackageManager
{
    /// <summary>
    /// KSwordKitԴ������Ϣ
    /// </summary>
    [Serializable]
    public class KitOriginConfig
    {
        public string Version;
        public int PackageCount;
        public List<string> PackageList;
    }

}
#endif