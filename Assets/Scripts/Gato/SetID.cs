using System;
using UnityEngine;

public class SetID : MonoBehaviour
{
    public static event Action<string> SetIDGame;

    public string MyID;
    public GameObject gameObject;

    public void X()
    {
        MyID = "id1";
        SetIDGame.Invoke(MyID);
        print(MyID);
        gameObject.SetActive(false);
    }

    public void O()
    {
        MyID = "id2";
        SetIDGame.Invoke(MyID);
        print(MyID);
        gameObject.SetActive(false);
    }
}
