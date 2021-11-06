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
    /// ������Ϣ��
    /// </summary>
    public class SceneInfo : UnityEngine.Object
    {
        /// <summary>
        /// ��������
        /// </summary>
        public string SceneName { get; internal set; }
        /// <summary>
        /// ������Դ����·��
        /// </summary>
        public string SceneAssetPath { get; internal set; }
        /// <summary>
        /// �첽��������
        /// </summary>
        public AsyncOperation AsyncOperation;
    }

}
