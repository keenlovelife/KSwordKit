/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: Loop.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: Ignore.
 *************************************************************************/
namespace KSwordKit.Core.EnhancedCoroutine
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class Loop : MonoBehaviour
    {
        const string KSwordKitName = "KSwordKit";
        const string NAME = "Loop";
        static Loop _instance;
        static Loop instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject(NAME).AddComponent<Loop>();
                return _instance;
            }
        }
        /// <summary>
        /// while 操作
        /// </summary>
        /// <param name="conditionFunc">条件判断</param>
        /// <param name="action">执行动作</param>
        /// <param name="timeIntervalOfWaitNextExecution">设置多长时间执行下一次循环，默认值表示一帧的时间</param>
        public static void While(System.Func<bool> conditionFunc, System.Action action, float timeIntervalOfWaitNextExecution = 0)
        {
            if (action == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 action 为空，Loop.While 将不会执行任何操作！");
                return;
            }
            if (conditionFunc != null && conditionFunc())
                instance.StartCoroutine(_while(conditionFunc, action, timeIntervalOfWaitNextExecution));
        }

        static IEnumerator _while(System.Func<bool> conditionFunc, System.Action action, float timeIntervalOfWaitNextExecution = 0)
        {
            if (timeIntervalOfWaitNextExecution <= 0)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSecondsRealtime(timeIntervalOfWaitNextExecution);
            action();
            if (conditionFunc())
                instance.StartCoroutine(_while(conditionFunc, action, timeIntervalOfWaitNextExecution));
        }
        /// <summary>
        /// do...While 操作
        /// </summary>
        /// <param name="doAction">执行动作</param>
        /// <param name="conditionFunc">条件判断</param>
        /// <param name="timeIntervalOfWaitNextExecution">设置多长时间执行下一次循环，默认值表示一帧的时间。</param>
        public static void Do___While(System.Action doAction, System.Func<bool> conditionFunc, float timeIntervalOfWaitNextExecution = 0)
        {
            if (doAction == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 doAction 为空，Loop.While 将不会执行任何操作！");
                return;
            }
            instance.StartCoroutine(do___while(doAction, conditionFunc, timeIntervalOfWaitNextExecution));
        }

        static IEnumerator do___while(System.Action doAction, System.Func<bool> conditionFunc, float timeIntervalOfWaitNextExecution = 0)
        {
            if (timeIntervalOfWaitNextExecution <= 0)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSecondsRealtime(timeIntervalOfWaitNextExecution);
            doAction();
            if (conditionFunc != null && conditionFunc())
                instance.StartCoroutine(do___while(doAction, conditionFunc, timeIntervalOfWaitNextExecution));
        }
        /// <summary>
        /// for 操作
        /// </summary>
        /// <param name="initAction">初始化操作，最开始执行，且只执行一次。</param>
        /// <param name="conditionFunc">条件判断，每次循环先进行判断；执行结果为true时，执行<paramref name="action"/>(),然后执行更新动作<paramref name="updateAction"/>(), 然后等待一下次条件检查；条件执行结果为false时，程序结束。</param>
        /// <param name="action">执行内容</param>
        /// <param name="updateAction">更新操作</param>
        /// <param name="timeIntervalOfWaitNextExecution">设置多长时间执行下一次循环，默认值表示一帧的时间。</param>
        public static void For(System.Action initAction, Func<bool> conditionFunc, System.Action action, System.Action updateAction, float timeIntervalOfWaitNextExecution = 0)
        {
            if (action == null)
            {
                Debug.LogWarning(KSwordKitName + ": 参数 action 为空，Loop.For 将不会执行任何操作！");
                return;
            }

            if (initAction != null)
                initAction();
            if (conditionFunc == null || !conditionFunc())
                return;

            instance.StartCoroutine(_for(initAction, conditionFunc, action, updateAction, timeIntervalOfWaitNextExecution));
        }
        static IEnumerator _for(System.Action initAction, Func<bool> conditionFunc, System.Action action, System.Action updateAction, float timeIntervalOfWaitNextExecution = 0)
        {
            if (timeIntervalOfWaitNextExecution <= 0)
                yield return new WaitForEndOfFrame();
            else
                yield return new WaitForSecondsRealtime(timeIntervalOfWaitNextExecution);
            action();
            if (updateAction != null)
                updateAction();

            if (conditionFunc != null && conditionFunc())
                instance.StartCoroutine(_for(initAction, conditionFunc, action, updateAction, timeIntervalOfWaitNextExecution));
        }
    }

}