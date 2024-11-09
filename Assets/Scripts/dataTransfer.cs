using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class dataTransfer : MonoBehaviour
{
    private string endpoint = "http://127.0.0.1:11435";
    private string productPriceEndpoint = "http://127.0.0.1:11436";
    public TMP_Dropdown productDropdown;
    public TMP_Text productPriceText;
    public GameObject popUp;
    public Image progressBar;
    public bool isAffordable=false;
    public TMP_Text confirmText;

    private Dictionary<string, float> productData = new Dictionary<string, float>();

    void Start()
    {
        popUp.SetActive(false);
        productDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        StartCoroutine(PostPlayerPrefsData());
    }

    IEnumerator PostPlayerPrefsData()
    {
        Dictionary<string, string> playerPrefsData = new Dictionary<string, string>()
        {
            { "UserName", PlayerPrefs.GetString("UserName", "DefaultName") },
            { "UserAge", PlayerPrefs.GetInt("UserAge", 0).ToString() },
            { "UserGender", PlayerPrefs.GetString("UserGender", "Male") },
            { "UserMaritalStatus", PlayerPrefs.GetString("UserMaritalStatus", "Single") },
            { "UserIncome", PlayerPrefs.GetFloat("UserIncome", 0f).ToString() },
            { "UserEmploymentSector", PlayerPrefs.GetString("UserEmploymentSector", "Private") }
        };

        string jsonData = JsonConvert.SerializeObject(playerPrefsData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                productData = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonResponse);
                PopulateDropdown(productData);
            }
        }
    }

    void PopulateDropdown(Dictionary<string, float> productData)
    {
        productDropdown.ClearOptions();
        List<string> productNames = new List<string>(productData.Keys);
        productDropdown.AddOptions(productNames);

        if (productNames.Count > 0)
        {
            UpdateProductPriceText(productNames[0]);
        }
    }

    void OnDropdownValueChanged(int index)
    {
        string selectedProduct = productDropdown.options[index].text;
        UpdateProductPriceText(selectedProduct);
    }

    void UpdateProductPriceText(string productName)
    {
        float price = productData[productName];
        productPriceText.text = $"Price: {price}";
    }

    public void onSubmitPressed()
    {
        popUp.SetActive(true);
        string selectedProduct = productDropdown.options[productDropdown.value].text;
        StartCoroutine(SendProductRequest(selectedProduct));
    }

    IEnumerator SendProductRequest(string selectedProduct)
    {
        var requestData = new { index = 1 };
        string jsonData = JsonConvert.SerializeObject(requestData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        string productPriceEndpointWithPath = "http://localhost:11436/get_value";

        using (UnityWebRequest request = new UnityWebRequest(productPriceEndpointWithPath, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);

            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request failed: " + request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonResponse);
                isAffordable = response.ContainsKey("value") && response["value"] != 0f;

                if (isAffordable)
                {
                    confirmText.text = "YES";
                    confirmText.color = Color.green;
                }
                else
                {
                    confirmText.text = "NO";
                    confirmText.color = Color.red;
                }

                //float conf = JsonConvert.DeserializeObject<float>(jsonResponse);
                //UpdateProgressBar(conf);
            }
        }
    }


void UpdateProgressBar(float price)
    {
        float maxPrice = 100000f;
        float fillAmount = Mathf.Clamp01(price / maxPrice);
        progressBar.fillAmount = fillAmount;
    }

    public void onClosePressed()
    {
        popUp.SetActive(false);
    }
}
