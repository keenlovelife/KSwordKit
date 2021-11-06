using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class MonoXmlTest : MonoBehaviour
{
    void Start()
    {
        var parser = new Mono.Xml.SecurityParser();
        parser.LoadXml(Resources.Load("Tutorial").ToString());
        var element = parser.ToXml();
        foreach(SecurityElement node in element.Children)
        {
            Debug.Log("node: tag=" + node.Tag + " ID=" + node.Attribute("ID") + " text=" + node.Attribute("text") + " people=" + node.Attribute("people"));
        }
    }

    void Update()
    {
        
    }
}
