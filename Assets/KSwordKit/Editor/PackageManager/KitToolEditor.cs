using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace KSwordKit.Editor
{
    [InitializeOnLoad]
    public class KitToolEditor
    {
        public class WebRequest
        {
            public UnityEngine.Networking.UnityWebRequest www;
            public System.Action<UnityEngine.Networking.UnityWebRequest> ResultAction;
            public System.Action<UnityEngine.Networking.UnityWebRequest> waitAction;
        }
        static List<WebRequest> WebRequestList = new List<WebRequest>();

        static List<System.Action> Actions = new List<System.Action>();
        static KitToolEditor()
        {
            EditorApplication.update += WebRequest_Update;
            EditorApplication.update += Action_update;
        }

        static void WebRequest_Update()
        {
            List<WebRequest> doneList = new List<WebRequest>();
            List<WebRequest> waitList = new List<WebRequest>();
            for(var i = 0; i < WebRequestList.Count; i++)
            {
                var webRequest = WebRequestList[i];
                if (webRequest.www.isDone)
                    doneList.Add(webRequest);
                else
                {
                    waitList.Add(webRequest);
                    webRequest.waitAction(webRequest.www);
                }
            }

            WebRequestList.Clear();
            WebRequestList.AddRange(waitList);
            waitList.Clear();
            waitList = null;

            foreach(var webRequest in doneList)
                webRequest.ResultAction(webRequest.www);
            doneList.Clear();
            doneList = null;
        }
        static void Action_update()
        {
            foreach (var action in Actions)
                action();
            Actions.Clear();
        }

        public static void AddWebRequest(WebRequest webRequest)
        {
            if(webRequest != null && 
                webRequest.www != null && 
                webRequest.ResultAction != null &&
                webRequest.waitAction != null)
            {
                webRequest.www.SendWebRequest();
                WebRequestList.Add(webRequest);
            }
        }

        public static void WaitNextFrame(System.Action task)
        {
            if (task == null) return;
            Actions.Add(task);
        }
    }
}
#endif