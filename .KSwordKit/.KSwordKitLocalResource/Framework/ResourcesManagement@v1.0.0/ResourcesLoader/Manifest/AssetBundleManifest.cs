/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: AssetBundleManifest.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-17
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KSwordKit.Contents.ResourcesManagement
{
    /// <summary>
    /// 一组资源包的数据结构
    /// <para>主包，内部记录了所有资源包。</para>
    /// <para>该文件的数据在打包时自动由 Unity 产生。</para>
    /// </summary>
    [Serializable]
    public class AssetBundleManifest
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string ResourceVersion = "1.0.0.0";
        /// <summary>
        /// 编译号
        /// </summary>
        public string ResourceBuildVersion = "1";
        /// <summary>
        /// .manifest 文件解析出来的版本号
        /// </summary>
        public string ManifestFileVersion = null;
        /// <summary>
        /// CRC校验码
        /// </summary>
        public string CRC = null;
        /// <summary>
        /// 资源包的名字
        /// <para>程序依据该值加载AssetBundle</para>
        /// </summary>
        public string AssetBundleName = null;
        /// <summary>
        /// 资源包的相对路径
        /// </summary>
        public string AssetBundlePath = null;
        /// <summary>
        /// 一组资源包
        /// <para>查看<see cref="ResourceManifest"/></para>
        /// </summary>
        public List<ResourceManifest> AssetBundleInfos;
    }
}
