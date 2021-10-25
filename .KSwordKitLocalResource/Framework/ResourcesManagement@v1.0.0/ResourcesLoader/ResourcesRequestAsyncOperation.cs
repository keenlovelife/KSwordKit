/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: IResourcesRequest.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-10
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Playables;

namespace KSwordKit.Contents.ResourcesManagement
{
    interface IResourcesRequestAsyncOperation
    {
        IEnumerator AsyncByResources(ResourceRequest resourceRequest, System.Action<ResourcesRequestAsyncOperation> asyncAction);
        IEnumerator AsyncByAssetBundle(UnityEngine.Networking.UnityWebRequest unityWebRequest, System.Action<ResourcesRequestAsyncOperation> asyncAction);
    }

    public class ResourcesRequestAsyncOperation: IResourcesRequestAsyncOperation
    {
        const string KSwordKitName = "KSwordKit";
        internal string _resourcePath;
        /// <summary>
        /// 加载的资源路径
        /// </summary>
        public string resourcePath { get { return _resourcePath; } }
        internal bool _isdone;
        /// <summary>
        /// 指示异步操作是否完成
        /// </summary>
        public bool isDone { get { return _isdone; } }
        internal float _progress;
        /// <summary>
        /// 指示异步操作的进度
        /// </summary>
        public float progress { get { return _progress; } }
        internal bool _allowSceneActivation;
        /// <summary>
        /// 当加载的资源是场景时，指示是否在场景加载完毕后激活场景
        /// </summary>
        public bool allowSceneActivation { 
            get { return _allowSceneActivation; }
            set
            {
                _allowSceneActivation = value;
                if(_allowSceneActivation && 
                    isDone && 
                    isStreamedSceneAssetBundle && 
                    !string.IsNullOrEmpty(SceneName))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
                }
            }
        }
        internal string _error = null;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string error { get { return _error; } }
        internal ulong _bytes;
        /// <summary>
        /// 返回字节长度
        /// </summary>
        public ulong bytes { get { return _bytes; } }
        internal byte[] _data = null;
        /// <summary>
        /// 二进制数据
        /// </summary>
        public byte[] data { 
            get 
            { 
                if(_data == null)
                {
                    _data = _unityWebRequest.downloadHandler.data;
                }
                return _data; 
            }
        }
        internal string _text = null;
        /// <summary>
        /// 文本数据
        /// </summary>
        public string text 
        { 
            get 
            { 
                if(_text == null)
                {
                    _text = UnityEngine.Networking.DownloadHandlerBuffer.GetContent(_unityWebRequest);
                }
                return _text; 
            } 
        }
        internal UnityEngine.Object _asset;
        /// <summary>
        /// 加载到的资源
        /// </summary>
        public UnityEngine.Object asset
        {
            get
            {
                if(_asset == null)
                {
                    if (assetBundle == null)
                        if (audioClip == null)
                            if (texture == null)
                                return null;
                }
                return _asset;
            }
        }
        internal UnityEngine.AssetBundle _assetBundle;
        /// <summary>
        /// AssetBundle资源包
        /// </summary>
        public UnityEngine.AssetBundle assetBundle {  
            get 
            {
                if(_assetBundle == null)
                {
                    var ab = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(_unityWebRequest);
                    _asset = ab;
                    _assetBundle = ab;
                }
                return _assetBundle; 
            }
        }
        internal UnityEngine.AudioClip _audioClip;
        /// <summary>
        /// 音乐数据
        /// </summary>
        public UnityEngine.AudioClip audioClip { 
            get {
                if(_audioClip == null)
                {
                    var ac = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(_unityWebRequest);
                    _asset = ac;
                    _audioClip = ac;
                }
                return _audioClip; 
            } 
        }
        internal Texture2D _texture;
        public Texture2D texture { 
            get {
                if (_texture == null)
                {
                    var t = UnityEngine.Networking.DownloadHandlerTexture.GetContent(_unityWebRequest);
                    _asset = t;
                    _texture = t;
                }
                return _texture; 
            } 
        }
        internal bool _isStreamedSceneAssetBundle;
        public bool isStreamedSceneAssetBundle{ 
            get 
            {
                if (assetBundle != null)
                    _isStreamedSceneAssetBundle = assetBundle.isStreamedSceneAssetBundle;
                return _isStreamedSceneAssetBundle; 
            } 
        }
        internal string _sceneName = null;
        /// <summary>
        /// 场景名称 
        /// </summary>
        public string SceneName 
        { 
            get 
            {
                if (isStreamedSceneAssetBundle)
                    _sceneName = System.IO.Path.GetFileNameWithoutExtension(resourcePath);
                return _sceneName; 
            } 
        }
        /// <summary>
        /// 通过 <see cref="Resources.LoadAsync(string)"/> 的方式异步加载资源
        /// </summary>
        /// <param name="resourceRequest"><see cref="Resources.LoadAsync(string)"/>返回值</param>
        /// <param name="asyncAction">异步回调动作</param>
        /// <returns>协程</returns>
        public IEnumerator AsyncByResources(ResourceRequest resourceRequest, System.Action<ResourcesRequestAsyncOperation> asyncAction)
        {      
            while (!resourceRequest.isDone)
            {
                _isdone = resourceRequest.isDone;
                _progress = resourceRequest.progress;
                asyncAction(this);
                yield return null;
            }

            _isdone = resourceRequest.isDone;
            _progress = 1;
            _asset = resourceRequest.asset;
         
            if (resourceRequest.asset == null)
                _error = KSwordKitName + ": 加载失败或者文件不存在! 请检查参数 assetPath 是否正确, assetPath=" + resourcePath;

            asyncAction(this);
        }
        internal UnityEngine.Networking.UnityWebRequest _unityWebRequest;
        public UnityEngine.Networking.UnityWebRequest UnityWebRequest { get { return _unityWebRequest; } }
        /// <summary>
        /// 通过 <see cref="UnityEngine.Networking.UnityWebRequest"/> 的方式异步加载资源
        /// </summary>
        /// <param name="unityWebRequest">unityWebRequest实例对象</param>
        /// <param name="asyncAction">异步回调动作</param>
        /// <returns>协程</returns>
        public IEnumerator AsyncByAssetBundle(UnityEngine.Networking.UnityWebRequest unityWebRequest, System.Action<ResourcesRequestAsyncOperation> asyncAction)
        {
            _unityWebRequest = unityWebRequest;
            var op = _unityWebRequest.SendWebRequest();
            while (!op.isDone)
            {
                _isdone = op.isDone;
                _progress = op.progress;
                _bytes = _unityWebRequest.downloadedBytes;

                asyncAction(this);
                yield return null;
            }
            _progress = 1;
            asyncAction(this);

            _isdone = op.isDone;
            _bytes = _unityWebRequest.downloadedBytes;
            if (string.IsNullOrEmpty(op.webRequest.error))
                _error = KSwordKitName + ": " + op.webRequest.error + " assetPath=" + resourcePath;

            asyncAction(this);
        }
        /// <summary>
        /// 析构方法
        /// </summary>
        ~ResourcesRequestAsyncOperation()
        {
            if (_unityWebRequest != null)
                _unityWebRequest.Dispose();
        }
    }
}

