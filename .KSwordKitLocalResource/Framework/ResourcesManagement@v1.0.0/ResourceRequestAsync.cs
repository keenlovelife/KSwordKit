/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: ResourceRequest.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace KSwordKit.Contents.ResourcesManagement
{
    public class ResourceRequestAsync : MonoBehaviour
    {
        const string KitName = "KSwordKit";
        public static ResourceRequestAsync New(string _assetPath) 
        {
            var pgo = GameObject.Find("AssetLoadingObjects");
            if(pgo == null)
                pgo = new GameObject("AssetLoadingObjects");
            var o = new GameObject("loading").AddComponent<ResourceRequestAsync>();
            o.gameObject.transform.parent = pgo.transform;
            o.assetPath = _assetPath;
            return o;
        }
        public static ResourceRequestAsync New(string _assetPath, string abname, string _url)
        {
            var pgo = GameObject.Find("AssetLoadingObjects");
            if(pgo == null)
                pgo = new GameObject("AssetLoadingObjects");
            var o = new GameObject("loading: "+abname).AddComponent<ResourceRequestAsync>();
            o.gameObject.transform.parent = pgo.transform;

            o.assetPath = _assetPath;
            if(_assetPath.EndsWith(".unity"))
            {
                o.sceneName = System.IO.Path.GetFileNameWithoutExtension(o.url);
            }

            o.assetBundleName = abname;
            o.url = _url;
            return o;
        }
        public static ResourceRequestAsync New(ResourceRequestAsync requestAsync, string _assetPath)
        {
            var r = requestAsync.gameObject.AddComponent<ResourceRequestAsync>();
            if(!r.gameObject.activeSelf)
            {
                r.gameObject.SetActive(true);
            }
            r.assetBundle = requestAsync.assetBundle;
            r.assetBundleName = requestAsync.assetBundleName;
            r.assetPath = _assetPath;
            r.url = requestAsync.url;
            r.sceneName = requestAsync.sceneName;
            r.isScene = requestAsync.isScene;
            return r;
        }
        string assetBundleName;
        string assetPath = null;
        string error = null;
        float progress = 0;
        UnityEngine.Object asset = null;
        double _count = 0;
        bool isdone = false;
        string url = null;
        bool allowSceneActivation = false;
        bool isScene = false;
        string sceneName = null;
        bool isLoading = false;
        AssetBundle assetBundle = null;

        public float Progress { get{ return progress;} }
        public string Error { get {  return error; }  }
        public UnityEngine.Object  Asset {
            get
            {
                return asset;
            }
        }
        public bool isDone
        {
            get
            { return isdone; }
        }
        public string URL
        {
            get
            {
                return url;
            }
            set{
                url = value;
                if(assetPath.EndsWith(".unity"))
                {
                    sceneName = System.IO.Path.GetFileNameWithoutExtension(url);
                }
            }
        }
        public string AssetPath{get{return assetPath;}}
        public string AssetBundleName{get{return assetBundleName;}}
        public bool AllowSceneActivation
        {
            get
            {
                return allowSceneActivation;
            }
            set
            {
                allowSceneActivation = value;
                if(isScene && value)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                    UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
                }
            }
        }
        public bool IsScene {get {return isScene;}}
        public string SceneName {get{return sceneName;}}
        public bool IsLoading{get{return isLoading;}}
        public AssetBundle AssetBundle{get{return assetBundle;}}
        static string ResourcePath
        {
            get
            {
                var dir = System.IO.Path.Combine(Application.streamingAssetsPath, "AssetBundles");
                if(Application.isEditor)
                {
                    if(!System.IO.Directory.Exists(dir))
                        return string.Empty;
                    else
                    {
                        var dirs = new System.IO.DirectoryInfo(dir).GetDirectories();
                        if(dirs.Length == 0)
                            return string.Empty;
                        return dirs[0].FullName;
                    }
                }
                else
                {
                    dir = System.IO.Path.Combine(dir, Application.platform.ToString());
                    if(System.IO.Directory.Exists(dir))
                        return dir;
                    else
                        return string.Empty;
                }
            }
        } 
        public event System.Action<ResourceRequestAsync> ResultEvent;
        public event System.Action<ResourceRequestAsync> ErrorEvent;
        public event System.Action<ResourceRequestAsync> ProgressEvent;
        public event System.Action<Scene, LoadSceneMode> sceneLoaded;

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            if(sceneLoaded != null)
            {
                sceneLoaded(arg0,arg1);
            }
        }


        public void Send<T>() where T:UnityEngine.Object
        {
            if(!isLoading)
            {   
                isLoading = true;
                StartCoroutine(send<T>());
            }
        }
        IEnumerator send<T>()  where T:UnityEngine.Object
        {
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url); 

            var asyncOp = www.SendWebRequest();

            while (!asyncOp.isDone)
            {
                progress = asyncOp.progress * 0.5f;
                if(ProgressEvent != null)
                    ProgressEvent(this);
                yield return null;
            }
            
            progress = 0.5f;
            error = asyncOp.webRequest.error;

            if(ProgressEvent != null)
            {    
                ProgressEvent(this);
                yield return null;
            }

            if (string.IsNullOrEmpty(www.error))
            {
                assetBundle = DownloadHandlerAssetBundle.GetContent(www);

                if(typeof(T) == typeof(AssetBundle))
                {                            
                    if(ProgressEvent != null)
                    {    
                        ProgressEvent(this);
                        yield return null;
                    }
                    asset = assetBundle as T;
                    isScene = assetBundle.isStreamedSceneAssetBundle;
                    if(isScene && allowSceneActivation)
                    {
                        AllowSceneActivation = true;
                    }
                    
                    isLoading = false;
                    isdone = true;

                    if(ResultEvent != null)
                    {
                        ResultEvent(this);
                    }
                }
                else
                {
                    yield return LoadAssetAsync<T>(assetBundle, System.IO.Path.GetFileNameWithoutExtension(assetPath));       
                }
            }
            else
            {
                isLoading = false;
                isdone = true;
                error = KitName +": 资源加载失败! " + error + "\npath = " + assetPath + "\nurl="+URL;
                if(ErrorEvent!=null)
                {
                    ErrorEvent(this);
                }
            }

            ErrorEvent = null;
            ProgressEvent = null;
            ResultEvent = null;
        }
        
        public IEnumerator LoadAssetAsync<T>(AssetBundle ab, string objectName) where T:UnityEngine.Object
        {
            var op = ab.LoadAssetAsync<T>(objectName);
            while(!op.isDone)
            {
                progress = op.progress *0.5f + 0.5f;
                if(ProgressEvent!=null)
                {
                    ProgressEvent(this);
                }
                yield return null;
            }
            progress = 1;
            if(ProgressEvent!=null)
            {
                ProgressEvent(this);
                yield return null;
            }

            if(op.asset == null)
            {
                error = KitName +": 资源加载失败! path = " + assetPath;
                if(ErrorEvent != null)
                {
                    ErrorEvent(this);
                }

                yield break;
            }
            else
            {
                if(typeof(T) == typeof(UnityEngine.Object))
                    asset = op.asset;
                else
                    asset = op.asset as T;
            }    

            if(ResultEvent != null)
            {
                ResultEvent(this);
            }    
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }
    }

}