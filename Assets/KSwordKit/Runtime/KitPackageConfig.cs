using System;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit
{
    [Serializable]
    public class KitPackageConfig
    {
        /// <summary>
        /// 包ID
        /// <para>ID在整个开发套件内有唯一性，否则不能导入。</para>
        /// <para>ID一般为包的名称+版本号; 例如: Enhanced Coroutine@v1.0.0</para>
        /// </summary>
        public string ID;
        /// <summary>
        /// 包名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 包版本号
        /// <para>版本号一般为类似v1.0.0的字符串，版本号请遵循语义化版本号规范:https://semver.org/lang/zh-CN/</para>
        /// </summary>
        public string Version;
        /// <summary>
        /// 包文件的MD5值
        /// </summary>
        public string MD5Value;
        /// <summary>
        /// 能否与旧版共存
        /// <para>如果值为false，则该包导入时将删除旧版本的包。</para>
        /// <para>默认为false</para>
        /// </summary>
        public bool liveWithOtherVersion;
        /// <summary>
        /// 包作者
        /// </summary>
        public string Author;
        /// <summary>
        /// 作者联系方式
        /// </summary>
        public string Contact;
        /// <summary>
        /// 作者的个人主页
        /// </summary>
        public string HomePage;
        /// <summary>
        /// 包创建日期
        /// <para>Date值一般为类似2020-02-02的字符串</para>
        /// </summary>
        public string Date;
        /// <summary>
        /// 包描述
        /// <para>介绍包的作用</para>
        /// </summary>
        public string Description;
        /// <summary>
        /// 该包所依赖的其他包列表
        /// <para>列表内容是其他包ID字符串</para>
        /// <para>根据 `独立无依赖原则`，请尽量保持该项为空。</para>
        /// </summary>
        public List<string> Dependencies;
        /// <summary>
        /// 该包的标签列表
        /// <para>比如 '热更新'、'多线程'等标签</para>
        /// </summary>
        public List<string> Tags;
        /// <summary>
        /// 该包内特殊文件设置
        /// <para>默认情况下，包导入项目中时，所有文件会导入到 `{KSwordKitRootDir}/package/{Name+Version}` 文件夹内。</para>
        /// <para>如果有些特殊文件需要在其他路径下才能正常工作，也可以使用该项单独设置。</para>
        /// </summary>
        public List<KitPackageConfigFileSetting> FileSettings;
        /// <summary>
        /// 该包导入到Unity项目后，所在的目录
        /// </summary>
        [NonSerialized]
        public string ImportRootDirectory;

    }
    [Serializable]
    public class KitPackageConfigFileSetting
    {
        /// <summary>
        /// 当包被卸载时，当前的文件是否允许同步卸载？
        /// </summary>
        public bool enableUninstall = true;
        /// <summary>
        /// 是否是文件夹
        /// </summary>
        public bool isDir;
        /// <summary>
        /// 该文件相对于包的根目录的路径
        /// </summary>
        public string SourcePath;
        /// <summary>
        /// 该文件希望导入的目标位置
        /// <para>如果 SourcePath 是文件夹，则其内所有文件将被导出到 TargetPath 的相应路径中。</para>
        /// <para>可以是相对位置（相对于项目根目录），也可以是绝对位置。</para>
        /// <para>可以包含一些特定字符串来表示实际的特定路径, 下面是所有可用的特定含义的字符串：</para>
        /// <para>Unity定义的一些特定文件目录：</para>
        /// <para>{Application.dataPath}</para>
        /// <para>{Application.temporaryCachePath}</para>
        /// <para>{Application.consoleLogPath}</para>
        /// <para>{Application.persistentDataPath}</para>
        /// <para>{Application.temporaryCachePath}</para>
        /// <para>{Application.streamingAssetsPath}</para>
        /// <para>KSwordKit定义的一些特定文件目录：</para>
        /// <para>{KSwordKitRootDir} //表示KSwordKit安装在项目中的根目录</para>
        /// <para>{PackageRootDir} // 表示当前包文件所在根目录</para>
        /// <para>{PackageRootDirWhenImport} // 表示当前包准备导入时，将被导入的根目录。</para>
        /// </summary>
        public string TargetPath;

        public static string TargetPathToRealPath(string targetPath)
        {
            if (targetPath == null) return null;
            targetPath = targetPath.Replace("{Application.dataPath}", Application.dataPath);
            targetPath = targetPath.Replace("{Application.temporaryCachePath}", Application.temporaryCachePath);
            targetPath = targetPath.Replace("{Application.consoleLogPath}", Application.consoleLogPath);
            targetPath = targetPath.Replace("{Application.persistentDataPath}", Application.persistentDataPath);
            targetPath = targetPath.Replace("{Application.temporaryCachePath}", Application.temporaryCachePath);
            targetPath = targetPath.Replace("{Application.streamingAssetsPath}", Application.streamingAssetsPath);
            targetPath = targetPath.Replace("{KSwordKitRootDir}",KitConst.KitInstallationDirectory);
            targetPath = targetPath.Replace("{PackageRootDir}", KitConst.KitPackagesRootDirectory);
            targetPath = targetPath.Replace("{PackageRootDirWhenImport}", System.IO.Path.Combine(KitConst.KitInstallationDirectory, KitConst.KitPackagesImportRootDirectory));
            return targetPath;
        }
    }
}
