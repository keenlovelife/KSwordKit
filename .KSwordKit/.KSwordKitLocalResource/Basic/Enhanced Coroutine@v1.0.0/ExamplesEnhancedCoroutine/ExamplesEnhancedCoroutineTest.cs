/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: ExamplesLoopTest.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamplesEnhancedCoroutineTest : MonoBehaviour
{
    [Header("UI元素")]
    public Text Text;
    public Image ProgressImage;
    public Text ProgressText;

    void Start()
    {
        // 常见的如下使用 for、while 和 do...while:
        for (var for_i = 0; for_i < 3; for_i++)
        {
            Debug.Log("这是 for 第" + for_i + "次循环。");
        }

        var while_i = 0;
        while(while_i < 3)
        {
            Debug.Log("这是 while 第" + while_i + "次循环。");
            while_i++;
        }

        var do__while_i = 0;
        do
        {
            Debug.Log("这是 do...while 第" + do__while_i + "次循环。");
            do__while_i++;

        } while (do__while_i < 3);


        // 每帧while循环一次
        var i = 0;
        KSwordKit.Core.EnhancedCoroutine.Loop.While(()=> i < 3, ()=>{
            Debug.Log("这是协程while 第"+ i +"次循环(每帧)。");
            i++;
        });

        // 每一秒执行一次while循环
        var j = 0;
        KSwordKit.Core.EnhancedCoroutine.Loop.While(()=> j < 3, ()=>{
            Debug.Log("这是 协程while 第" + j + "次While 循环(每秒)。");
            j++;
        }, 1);

        // 每帧执行一次 do while 循环
        var k = 0;
        KSwordKit.Core.EnhancedCoroutine.Loop.Do___While(()=>{
            Debug.Log("这是 协程do while 第" + k +"次循环(每帧)。");
            k++;
        }, ()=> k < 3);

        // 每秒执行一次 do while 循环
        var t = 0;
        KSwordKit.Core.EnhancedCoroutine.Loop.Do___While(()=>{
            Debug.Log("这是 协程do while 第" + t + "次循环(每秒)。");
            t++;
        }, ()=> t < 3, 1);

        // 每帧执行一次 for 循环
        var h = 0;
        KSwordKit.Core.EnhancedCoroutine.Loop.For(() => h = 0, ()=> h < 3, () => {
            Debug.Log("这是 协程for 第" + h + "次循环(每帧)。");
        }, ()=> h++);

        // 每秒执行一次 for 循环
        var p = 0;
        KSwordKit.Core.EnhancedCoroutine.Loop.For(() => p = 0, () => p < 3, () => {
            Debug.Log("这是 协程for 第" + p + "次循环(每秒)。");
        }, () => p++, 1);

        // Frame 类的应用场景
        bool isdone = false;
        string text = null;
        string error = null;
        var thread = new System.Threading.Thread(() => {
            Debug.Log("在子线程读取文本，ThreadId = " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            try
            {
                text = System.IO.File.ReadAllText("Assets/Examples/ExamplesLoop/ExamplesEnhancedCoroutineTest.cs");
                isdone = true;
            }catch(System.Exception e)
            {
                error = e.Message;
                isdone = true;
            }
        });
        thread.Start();
        KSwordKit.Core.EnhancedCoroutine.Frame.WaitWhile(() => !isdone, () => {
            Debug.Log("在UI线程中查看读取结果，ThreadId = " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            if (string.IsNullOrEmpty(error))
                Text.text = text;
            else
                Text.text = error;
        });

        float progress = 0;
        float max = 10;
        KSwordKit.Core.EnhancedCoroutine.Loop.For(() => progress = 0, () => progress < 11, ()=> {
            var maxwidth = ProgressImage.transform.parent.GetComponent<RectTransform>().rect.width;
            ProgressImage.rectTransform.sizeDelta = new Vector2(progress / max * maxwidth, ProgressImage.rectTransform.rect.height);
            ProgressText.text = ((progress / max) * 100).ToString("f2") + "%";
            if (progress == 10)
            {
                KSwordKit.Core.EnhancedCoroutine.Frame.WaitTime(1, () => {
                    ProgressText.text = "UI元素：进度条 100%，已经渲染完成！";
                    KSwordKit.Core.EnhancedCoroutine.Frame.WaitTime(new System.TimeSpan(0, 0, 0, 0, 2500), () => {
                        ProgressText.text = "done!";
                    });
                });
            }
        
        },()=> progress++);
    }
}
