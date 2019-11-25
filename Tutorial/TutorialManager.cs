using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Enum storing the different types of events which can be triggered
public enum TutorialType { Flavour, Moving, Light, Crosses, Villagers, Cloak, Shadow, Dash, Hypnotise, Slayer, Exiting };

/*********************************************************************************************************************
Manager for handling the triggers for tutorial events
*********************************************************************************************************************/
public class TutorialManager : MonoBehaviour {
    // The parent object in the canvas for the HUD
    GameObject hudObject = null;
    // The parent object in the canvas for the dialogue menu
    GameObject chatObject = null;
    // The gamemanager
    [HideInInspector]
    public GameManager gameManager = null;

    // Whether a tutorial popup is currently being shown
    bool showingTutorial = true;
    [Tooltip("A list of the text to be displayed for each tutorial event")]
    public List<string> tutorialTexts = new List<string>();
    // The current tutorial being displayed, if any
    TutorialType currentTutorial;
    // The timer used for determining when a character is displayed
    float chatTimer = 0;
    // Index of the next character to be displayed
    int count = 0;

    // Reference to the continue button for the tutorial events
    Button continueButton = null;

    private void Start() {
        foreach (TutorialTrigger trigger in FindObjectsOfType<TutorialTrigger>()) {
            trigger.tutorialManager = this;
        }
    }

    private void Update() {
        // Assings the neccesary variables
        if (gameManager == null) {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null) {
                gameManager.inDialogue = false;
            }
        }
        else {
            if (showingTutorial == false) {
                gameManager.inDialogue = false;
            }
            else {
                gameManager.inDialogue = true;
            }
        }
        if (hudObject == null) {
            hudObject = GameObject.Find("HUD");
            if (hudObject != null) {
                hudObject.SetActive(false);
            }
        }
        if (chatObject == null) {
            chatObject = GameObject.Find("ChatObject");
            if (chatObject != null) {
                continueButton = chatObject.GetComponentInChildren<Button>(true);
                chatObject.SetActive(false);
            }
        }
        // Sets time scale when in tutorial event
        if (showingTutorial == true && Time.timeScale != 0) {
            Time.timeScale = 0;
        }
        if (showingTutorial == false && chatObject.activeSelf == true) {
            chatObject.SetActive(false);
            hudObject.SetActive(true);
        }

        if (showingTutorial == true && gameManager != null) {
            if (count <= tutorialTexts[(int)currentTutorial].Length - 1) {
                // Normal text scrolling
                chatTimer += Time.fixedUnscaledDeltaTime * gameManager.uiManager.textSpeed;
                // Skipping
                if (Input.anyKeyDown) {
                    if (tutorialTexts[(int)currentTutorial][count] == '~') {
                        chatObject.GetComponentInChildren<Text>().text = "";
                        count++;
                    }
                    else {
                        while (count <= tutorialTexts[(int)currentTutorial].Length - 1) {
                            if (tutorialTexts[(int)currentTutorial][count] == ';') {
                                chatObject.GetComponentInChildren<Text>().text += '\n';
                                count++;
                                break;
                            }
                            else if (tutorialTexts[(int)currentTutorial][count] == '~') {
                                break;

                            }
                            else {
                                chatObject.GetComponentInChildren<Text>().text += tutorialTexts[(int)currentTutorial][count];
                                count++;
                            }
                        }
                    }
                }
                else {
                    while (chatTimer >= 1 && count < tutorialTexts[(int)currentTutorial].Length) {
                        if (tutorialTexts[(int)currentTutorial][count] == ';') {
                            chatObject.GetComponentInChildren<Text>().text += '\n';
                        }
                        else if (tutorialTexts[(int)currentTutorial][count] == '~') {
                            break;
                        }
                        else {
                            chatObject.GetComponentInChildren<Text>().text += tutorialTexts[(int)currentTutorial][count];
                        }
                        chatTimer -= 1;
                        count++;
                    }
                }
            }
            else {
                continueButton.interactable = true;
            }
        }
    }

    // Displays the dialogue object and disables the hud
    // Also handles what events, if any, are called for specific events
    public void displayTutorial(TutorialType tutorial) {
        // Resets values
        Time.timeScale = 0;
        showingTutorial = true;
        chatObject.GetComponentInChildren<Text>().text = "";
        continueButton.interactable = false;
        count = 0;
        chatTimer = 0;
        currentTutorial = tutorial;

        chatObject.SetActive(true);
        hudObject.SetActive(false);
        switch (tutorial) {
            case TutorialType.Moving:
                break;
            case TutorialType.Light:
                break;
            case TutorialType.Crosses:
                break;
            case TutorialType.Villagers:
                break;
            case TutorialType.Cloak:
                break;
            case TutorialType.Shadow:
                break;
            case TutorialType.Dash:
                break;
            case TutorialType.Hypnotise:
                enableHypnoGuard();
                break;
            case TutorialType.Slayer:
                enableSlayer();
                break;
            case TutorialType.Exiting:
                break;
        }
    }

    // Called to return to play when in a tutorial event
    public void returnToPlay() {
        chatObject.SetActive(false);
        hudObject.SetActive(true);
        showingTutorial = false;
        Time.timeScale = 1;
        gameManager.inDialogue = false;
        gameManager.nightStart = false;
    }

    // Enables a specifc guard
    public void enableHypnoGuard() {
        foreach (GuardControl guard in gameManager.obstacles[0].GetComponentsInChildren<GuardControl>(true)) {
            if (guard.name.Contains("Hypno")) {
                guard.enabled = true;
            }
        }
    }

    // Enables the slayer
    public void enableSlayer() {
        SlayerControl slayer = gameManager.obstacles[0].GetComponentInChildren<SlayerControl>();
        slayer.enabled = true;
        slayer.alertness = 9;
    }
}