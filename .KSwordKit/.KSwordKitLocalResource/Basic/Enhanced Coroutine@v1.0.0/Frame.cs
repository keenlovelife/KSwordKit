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
        /// ��һִ֡��
        /// <para>��һ֡ʱ�����߳���ִ�� <paramref name="action"/></para>
        /// </summary>
        /// <param name="action">�ص�����</param>
        public static void NextFrame(System.Action action)
        {
            if(action == null)
            {
                Debug.LogWarning(KSwordKitName + ": ���� action Ϊ�գ�Frame.Next ������ִ���κβ�����");
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
        /// �ȴ�һ��ʱ�䣬Ȼ��ִ��ĳ������
        /// </summary>
        /// <param name="timeSpan">һ��ʱ��</param>
        /// <param name="action">��Ҫִ�еĲ���</param>
        public static void WaitTime(System.TimeSpan timeSpan, System.Action action)
        {
            if(action == null)
            {
                Debug.LogWarning(KSwordKitName + ": ���� action Ϊ�գ�Frame.Wait ������ִ���κβ�����");
                return;
            }
            instance.StartCoroutine(_wait((float)timeSpan.TotalSeconds, action));
        }
        /// <summary>
        /// �ȴ�һ��ʱ�䣬Ȼ��ִ��ĳ������
        /// </summary>
        /// <param name="seconds">һ��ʱ��</param>
        /// <param name="action">��Ҫִ�еĲ���</param>
        public static void WaitTime(float seconds, System.Action action)
        {
            if (action == null)
            {
                Debug.LogWarning(KSwordKitName + ": ���� action Ϊ�գ�Frame.Wait ������ִ���κβ�����");
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
        /// �ȴ������� <paramref name="conditionFunc"/>() == true����һֱ�ȴ���ȥ�������ڵȴ���ִ�в��� <paramref name="action"/>() ����������
        /// <para>ģ�� while �﷨��while ������Ϊ trueʱ������ѭ������������ѭ�����������ǽ�ѭ����Ϊ�ȴ�������ѭ��ʱ����ִ�� <paramref name="action"/>()�����������ض�����</para>
        /// <para>��� <paramref name="conditionFunc"/>() ִ�н��Ϊ true ʱ����ִ���κβ������ȴ���һ֡, Ȼ���ظ������裻</para>
        /// <para>��� <paramref name="conditionFunc"/>() ִ�н��Ϊ false ʱ�������ȴ���ִ�� <paramref name="action"/>()</para>
        /// <para>����ʼʱ��<paramref name="conditionFunc"/>() == false�������ֱ�� ִ��<paramref name="action"/>()</para>
        /// </summary>
        /// <param name="conditionFunc">��������,����true��false</param>
        /// <param name="action">�ص�����</param>
        public static void WaitWhile(Func<bool> conditionFunc, System.Action action)
        {
            if (conditionFunc == null)
            {
                Debug.LogWarning(KSwordKitName + ": ���� conditionFunc Ϊ�գ�Frame.Next ������ִ���κβ�����");
                return;
            }
            if (action == null)
            {
                Debug.LogWarning(KSwordKitName + ": ���� action Ϊ�գ�Frame.Next ������ִ���κβ�����");
                return;
            }
            if (conditionFunc())
                NextFrame(() => WaitWhile(conditionFunc, action));
            else
                action();
        }
    }

}
