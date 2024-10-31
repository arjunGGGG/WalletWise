using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TMPro;

public class ChatSystem : MonoBehaviour
{
    public TMP_Text chatContent; // Displays chat messages
    public TMP_InputField chatInput; // Input field for user text
    public ScrollRect scrollRect; // For scrolling the chat content

    private string chatLog = ""; // Logs the chat conversation
    private string apiUrl = "http://127.0.0.1:11434/api/chat"; // Chat API endpoint
    private List<JObject> messages = new List<JObject>(); // Conversation history

    void Start()
    {
        // Send user data silently to the API
        SendUserDataToAPI();

        // Set up listener for chat input submission
        chatInput.onEndEdit.AddListener(delegate {
            if (chatInput.isFocused && Input.GetKeyDown(KeyCode.Return)) SendMessage();
        });
    }

    private void SendUserDataToAPI()
    {
        // Retrieve user data from PlayerPrefs
        string userName = PlayerPrefs.GetString("UserName", "User");
        int userAge = PlayerPrefs.GetInt("UserAge", 0);
        string userGender = PlayerPrefs.GetString("UserGender", "Not Specified");
        string userMaritalStatus = PlayerPrefs.GetString("UserMaritalStatus", "Not Specified");
        float userIncome = PlayerPrefs.GetFloat("UserIncome", 0f);
        string employmentSector = PlayerPrefs.GetString("UserEmploymentSector", "Not Specified");

        // Create a message with the user's data
        string userDataMessage = string.Format(
            "user's name: {0}, age: {1}, gender: {2}, marital status: {3}, annual income: {4} LPA, employment sector: {5}. " +
            "This is the user data which is for your info only. Now remember this and now greet the user in under 30 words. " +
            "Don't mention these facts to user. They are just for your context.",
            userName,
            userAge,
            userGender,
            userMaritalStatus,
            userIncome,
            employmentSector
        );

        // Add the user data message to the messages list
        messages.Add(new JObject
        {
            ["role"] = "system", // Sending this as a system message for context
            ["content"] = userDataMessage
        });

        // Send the data to the API
        StartCoroutine(SendApiRequest());
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            string message = chatInput.text;
            AppendToChatLog("\nYou: " + message); // Append user message to chat log

            // Add user message to the list
            messages.Add(new JObject
            {
                ["role"] = "user",
                ["content"] = message
            });

            chatInput.interactable = false; // Disable input field while waiting for response

            StartCoroutine(SendApiRequest());
            chatInput.text = ""; // Clear the input field
            chatInput.ActivateInputField(); // Focus back on the input field
        }
    }

    private void AppendToChatLog(string message)
    {
        chatLog += message + "\n"; // Add message to chat log
        UpdateChatDisplay(); // Update the chat display
    }

    private void UpdateChatDisplay()
    {
        chatContent.text = chatLog; // Update chat content
        Canvas.ForceUpdateCanvases(); // Force canvas to update
        scrollRect.verticalNormalizedPosition = 0f; // Scroll to the bottom
    }

    private IEnumerator SendApiRequest()
    {
        var jsonPayload = new JObject
        {
            ["model"] = "arjungupta/aria:9b", // Use your specific model
            ["messages"] = new JArray(messages) // Include messages in the request
        };

        byte[] postData = Encoding.UTF8.GetBytes(jsonPayload.ToString());

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(postData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest(); // Send request to the server

        // Re-enable the input field after receiving a response
        chatInput.interactable = true;

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("API Error: " + request.error); // Log any errors
        }
        else
        {
            string rawResponse = request.downloadHandler.text; // Get the raw response
            Debug.Log("Raw Response: " + rawResponse);

            string currentBotResponse = "";

            // Process response
            string[] ndjsonObjects = rawResponse.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string ndjson in ndjsonObjects)
            {
                try
                {
                    JObject jsonResponse = JObject.Parse(ndjson);
                    string assistantResponsePart = jsonResponse["message"]["content"].ToString().Trim();
                    bool isDone = jsonResponse["done"].Value<bool>();

                    currentBotResponse += assistantResponsePart + " "; // Build the bot response

                    if (isDone)
                    {
                        AppendToChatLog("\nBot: " + currentBotResponse.Trim()); // Append bot response to chat log
                        messages.Add(new JObject { ["role"] = "assistant", ["content"] = currentBotResponse.Trim() });
                        currentBotResponse = ""; // Reset for next message
                    }
                }
                catch (JsonReaderException ex)
                {
                    Debug.LogError("JSON Parsing Error: " + ex.Message); // Log any JSON parsing errors
                }
            }
        }
    }
}
