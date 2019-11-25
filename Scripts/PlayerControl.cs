using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/************************************************************************************************************
Class for handling player movement, abilities, health, and other things affecting the player
************************************************************************************************************/
public class PlayerControl : MonoBehaviour {
    // Variables for the players health
    // The max is set by the manager in order to allow for upgrades
    [Header("Health")]
    [Tooltip("The current health of the vampire")]
    public int health = 6;
    [Tooltip("The maximum health of the vampire")]
    public int maxHealth = 6;

    // Movement variables for the player
    [Header("Movement")]
    [Tooltip("The current speed the vampire is moving at")]
    public float moveSpeed;
    [Tooltip("The default speed the vampire moves at")]
    public float defaultSpeed = 5;
    [Tooltip("The speed the vampire moves at when slowed")]
    public float slowedSpeed = 3;
    [Tooltip("Is the player sprinting")]
    public bool sprinting = false;

    // The modifier for the horizontal camera movement
    [Tooltip("The speed at which you can rotate player")]
    public float cameraSens = 1;

    // A reference to the ui manager
    UIManager uiManager = null;
    // A reference to the game manager
    GameManager gameManager = null;
    // A reference to the player's camera
    CameraControl cameraControl = null;

    // Particle Sytstems
    // Particle for the vampire standing in light
    ParticleSystem lightP;
    // Particle for when the vampire begins eating someone
    ParticleSystem consumeP;
    // Particle for when the vampire cloaks
    ParticleSystem cloakP;
    // Particle for the vampire uses the shadow ability
    ParticleSystem shadowP;
    // Particle for the vampire dash ability
    ParticleSystem dashP;
    // Particle for vampire's hypnotise ability
    ParticleSystem hypnoP;

    // Reference to the vampire's Animator
    Animator animator = null;

    // Reference to the sound manager
    SoundManager soundManager = null;

    // Abilities
    [Space(10)]
    [Tooltip("Whether the vampire is currently in light")]
    public bool inLight = false;
    // The current modifier for the ability cooldowns
    float cooldownModifier = 1.0f;
    // Crosses
    [Tooltip("The amount of time near a cross before it damages the player")]
    public float crossDamageTimerMax = 5.0f;
    // The current timer for the cross
    float crossDamageTimer = 5.0f;

    // Consuming
    // Is the player currently grappling
    bool grappling = false;
    // The villager that is currently being grappled
    BaseControl grappledObject = null;
    // When consume reaches this value, a heart is consumed
    // Counts up from zero
    [Header("Consume Ability")]
    [Tooltip("The cooldown timer for the Consume ability.\n When the timer is below zero the ability cannot be used")]
    public float consume = 1.5f;
    [Tooltip("The maximum value the cooldown timer can reach")]
    public float maxConsume = 1.5f;

    // Cloaking
    [HideInInspector]
    public bool cloaked = false;
    [Header("Cloak Ability")]
    [Tooltip("The cooldown timer for the Cloak ability.\n When the timer is below zero the ability stops.\n When the timer is below maxCloaked the ability cannot be activated")]
    // Counts up from zero, cannot cloak if below rechargeTimer
    public float cloakCoolTimer = 5.0f;
    [Tooltip("The maximum value the cooldown timer can reach")]
    // The maximum amount cloakedTimer will increase to
    public float maxCloakedTimer = 5.0f;

    // The amount of time the cloak is active for
    // Set by the gamemanager to allow for upgrades
    [HideInInspector]
    public float maxCloakDuration = 3.0f;
    // The current amount of time the cloak has been active for
    float cloakDurationTimer = 3.0f;

    // Shadow
    [Tooltip("The shadow prefab which is spawned by the ability")]
    public GameObject shadowPrefab = null;
    // The shadow object when it is created
    [HideInInspector]
    public GameObject shadowObject = null;
    // Whether the shadow is active or not
    [HideInInspector]
    public bool shadowActive = false;
    // Counts up from zero, cannot use shadow if below zero
    [Header("Shadow Ability")]
    [Tooltip("The cooldown timer for the Shadow ability.\n When the timer is below zero the ability cannot be activated")]
    public float shadowCoolTimer = 5.0f;
    // The maximum amount shadowTimer can increase to
    [Tooltip("The maximum value the cooldown timer can reach")]
    public float maxShadowCool = 5.0f;

    // The amount of time the shadow is active for
    // Set by the gamemanager to allow for upgrades
    [HideInInspector]
    public float maxshadowDuration = 5.0f;
    // The current amount of time the shadow has been active for
    float shadowDurationTimer = 5.0f;

    // Dash
    // Is the player currently dashing
    bool dashing = false;
    [Header("Dash Ability")]
    // Counts up from zero, cannot use shadow if below zero
    [Tooltip("The cooldown timer for the Dash ability.\n When the timer is below zero the ability cannot be activated")]
    public float dashTimer = 5.0f;
    [Tooltip("The maximmum value the cooldown timer can reach")]
    // The maximum amount batTimer can increase to
    public float maxDashTimer = 5.0f;
    [Tooltip("The amount of time the dash is active for")]
    public float dashTime = 0.1f;
    [Tooltip("The amount the base speed is multiplied by when dashing")]
    public float dashSpeed = 25.0f;

    // Hypnotise
    [Header("Hypnotise Ability")]
    [Tooltip("Whether the vampire is currently hypnotising someone")]
    public bool hypno = false;
    // Counts up from zero, cannot use hypnotise if below zero
    [Tooltip("The cooldown timer for the Hypnotise ability.\n When the timer is below zero the ability cannot be activated")]
    public float hypnoCoolTimer = 5.0f;
    // The range that the vampire can hypnotise 
    [Tooltip("The range at which the vampire can hypnotise an enemy")]
    public float hypnoRange = 5.0f;
    // The maximum amount hypnotimer can increase to
    [Tooltip("The maximum value the cooldown timer can reach")]
    public float maxHypnoCool = 5.0f;
    // Stores which enemy the player has targeted
    List<BaseControl> hypnoEnemies = new List<BaseControl>();

    // The amount of time the hypnosis is active for
    // Set by the gamemanager to allow for upgrades
    [HideInInspector]
    public float maxHypnoDuration = 5.0f;
    // The current amount of time the hypnosis has been active for
    float hypnoDurationTimer = 5.0f;

    // Sunburn count
    [HideInInspector]
    public float sunburn = 0;

    // Stores the direction the cross will move the player
    // This is stored so that the random directions will be combined and keep a roughly similar direction
    Vector3 crossMove = new Vector3();

    // Whether the trigger was pressed this frame
    bool triggerDown = false;

    // Initialisation function called by the gamemanager
    public void init() {
        // Set GameManager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        // Get base values from manager
        maxCloakDuration = gameManager.cloakDuration;
        maxshadowDuration = gameManager.shadowDuration;
        dashTime = gameManager.dashDuration;
        maxHypnoDuration = gameManager.stunDuration;
        maxConsume = gameManager.consumeTime;
        maxHealth = gameManager.maxHealth;
        health = maxHealth;

        // Set UI
        uiManager = gameManager.uiManager;
        uiManager.updateHealthBar(health, maxHealth);

        // Set Camera
        cameraControl = gameManager.townCamera.GetComponent<CameraControl>();
        
        // Set Movement Speed
        moveSpeed = defaultSpeed;

        // Set Sound
        soundManager = gameManager.soundManager;

        animator = GetComponentInChildren<Animator>();
 
        // Assigning particles
        foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>()) {
            if (particles.name.Contains("Light")) {
                lightP = particles;
            }
            else if (particles.name.Contains("Death")) {
                lightP = particles;
            }
            else if (particles.name.Contains("Consume")) {
                consumeP = particles;
            }
            else if (particles.name.Contains("Cloak")) {
                cloakP = particles;
            }
            else if (particles.name.Contains("Shadow")) {
                shadowP = particles;
            }
            else if (particles.name.Contains("Dash")) {
                dashP = particles;
            }
            else if (particles.name.Contains("Hypnosis")) {
                hypnoP = particles;
            }
        }
    }

    // Update function called every frame
    void Update() {
        Shader.SetGlobalVector("_PlayerPos", transform.position);

        if (gameManager != null && (SceneManager.GetActiveScene().name == "Town" || SceneManager.GetActiveScene().name == "Tutorial")) {
            if (grappling == false) { 
                /*****************************************************************************************
                 Movement Method 3
                 Rotating Camera 
                 - WASD
                 - Mouse for direction
                 - Follow camera direction
                 ******************************************************************************************/
                // Movement
                float rawVert = Input.GetAxis("VerticalMove");
                if (Input.GetAxis("Sprint") != 0) { 
                    // Sprinting
                    if (rawVert > 0) {
                        rawVert *= 2;
                        sprinting = true;
                    }
                }
                else {
                    sprinting = false;
                }
                Vector3 vert = transform.TransformDirection(Vector3.forward) * rawVert;
                float rawHori = Input.GetAxis("HorizontalMove");
                Vector3 hori = transform.TransformDirection(Vector3.right) * rawHori;
                Vector3 delta = hori + vert;

                // Values for animator
                if (delta != Vector3.zero) {
                    animator.SetBool("Moving", true);
                }
                else {
                    animator.SetBool("Moving", false);
                }
                animator.SetFloat("VelX", rawHori);
                animator.SetFloat("VelY", rawVert);

                // The actual movement
                GetComponent<CharacterController>().Move(delta * moveSpeed * Time.deltaTime);

                // Camera
                transform.Rotate(0, (Input.GetAxis("HorizontalLook") * (cameraSens * 100) * Time.deltaTime), 0);

                if (gameManager.gamePaused == false) {
                    // Makes the camera follow the player
                    gameManager.townCamera.transform.position = new Vector3(Mathf.Cos(-transform.rotation.eulerAngles.y * Mathf.Deg2Rad + (Mathf.PI * 0.5f)), 0, Mathf.Sin(-transform.rotation.eulerAngles.y * Mathf.Deg2Rad + (Mathf.PI * 0.5f))) * cameraControl.cameraOffset.z;
                    gameManager.townCamera.transform.position += new Vector3(0, cameraControl.cameraOffset.y, 0) + transform.position;
                    gameManager.townCamera.transform.LookAt(transform);
                }
            }

            // Consuming
            if (Input.GetAxis("Consume") == 1 && triggerDown == false) {
                triggerDown = true;

                Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
                RaycastHit hit;
                // Hitting an enemy
                if (Physics.Raycast(ray, out hit)) {
                    // In range of target
                    if (hit.distance <= 1.5f) {
                        BaseControl enemy = hit.transform.GetComponent<BaseControl>();
                        // Hitting an enemy
                        if (enemy != null) {
                            Vector3 direc = (hit.transform.position - transform.position).normalized;
                            // Behind the enemy
                            if (Vector3.Dot(direc, hit.transform.forward) > 0) {
                                // Can't consume slayer
                                if (enemy.GetComponent<SlayerControl>() == null) {
                                    animator.SetBool("Consuming", true);

                                    // Play bite SFX
                                    soundManager.playerBiteSFX();

                                    // Play damage particles 
                                    enemy.damageParticlePlay();

                                    grappling = true;
                                    grappledObject = enemy;
                                    enemy.grappled = true;
                                    consume = maxConsume;
                                }
                                else {
                                    // Alerts slayer
                                    enemy.alertness = 9;
                                }
                            }
                        }
                    }
                }
            }
            // Initial consuming
            else if (Input.GetAxis("Consume") == 0 && triggerDown == true) {
                triggerDown = false;
                animator.SetBool("Consuming", false);
                if (grappling == true) {
                    grappling = false;
                    grappledObject.grappled = false;
                    grappledObject.alertness = 9;
                    grappledObject.Disturbance = transform.position;
                    grappledObject = null;
                }
            }
            // While consuming
            if (grappling == true) {
                consume -= Time.deltaTime;
                if (consume <= 0) {
                    // Play consume SFX
                    soundManager.playerConsumeSFX();

                    healDamage(1);
                    grappledObject.takeDamage(1);
                    consume = maxConsume;

                    // On victim death
                    if (grappledObject.health <= 0) {
                        grappledObject = null;
                        grappling = false;
                        animator.SetBool("Consuming", false);
                    }
                }
            }

            // Cloak Timings
            if (cloaked == true) {
                cloakDurationTimer -= cooldownModifier * Time.deltaTime;
                if (cloakDurationTimer <= 0) {
                    cloak(true);
                }
            }
            else {
                if (cloakCoolTimer < maxCloakedTimer) {
                    cloakCoolTimer += Time.deltaTime;
                }
            }

            // Shadow Timings
            if (shadowActive == true) {
                shadowObject.transform.Translate(Vector3.forward * 2.0f * Time.deltaTime, Space.Self);

                shadowDurationTimer -= cooldownModifier * Time.deltaTime;
                if (shadowDurationTimer <= 0) {
                    shadow(true);
                }
            }
            else {
                if (shadowCoolTimer < maxShadowCool) {
                    shadowCoolTimer += Time.deltaTime;
                }
            }

            // Dash Timing
            dashTimer = Mathf.Min(dashTimer + cooldownModifier * Time.deltaTime, maxDashTimer);
            if (dashTimer >= dashTime) {
                dashing = false;
                moveSpeed = defaultSpeed;
                dashP.Stop();
                animator.SetBool("Dash", false);
            }

            // Hynpotise Timing
            if (hypno == true) {
                hypnoDurationTimer -= cooldownModifier * Time.deltaTime;
                if (hypnoDurationTimer <= 0) {
                    hypnotise(true);
                }
            }
            else {
                if (hypnoCoolTimer < maxHypnoCool) {
                    hypnoCoolTimer += Time.deltaTime;
                }
            }

            // Sunburns
            if (sunburn >= 10) {
                sunburn -= 10;
                takeDamage(1);
            }
            // Cross damage
            if (crossDamageTimer <= crossDamageTimerMax) {
                crossDamageTimer = Mathf.Min(crossDamageTimer + Time.deltaTime, crossDamageTimerMax);
            }

            // Ability Inputs
            if (grappling == false && Time.timeScale == 1 && gameManager.inDialogue == false) {
                if (Input.GetAxis("Cloak") != 0) {
                    cloak();
                }
                if (Input.GetAxis("Shadow") != 0) {
                    shadow();
                }
                if (Input.GetAxis("Dash") != 0) {
                    dash();
                }
                if (Input.GetAxis("Hypnotise") != 0) {
                    hypnotise();
                }
            }

            // Update cooldowns
            List<float> cooldowns = new List<float>();
            cooldowns.Add(1 - cloakCoolTimer / maxCloakedTimer);
            cooldowns.Add(1 - shadowCoolTimer / maxShadowCool);
            cooldowns.Add(1 - dashTimer / maxDashTimer);
            cooldowns.Add(1 - hypnoCoolTimer / maxHypnoCool);

            uiManager.updateAbiltyCooldowns(cooldowns);
        }
    }

    // Handles taking damage
    public void takeDamage(int damage) {
        // Play damage SFX
        soundManager.playerDamageSFX(soundManager.playerDamage.Length);

        health -= damage;
        uiManager.updateHealthBar(health, maxHealth);

        // Vampire dies
        if (health <= 0) {
            // Play death SFX
            soundManager.playerDeathSFX();
            // Restart Level 
            gameManager.loseGame();
        }
    }

    // Handles healing damage
    public void healDamage(int heal) {
        health = Mathf.Min(health + heal, maxHealth);
        uiManager.updateHealthBar(health, maxHealth);
    }

    // Handles enabling and disabling the cloak ability
    public void cloak(bool deactivate = false) {
        if (deactivate == true && cloaked == true) {
            cloaked = false;

            // Stop particle system
            if (cloakP.isPlaying) {
                cloakP.Stop();
                cloakP.Clear();
            }

            // Make opaque
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>()) {
                renderer.enabled = true;
            }
        }
        else if (cloakCoolTimer >= maxCloakedTimer && cloaked == false) {
            disableAbilities();
            cloakDurationTimer = maxCloakDuration;
            cloakCoolTimer = 0;
            cloaked = true;

            // Play particle system
            if (!cloakP.isPlaying) {
                cloakP.Clear();
                cloakP.Play();
            }

            // Play cloak SFX
            soundManager.playerInvisSFX();

            // Make transparent
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>()) {
                if (!renderer.name.Contains("Mark")) {
                    renderer.enabled = false;
                }
            }
        }
    }

    // Handles enabling and disabling the shadow ability
    public void shadow(bool deactivate = false) {
        if (deactivate == true && shadowActive == true) {
            // Remove old shadow
            shadowActive = false;
            Destroy(shadowObject);
            shadowObject = null;

            // Turn shadows back on
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
        else if (shadowCoolTimer >= maxShadowCool && shadowActive == false) {
            disableAbilities();
            shadowDurationTimer = maxshadowDuration;
            shadowCoolTimer = 0;
            // Create shadow
            shadowActive = true;

            // Play shadow particle
            if (!shadowP.isPlaying) {
                shadowP.Play();
            }

            animator.SetTrigger("Shadow");

            // Play shadow SFX
            soundManager.playerShadowSFX();

            // Makes shadow transparent
            shadowObject = Instantiate(shadowPrefab, transform.position, transform.rotation);

            Color newColour = Color.black;
            newColour.a = 0.75f; 
            // Creates shadow vampire
            foreach (Renderer renderer in shadowObject.GetComponentsInChildren<Renderer>()) {
                renderer.material.color = newColour;
            }
        }
    }

    // Handles enabling and disabling the bat Dash ability
    public void dash() {
        if (dashTimer >= maxDashTimer && GetComponent<CharacterController>().velocity != Vector3.zero) {
            disableAbilities();
            animator.SetBool("Dash", true);

            dashTimer = 0;

            // Remove particles
            if (dashP.isPlaying) {
                dashP.Stop();
            }
            // Spawn particles
            if (!dashP.isPlaying) {
                dashP.Play();
            }

            moveSpeed = defaultSpeed * dashSpeed;

            soundManager.playerDashSFX();

            dashing = true;
        }
    }  

    // Handles enabling and disabling the Hypnotise ability
    public void hypnotise(bool deactivate = false) {
        if (deactivate == true && hypno == true) {
            // Reset Hypnotise
            hypno = false;

            // Stop particle system
            if (hypnoP.isPlaying) {
                hypnoP.Stop();
            }

            if (hypnoEnemies.Count != 0) {
                foreach (BaseControl enemy in hypnoEnemies) { 
                    enemy.hypnotised = false;
                }
            }
        }
        else if (hypnoCoolTimer >= maxHypnoCool && hypno == false) {
            disableAbilities();
            hypno = false;

            for (int i = -3; i <= 3; i++) {
                Vector3 direc = transform.forward * hypnoRange;
                direc += transform.right * i;
                Ray ray = new Ray(transform.position, direc);
                RaycastHit hit;

                Debug.DrawLine(transform.position, transform.position + direc, Color.red, 1);

                if (Physics.Raycast(ray, out hit, hypnoRange)){
                    if (hit.transform.GetComponentInParent<BaseControl>() != null) {
                        hypno = true;

                        hypnoEnemies.Add(hit.transform.GetComponentInParent<BaseControl>());
                        hypnoEnemies[hypnoEnemies.Count - 1].hypnotised = true;
                    }
                }
            }

            if (hypno == true) {
                hypnoCoolTimer = 0;
                hypnoDurationTimer = maxHypnoDuration;

                animator.SetTrigger("Hypnotise");

                soundManager.playerHypnoSFX();

                // Spawn particles
                if (!hypnoP.isPlaying) {
                    hypnoP.Play();
                }
            }
        }
    }

    // Function for disabling abliltys while others are in use
    void disableAbilities() {
        if (cloaked == true) {
            cloak(true);
        }
        if (shadowActive == true) {
            shadow(true);
        }
        if (dashing == true) {
            dashing = false;
            dashP.Stop();
            moveSpeed = defaultSpeed;
        }
        if (hypno == true) {
            hypnotise(true);
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Vampire triggers escape
        if (other.tag == "Escape") {
            // Win game if finished all nights
            if (gameManager.dayIndex >= gameManager.objectiveList.Count - 1) {
                gameManager.winGame();
            }
            else if (SceneManager.GetActiveScene().name == "Tutorial") {
                gameManager.loadMenu();
            }
            // Move to next night
            else {
                gameManager.nightEnd();
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        // Vampire enters light        
        if (other.tag == "Detterent") {
            if (other.name.Contains("Light")) {
                //if (other.transform.parent.GetComponent<GuardControl>() != null) {
                if (other.transform.GetComponentInParent<GuardControl>() != null) {
                    Ray ray = new Ray(transform.position, other.transform.parent.position - transform.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("Canvas"))) {
                        if (hit.transform.parent == null) {
                            return;
                        }
                        else if (hit.transform.parent != other.transform.parent) {
                            if (inLight == true) {
                                inLight = false;

                                // Default Cooldowns 
                                cooldownModifier = 1.0f;

                                // Remove visual cue
                                if (lightP.isPlaying) {
                                    lightP.Stop();
                                }
                            }
                            return;
                        }
                    }
                }
                inLight = true;

                if (dashing == false) {
                    moveSpeed = slowedSpeed;
                }

                // Cool downs are longer
                cooldownModifier = 2.0f;

                // Visual cue
                if (!lightP.isPlaying) {
                    lightP.Play();
                }
            }
            // In range of a cross
            else if (other.transform.name.Contains("Cross")) {
                if (grappling == false) {
                    crossMove += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

                    if (new Vector2(Input.GetAxis("HorizontalMove"), Input.GetAxis("VerticalMove")).sqrMagnitude == 0) {
                        GetComponent<CharacterController>().Move(crossMove.normalized * moveSpeed * 0.05f * Time.deltaTime);
                    }
                    else {
                        GetComponent<CharacterController>().Move(crossMove.normalized * moveSpeed * 0.25f * Time.deltaTime);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        // Vampire exits light
        if (other.tag == "Detterent") {
            if (other.name.Contains("Light")) {
                if (inLight == true) {
                    inLight = false;

                    if (dashing == false) { 
                        moveSpeed = defaultSpeed;
                    }

                    // Default Cooldowns 
                    cooldownModifier = 1.0f;

                    // Defualt movement speed
                    moveSpeed = defaultSpeed;

                    // Remove visual cue
                    if (lightP.isPlaying) {
                        lightP.Stop();
                    }
                }
            }
            else if (other.name.Contains("Cross")) {
                // Stop taking damage
                crossMove = Vector3.zero;
            }
        }
    }
}
