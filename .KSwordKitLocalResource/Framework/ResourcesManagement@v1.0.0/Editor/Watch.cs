/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: Watch.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-13
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace KSwordKit.Contents.ResourcesManagement.Editor
{
    public class Watch
    {
        /// <summary>
        /// 记录参数action执行时间的秒表
        /// </summary>
        /// <param name="action">要执行的任务</param>
        /// <returns>参数action执行时间的秒表对象</returns>
        public static Stopwatch Do(System.Action action)
        {
            var watch = new Stopwatch();
            watch.Start();
            if (action != null)
                action();
            watch.Stop();
            return watch;
        }
    }

}
