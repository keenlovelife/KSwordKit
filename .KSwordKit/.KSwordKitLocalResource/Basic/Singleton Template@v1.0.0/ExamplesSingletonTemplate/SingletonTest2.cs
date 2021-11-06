/*************************************************************************
 *  Copyright (C), 2020-2021. All rights reserved.
 *
 *  FileName: SingletonTest2.cs
 *  Author: ks   
 *  Version: 1.0.0   
 *  CreateDate: 2020-6-7
 *  File Description: Ignore.
 *************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonTest2 : KSwordKit.Core.SingletonTemplate<SingletonTest2>
{
    public int Add(int i, int j){
        return i + j;
    }
}
