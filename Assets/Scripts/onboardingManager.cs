using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class onboardingManager : MonoBehaviour
{
    // Personal data elements
    public TMP_InputField nameInput;
    public TMP_InputField ageInput;
    public TMP_Dropdown genderDropdown;
    public TMP_Dropdown maritalStatusDropdown;
    public TMP_InputField incomeInput;
    public TMP_Dropdown employmentSectorDropdown;

    // Panels for onboarding
    public GameObject introPanel;
    public GameObject userDataPanel;

    private void Start()
    {
        introPanel.SetActive(false);
        userDataPanel.SetActive(false);

        if (PlayerPrefs.HasKey("HasCompletedOnboarding") && PlayerPrefs.GetInt("HasCompletedOnboarding") == 1)
        {
            LoadMainApp();
            return;
        }

        // Show intro panel initially
        introPanel.SetActive(true);
        userDataPanel.SetActive(false);

        LoadData();
    }


    private void SubmitPersonalData()
    {
        string userName = nameInput.text;
        int userAge = int.Parse(ageInput.text);
        string userGender = genderDropdown.options[genderDropdown.value].text;
        string userMaritalStatus = maritalStatusDropdown.options[maritalStatusDropdown.value].text;
        float userIncome = float.Parse(incomeInput.text);
        string userEmploymentSector = employmentSectorDropdown.options[employmentSectorDropdown.value].text;

        PlayerPrefs.SetString("UserName", userName);
        PlayerPrefs.SetInt("UserAge", userAge);
        PlayerPrefs.SetString("UserGender", userGender);
        PlayerPrefs.SetString("UserMaritalStatus", userMaritalStatus);
        PlayerPrefs.SetFloat("UserIncome", userIncome);
        PlayerPrefs.SetString("UserEmploymentSector", userEmploymentSector);

        introPanel.SetActive(false);
        userDataPanel.SetActive(false);
    }

    public void OnGoPressed()
    {
        introPanel.SetActive(false);
        userDataPanel.SetActive(true);
    }

    public void OnSubmitPressed()
    {
        SubmitPersonalData();
        PlayerPrefs.SetInt("HasCompletedOnboarding", 1);
        PlayerPrefs.Save();
        LoadMainApp();
    }

    private void LoadData()
    {
        if (PlayerPrefs.HasKey("UserName"))
        {
            nameInput.text = PlayerPrefs.GetString("UserName");
            ageInput.text = PlayerPrefs.GetInt("UserAge").ToString();
            genderDropdown.value = genderDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("UserGender"));
            maritalStatusDropdown.value = maritalStatusDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("UserMaritalStatus"));
            incomeInput.text = PlayerPrefs.GetFloat("UserIncome").ToString();
            employmentSectorDropdown.value = employmentSectorDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("UserEmploymentSector"));
        }
    }

    private void LoadMainApp()
    {
        SceneManager.LoadScene("Home");
    }
}