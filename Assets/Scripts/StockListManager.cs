using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class AssetListManager : MonoBehaviour
{
    public GameObject formPanel;
    public TMP_InputField assetNameInput;
    public TMP_InputField assetPriceInput;
    public TMP_InputField assetQuantityInput;
    public Button submitButton;
    public Button cancelButton;
    public Button refreshButton;

    public GameObject assetPrefab;
    public Transform contentParent;

    private List<Asset> assetList = new List<Asset>();
    private string apiKey = "Q7WUAFCRDN132MSQ";

    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        cancelButton.onClick.AddListener(OnCancel);
        refreshButton.onClick.AddListener(UpdateAssetPrices);

        LoadAssetsFromPrefs();
    }

    public void OnSubmit()
    {
        if (!string.IsNullOrEmpty(assetNameInput.text) && !string.IsNullOrEmpty(assetPriceInput.text) && !string.IsNullOrEmpty(assetQuantityInput.text))
        {
            string name = assetNameInput.text;
            StartCoroutine(ValidateAsset(name));
        }
        else
        {
            Debug.LogWarning("Please fill in all fields before submitting.");
        }
    }

    private IEnumerator ValidateAsset(string assetName)
    {
        string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={assetName}&apikey={apiKey}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API Request failed: " + www.error);
                yield break;
            }

            string jsonResponse = www.downloadHandler.text;
            string priceKey = "\"05. price\": \"";
            int priceIndex = jsonResponse.IndexOf(priceKey);

            if (priceIndex != -1)
            {
                priceIndex += priceKey.Length;
                int priceEndIndex = jsonResponse.IndexOf("\"", priceIndex);
                string priceString = jsonResponse.Substring(priceIndex, priceEndIndex - priceIndex);

                if (float.TryParse(priceString, out float currentPrice))
                {
                    float initialPrice = float.Parse(assetPriceInput.text);
                    Asset newAsset = new Asset(assetName, int.Parse(assetQuantityInput.text), initialPrice, currentPrice);
                    assetList.Add(newAsset);

                    SaveAssetsToPrefs();

                    CreateAssetPanel(newAsset);

                    assetNameInput.text = "";
                    assetPriceInput.text = "";
                    assetQuantityInput.text = "";
                    formPanel.SetActive(false);
                }
                else
                {
                    Debug.LogError("Failed to parse the extracted price.");
                }
            }
            else
            {
                Debug.LogError("Incorrect stock symbol entered.");
                OnCancel();
            }
        }
    }

    private void CreateAssetPanel(Asset asset)
    {
        GameObject newAssetPanel = Instantiate(assetPrefab, contentParent);
        newAssetPanel.transform.Find("name").GetComponent<TextMeshProUGUI>().text = asset.assetName;
        newAssetPanel.transform.Find("quantity").GetComponent<TextMeshProUGUI>().text = "Quantity: " + asset.assetQuantity;

        // Calculate profit percentage based on current price from API
        float profitPercentage = ((asset.currentPrice - asset.assetPrice) / asset.assetPrice) * 100;
        TextMeshProUGUI profitText = newAssetPanel.transform.Find("profit").GetComponent<TextMeshProUGUI>();
        profitText.text = $"Profit: {profitPercentage:F2}%";

        profitText.color = profitPercentage > 5 ? Color.green : profitPercentage < -5 ? Color.red : Color.grey;

        // Show current price from the API
        TextMeshProUGUI priceText = newAssetPanel.transform.Find("price").GetComponent<TextMeshProUGUI>();
        priceText.text = "Price: " + asset.currentPrice.ToString("F2");
    }

    public void OnCancel()
    {
        assetNameInput.text = "";
        assetPriceInput.text = "";
        assetQuantityInput.text = "";
        formPanel.SetActive(false);
    }

    public void ShowForm()
    {
        formPanel.SetActive(true);
    }

    private void SaveAssetsToPrefs()
    {
        string json = JsonConvert.SerializeObject(assetList);
        PlayerPrefs.SetString("AssetList", json);
        PlayerPrefs.Save();
    }

    private void LoadAssetsFromPrefs()
    {
        if (PlayerPrefs.HasKey("AssetList"))
        {
            string json = PlayerPrefs.GetString("AssetList");
            assetList = JsonConvert.DeserializeObject<List<Asset>>(json);

            foreach (Asset asset in assetList)
            {
                CreateAssetPanel(asset);
            }
        }
    }

    // Update prices for all assets
    public void UpdateAssetPrices()
    {
        foreach (Asset asset in assetList)
        {
            StartCoroutine(UpdatePriceForAsset(asset));
        }
    }

    private IEnumerator UpdatePriceForAsset(Asset asset)
    {
        string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={asset.assetName}&apikey={apiKey}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API Request failed for asset: " + asset.assetName + " - " + www.error);
                yield break;
            }

            string jsonResponse = www.downloadHandler.text;
            string priceKey = "\"05. price\": \"";
            int priceIndex = jsonResponse.IndexOf(priceKey);

            if (priceIndex != -1)
            {
                priceIndex += priceKey.Length;
                int priceEndIndex = jsonResponse.IndexOf("\"", priceIndex);
                string priceString = jsonResponse.Substring(priceIndex, priceEndIndex - priceIndex);

                if (float.TryParse(priceString, out float currentPrice))
                {
                    asset.currentPrice = currentPrice; // Update current price
                    SaveAssetsToPrefs(); // Save updated prices

                    // Find and update the UI for this asset
                    foreach (Transform assetPanel in contentParent)
                    {
                        if (assetPanel.Find("name").GetComponent<TextMeshProUGUI>().text == asset.assetName)
                        {
                            float profitPercentage = ((currentPrice - asset.assetPrice) / asset.assetPrice) * 100;

                            assetPanel.Find("price").GetComponent<TextMeshProUGUI>().text = "Price: " + currentPrice.ToString("F2");
                            TextMeshProUGUI profitText = assetPanel.Find("profit").GetComponent<TextMeshProUGUI>();
                            profitText.text = $"Profit: {profitPercentage:F2}%";

                            profitText.color = profitPercentage > 5 ? Color.green : profitPercentage < -5 ? Color.red : Color.grey;
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class Asset
{
    public string assetName;
    public int assetQuantity;
    public float assetPrice; // Initial price entered by the user
    public float currentPrice; // Current price from the API

    public Asset(string name, int quantity, float initialPrice, float currentPrice)
    {
        assetName = name;
        assetQuantity = quantity;
        assetPrice = initialPrice;
        this.currentPrice = currentPrice;
    }
}
