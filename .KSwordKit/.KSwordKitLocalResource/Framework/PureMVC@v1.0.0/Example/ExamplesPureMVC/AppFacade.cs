namespace KSwordKit
{
    using PureMVC.Interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AppFacade : PureMVC.Patterns.Facade.Facade
    {
        public const string AppFacadeKey = "AppFacadeKey";
        public static string StartUP = "StartUP";
        public const string TestPanel_ClickedYES = "TestPanel_ClickedYES";

        public AppFacade(string key) : base(AppFacadeKey)
        {

        }

        /// <summary>
        /// 注册Command，建立Command与Notification之间的映射
        /// </summary>
        protected override void InitializeController()
        {
            base.InitializeController();
            RegisterCommand(StartUP, () => new StartUPCommand());
        }
        /// <summary>
        /// 注册Mediator
        /// </summary>
        protected override void InitializeView()
        {
            base.InitializeView();

        }
        /// <summary>
        /// 注册Proxy
        /// </summary>
        protected override void InitializeModel()
        {
            base.InitializeModel();

        }

        public void Launch()
        {
            SendNotification(StartUP);
        }
    }

}