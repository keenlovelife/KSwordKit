/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: ResourceObject.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-15
 *  File Description: Ignore.
 *************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KSwordKit.Contents.ResourcesManagement
{
    /// <summary>
    /// 一项资源的数据结构
    /// </summary>
    [Serializable]
    public class ResourceObject
    {
        /// <summary>
        /// 是否是场景资源
        /// </summary>
        public bool IsScene = false;
        /// <summary>
        /// AssetBundle名称
        /// <para>指示该资源位于哪个资源包内</para>
        /// </summary>
        public string AssetBundleName = null;
        /// <summary>
        /// 资源路径
        /// <para>该资源在项目中的相对路径</para>
        /// </summary>
        public string ResourcePath = null;
        /// <summary>
        /// 资源的文件扩张名
        /// <para>不同类型的资源加载方式不同</para>
        /// </summary>
        public string FileExtensionName = null;
        /// <summary>
        /// 对象名称
        /// <para>用于在AssetBundle中加载资源，在使用诸如 ab.LoadAsset 时当做参数.</para>
        /// <para>当<see cref="IsScene"/> 为true时，该名字为场景名称，可用于方法 <see cref="UnityEngine.SceneManagement.SceneManager.LoadScene(string)"/>中作为参数，加载场景。</para>
        /// </summary>
        public string ObjectName = null;
        /// <summary>
        /// 资源对象
        /// </summary>
        [NonSerialized]
        public UnityEngine.Object Object = null;
        [NonSerialized]
        bool asyncLoadAbr_isdone;
        [NonSerialized]
        string asyncLoad_error;
        [NonSerialized]
        bool asyncloaded;
        SceneInfo SceneInfo
        {
            get
            {
                if (_SceneInfo == null)
                {
                    _SceneInfo = new SceneInfo();
                    _SceneInfo.SceneAssetPath = ResourcePath;
                    _SceneInfo.SceneName = ObjectName;
                }
                return _SceneInfo;
            }
        }
        [NonSerialized]
        SceneInfo _SceneInfo;
        event System.Action<bool, float, string, UnityEngine.Object> asyncLoadAbrEvent;
        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path">输入的路径</param>
        /// <param name="assetBundle">资源包</param>
        /// <param name="asyncAction">回调动作</param>
        /// <returns>协程</returns>
        public IEnumerator AsyncLoad(string path, AssetBundle assetBundle, System.Action<bool, float, string, UnityEngine.Object> asyncAction)
        {
            // 如果该资源已经加载过了，程序返回该资源的加载情况
            if (asyncloaded)
            {
                if (asyncLoadAbr_isdone)
                {
                    asyncAction(false, 1, null, null);
                    if (!string.IsNullOrEmpty(asyncLoad_error))
                        ResourcesManager.NextFrame(() => asyncAction(asyncLoadAbr_isdone, 1, asyncLoad_error, null));
                    else
                        ResourcesManager.NextFrame(() => asyncAction(asyncLoadAbr_isdone, 1, null, Object));
                }
                else
                    asyncLoadAbrEvent += asyncAction;
                yield break;
            }

            // 标记该资源已被加载
            asyncloaded = true;
            asyncLoad_error = null;
            // 设置回调函数
            asyncLoadAbrEvent += asyncAction;
            // 如果是场景资源返回错误
            if (IsScene)
            {
                asyncLoadAbrEvent(false, 1, null, null);
                yield return null;
                asyncLoad_error = ResourcesManager.KSwordKitName + ": 资源加载失败! 该资源是场景资源，无法加载! 请使用请检查参数 assetPath 是否正确, assetPath=" + path;
                asyncLoadAbrEvent(true, 1, asyncLoad_error, null);
                asyncLoadAbrEvent -= asyncAction;

                asyncLoadAbr_isdone = true;
                yield break;
            }

            // 开始加载
            var assetBundleRequest = assetBundle.LoadAssetAsync(ObjectName);
            while (!assetBundleRequest.isDone)
            {
                asyncLoadAbrEvent(false, assetBundleRequest.progress, null, null);
                yield return null;
            }
            // 加载完成后更新进度信息
            if (assetBundleRequest.progress != 1)
            {
                asyncLoadAbrEvent(false, 1, null, null);
                yield return null;
            }
            // 检查加载完成情况
            if (assetBundleRequest.asset == null)
            {
                asyncLoad_error = ResourcesManager.KSwordKitName + ": 资源加载失败! 请检查参数 assetPath 是否正确, assetPath=" + path;
                asyncLoadAbrEvent(true, 1, asyncLoad_error, null);
            }
            else
            {
                try
                {
                    if (assetBundleRequest.asset != null)
                    {
                        // 资源加载成功后，存入资源缓存中
                        Object = assetBundleRequest.asset;
                        asyncLoadAbrEvent(true, 1, null, Object);
                    }
                    else
                    {
                        asyncLoad_error = ResourcesManager.KSwordKitName + ": 资源加载失败! 请检查参数 assetPath 是否正确, assetPath=" + path;
                        asyncLoadAbrEvent(true, 1, asyncLoad_error, null);
                    }
                }
                catch (System.Exception e)
                {
                    asyncLoad_error = ResourcesManager.KSwordKitName + ": 资源加载失败! 请检查参数 assetPath 是否正确, assetPath=" + path;
                    asyncLoadAbrEvent(true, 1, asyncLoad_error, null);
                }
            }
            asyncLoadAbrEvent -= asyncAction;
            asyncLoadAbr_isdone = true;
        }
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="path">输入的路径</param>
        /// <param name="sceneRequestFunc">执行可以获取异步加载场景异步请求对象的回调函数，输入的参数是场景的名称。查看<see cref="UnityEngine.SceneManagement.SceneManager"/>相关异步加载场景的更多API</param>
        /// <param name="asyncAction">回调动作</param>
        /// <returns></returns>
        public IEnumerator AsyncLoadScene(string path, Func<string, AsyncOperation> sceneRequestFunc, System.Action<bool, float, string, SceneInfo> asyncAction)
        {
            string error = null;

            // 如果不是场景资源返回错误
            if (!IsScene)
            {
                asyncAction(false, 1, null, SceneInfo);
                yield return null;
                error = ResourcesManager.KSwordKitName + ": 资源加载失败! 该资源不是场景资源，无法加载! 请使用 `AsyncLoad` 重新尝试加载! 请检查参数 assetPath 是否正确, assetPath=" + path;
                asyncAction(true, 1, error, SceneInfo);
                yield break;
            }
            // 开始尝试加载
            AsyncOperation sceneRequest = null;
            try
            {
                sceneRequest = sceneRequestFunc(SceneInfo.SceneName);
                SceneInfo.AsyncOperation = sceneRequest;
            }
            catch(System.Exception e)
            {
                error = ResourcesManager.KSwordKitName + ": 资源加载失败! 执行参数 `sceneRequestFunc()` 获得场景异步请求操作对象失败， " + e.Message + "\n请使用请检查参数 assetPath 是否正确, assetPath=" + path;
            }
            // 更新进度和场景信息数据
            asyncAction(false, 0, null, SceneInfo);
            yield return null;
            // 检查加载情况
            if (string.IsNullOrEmpty(error))
            {
                yield return asyncLoadScene(sceneRequest, (isdone, progress) => {
                    if (isdone)
                    {
                        asyncAction(true, progress, null, SceneInfo);
                        return;
                    }

                    if (progress == 0)
                        return;
                    asyncAction(false, progress, null, SceneInfo);
                });
            }
            else
            {
                asyncAction(false, 1, null, SceneInfo);
                yield return null;
                asyncAction(true, 1, error, SceneInfo);
            }
        }
        IEnumerator asyncLoadScene(AsyncOperation sceneRequest, System.Action<bool,float> asyncAction)
        {
            while (!sceneRequest.isDone)
            {
                asyncAction(false, sceneRequest.progress);
                yield return null;
            }
            // 加载完成后更新进度信息
            if (sceneRequest.progress != 1)
            {
                asyncAction(false, 1);
                yield return null;
            }
            asyncAction(true, 1);
        }
    }
}
