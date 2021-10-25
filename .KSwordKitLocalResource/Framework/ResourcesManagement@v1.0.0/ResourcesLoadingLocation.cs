/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: Resource.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-10
 *  File Description: Ignore.
 *************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit.Contents.ResourcesManagement
{
    /// <summary>
    /// 关于资源加载方式的枚举
    /// </summary>
    public enum ResourcesLoadingLocation
    {
        /// <summary>
        /// 本地资源，通过使用 Unity 默认的 Resources.Load 加载
        /// <para>这种加载方式，能够在开发阶段快速调试查看结果。</para>
        /// </summary>
        Resources,
        /// <summary>
        /// 本地资源，位于 StreamingAssets 中，通过 UnityWebRequest 加载
        /// <para>这种方式有利于测试版本开发。</para>
        /// </summary>
        StreamingAssetsPath,
        /// <summary>
        /// 本地资源的持久化位置
        /// <para>一般由 Remote 下载完好后，写入 PersistentPath 中，然后，在进行加载。</para>
        /// <para>适用于发布版</para>
        /// </summary>
        PersistentDataPath,
        /// <summary>
        /// 远程资源，使用前，需要通过 UnityWebRequest 网络下载到本地，然后再加载
        /// <para>经过 Resources、StreamingAssets的不断地调试测试，再由Remote进行测试之后，发布版本基本没问题了。</para>
        /// <para>这种方式用于发布版。</para>
        /// </summary>
        RemotePath
    }
}

