using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/************************************************************************************************************
Class for handling game mechanics
************************************************************************************************************/
public enum GoalType { NOBLE, DRUNKARD, GUARD, SLAYER, MAYOR };
public enum Abilities { Cloak, Shadow, Dash, Hypnosis, Consume, Health };
public class GameManager : MonoBehaviour {
    // References
    [HideInInspector]
    public UIManager uiManager = null;
    [HideInInspector]
    public SoundManager soundManager = null;
    [HideInInspector]
    public PlayerControl player = null;
    [HideInInspector]
    public SceneNotManager sceneNotManager = null;

    // List of objects in the scene 
    [Tooltip("The different objects in the scene for each night")]
    public List<GameObject> obstacles = new List<GameObject>();

    // Objectives
    [Tooltip("The list of quests for each night")]
    public ObjectiveList objectiveList = new ObjectiveList();
    // Copy of objective list
    [Tooltip("Copy of objective list")]
    public ObjectiveList originalObjectiveList = new ObjectiveList();
    // Night number
    [Tooltip("What night it is")]
    public int dayIndex = 1;

    // Game bools
    [HideInInspector]
    // Game paused
    public bool gamePaused = false;
    [HideInInspector]
    // Game lost
    public bool gameOver = false;
    [HideInInspector]
    // Completed night
    public bool nightEnded = false;
    [HideInInspector]
    // Completed game 
    public bool gameWon = false;
    [HideInInspector]
    // Start of night
    public bool nightStart = false;
    [HideInInspector]
    // Chat dialogue
    public bool inDialogue = false;
    [HideInInspector]
    // Inbetween nights screen
    public bool blackscreen = false;

    // Directional light
    [Header("Directional Light")]
    Light sun = null;
    // Colour night starts at
    public Color startColour;
    // Intesity night starts at
    public float startIntensity;
    // Intesity night ends at
    public Color endColour;
    // Intesity night ends at
    public float endIntensity;

    // Length of nights
    [Header("Length of nights")]
    [Tooltip("How long the night will last")]
    public float dayLength = 300;
    [Tooltip("The timer of the night")]
    public float dayTimer = 300;

    // How alert the guards are of the player
    [Tooltip("Alertness of guards and townfolk")]
    public float alertness = 0;

    // Bird eye camera
    Vector3 cameraTarget = new Vector3(60.0f, 50.0f, 0);
    // Speed the camera moves to and from bird eye view
    float cameraSpeed = 25.0f;
    [HideInInspector]
    // Rotation of camera
    public Quaternion cameraRot;

    // In menu
    bool menuPress = false;

    [HideInInspector]
    // Camera that is looking at player
    public Camera townCamera = null;

    int numConsumed = 0;
    [Header("Upgrades")]
    // How alert the guards are of the player
    [Tooltip("Alertness of guards and townfolk")]
    public int upgrades = 0;

    [Space(5)]
    [Tooltip("How much the upgrade affects the ability")]
    public float cloakDuration = 3.0f;
    [Tooltip("Max amount of cloak upgrades that player can have")]
    public int maxCloakUpgrades = 6;
    [Tooltip("How many cloak upgrades the player has")]
    public int numCloakUpgrades = 0;

    [Space(5)]
    [Tooltip("How much the upgrade affects the ability")]
    public float shadowDuration = 3.0f;
    [Tooltip("Max amount of shadow upgrades that player can have")]
    public int maxShadowUpgrades = 6;
    [Tooltip("How many shadow upgrades the player has")]
    public int numShadowUpgrades = 0;

    [Space(5)]
    [Tooltip("How much the upgrade affects the ability")]
    public float dashDuration = 0.1f;
    [Tooltip("Max amount of dash upgrades that player can have")]
    public int maxDashUpgrades = 5;
    [Tooltip("How many dash upgrades the player has")]
    public int numDashUpgrades = 0;

    [Space(5)]
    [Tooltip("How much the upgrade affects the ability")]
    public float stunDuration = 5.0f;
    [Tooltip("Max amount of stun upgrades that player can have")]
    public int maxStunUpgrades = 5;
    [Tooltip("How many stun upgrades the player has")]
    public int numStunUpgrades = 0;

    [Space(5)]
    [Tooltip("How much the upgrade affects the ability")]
    public float consumeTime = 1.6f;
    [Tooltip("Max amount of consume upgrades that player can have")]
    public int maxConsumeUpgrades = 6;
    [Tooltip("How many consume upgrades the player has")]
    public int numConsumeUpgrades = 0;

    [Space(5)]
    [Tooltip("How much the upgrade affects the ability")]
    public int maxHealth = 3;
    [Tooltip("Max amount of health upgrades that player can have")]
    public int maxHealthUpgrades = 3;
    [Tooltip("How many heatlh upgrades the player has")]
    public int numHealthUpgrades = 0;

    // Main init used for project
    void Start() {
        init();
    }

    void Update() {
        // Lock the mouse to the middle of the screen
        if (Input.GetMouseButtonDown(0)) {
            Cursor.lockState = CursorLockMode.Confined;
        }
        // Check if playable scene
        if (SceneManager.GetActiveScene().name == "Town" || SceneManager.GetActiveScene().name == "Tutorial") {
            // Pause
            if (Input.GetAxisRaw("Menu") == 1 && menuPress == false && inDialogue == false && nightEnded == false) {
                menuPress = true;
                // Open pause menu first 
                if (uiManager.settingsOpen == true) {
                    uiManager.settingsOpen = false;
                }
                else {
                    gamePaused = !gamePaused;
                }
                uiManager.updateUI();
                // When paused set time to zero, make cursor visible, move camera and pause audio
                if (gamePaused == true) {
                    Time.timeScale = 0;
                    Cursor.visible = true;
                    soundManager.pauseAudio();
                    uiManager.setBirdCamera();
                }
                // Unpause audio and reset ui
                else {
                    Cursor.visible = false;
                    soundManager.unPauseAudio();
                    uiManager.resume();
                }
            }
            // Exit menu
            else if (Input.GetAxisRaw("Menu") == 0 && menuPress == true) {
                menuPress = false;
            }

            // Set bird eye camera
            if (gamePaused == true) {
                townCamera.transform.position = Vector3.MoveTowards(townCamera.transform.position, cameraTarget, Time.unscaledDeltaTime * cameraSpeed);
                townCamera.cullingMask = -1;
            }
            // Fix layer camera
            else {
                townCamera.cullingMask = ~(1 << LayerMask.NameToLayer("People"));
            }

            // Countdown sun timer
            dayTimer -= Time.deltaTime;
            // Move directional light
            if (dayTimer <= 0) {
                player.sunburn += Mathf.Pow(-dayTimer * 0.1f, 2) * Time.deltaTime;
            }
            // Update UI timer
            uiManager.updateClock();
            // Change directional light colour
            updateSun();

            // Calculates the average awareness of all villagers in the town
            if (obstacles[dayIndex] != null) {
                int temp = 0;
                int count = 0;
                // Find all villagers
                // For each villarger get awareness 
                foreach (BaseControl npc in obstacles[dayIndex].GetComponentsInChildren<BaseControl>()) {
                    count++;
                    temp += npc.alertness;
                }
                // If there are villagers in town
                if (count > 0) {
                    // Average awareness
                    temp /= count;
                }
                // No villagers
                else {
                    temp = 0;
                }
                // Set to closest int between 0 & 9
                temp = Mathf.Clamp(temp, 0, 9);
                // Update awareneess on UI
                if (temp != alertness) {
                    alertness = temp;
                    uiManager.updateAwareness();
                }
            }
        }
    }

    // Sort obstacles
    int sortByName(GameObject go1, GameObject go2) {
        return go1.name.CompareTo(go2.name);
    }

    public void init() {
        Time.timeScale = 0;

        // Find objects
        sceneNotManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneNotManager>();
        townCamera = GameObject.FindGameObjectWithTag("TownCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        uiManager = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIManager>();
        uiManager.gameManager = this;

        // Obstacles
        // Find obstacles
        obstacles = new List<GameObject>();
        // Check what night it is
        GameObject parent = GameObject.FindGameObjectWithTag("Nights");
        // Add objects of that night to list
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true)) {
            if (child.parent == parent.transform) {
                obstacles.Add(child.gameObject);
            }
        }
        // Sort obstacles
        obstacles.Sort(sortByName);

        // Obstacle list
        if (obstacles.Count > 0) {
            // Set all obstacles it inactive
            for (int i = 0; i < obstacles.Count; i++) {
                if (obstacles[i] != null) {
                    obstacles[i].SetActive(false);
                }
            }
            // Set obstacles in night to active
            if (obstacles[dayIndex] != null) {
                obstacles[dayIndex].SetActive(true);
                foreach (BaseControl npc in obstacles[dayIndex].GetComponentsInChildren<BaseControl>()) {
                    npc.init();
                    npc.gameManager = this;
                    npc.player = player;
                }
            }
        }

        // Variables
        dayTimer = dayLength;
        gamePaused = false;
        gameOver = false;
        gameWon = false;
        nightEnded = false;
        nightStart = true;
        blackscreen = false;

        // Init
        soundManager.init();
        soundManager.playBackgroundMusic(dayIndex);
        uiManager.init();
        player.init();

        // Sun
        foreach (Light light in FindObjectsOfType<Light>()){
            if (light.transform.parent == null) {
                sun = light;
            }
        }
    }

    // Update objectives list
    public void consumedObjective(GoalType consumed) {
        // Count of how many villagers are eaten
        numConsumed++;
        // Guards are worth two points
        if (consumed == GoalType.GUARD) {
            numConsumed++;
        }
        // For every five villagers consumed player gets extra upgrade point
        if (numConsumed >= 5) {
            upgrades++;
            numConsumed -= 5;
        }

        // Int for removing an objective
        int index = -1;
        // Remove villager from quest
        for (int i = 0; i < objectiveList[dayIndex].Count; i++) { 
            if (objectiveList[dayIndex][i] == consumed) {
                index = i;
                break;
            }
        }
        // Remove objective
        if (index != -1) {
            objectiveList[dayIndex].RemoveAt(index);
            if (objectiveList[dayIndex].Count == 0) {
                upgrades++;
            }
        }

        uiManager.updateObjectives();

        // Open Door
        if (objectiveList[dayIndex].Count == 0) {
            soundManager.objWonSFX();
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door"); 
            foreach (GameObject door in doors) {
                Destroy(door);
            }
        }
    }

    // Sun rises as game progresses
    void updateSun() {
        float value = 1 - dayTimer / dayLength;
        sun.intensity = Mathf.Lerp(startIntensity, endIntensity, value);
        sun.color = Color.Lerp(startColour, endColour, value);
    }

    // Update UI
    public void nightEnd() {
        soundManager.dayEndSFX();
        nightEnded = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        uiManager.updateUI();
    }

    // Move to next night
    public void endDay() {
        dayIndex++;
        blackscreen = true;
        uiManager.updateUI();
        soundManager.stopBackgroundMusic();
        string sceneName = SceneManager.GetActiveScene().name;
        sceneNotManager.launchNight(this);
        Time.timeScale = 1;
    }

    // Restart night
    public void restartNight() {
        soundManager.stopBackgroundMusic();
        objectiveList[dayIndex].goals = new List<GoalType>(originalObjectiveList[dayIndex].goals);
        string sceneName = SceneManager.GetActiveScene().name;
        sceneNotManager.launchNight(this);
    }

    // Lost game
    public void loseGame() {
        soundManager.gameoverSFX();
        gameOver = true;
        gamePaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        uiManager.updateUI();
    }

    // Finished game
    public void winGame() {
        soundManager.victorySFX();
        gameWon = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        uiManager.updateUI();
    }

    // Update UI
    public void startDay() {
        nightStart = false;
        inDialogue = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        uiManager.updateUI();
    }

    // Return to main menu
    public void loadMenu() {
        Time.timeScale = 1;
        blackscreen = true;
        uiManager.updateUI();
        Cursor.visible = true;
        gamePaused = false;
        gameOver = false;
        soundManager.stopBackgroundMusic();
        sceneNotManager.returnToMainMenu();
    }

    // Upgrade abilitys
    public void upgradeAbiltiies(Abilities ability) {
        if (upgrades > 0) {
            switch (ability) {
                // Cloak upgrade
                case Abilities.Cloak:
                    // Minus upgrade point, upgrade ability, add point for max check, add it to player
                    if (numCloakUpgrades < maxCloakUpgrades) {
                        upgrades--;
                        cloakDuration += 0.6f;
                        numCloakUpgrades++;
                        player.maxCloakDuration = cloakDuration;
                    }
                    break;
                // Shadow upgrade
                case Abilities.Shadow:
                    // Minus upgrade point, upgrade ability, add point for max check, add it to player
                    if (numShadowUpgrades < maxShadowUpgrades) {
                        upgrades--;
                        shadowDuration += 0.6f;
                        numShadowUpgrades++;
                        player.maxshadowDuration = shadowDuration;
                    }
                    break;
                // Dash upgrade
                case Abilities.Dash:
                    // Minus upgrade point, upgrade ability, add point for max check, add it to player
                    if (numDashUpgrades < maxDashUpgrades) {
                        upgrades--;
                        dashDuration += 0.02f;
                        numDashUpgrades++;
                        player.dashTime = dashDuration;
                    }
                    break;
                // Hypnosis upgrade
                case Abilities.Hypnosis:
                    // Minus upgrade point, upgrade ability, add point for max check, add it to player
                    if (numStunUpgrades < maxStunUpgrades) {
                        upgrades--;
                        stunDuration += 1.0f;
                        numStunUpgrades++;
                        player.maxHypnoDuration = stunDuration;
                    }
                    break;
                // Consume upgrade
                case Abilities.Consume:
                    // Minus upgrade point, upgrade ability, add point for max check, add it to player
                    if (numConsumeUpgrades < maxConsumeUpgrades) {
                        upgrades--;
                        consumeTime -= 0.125f;
                        numConsumeUpgrades++;
                        player.maxConsume = consumeTime;
                    }
                    break;
                // Health upgrade
                case Abilities.Health:
                    // Minus upgrade point, upgrade ability, add point for max check, add it to player
                    if (numHealthUpgrades < maxHealthUpgrades) {
                        upgrades--;
                        maxHealth += 1;
                        numHealthUpgrades++;
                        player.maxHealth = maxHealth;
                        player.health = maxHealth;
                    }
                    break;
            }
        }
    }
}