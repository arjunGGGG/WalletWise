using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;

    public GameObject optionsScreen;
    public Chat chatScript;

    private void Start()
    {
        if (optionsScreen != null)
        {
            optionsScreen.SetActive(false);
        }
    }

    public void OnSubmit()
    {
        string ipAddress = ipInputField.text;
        string port = portInputField.text;

        if (!string.IsNullOrEmpty(ipAddress) && !string.IsNullOrEmpty(port))
        {
            if (chatScript != null)
            {
                chatScript.ipAddress = ipAddress;
                chatScript.port = port;

                chatScript.UpdateApiUrl();
            }

            Debug.Log("updated api url to: " + ipAddress + ":" + port);

            if (optionsScreen != null)
            {
                optionsScreen.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("both ip and port are required.");
        }
    }

    public void OnCancel()
    {
        Debug.Log("action cancelled.");

        ipInputField.text = "";
        portInputField.text = "";

        if (optionsScreen != null)
        {
            optionsScreen.SetActive(false);
        }
    }

    public void OnOptions()
    {
        if (optionsScreen != null)
        {
            optionsScreen.SetActive(true);
        }
    }
}
