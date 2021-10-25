using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplesXLuaTest : MonoBehaviour
{

    void Start()
    {
        XLua.LuaEnv luaenv = new XLua.LuaEnv();
        luaenv.DoString("CS.UnityEngine.Debug.Log('hello, world')");
        luaenv.Dispose();
    }


    void Update()
    {
        
    }
}
