using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PieChartManager : MonoBehaviour
{
    public Image[] imagesPieChart; // Array of Pie Chart segments (images)
    public float[] values;         // Array of asset values (Gold, Land, Stocks, MF)

    public TMP_Text NET;           // Total value text

    private string goldListName = "GoldList";
    private string landListName = "LandList";
    private string stockListName = "StockList";
    private string mfListName = "MFList";

    void Start()
    {
        // Fetch asset values and set the values
        SetValues();
    }

    void Update()
    {
        // If needed, update the pie chart continuously, or use other conditions.
    }

    public void SetValues()
    {
        // Get the sum of asset values
        float goldValue = SumAssetValue(goldListName);
        float landValue = SumAssetValue(landListName);
        float stockValue = SumAssetValue(stockListName);
        float mfValue = SumAssetValue(mfListName);

        // Update the values array
        values = new float[] { goldValue, landValue, stockValue, mfValue };

        // Update the NET text with total value of all assets
        float totalValue = goldValue + landValue + stockValue + mfValue;
        NET.text = totalValue.ToString("F2");

        // Now update the pie chart with the new values
        SetPieChartValues(values);
    }

    private void SetPieChartValues(float[] valuesToSet)
    {
        // Calculate total value
        float totalValue = 0;
        for (int i = 0; i < valuesToSet.Length; i++)
        {
            totalValue += valuesToSet[i];
        }

        // Update pie chart segments
        float accumulatedFillAmount = 0;
        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            float percentage = valuesToSet[i] / totalValue;
            accumulatedFillAmount += percentage;

            // Set the fill amount of each image (pie chart segment)
            imagesPieChart[i].fillAmount = accumulatedFillAmount;
        }
    }

    private float SumAssetValue(string assetListName)
    {
        float totalValue = 0;

        // Fetch the asset data from PlayerPrefs and calculate total value
        if (PlayerPrefs.HasKey(assetListName))
        {
            string json = PlayerPrefs.GetString(assetListName);

            // Deserialize JSON into a list of assets using JsonConvert
            List<Asset> assetList = JsonConvert.DeserializeObject<List<Asset>>(json);

            foreach (var asset in assetList)
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
