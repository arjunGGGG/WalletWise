using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuManager : MonoBehaviour
{
    // Method to be called when the button is clicked
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
