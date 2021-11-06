using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit
{
    public class KSwordKitConfig : ScriptableObject
    {
        public string KitInstallationPath = "";
        public string KitVersion = "";
        public List<string> KitImportedPackageList;
    }
}
