/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: Frame.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-30
 *  File Description: Ignore.
 *************************************************************************/
namespace KSwordKit.Core.EnhancedCoroutine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Frame : MonoBehaviour
    {
        const string KSwordKitName = "KSwordKit";
        const string NAME = "Frame";
        static Frame _instance;
        static Frame instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject(NAME).AddComponent<Frame>();
                return _instance;
            }
        }

        /// <summary>
        /// 下一帧执行
        /// <para>下一帧时在主线程中执行 <paramref name="action"/></para>
        /// </summary>
        /// <param name="action">回调动作</param>
        public static void NextFrame(System.Action action)
        {
            if(action == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 action 为空，Frame.Next 将不会执行任何操作！");
                return;
            }
            instance.StartCoroutine(_next(action));
        }
        static IEnumerator _next(System.Action action)
        {
            yield return null;
            action();
        }
        /// <summary>
        /// 等待一段时间，然后执行某操作。
        /// </summary>
        /// <param name="timeSpan">一段时间</param>
        /// <param name="action">将要执行的操作</param>
        public static void WaitTime(System.TimeSpan timeSpan, System.Action action)
        {
            if(action == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 action 为空，Frame.Wait 将不会执行任何操作！");
                return;
            }
            instance.StartCoroutine(_wait((float)timeSpan.TotalSeconds, action));
        }
        /// <summary>
        /// 等待一段时间，然后执行某操作。
        /// </summary>
        /// <param name="seconds">一段时间</param>
        /// <param name="action">将要执行的操作</param>
        public static void WaitTime(float seconds, System.Action action)
        {
            if (action == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 action 为空，Frame.Wait 将不会执行任何操作！");
                return;
            }
            instance.StartCoroutine(_wait(seconds, action));
        }
        static IEnumerator _wait(float seconds, System.Action action)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action();
        }
        /// <summary>
        /// 等待；条件 <paramref name="conditionFunc"/>() == true，会一直等待下去；否则不在等待，执行参数 <paramref name="action"/>() ，并结束。
        /// <para>模拟 while 语法，while 的条件为 true时，继续循环，否则跳出循环；这里则是将循环变为等待，跳出循环时，则执行 <paramref name="action"/>()，用以满足特定需求。</para>
        /// <para>如果 <paramref name="conditionFunc"/>() 执行结果为 true 时，不执行任何操作，等待下一帧, 然后重复本步骤；</para>
        /// <para>如果 <paramref name="conditionFunc"/>() 执行结果为 false 时，结束等待，执行 <paramref name="action"/>()</para>
        /// <para>如果最开始时，<paramref name="conditionFunc"/>() == false，程序会直接 执行<paramref name="action"/>()</para>
        /// </summary>
        /// <param name="conditionFunc">条件函数,返回true或false</param>
        /// <param name="action">回调动作</param>
        public static void WaitWhile(Func<bool> conditionFunc, System.Action action)
        {
            if (conditionFunc == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 conditionFunc 为空，Frame.Next 将不会执行任何操作！");
                return;
            }
            if (action == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 action 为空，Frame.Next 将不会执行任何操作！");
                return;
            }
            if (conditionFunc())
                NextFrame(() => WaitWhile(conditionFunc, action));
            else
                action();
        }
    }

}
