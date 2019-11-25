using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/************************************************************************************************************
Class for handeling scene management and maneuvering through the scenes
************************************************************************************************************/
public class SceneNotManager : MonoBehaviour {
    [HideInInspector]
    // Loading bar
    public Image progressBar = null;
    [HideInInspector]
    // Loading text
    public Text progressText = null;

    // Plugged in controller
    string[] controllers;


    // Saves Scene manager between scenes
    public void Start() {
        DontDestroyOnLoad(this);
        Application.backgroundLoadingPriority = ThreadPriority.High;
    }
    
    // Load town scene and manager scene while bringing up a loading scene
    public void launchScene() {
        StartCoroutine(loadTown());
    }
    
    // Reloads town scene and brings up a loading screen
    public void launchNight(GameManager manager) {
        StartCoroutine(loadNight(manager));
    }

    // Load tutorial scene and manager scene while bringing up a loading scene
    public void launchTutorial() {
        StartCoroutine(loadTutorial());
    }

    IEnumerator loadTown() {
        yield return null;

        // Load loading screen
        AsyncOperation loading = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        while (!loading.isDone) {
            yield return null;
        }

        // Hides title screen UI
        GameObject.FindGameObjectWithTag("UICanvas").SetActive(false);

        // Find loading screen objects
        foreach (RectTransform rect in GameObject.FindGameObjectWithTag("Respawn").GetComponentsInChildren<RectTransform>()) {
            if (rect.name == "Progress Bar") {
                progressBar = rect.GetComponent<Image>();
            }
            else if (rect.name == "Progress") {
                progressText = rect.GetComponent<Text>();
            }
        }

        // Loads new active scene
        AsyncOperation townLoading = SceneManager.LoadSceneAsync("Town", LoadSceneMode.Additive);
        townLoading.allowSceneActivation = false;
        while (!townLoading.isDone) {
            // Update loading screen
            progressBar.fillAmount = townLoading.progress * 0.5f;
            progressText.text = (townLoading.progress * 50) + "%";

            // Check if the load has finished
            if (townLoading.progress >= 0.9f) {
                break;
            }
            yield return null;
        }

        // Load manager scene
        loading = SceneManager.LoadSceneAsync("ManagerScene", LoadSceneMode.Additive);
        loading.allowSceneActivation = false;
        while (!loading.isDone) {
            // Update loading screen
            progressBar.fillAmount = (loading.progress * 0.5f) + 0.5f;
            progressText.text = (loading.progress * 50) + 50.0f + "%";

            // Check if the load has finished
            if (loading.progress >= 0.9f) {
                progressText.text = "Press any key to continue...";
                if (Input.anyKeyDown == true) {
                    townLoading.allowSceneActivation = true;
                    loading.allowSceneActivation = true;
                }
            }
            yield return null;
        }

        // Set town as active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Town"));

        // Unload title screen
        SceneManager.UnloadSceneAsync("Title");

        // Unload loading screen
        SceneManager.UnloadSceneAsync("Loading");

        // Resets referances to uiManager and Main Menu
        ControllerHandling controllerHandling = GetComponent<ControllerHandling>();
        controllerHandling.uiManager = null;
        controllerHandling.mainMenu = null;
    }
   
    IEnumerator loadNight(GameManager manager) {
        yield return null;

        // Load loading screen
        AsyncOperation loading = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        while (!loading.isDone) {
            yield return null;
        }

        // Hides town UI
        GameObject.FindGameObjectWithTag("UICanvas").SetActive(false);

        // Find loading screen objects
        foreach (RectTransform rect in GameObject.FindGameObjectWithTag("Respawn").GetComponentsInChildren<RectTransform>()) {
            if (rect.name == "Progress Bar") {
                progressBar = rect.GetComponent<Image>();
            }
            else if (rect.name == "Progress") {
                progressText = rect.GetComponent<Text>();
            }
        }

        // Change active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("ManagerScene"));
        // Unload town scene
        loading = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Town"));
        while (!loading.isDone) {
            yield return null;
        }

        // Reload town scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Town", LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone) {
            // Update loading screen
            progressBar.fillAmount = asyncOperation.progress;
            progressText.text = (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f) {
                progressText.text = "Press any key to continue...";
                if (Input.anyKey == true) {
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }

        // Set town as active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Town"));
        // Unload loading screen
        SceneManager.UnloadSceneAsync("Loading");
        // Launch game manager init
        manager.init();

        // Resets referances to uiManager and Main Menu
        ControllerHandling controllerHandling = GetComponent<ControllerHandling>();
        controllerHandling.uiManager = null;
        controllerHandling.mainMenu = null;
    }

    IEnumerator loadTutorial() {
        yield return null;

        // Load loading screen
        AsyncOperation loading = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        while (!loading.isDone) {
            yield return null;
        }

        // Hides title screen UI
        GameObject.FindGameObjectWithTag("UICanvas").SetActive(false);

        // Find loading screen objects
        foreach (RectTransform rect in GameObject.FindGameObjectWithTag("Respawn").GetComponentsInChildren<RectTransform>()) {
            if (rect.name == "Progress Bar") {
                progressBar = rect.GetComponent<Image>();
            }
            else if (rect.name == "Progress") {
                progressText = rect.GetComponent<Text>();
            }
        }

        // Loads new active scene
        AsyncOperation townLoading = SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Additive);
        townLoading.allowSceneActivation = false;
        while (!townLoading.isDone) {
            // Update loading screen
            progressBar.fillAmount = townLoading.progress * 0.5f;
            progressText.text = (townLoading.progress * 50) + "%";

            // Check if the load has finished
            if (townLoading.progress >= 0.9f) {
                break;
            }
            yield return null;
        }

        // Load manager scene
        loading = SceneManager.LoadSceneAsync("ManagerScene", LoadSceneMode.Additive);
        loading.allowSceneActivation = false;
        while (!loading.isDone) {
            // Update loading screen
            progressBar.fillAmount = (loading.progress * 0.5f) + 0.5f;
            progressText.text = (loading.progress * 50) + 50.0f + "%";

            // Check if the load has finished
            if (loading.progress >= 0.9f) {
                progressText.text = "Press any key to continue...";
                if (Input.anyKeyDown == true) {
                    townLoading.allowSceneActivation = true;
                    loading.allowSceneActivation = true;
                }
            }
            yield return null;
        }

        // Set Tutorial as active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Tutorial"));

        // Unload title screen
        SceneManager.UnloadSceneAsync("Title");

        // Unload loading screen
        SceneManager.UnloadSceneAsync("Loading");

        // Resets referances to uiManager and Main Menu
        ControllerHandling controllerHandling = GetComponent<ControllerHandling>();
        controllerHandling.uiManager = null;
        controllerHandling.mainMenu = null;
    }

    // Return to main menu form town
    public void returnToMainMenu() {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.UnloadSceneAsync("ManagerScene");
        SceneManager.LoadScene("Title");

        // Resets referances to uiManager and Main Menu
        ControllerHandling controllerHandling = GetComponent<ControllerHandling>();
        controllerHandling.uiManager = null;
        controllerHandling.mainMenu = null;
    }

    // Return to main menu from tutorial
    public void returnFromTutorial() {
        SceneManager.UnloadSceneAsync("Tutorial");
        SceneManager.UnloadSceneAsync("ManagerScene");
        SceneManager.LoadScene("Title");

        // Resets referances to uiManager and Main Menu
        ControllerHandling controllerHandling = GetComponent<ControllerHandling>();
        controllerHandling.uiManager = null;
        controllerHandling.mainMenu = null;
    }
}
