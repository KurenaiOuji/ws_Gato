using System;
using UnityEngine;

public class SetID : MonoBehaviour
{
    public static event Action<string> SetIDGame;

    public string MyID;
    public GameObject gameObject;

    public void X()
    {
        MyID = "1";
        SetIDGame.Invoke(MyID);
        print(MyID);
        gameObject.SetActive(false);
    }

    public void O()
    {
        MyID = "2";
        SetIDGame.Invoke(MyID);
        print(MyID);
        gameObject.SetActive(false);
    }
}
