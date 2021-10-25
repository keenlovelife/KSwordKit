using PureMVC.Interfaces;
using PureMVC.Patterns.Facade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    KSwordKit.AppFacade facade;
    void Start()
    {
        facade = Facade.GetInstance(typeof(KSwordKit.AppFacade).FullName, ()=> new KSwordKit.AppFacade(typeof(KSwordKit.AppFacade).FullName)) as KSwordKit.AppFacade;
        facade.Launch();
    }

    void Update()
    {
        
    }
}
