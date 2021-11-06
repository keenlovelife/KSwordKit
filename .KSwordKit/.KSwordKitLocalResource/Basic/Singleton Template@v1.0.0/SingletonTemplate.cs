/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: SingletonTemplate.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: 继承自 MonoBehaviour 单例模板的通用基类
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit.Core
{
    public class MonoBehaviourSingletonTemplate<T> : MonoBehaviour where T : MonoBehaviour
    {
        static string className = typeof(T).ToString();
        public static string ClassName { get{return className;} }

        static T _instance;
        public static T Instance{
            get{
                if(_instance == null)
                    _instance = new GameObject(ClassName).AddComponent<T>();
                return _instance;
            }
        }
    }

    public abstract class SingletonTemplate<T> where T: class, new() 
    {        
        static string className = typeof(T).ToString();
        public static string ClassName { get{return className;} }
        static T _instance = new T();
        public static T Instance{ get {return _instance; }}
    }
}
