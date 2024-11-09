using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections;

public class AssetListManager : MonoBehaviour
{
    public GameObject formPanel;
    public TMP_InputField assetNameInput;
    public TMP_InputField assetPriceInput;
    public TMP_InputField assetQuantityInput;
    public Button submitButton;
    public Button cancelButton;

    public GameObject assetPrefab;
    public Transform contentParent;

    private List<Asset> assetList = new List<Asset>();
    public string assetListName = "AssetList";
    public bool stockMode = false;
    private string apiKey = "your_alpha_vantage_api_key";

    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        cancelButton.onClick.AddListener(OnCancel);

        LoadAssetsFromPrefs();
    }

    public void OnSubmit()
    {
        string name = assetNameInput.text;
        string priceText = assetPriceInput.text;
        string quantityText = assetQuantityInput.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(priceText) || string.IsNullOrEmpty(quantityText))
        {
            Debug.LogWarning("Please fill in all fields before submitting.");
            return;
        }

        if (stockMode)
        {
            StartCoroutine(ValidateAsset(name, priceText, quantityText));
        }
        else
        {
            float initialPrice = float.Parse(priceText);
            int quantity = int.Parse(quantityText);
            Asset newAsset = new Asset(name, quantity, initialPrice, initialPrice);
            assetList.Add(newAsset);
            SaveAssetsToPrefs();
            CreateAssetPanel(newAsset);
        }

        OnCancel();
    }

    private IEnumerator ValidateAsset(string assetName, string priceText, string quantityText)
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
                    float initialPrice = float.Parse(priceText);
                    int quantity = int.Parse(quantityText);
                    Asset newAsset = new Asset(assetName, quantity, initialPrice, currentPrice);
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
                    Debug.LogError("Unable to parse the price from the API response.");
                }
            }
            else
            {
                Debug.LogError("Unable to find price in API response.");
            }
        }
    }

    private void CreateAssetPanel(Asset asset)
    {
        GameObject newAssetPanel = Instantiate(assetPrefab, contentParent);

        TextMeshProUGUI nameText = newAssetPanel.transform.Find("nameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI quantityText = newAssetPanel.transform.Find("quantityText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceText = newAssetPanel.transform.Find("priceText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI profitText = newAssetPanel.transform.Find("profitText")?.GetComponent<TextMeshProUGUI>();
        Image arrowImage = newAssetPanel.transform.Find("arrow")?.GetComponent<Image>();

        if (nameText == null || quantityText == null || priceText == null)
        {
            Debug.LogError("One or more of the TextMeshProUGUI components were not found.");
            return;
        }

        nameText.text = asset.assetName;
        quantityText.text = "Quantity: " + asset.assetQuantity;
        priceText.text = "Price: " + asset.assetPrice.ToString("F2");

        if (stockMode)
        {
            float profitPercentage = ((asset.currentPrice - asset.assetPrice) / asset.assetPrice) * 100;
            profitText.text = profitPercentage.ToString("F2") + "%";

            if (profitPercentage < -5f)
            {
                profitText.color = Color.red;
                arrowImage.color = Color.red;
                arrowImage.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else if (profitPercentage > 5f)
            {
                profitText.color = Color.green;
                arrowImage.color = Color.green;
            }
            else
            {
                profitText.color = Color.black;
                arrowImage.gameObject.SetActive(true);
            }
        }
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
        PlayerPrefs.SetString(assetListName, json);
        PlayerPrefs.Save();
    }

    private void LoadAssetsFromPrefs()
    {
        if (PlayerPrefs.HasKey(assetListName))
        {
            string json = PlayerPrefs.GetString(assetListName);
            assetList = JsonConvert.DeserializeObject<List<Asset>>(json);

            foreach (Asset asset in assetList)
            {
                CreateAssetPanel(asset);
            }
        }
    }

    [System.Serializable]
    public class Asset
    {
        public string assetName;
        public int assetQuantity;
        public float assetPrice;
        public float currentPrice;

        public Asset(string name, int quantity, float initialPrice, float currentPrice)
        {
            assetName = name;
            assetQuantity = quantity;
            assetPrice = initialPrice;
            this.currentPrice = currentPrice;
        }
    }
}
