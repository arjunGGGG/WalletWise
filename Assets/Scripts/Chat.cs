using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TMPro;

public class Chat : MonoBehaviour
{
    public TMP_Text chatContent; // Optional, can be used for debugging
    public TMP_InputField chatInput; // Input field for user text
    public ScrollRect scrollRect; // For scrolling the chat content
    public Transform chatContentPanel; // The container to hold chat bubbles

    public GameObject userBubblePrefab; // Prefab for user chat bubble
    public GameObject botBubblePrefab; // Prefab for bot chat bubble

    public string ipAddress = "127.0.0.1"; // Default ip address
    public string port = "11434"; // Default port
    private string apiUrl; // Chat API endpoint
    private List<JObject> messages = new List<JObject>(); // Conversation history

    void Start()
    {
        UpdateApiUrl();

        SendUserDataToAPI();

        chatInput.onEndEdit.AddListener(delegate {
            if (chatInput.isFocused && Input.GetKeyDown(KeyCode.Return)) SendMessage();
        });
    }

    public void UpdateApiUrl()
    {
        apiUrl = $"http://{ipAddress}:{port}/api/chat";
        Debug.Log("Updated API URL: " + apiUrl);
    }

    private void SendUserDataToAPI()
    {
        string userName = PlayerPrefs.GetString("UserName", "User");
        int userAge = PlayerPrefs.GetInt("UserAge", 0);
        string userGender = PlayerPrefs.GetString("UserGender", "Not Specified");
        string userMaritalStatus = PlayerPrefs.GetString("UserMaritalStatus", "Not Specified");
        float userIncome = PlayerPrefs.GetFloat("UserIncome", 0f);
        string employmentSector = PlayerPrefs.GetString("UserEmploymentSector", "Not Specified");

        string userDataMessage = string.Format(
            "User's name: {0}, age: {1}, gender: {2}, marital status: {3}, annual income: {4} LPA, employment sector: {5}. " +
            "This is the user data, for your info only. Greet the user in under 30 words, but don’t mention these facts.",
            userName, userAge, userGender, userMaritalStatus, userIncome, employmentSector
        );

        messages.Add(new JObject
        {
            ["role"] = "system",
            ["content"] = userDataMessage
        });

        StartCoroutine(SendApiRequest());
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            string message = chatInput.text;
            AppendUserMessage(message);

            messages.Add(new JObject
            {
                ["role"] = "user",
                ["content"] = message
            });

            chatInput.interactable = false;

            StartCoroutine(SendApiRequest());
            chatInput.text = "";
            chatInput.ActivateInputField();
        }
    }

    private void AppendUserMessage(string message)
    {
        GameObject userBubble = Instantiate(userBubblePrefab, chatContentPanel);
        TMP_Text userText = userBubble.GetComponentInChildren<TMP_Text>();
        userText.text = message;
        ScrollToLatestMessage();
    }

    private void AppendBotMessage(string message)
    {
        GameObject botBubble = Instantiate(botBubblePrefab, chatContentPanel);
        TMP_Text botText = botBubble.GetComponentInChildren<TMP_Text>();
        botText.text = message;
        ScrollToLatestMessage();
    }

    private void ScrollToLatestMessage()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // Scroll to the bottom
    }

    private IEnumerator SendApiRequest()
    {
        var jsonPayload = new JObject
        {
            ["model"] = "arjungupta/aria:9b", // Use your specific model
            ["messages"] = new JArray(messages)
        };

        byte[] postData = Encoding.UTF8.GetBytes(jsonPayload.ToString());

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(postData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        chatInput.interactable = true;

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("API error: " + request.error);
        }
        else
        {
            string rawResponse = request.downloadHandler.text;
            Debug.Log("Raw response: " + rawResponse);

            string currentBotResponse = "";

            string[] ndjsonObjects = rawResponse.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string ndjson in ndjsonObjects)
            {
                try
                {
                    JObject jsonResponse = JObject.Parse(ndjson);
                    string assistantResponsePart = jsonResponse["message"]["content"].ToString().Trim();
                    bool isDone = jsonResponse["done"].Value<bool>();

                    currentBotResponse += assistantResponsePart + " ";

                    if (isDone)
                    {
                        AppendBotMessage(currentBotResponse.Trim());
                        messages.Add(new JObject { ["role"] = "assistant", ["content"] = currentBotResponse.Trim() });
                        currentBotResponse = "";
                    }
                }
                catch (JsonReaderException ex)
                {
                    Debug.LogError("JSON parsing error: " + ex.Message);
                }
            }
        }
    }
}
