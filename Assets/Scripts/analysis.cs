using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Analysis : MonoBehaviour
{
    private string endpoint = "http://127.0.0.1:11435";
    public string sendData = "5";

    void Start()
    {
        StartCoroutine(PostDataCoroutine(sendData));
    }

    IEnumerator PostDataCoroutine(string data)
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
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
        }
    }
}
