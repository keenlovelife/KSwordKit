using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit
{
    /// <summary>
    /// KSwordKit中的所有常量
    /// <para>但不包括其他包内定义的常量，其他包内使用的常量由包自己定义和使用。</para>
    /// <para>这里的常量【请勿修改】，以防止程序出错。</para>
    /// </summary>
    public class KitConst
    {
        /// <summary>
        /// KSwordKit名字
        /// </summary>
        public const string KitName = "KSwordKit";
        public const string KitVersion = "v1.0.0";
        /// <summary>
        /// KSwordKit安装根目录
        /// <para>此目录指示了KSwordKit的被用户安装后，所在的根目录，也是KSwordKit的工作目录</para>
        /// <para>KSwordKit内导入的所有包也会出现在该目录内，参与项目程序的最终编译。</para>
        /// </summary>
        public static string KitInstallationDirectory
        {
            get
            {
                if (config == null)
                    config = Resources.Load<KSwordKitConfig>("KSwordKitConfig");
                if (config == null) return null;
                return config.KitInstallationPath;
            }
        }
        static KSwordKit.KSwordKitConfig config;
        /// <summary>
        /// KSwordKit已导入项目中的所有包所在的根目录
        /// <para>此目录指示了在点击 `导入包` 时，该包将会被导入到该目录中。</para>
        /// </summary>
        public const string KitPackagesImportRootDirectory = "Packages";
        /// <summary>
        /// 包内配置文件名
        /// </summary>
        public const string KitPackageConfigFilename = "kitPackageConfig.json";
        /// <summary>
        /// 检查更新时，从源中获取的包配置文件名
        /// </summary>
        public const string KitOriginPackageConfigFilename = ".KitConfig.json";
        /// <summary>
        /// KSwordKit可用的所有的包所在的根目录
        /// <para>此目录指示了在用户执行 `导入包`时, 将此处存取数据。</para>
        /// <para>该目录在当前Unity项目根目录下，不会出现在 `Assets` 目录中，避免被Unity编辑器编译。</para>
        /// </summary>
        public const string KitPackagesRootDirectory = ".KSwordKit/Packages";
        /// <summary>
        /// KSwordKit检查更新地址
        /// </summary>
        public const string KitCheckForUpdates = "https://gitee.com/keenlovelife/KSwordKit/raw/main/.KitConfig.json";
        /// <summary>
        /// KSwordKit release包地址
        /// </summary>
        public const string KitReleaseURL = "https://github.com/keenlovelife/KSwordKit/releases";
        /// <summary>
        /// KSwordKit 包目录的源请求路径
        /// </summary>
        public const string KitOriginPackagesURL = "https://gitee.com/keenlovelife/KSwordKit/raw/main/.KSwordKit/kkp";
    }
}
