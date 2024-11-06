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
    public TMP_Text chatContent; // displays chat messages
    public TMP_InputField chatInput; // input field for user text
    public ScrollRect scrollRect; // for scrolling the chat content

    public string ipAddress = "127.0.0.1"; // default ip address
    public string port = "11434"; // default port
    private string apiUrl; // chat api endpoint
    private string chatLog = ""; // logs the chat conversation
    private List<JObject> messages = new List<JObject>(); // conversation history

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
        Debug.Log("updated api url: " + apiUrl);
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
            "user's name: {0}, age: {1}, gender: {2}, marital status: {3}, annual income: {4} LPA, employment sector: {5}. " +
            "this is the user data which is for your info only. now remember this and now greet the user in under 30 words. " +
            "don't mention these facts to user. they are just for your context.",
            userName,
            userAge,
            userGender,
            userMaritalStatus,
            userIncome,
            employmentSector
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
            AppendToChatLog("\nYou: " + message);

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

    private void AppendToChatLog(string message)
    {
        chatLog += message + "\n";
        UpdateChatDisplay();
    }

    private void UpdateChatDisplay()
    {
        chatContent.text = chatLog;
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private IEnumerator SendApiRequest()
    {
        var jsonPayload = new JObject
        {
            ["model"] = "arjungupta/aria:9b", // use your specific model
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
            Debug.LogError("api error: " + request.error);
        }
        else
        {
            string rawResponse = request.downloadHandler.text;
            Debug.Log("raw response: " + rawResponse);

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
                        AppendToChatLog("\nBot: " + currentBotResponse.Trim());
                        messages.Add(new JObject { ["role"] = "assistant", ["content"] = currentBotResponse.Trim() });
                        currentBotResponse = "";
                    }
                }
                catch (JsonReaderException ex)
                {
                    Debug.LogError("json parsing error: " + ex.Message);
                }
            }
        }
    }
}
