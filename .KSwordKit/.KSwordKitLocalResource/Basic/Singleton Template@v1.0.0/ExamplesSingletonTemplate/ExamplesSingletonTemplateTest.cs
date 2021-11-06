/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: ExamplesSingletonTemplateTest.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplesSingletonTemplateTest: KSwordKit.Core.MonoBehaviourSingletonTemplate<ExamplesSingletonTemplateTest>
{
    public void Log(){
        Debug.Log(ExamplesSingletonTemplateTest.ClassName + "类是继承了单例泛型模板的类，它的对象名为："+ ExamplesSingletonTemplateTest.Instance.gameObject.name);
    }
}
