/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: ExamplesSingletonTemplateTest2.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplesSingletonTemplateTest2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ExamplesSingletonTemplateTest.Instance.Log();

        Debug.Log("1 + 2 = "+ SingletonTest2.Instance.Add(1,2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
