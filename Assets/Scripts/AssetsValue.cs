using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AssetsValue : MonoBehaviour
{
    public TMP_Text goldValueText;
    public TMP_Text landValueText;
    public TMP_Text stocksValueText;
    public TMP_Text mutualFundsValueText;
    public TMP_Text netText;

    private string goldListName = "GoldList";
    private string landListName = "LandList";
    private string stockListName = "StockList";
    private string mfListName = "MFList";

    private void Start()
    {
        UpdateNetValues();
    }

    private void UpdateNetValues()
    {
        goldValueText.text = SumAssetValue(goldListName).ToString("F2");
        landValueText.text = SumAssetValue(landListName).ToString("F2");
        stocksValueText.text = SumAssetValue(stockListName).ToString("F2");
        mutualFundsValueText.text = SumAssetValue(mfListName).ToString("F2");

        float totalHoldings = SumAssetValue(goldListName) + SumAssetValue(landListName) +
                              SumAssetValue(stockListName) + SumAssetValue(mfListName);

        float income = PlayerPrefs.GetFloat("UserIncome", 1)*100000;

        float baseRatio = totalHoldings / income;
        float adjustedRatio = AdjustRatioForUserFactors(baseRatio);

        netText.text = totalHoldings.ToString("F2");

        if (adjustedRatio < 3.0f)
            netText.color = Color.red;   // Vulnerable
        else if (adjustedRatio < 5.0f)
            netText.color = Color.black; // Stable
        else
            netText.color = Color.green; // Healthy
    }

    private float AdjustRatioForUserFactors(float baseRatio)
    {
        // Load user info
        int age = PlayerPrefs.GetInt("UserAge", 25);
        string maritalStatus = PlayerPrefs.GetString("UserMaritalStatus", "Single");
        string employmentSector = PlayerPrefs.GetString("UserEmploymentSector", "Private");

        // Age-based adjustments
        if (age < 30)
            baseRatio *= 1.2f;
        else if (age >= 50)
            baseRatio *= 0.9f;

        // Marital status adjustment
        if (maritalStatus == "Married")
            baseRatio *= 0.95f;

        // Employment sector adjustment
        if (employmentSector == "Government")
            baseRatio *= 0.9f;
        else if (employmentSector == "Self-employed")
            baseRatio *= 1.1f;

        return baseRatio;
    }

    private float SumAssetValue(string assetListName)
    {
        float totalValue = 0;

        if (PlayerPrefs.HasKey(assetListName))
        {
            string json = PlayerPrefs.GetString(assetListName);
            List<Asset> assetList = JsonConvert.DeserializeObject<List<Asset>>(json);

            foreach (Asset asset in assetList)
            {
                totalValue += asset.currentPrice * asset.assetQuantity;
            }
        }

        return totalValue;
    }

    [System.Serializable]
    public class Asset
    {
        public string assetName;
        public int assetQuantity;
        public float assetPrice;
        public float currentPrice;
    }
}
