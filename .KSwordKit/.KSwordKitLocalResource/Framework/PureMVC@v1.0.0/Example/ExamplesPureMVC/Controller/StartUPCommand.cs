using PureMVC.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUPCommand : PureMVC.Patterns.Command.SimpleCommand
{
    public override void Execute(INotification notification)
    {
        var testpanel = Resources.Load<GameObject>("testPanel");
        var tp = GameObject.Instantiate(testpanel, GameObject.Find("Canvas").transform);
        Facade.RegisterMediator(new TestPanelMediator(tp));

        Facade.RegisterMediator(new TestYesWindowMediator(Resources.Load<GameObject>("TestYesWindow")));
    }
}
