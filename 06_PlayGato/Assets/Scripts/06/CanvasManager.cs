using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas[] canvas;
    public TextMeshProUGUI Error;

    private void OnEnable()
    {
        DataGato.usernameUpdated += usernameUpdated;
        DataGato.usernameError += UsernameError;
        DataGato.noUsername += NoUsername;
    }

    private void OnDisable()
    {
        DataGato.usernameUpdated -= usernameUpdated;
        DataGato.usernameError -= UsernameError;
        DataGato.noUsername -= NoUsername;
    }

    void usernameUpdated()
    {
        canvas[2].gameObject.SetActive(false);
        canvas[0].gameObject.SetActive(false);
        canvas[1].gameObject.SetActive(true);
    }

    void UsernameError()
    {
        canvas[2].gameObject.SetActive(false);
        canvas[2].gameObject.SetActive(true);
        Error.text ="Username Allready Exist";
    }

    void NoUsername()
    {
        canvas[2].gameObject.SetActive(false);
        canvas[2].gameObject.SetActive(true);
        Error.text = "Please Use a Valid Username";
    }
}
