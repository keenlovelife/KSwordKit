using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPanelMediator : PureMVC.Patterns.Mediator.Mediator
{
    public new const string NAME = "TestPanelMediator";
    private TestPanel View;
    public TestPanelMediator(object viewCommponent):base(NAME, viewCommponent)
    {
        View = ((GameObject)ViewComponent).GetComponent<TestPanel>();
        Debug.Log(NAME + " 对象已创建");
        View.TestButton.onClick.AddListener(OnViewTestButtonClicked);
    }

    public void OnViewTestButtonClicked()
    {
        Debug.Log("确认按钮点击了");
        SendNotification(KSwordKit.AppFacade.TestPanel_ClickedYES);
    }

    public override string[] ListNotificationInterests()
    {
        return base.ListNotificationInterests();
    }
}
