using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/************************************************************************************************************
Class for handling all ui systems in the project
*************************************************************************************************************/
public class UIManager : MonoBehaviour {
    // References
    [HideInInspector]
    public GameManager gameManager = null;
    // Box for upgrades
    public GameObject boxObject = null;
    // Health UI
    RectTransform healthSlider = null;
    // Arrow on clock timer
    RectTransform arrowObject = null;
    // List of objectives
    List<Text> objCounts = new List<Text>();
    // Awareness Text
    Text awarenessText = null;
    [HideInInspector]
    // Night number Text
    public Text nightText = null;

    // References to all UI objects
    [HideInInspector]
    // HUD
    public RectTransform hudObject = null;
    [HideInInspector]
    // Pause menu
    public RectTransform pauseObject = null;
    [HideInInspector]
    // Game lost
    public RectTransform gameOverObject = null;
    [HideInInspector]
    // Completed night
    public RectTransform dayOverObject = null;
    [HideInInspector]
    // Victory
    public RectTransform gameWonObject = null;
    [HideInInspector]
    // Dialogue
    public RectTransform chatObject = null;
    [HideInInspector]
    // Blackscreen 
    public RectTransform blackScreenObject = null;
    [HideInInspector]
    // Settings menu
    public RectTransform settingsObject = null;
    [HideInInspector]
    // Contolls menu
    public RectTransform controllsObject = null;
    [HideInInspector]
    // Abilitys
    public RectTransform abilitiesObject = null;

    [HideInInspector]
    // In settings menu
    public bool settingsOpen = false;
    [HideInInspector]
    // In controlls menu
    public bool controllsOpen = false;

    // List of dialogues for each night
    public List<string> chatText = new List<string>();

    // How fast dialogue moves
    float chatTimer = 0;
    // Which character is displayed next
    int count = 0;

    // Settings variables
    [Header("Setiting Values")]
    [Tooltip("Volume of background music")]
    public float bgmSound = 1;
    [Tooltip("Volume of game sound effects")]
    public float gameSound = 1;
    [Tooltip("Speed of dialogue")]
    public float textSpeed = 10;
    [Tooltip("Mouse sensitivity")]
    public float mouseSpeed = 3;
    [Tooltip("Brightness")]
    public float brightness = 1;

    // Start of sceen to black so unnecessary things arent showing
    void Start() {
        foreach (RectTransform rect in GetComponentsInChildren<RectTransform>(true)) {
            if (rect.parent == transform) {
                if (rect.name == "BlackScreen") {
                    blackScreenObject = rect;
                    blackScreenObject.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    void Update() {
        // Writes text onto screen at the start of night
        if (gameManager != null) {
            if (gameManager.nightStart == true) {
                gameManager.inDialogue = true;
                if (count <= chatText[gameManager.dayIndex].Length - 1) {
                    chatObject.GetComponentInChildren<Button>().interactable = false;
                    // Normal text scrolling
                    chatTimer += Time.fixedUnscaledDeltaTime * gameManager.uiManager.textSpeed;
                    // Skipping
                    if (Input.anyKeyDown) {
                        if (chatText[gameManager.dayIndex][count] == '~') {
                            chatObject.GetComponentInChildren<Text>().text = "";
                            count++;
                        }
                        else {
                            while (count <= chatText[gameManager.dayIndex].Length - 1) {
                                if (chatText[gameManager.dayIndex][count] == ';') {
                                    chatObject.GetComponentInChildren<Text>().text += '\n';
                                    count++;
                                    break;
                                }
                                else if (chatText[gameManager.dayIndex][count] == '~') {
                                    break;

                                }
                                else {
                                    chatObject.GetComponentInChildren<Text>().text += chatText[gameManager.dayIndex][count];
                                    count++;
                                }
                            }
                        }
                    }
                    else {
                        while (chatTimer >= 1 && count < chatText[gameManager.dayIndex].Length) {
                            if (chatText[gameManager.dayIndex][count] == ';') {
                                chatObject.GetComponentInChildren<Text>().text += '\n';
                            }
                            else if (chatText[gameManager.dayIndex][count] == '~') {
                                break;
                            }
                            else {
                                chatObject.GetComponentInChildren<Text>().text += chatText[gameManager.dayIndex][count];
                            }
                            chatTimer -= 1;
                            count++;
                        }
                    }
                }
                else {
                    chatObject.GetComponentInChildren<Button>().interactable = true;
                }
            }
        }
    }

    public void init() {
        // Retrieve settings
        bgmSound = PlayerPrefs.GetFloat("BGMVolume", 1.0f); 
        gameSound = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        textSpeed = PlayerPrefs.GetFloat("textSpeed", 1.0f);
        mouseSpeed = PlayerPrefs.GetFloat("mouseSpeed", 1.0f);
        brightness = PlayerPrefs.GetFloat("brightness", 1.0f);

        // Set UI objects 
        foreach (RectTransform rect in GetComponentsInChildren<RectTransform>(true)) {
            if (rect.parent == transform) {
                if (rect.name == "HUD") {
                    hudObject = rect;
                }
                else if (rect.name == "Pause") {
                    pauseObject = rect;
                }
                else if (rect.name == "GameOver") {
                    gameOverObject = rect;
                }
                else if (rect.name == "DayOver") {
                    dayOverObject = rect;
                }
                else if (rect.name == "GameWon") {
                    gameWonObject = rect;
                }
                else if (rect.name == "ChatObject") {
                    chatObject = rect;
                }
                else if (rect.name == "BlackScreen") {
                    blackScreenObject = rect;
                }
                else if (rect.name == "Settings") {
                    settingsObject = rect;
                }
                else if (rect.name == "Controlls") {
                    controllsObject = rect;
                }
            }
            else if (rect.name == "Skills") {
                abilitiesObject = rect;
            }
        }

        // Set UI objects
        foreach (RectTransform rect in hudObject.GetComponentsInChildren<RectTransform>()) {
            // Health
            if (rect.name == "Health") {
                healthSlider = rect;
            }
            // Quests
            else if (rect.name == "Quests") {
                foreach (Text text in rect.GetComponentsInChildren<Text>()) {
                    if (text.name.Contains("Drunkard")) {
                        objCounts.Add(text);
                    }
                    else if (text.name.Contains("Noble")) {
                        objCounts.Add(text);
                    }
                    else if (text.name.Contains("Guard")) {
                        objCounts.Add(text);
                    }
                    else if (text.name.Contains("Mayor")) {
                        objCounts.Add(text);
                    }
                }
            }
            // Night Number
            else if (rect.name == "nightText") {
                nightText = rect.GetComponent<Text>();
            }
            // Awareness
            else if (rect.name == "awarenessText") {
                awarenessText = rect.GetComponent<Text>();
            }
            // Clock arrow
            else if (rect.name == "DayArrow") {
                arrowObject = rect;
            }
        }

        // Update settings
        setBGMVolume(bgmSound);
        setSFXVolume(gameSound);
        setTextSpeed(textSpeed);
        setMouseSensitivty(mouseSpeed);
        setBrightness(brightness);

        // Update UI's
        createUpgradeBoxes();
        updateObjectives();
        updateNight();
        updateUI();
    }

    // Change image so that it fills up and goes down according to ability cooldowns
    public void updateAbiltyCooldowns(List<float> cooldowns) {
        int index = 0;
        foreach (Image image in abilitiesObject.GetComponentsInChildren<Image>()) {
            if (image.name.Contains("Cooldown")) {
                image.fillAmount = cooldowns[index];
                index++;
            }
        }
    }

    // Updates the health UI image
    public void updateHealthBar(int health, int maxHealth) {
        float percent = health / 6.0f;
        healthSlider.GetComponent<Image>().fillAmount = percent;
    }

    // Update quest numbers
    public void updateObjectives() {
        int drunkards = 0;
        int nobles = 0;
        int guards = 0;
        int mayor = 0;

        // Check how many villagers need to be eaten 
        // Add number for each villager
        for (int i = 0; i < gameManager.objectiveList[gameManager.dayIndex].Count; i++) {
            switch (gameManager.objectiveList[gameManager.dayIndex][i]) {
                case GoalType.DRUNKARD:
                    drunkards++;
                    break;
                case GoalType.GUARD:
                    guards++;
                    break;
                case GoalType.NOBLE:
                    nobles++;
                    break;
                case GoalType.MAYOR:
                    mayor++;
                    break;
            }
        }

        // Set the number on UI
        foreach (Text objective in objCounts) {
            if (objective.name.Contains("Drunkard")) {
                objective.text = drunkards.ToString();
            }
            if (objective.name.Contains("Noble")) {
                objective.text = nobles.ToString();
            }
            if (objective.name.Contains("Guard")) {
                objective.text = guards.ToString();
            }
            if (objective.name.Contains("Mayor")) {
                objective.text = mayor.ToString();
            }
        }
    }

    // Change the night number
    public void updateNight() {
        nightText.text = (gameManager.dayIndex + 1).ToString();
    }

    // Change awareness number
    public void updateAwareness() {
        awarenessText.text = Mathf.FloorToInt(gameManager.alertness).ToString();
    }

    // Update clock arrow
    // Rotate as night moves along
    public void updateClock() {
        if (gameManager != null) {
            float percent = Mathf.Clamp(gameManager.dayTimer / gameManager.dayLength, 0, 1);
            float value = percent * 175.0f - 85.0f;
            arrowObject.localEulerAngles = new Vector3(0, 0, value);
        }
    }

    // Update all UI
    public void updateUI() {
        // Set all UI to inactive
        gameOverObject.gameObject.SetActive(false);
        blackScreenObject.gameObject.SetActive(false);
        chatObject.gameObject.SetActive(false);
        gameWonObject.gameObject.SetActive(false);
        hudObject.gameObject.SetActive(false);
        pauseObject.gameObject.SetActive(false);
        dayOverObject.gameObject.SetActive(false);
        settingsObject.gameObject.SetActive(false);
        controllsObject.gameObject.SetActive(false);

        // Select button when controller is plugged in
        if (gameManager.sceneNotManager.GetComponent<ControllerHandling>().controllerInControl == true) {
            selectButton(gameManager.sceneNotManager.GetComponent<ControllerHandling>());
        }
        // Deselct button when controller is taken out
        else {
            gameManager.sceneNotManager.GetComponent<ControllerHandling>().deselectButton();
        }

        // Player has lost the game
        if (gameManager.gameOver == true) {
            gameOverObject.gameObject.SetActive(true);
        }
        // Player has won the game
        else if (gameManager.gameWon == true) {
            gameWonObject.gameObject.SetActive(true);
        }
        else {
            // The night has ended
            if (gameManager.nightEnded == true) {
                dayOverObject.gameObject.SetActive(true);
                updateUpgrades();
            }
            // The night has started
            else if (gameManager.nightStart == true) {
                chatObject.gameObject.SetActive(true);
            }
            // Start of scene
            else if (gameManager.blackscreen == true) {
                blackScreenObject.gameObject.SetActive(true);    
            }
            // The game has been paused  
            else if (gameManager.gamePaused == true) {
                // Settings menu
                if (settingsOpen == true) {
                    settingsObject.gameObject.SetActive(true);
                }
                // Controlls Menu
                else if (controllsOpen == true) {
                    controllsObject.gameObject.SetActive(true);
                }
                // Pause Menu
                else {
                    pauseObject.gameObject.SetActive(true);
                    setBirdCamera();
                }
            }
            // The game is being played
            else {
                hudObject.gameObject.SetActive(true);
            }
        }
    }

    // Move camera into birds eye view
    public void setBirdCamera() {
        gameManager.cameraRot = gameManager.townCamera.transform.rotation;
        gameManager.townCamera.transform.rotation = Quaternion.Euler(45.0f, 270.0f, 0.0f);
    }
    // Move camera back to normal position
    void resetCamera() {
        gameManager.townCamera.transform.rotation = gameManager.cameraRot;
    }

    // Exit Pause menu and resume game
    public void resume() {
        Time.timeScale = 1;
        gameManager.gamePaused = false;
        resetCamera();
        updateUI();
    }

    // Return to main menu
    public void loadMenu() {
        gameManager.loadMenu();
    }

    // Start next night
    public void endDay() {
        gameManager.endDay();
    }

    // Restart night
    public void restartNight() {
        gameManager.restartNight();
    }

    // Exit Dialogue
    public void startDay() {
        gameManager.startDay();
    }

    // Open Settings
    public void openSettings() {
        settingsOpen = true;
        // Set sliders to player prefs
        foreach (Slider slider in settingsObject.GetComponentsInChildren<Slider>()) {
            if (slider.name == "BGM Volume") {
                slider.value = gameManager.soundManager.backgroundSFX.volume;
            }
            else if (slider.name == "SFX Volume") {
                slider.value = gameManager.soundManager.playerSFX.volume;
            }
            else if (slider.name == "TextSpeed") {
                slider.value = textSpeed;
            }
            else if (slider.name == "Mouse Sensitivity") {
                slider.value = mouseSpeed;
            }
            else if (slider.name == "Brightness") {
                slider.value = brightness;
            }
        }
        updateUI();
    }

    // Close settings
    public void closeSettings() {
        settingsOpen = false;
        updateUI();
    }

    // Open controlls menu
    public void openControlls() {
        controllsOpen = true;
        updateUI();
    }

    // Close controlls menu
    public void closeControlls() {
        controllsOpen = false;
        updateUI();
    }

    // When slider is moved set player prefs
    // Set background music volume
    public void setBGMVolume(float volume) {
        if (gameManager != null) {
            bgmSound = volume;
            PlayerPrefs.SetFloat("BGMVolume", volume);
            gameManager.soundManager.setBGMVolume(volume);
        }
    }

    // When slider is moved set player prefs
    // Set game sounds volume
    public void setSFXVolume(float volume) {
        if (gameManager != null) {
            gameSound = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            gameManager.soundManager.setSFXVolume(volume);
        }
    }

    // When slider is moved set player prefs
    // Set dialogue speed
    public void setTextSpeed(float speed) {
        textSpeed = speed;
        PlayerPrefs.SetFloat("textSpeed", speed);
    }

    // When slider is moved set player prefs
    // Set mouse sensitivity
    public void setMouseSensitivty(float sens) {
        if (gameManager != null) {
            mouseSpeed = sens;
            PlayerPrefs.SetFloat("mouseSpeed", sens);
            gameManager.player.cameraSens = mouseSpeed;
        }
    }

    // When slider is moved set player prefs
    // Set brightness
    public void setBrightness(float intensity) {
        if (gameManager != null) {
            brightness = intensity;
            PlayerPrefs.SetFloat("brightness", intensity);
            gameManager.startIntensity = brightness;
            gameManager.endIntensity = brightness + 3;
        }
    }

    // When controller is plugged in select UI button
    public void selectButton(ControllerHandling controllerHandling) {
        if (controllerHandling != null) {
            controllerHandling.selectButton(GetComponentInChildren<Button>());
        }
    }

    // Create upgrade boxes
    void createUpgradeBoxes() {
        // Size of boxes
        int offset = 225;
        int totalSize = 850;

        // For each upgrade create the amount of boxes avaiable for the upgrade
        foreach (Text text in dayOverObject.GetComponentsInChildren<Text>()) {
            if (text.name == "CloakText") {
                int width = totalSize / gameManager.maxCloakUpgrades;

                for (int i = 0; i < gameManager.maxCloakUpgrades; i++) {
                    RectTransform box = Instantiate(boxObject).GetComponent<RectTransform>();
                    box.SetParent(text.transform.parent, false);

                    box.sizeDelta = new Vector2(width, box.rect.height);
                    box.localPosition = new Vector2(offset + width * i, 0);
                }
            }
            else if (text.name == "ShadowText") {
                int width = totalSize / gameManager.maxShadowUpgrades;

                for (int i = 0; i < gameManager.maxShadowUpgrades; i++) {
                    RectTransform box = Instantiate(boxObject).GetComponent<RectTransform>();
                    box.SetParent(text.transform.parent, false);

                    box.sizeDelta = new Vector2(width, box.rect.height);
                    box.localPosition = new Vector2(offset + width * i, 0);
                }
            }
            else if (text.name == "DashText") {
                int width = totalSize / gameManager.maxDashUpgrades;

                for (int i = 0; i < gameManager.maxDashUpgrades; i++) {
                    RectTransform box = Instantiate(boxObject).GetComponent<RectTransform>();
                    box.SetParent(text.transform.parent, false);

                    box.sizeDelta = new Vector2(width, box.rect.height);
                    box.localPosition = new Vector2(offset + width * i, 0);
                }
            }
            else if (text.name == "HypnosisText") {
                int width = totalSize / gameManager.maxStunUpgrades;

                for (int i = 0; i < gameManager.maxStunUpgrades; i++) {
                    RectTransform box = Instantiate(boxObject).GetComponent<RectTransform>();
                    box.SetParent(text.transform.parent, false);

                    box.sizeDelta = new Vector2(width, box.rect.height);
                    box.localPosition = new Vector2(offset + width * i, 0);
                }
            }
            else if (text.name == "ConsumeText") {
                int width = totalSize / gameManager.maxConsumeUpgrades;

                for (int i = 0; i < gameManager.maxConsumeUpgrades; i++) {
                    RectTransform box = Instantiate(boxObject).GetComponent<RectTransform>();
                    box.SetParent(text.transform.parent, false);

                    box.sizeDelta = new Vector2(width, box.rect.height);
                    box.localPosition = new Vector2(offset + width * i, 0);
                }
            }
            else if (text.name == "HealthText") {
                int width = totalSize / gameManager.maxHealthUpgrades;

                for (int i = 0; i < gameManager.maxHealthUpgrades; i++) {
                    RectTransform box = Instantiate(boxObject).GetComponent<RectTransform>();
                    box.SetParent(text.transform.parent, false);

                    box.sizeDelta = new Vector2(width, box.rect.height);
                    box.localPosition = new Vector2(offset + width * i, 0);
                }
            }
        }
    }

    // Remove upgrade buttons
    void removeButtons() {
        foreach (Button button in dayOverObject.GetComponentsInChildren<Button>()) {
            if (button.name == "Cloak") {
                if (gameManager.numCloakUpgrades >= gameManager.maxCloakUpgrades) {
                    button.interactable = false;
                }
            }
            if (button.name == "Dash") {
                if (gameManager.numDashUpgrades >= gameManager.maxDashUpgrades) {
                    button.interactable = false;
                }
            }
            if (button.name == "Shadow") {
                if (gameManager.numShadowUpgrades >= gameManager.maxShadowUpgrades) {
                    button.interactable = false;
                }
            }
            if (button.name == "Hypnosis") {
                if (gameManager.numStunUpgrades >= gameManager.maxStunUpgrades) {
                    button.interactable = false;
                }
            }
            if (button.name == "Health") {
                if (gameManager.numHealthUpgrades >= gameManager.maxHealthUpgrades) {
                    button.interactable = false;
                }
            }
            if (button.name == "Consume") {
                if (gameManager.numConsumeUpgrades >= gameManager.maxConsumeUpgrades) {
                    button.interactable = false;
                }
            }
        }
    }

    // Update upgrade buttons
    public void updateUpgrades() {
        removeButtons();

        // For each upgrade add to slider and fill in box
        foreach (Text text in dayOverObject.GetComponentsInChildren<Text>()) {
            if (text.name == "Upgrade Info") {
                text.text = gameManager.upgrades.ToString();
            }
            else if (text.name == "CloakText") {
                text.text =  gameManager.cloakDuration.ToString("F2");
                foreach (Image image in text.transform.parent.GetComponentsInChildren<Image>()) {
                    if (image.name == "Slider") {
                        image.fillAmount = gameManager.numCloakUpgrades / (float)gameManager.maxCloakUpgrades;
                        break;
                    }
                }
            }
            else if (text.name == "ShadowText") {
                text.text = gameManager.shadowDuration.ToString("F2");
                foreach (Image image in text.transform.parent.GetComponentsInChildren<Image>()) {
                    if (image.name == "Slider") {
                        image.fillAmount = gameManager.numShadowUpgrades / (float)gameManager.maxShadowUpgrades;
                        break;
                    }
                }
            }
            else if (text.name == "DashText") {
                text.text = gameManager.dashDuration.ToString("F2");
                foreach (Image image in text.transform.parent.GetComponentsInChildren<Image>()) {
                    if (image.name == "Slider") {
                        image.fillAmount = gameManager.numDashUpgrades / (float)gameManager.maxDashUpgrades;
                        break;
                    }
                }
            }
            else if (text.name == "HypnosisText") {
                text.text = gameManager.stunDuration.ToString("F2");
                foreach (Image image in text.transform.parent.GetComponentsInChildren<Image>()) {
                    if (image.name == "Slider") {
                        image.fillAmount = gameManager.numStunUpgrades / (float)gameManager.maxStunUpgrades;
                        break;
                    }
                }
            }
            else if (text.name == "ConsumeText") {
                text.text = gameManager.consumeTime.ToString("F2");
                foreach (Image image in text.transform.parent.GetComponentsInChildren<Image>()) {
                    if (image.name == "Slider") {
                        image.fillAmount = gameManager.numConsumeUpgrades / (float)gameManager.maxConsumeUpgrades;
                        break;
                    }
                }
            }
            else if (text.name == "HealthText") {
                text.text = gameManager.maxHealth.ToString();
                foreach (Image image in text.transform.parent.GetComponentsInChildren<Image>()) {
                    if (image.name == "Slider") {
                        image.fillAmount = gameManager.numHealthUpgrades / (float)gameManager.maxHealthUpgrades;
                        break;
                    }
                }
            }
        }
    }

    // When clicked in game will update upgrades
    public void upgradeAbility(int abilityIndex) {
        Abilities ability = (Abilities)abilityIndex;
        gameManager.upgradeAbiltiies(ability);
        updateUpgrades();
    }
}
