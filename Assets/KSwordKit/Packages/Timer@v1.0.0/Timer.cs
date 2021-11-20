#define KSwordKit_HAVE_INSTALLEDPACKAGES

/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: Timer.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: 一个用于延时调用的类
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSwordKit.Packages
{
    public class Timer : MonoBehaviour
    {
        const string NAME = "Timer";
        static Timer _instance;
        static Timer instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject(NAME).AddComponent<Timer>();
                return _instance;
            }
        }

        public static void Start(float _delay, System.Action _action)
        {
            if (!instance.isActiveAndEnabled)
            {
                instance.gameObject.SetActive(true);
                instance.enabled = true;
            }
            instance.StartCoroutine(delay(_delay, _action));
        }
        static IEnumerator delay(float time, System.Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            if (action != null)
                action();
        }

        void Start()
        {

        }
        void Update()
        {

        }
    }
}
