using PureMVC.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestYesWindowMediator : PureMVC.Patterns.Mediator.Mediator
{
    public new const string NAME = "TestYesWindowMediator";
    GameObject prefab;
    TestYesWindow View;
    public TestYesWindowMediator(object viewCommponent):base(NAME, viewCommponent)
    {
        prefab = viewCommponent as GameObject;
        Debug.Log(NAME + " 对象已创建");
    }

    public override string[] ListNotificationInterests()
    {
        return new string[] {
            KSwordKit.AppFacade.TestPanel_ClickedYES
        };
    }
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case KSwordKit.AppFacade.TestPanel_ClickedYES:
                Open();
                break;
        }
    }

    public void Open()
    {
        View = GameObject.Instantiate(prefab, GameObject.Find("Canvas").transform).GetComponent<TestYesWindow>();
        View.CloseButton.onClick.AddListener(TestYesWindowCloseButtonClicked);
        Debug.Log(View.name + " 界面已打开");

    }
    public void Close()
    {
        Debug.Log(View.name + " 界面已关闭");
        GameObject.Destroy(View.gameObject);
    }
    void TestYesWindowCloseButtonClicked()
    {
        Debug.Log("TestYesWindow关闭按钮已被点击");

        View.gameObject.SetActive(false);
        Close();
    }
}
