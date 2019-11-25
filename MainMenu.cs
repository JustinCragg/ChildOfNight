using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/************************************************************************************************************
Class for managing the main menu scene
Handles buttons, swapping UI, etc.
************************************************************************************************************/
public class MainMenu : MonoBehaviour {
    [Tooltip("Reference to the scene manager")]
    public SceneNotManager sceneNotManager = null;

    [Tooltip("Reference to the audio source used for background music")]
    public AudioSource background;

    [Tooltip("Reference to the main menu in the canvas")]
    public RectTransform mainMenu = null;
    [Tooltip("Reference to the settings menu in the canvas")]
    public RectTransform settingsMenu = null;
    [Tooltip("Reference to the credits menu in the canvas")]
    public RectTransform credits = null;
    [Tooltip("Reference to the controls menu in the canvas")]
    public RectTransform controlls = null;

    // Player setting vars
    public float bgmSound = 1;
    public float gameSound = 1;
    public float textSpeed = 60;
    public float mouseSpeed = 3;
    public float brightness = 1;

    void Start() {
        // Retrieve player settings
        bgmSound = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        gameSound = PlayerPrefs.GetFloat("SFXVolume", 1.0f);;
        textSpeed = PlayerPrefs.GetFloat("textSpeed", 1.0f);
        mouseSpeed = PlayerPrefs.GetFloat("mouseSpeed", 1.0f);
        brightness = PlayerPrefs.GetFloat("brightness", 1.0f);
    }

    // Handles input from the start game button
    public void startButton() {
        sceneNotManager.launchScene();
    }

    // Handles input from the start tutorial button
    public void tutorialButton() {
        sceneNotManager.launchTutorial();
    }

    // Handles input from the open settings button
    public void settingsButton() {
        mainMenu.gameObject.SetActive(false);
        settingsMenu.gameObject.SetActive(true);
        settingsMenu.GetComponentInChildren<Button>().Select();
    }

    // Handles input from the open credits button
    public void creditsButton() {
        mainMenu.gameObject.SetActive(false);
        credits.gameObject.SetActive(true);
        credits.GetComponentInChildren<Button>().Select();
    }

    // Handles input from the open controls button
    public void controllsButton() {
        mainMenu.gameObject.SetActive(false);
        controlls.gameObject.SetActive(true);
        controlls.GetComponentInChildren<Button>().Select();
    }

    // Handles the input from the exit game buttons
    public void exitButton() {
        Application.Quit();
    }

    // Handles the input from the exit settings button
    public void returnSettingsButton() {
        settingsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        mainMenu.GetComponentInChildren<Button>().Select();
    }

    // Handles input from the exit credits button
    public void returnCreditsButton() {
        credits.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        mainMenu.GetComponentInChildren<Button>().Select();
    }

    // Handles input from the exit controls button
    public void returnControllsButton() {
        controlls.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        mainMenu.GetComponentInChildren<Button>().Select();
    }

    // Sets the background volume from the player prefs
    public void setBGMVolume(float volume) {
        bgmSound = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
        background.volume = volume;
    }

    // Sets the sound effects volume from the player prefs
    public void setSFXVolume(float volume) {
        gameSound = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // Sets the text speed from the player prefs
    public void setTextSpeed(float speed) {
        textSpeed = speed;
        PlayerPrefs.SetFloat("textSpeed", speed);
    }

    // Sets the mouse sensitivity from the player prefs
    public void setMouseSensitivity(float sens) {
        mouseSpeed = sens;
        PlayerPrefs.SetFloat("mouseSpeed", sens);
    }

    // Sets the brightness from the player prefs
    public void setBrightness(float intensity) {
        brightness = intensity;
        PlayerPrefs.SetFloat("brightness", intensity);
    }

    // Highlights and selects the first button in the hierarchy
    // Used when the controller is plugged in
    public void selectButton(ControllerHandling controllerHandling) {
        controllerHandling.selectButton(GetComponentInChildren<Button>());
    }
}
