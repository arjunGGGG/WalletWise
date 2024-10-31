using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnboardingManager : MonoBehaviour
{
    // Intro screen elements
    public GameObject introPanel;
    public Button startOnboardingButton;

    // Personal data elements
    public TMP_InputField nameInput;
    public TMP_InputField ageInput;
    public TMP_Dropdown genderDropdown;
    public TMP_Dropdown maritalStatusDropdown;
    public Button personalSubmitButton;

    // Financial data elements
    public TMP_InputField incomeInput;
    public TMP_Dropdown employmentSectorDropdown;
    public Button financialSubmitButton;

    public Button letsGoButton;

    // Panels for onboarding
    public GameObject personalDataPanel;
    public GameObject financialDataPanel;
    public GameObject letsGoPanel;

    private void Start()
    {
        PlayerPrefs.DeleteAll();

        if (PlayerPrefs.HasKey("HasCompletedOnboarding") && PlayerPrefs.GetInt("HasCompletedOnboarding") == 1)
        {
            LoadMainApp();
            return;
        }

        // Show intro panel initially and hide other panels
        introPanel.SetActive(true);
        personalDataPanel.SetActive(false);
        financialDataPanel.SetActive(false);
        letsGoPanel.SetActive(false);

        // Add listeners for buttons
        startOnboardingButton.onClick.AddListener(StartOnboarding);
        personalSubmitButton.onClick.AddListener(SubmitPersonalData);
        financialSubmitButton.onClick.AddListener(SubmitFinancialData);
        letsGoButton.onClick.AddListener(OnLetsGoPressed);

        LoadPersonalData();
        LoadFinancialData();
    }

    private void StartOnboarding()
    {
        introPanel.SetActive(false);
        personalDataPanel.SetActive(true);
    }

    private void SubmitPersonalData()
    {
        string userName = nameInput.text;
        int userAge = int.Parse(ageInput.text);
        string userGender = genderDropdown.options[genderDropdown.value].text;
        string userMaritalStatus = maritalStatusDropdown.options[maritalStatusDropdown.value].text;

        PlayerPrefs.SetString("UserName", userName);
        PlayerPrefs.SetInt("UserAge", userAge);
        PlayerPrefs.SetString("UserGender", userGender);
        PlayerPrefs.SetString("UserMaritalStatus", userMaritalStatus);

        personalDataPanel.SetActive(false);
        financialDataPanel.SetActive(true);
    }

    private void SubmitFinancialData()
    {
        float userIncome = float.Parse(incomeInput.text);
        string userEmploymentSector = employmentSectorDropdown.options[employmentSectorDropdown.value].text;

        PlayerPrefs.SetFloat("UserIncome", userIncome);
        PlayerPrefs.SetString("UserEmploymentSector", userEmploymentSector);

        financialDataPanel.SetActive(false);
        letsGoPanel.SetActive(true);
    }

    private void OnLetsGoPressed()
    {
        PlayerPrefs.SetInt("HasCompletedOnboarding", 1);
        PlayerPrefs.Save();
        LoadMainApp();
    }

    private void LoadPersonalData()
    {
        if (PlayerPrefs.HasKey("UserName"))
        {
            nameInput.text = PlayerPrefs.GetString("UserName");
            ageInput.text = PlayerPrefs.GetInt("UserAge").ToString();
            genderDropdown.value = genderDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("UserGender"));
            maritalStatusDropdown.value = maritalStatusDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("UserMaritalStatus"));
        }
    }

    private void LoadFinancialData()
    {
        if (PlayerPrefs.HasKey("UserIncome"))
        {
            incomeInput.text = PlayerPrefs.GetFloat("UserIncome").ToString();
            employmentSectorDropdown.value = employmentSectorDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("UserEmploymentSector"));
        }
    }

    private void LoadMainApp()
    {
        SceneManager.LoadScene("Home");
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
