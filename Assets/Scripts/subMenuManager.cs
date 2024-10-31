using UnityEngine;
using UnityEngine.SceneManagement;

public class subMenuManager : MonoBehaviour
{
    public void LoadStocksScene()
    {
        SceneManager.LoadScene("Stocks");
    }
    public void LoadMFScene()
    {
        SceneManager.LoadScene("MF");
    }
    public void LoadGoldScene()
    {
        SceneManager.LoadScene("Gold");
    }
    public void LoadLandScene()
    {
        SceneManager.LoadScene("Land");
    }
}
