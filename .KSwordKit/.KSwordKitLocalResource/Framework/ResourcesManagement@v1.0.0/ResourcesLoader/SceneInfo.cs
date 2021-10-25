/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: SceneInfo.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-7-2
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit.Contents.ResourcesManagement
{
    /// <summary>
    /// 场景信息类
    /// </summary>
    public class SceneInfo : UnityEngine.Object
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName { get; internal set; }
        /// <summary>
        /// 场景资源所在路径
        /// </summary>
        public string SceneAssetPath { get; internal set; }
        /// <summary>
        /// 异步操作对象
        /// </summary>
        public AsyncOperation AsyncOperation;
    }

}
