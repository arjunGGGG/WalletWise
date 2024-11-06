using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuManager : MonoBehaviour
{
    public void LoadHomeScene()
    {
        SceneManager.LoadScene("Home");
    }
    public void LoadHoldingScene()
    {
        SceneManager.LoadScene("Holdings");
    }
    public void LoadChatbotScene()
    {
        SceneManager.LoadScene("Chatbot");
    }
}
