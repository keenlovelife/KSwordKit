using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime Birthdaty { get; set; }
}


public class LitJsonTest : MonoBehaviour
{

    void Start()
    {
        PersonToJson();
        JsonToPerson();
    }

    void PersonToJson()
    {
        Person bill = new Person();
        bill.Name = "William Shakespeare";
        bill.Age = 51;
        bill.Birthdaty = new DateTime(1564, 4, 26);

        string json_bill = JsonMapper.ToJson(bill);
        Debug.Log(json_bill);
    }
    
    void JsonToPerson()
    {
        string json = @"
            {
                ""Name"" : ""Thomas More"",
                ""Age"" : 57,
                ""Birthday"" :""02/07/1478 00:00:00""
            }
        ";

        Person thomas = JsonMapper.ToObject<Person>(json);
        Debug.Log("Thomas age: " + thomas.Age);
    }

    void Update()
    {
        
    }


}
