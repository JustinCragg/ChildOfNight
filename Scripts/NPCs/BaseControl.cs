using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/************************************************************************************************************
Base class for all NPCs
Handles health, random movement and visibility of the player
************************************************************************************************************/
public class BaseControl : MonoBehaviour {
    // What type of villager this is
    protected GoalType charType;

    // Reference to the camera this npc uses
    Camera npcCamera = null;

    // Reference to the NavMeshAgent for this character
    protected NavMeshAgent agent = null;
    // Reference to the animator for this character
    protected Animator animator = null;
    // Reference to the player
    [HideInInspector]
    public PlayerControl player = null;
    // Reference to GameManager
    [HideInInspector]
    public GameManager gameManager = null;

    // The audio source this npc uses
    AudioSource audioSource = null;
    // The particles for the npc
    ParticleSystem deathParticle;
    ParticleSystem damageParticle;

    [Header("Base Stats")]
    [Tooltip("The current health of the NPC")]
    // Current health
    public int health = 1;
    [Tooltip("The max health of the NPC")]
    // Max health
    public int maxHealth = 1;

    // Is the npc fleeing
    protected bool fleeing = false;

    [Tooltip("Is the NPC currently being grappled")]
    // Whether the character is currently being consumed
    public bool grappled = false;

    [Tooltip("Has the NPC been hypnotised")]
    // Whether the character is currently being consumed
    public bool hypnotised = false;

    [Tooltip("The range at which those within will be alerted")]
    public float alertRange = 25.0f;

    [Tooltip("Is the player currently visible by this NPC")]
    // Whether the player is visible or not
    public bool playerVisible = false;

    [Header("AI Stuff")]
    [Tooltip("The current level of alertness the NPC is at.\n Where 0 is less alert")]
    [Range(0,9)]
    // The current level of alertness the character is in (0-9)?
    public int alertness = 0;
    // The counter used to increase/decrease alertness
    [Tooltip("Keeps track of whether the NPC's alterness is increasing or decreasing")]
    [Range(-1.0f, 1.0f)]
    public float alertCounter = 0;
    // The minimum amount the alertness can be reduced to after begin raised above it
    [Tooltip("The minimum amount the alertness can return to over time")]
    [Range(0,9)]
    public int minCalmDown = 5;

    [Tooltip("The rate the alertness level increases at")]
    public float alertnessIncrease = 2.5f;
    [Tooltip("The rate the alertness level decreases at")]
    public float alertnessDecrease = 0.3f;

    [Header("Move Speeds")]
    [Tooltip("The base speed for the npc")]
    public float normalSpeed = 1.0f;
    [Tooltip("The speed for the npc when it needs to move faster")]
    public float fastSpeed = 2.0f;
    [Tooltip("The speed for the npc when it needs to move slower")]
    public float slowSpeed = 0.5f;

    [Tooltip("Range that player can be seen by NPC's")]
    public float basePassiveVisionRange = 3;
    // The current passive vision range for the npc
    float passiveVisionRange = 3;

    // The location of a disturbance
    [Tooltip("The location of the most recent 'disturbance' the NPC will react to")]
    Vector3 disturbance = Vector3.positiveInfinity;
    public Vector3 Disturbance {
        get {
            // returns the player's position if not invalid
            if (disturbance.x == Mathf.Infinity) {
                disturbance = player.transform.position;
                return player.transform.position;
            }
            else {
                return new Vector3(disturbance.x, transform.position.y, disturbance.z);
            }
        }
        set {
            disturbance = value;
        }
    }

    // Initialiser
    public virtual void init() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        npcCamera = GetComponentInChildren<Camera>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected virtual void Update() {
        // Reveal the player if they are cloaked
        if (Vector3.Distance(transform.position, player.transform.position) < 1.5f) {
            if (player.cloaked == true) {
                player.cloak(true);
            }
        }

        // Update animations
        if (animator != null) {
            if (agent.velocity == Vector3.zero) {
                animator.SetBool("Moving", false);
            }
            else {
                animator.SetBool("Moving", true);
            }
        }
    }

    // Takes damage
    public void takeDamage(int damage) {
        health -= damage;

        // Play damage sound
        if (gameObject.tag == "Guard") {
            gameManager.soundManager.guardDamageSFX(audioSource);
        }
        else if (gameObject.tag == "Drunkard") {
            gameManager.soundManager.drunkardDamageSFX(audioSource);
        }
        else if (gameObject.tag == "Noble") {
            gameManager.soundManager.nobleDamageSFX(audioSource);
        }
        
        // NPC bitten
        if (maxHealth > 1) {
            animator.SetTrigger("Damaged");
        }

        // NPC dies
        if (health <= 0) {
            if (animator != null) {
                animator.SetTrigger("Dead");
                // Stop NPC from moving
                agent.SetDestination(transform.position);
                // Remove an objective
                gameManager.consumedObjective(charType);
                
                // Play death sound
                if (gameObject.tag == "Guard") {
                    gameManager.soundManager.guardDeathSFX(audioSource);
                }
                else if (gameObject.tag == "Drunkard") {
                    gameManager.soundManager.drunkardDeathSFX(audioSource);
                }
                else if (gameObject.tag == "Noble") {
                    gameManager.soundManager.nobleDeathSFX(audioSource);
                }
            }
        }
    }

    // NPC damage particles
    public void damageParticlePlay() {
        foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>()) {
            if (particles.name.Contains("DamageParticle")) {
                damageParticle = particles;
                damageParticle.Play();
            }
        }
    }

    // Called at end of animation and gets rid of NPC
    public void destroyNPC() {
        deathParticle = gameObject.GetComponentInChildren<ParticleSystem>();
        deathParticle.transform.parent = null;
        deathParticle.Play();
        Destroy(gameObject);
    }

    // Updates the alertness of the noble
    protected void updateAlertness() {
        Vector3 playerPos = isPlayerVisible();

        if (playerPos.y != Mathf.Infinity) {
            float value = (1 - (Vector3.Distance(transform.position, playerPos) / npcCamera.farClipPlane)) * 5.0f;
            // Player is visible
            if (player.inLight == true) {
                alertCounter += Time.deltaTime * alertnessIncrease + value;
                alertCounter = Mathf.Clamp(alertCounter, -1.0f, 1.0f);
            }            
            else if (player.sprinting == true) {
                passiveVisionRange = basePassiveVisionRange * 2.0f;
                alertCounter += Time.deltaTime * alertnessIncrease + value;
                alertCounter = Mathf.Clamp(alertCounter, -1.0f, 1.0f);
            }
            else {
                passiveVisionRange = basePassiveVisionRange;
                alertCounter += Time.deltaTime * (alertnessIncrease + value) * 0.5f;
                alertCounter = Mathf.Clamp(alertCounter, -1.0f, 1.0f);
            }
        }
        else {
            // Player not visible
            alertCounter -= Time.deltaTime * alertnessDecrease;
            alertCounter = Mathf.Clamp(alertCounter, -1.0f, 1.0f);
        }
        if (alertCounter <= -1) {
            // decrease alert
            if (alertness >= minCalmDown) {
                alertness = Mathf.Max(minCalmDown, alertness - 1);
            }
            else {
                alertness = Mathf.Max(0, alertness - 1);
            }

            alertCounter = 1;
        }
        else if (alertCounter >= 1) {
            // increase alert
            alertness = Mathf.Min(9, alertness + 1);
            alertCounter = -1;
        }
    }

    // Checks whether the player is visible to the character or not
    protected virtual Vector3 isPlayerVisible() {
        playerVisible = false;
        // Does the shadow (ability) exist
        if (player.shadowActive == true) {
            Vector3 point = player.shadowObject.transform.position;
            Vector3 origin = transform.position + transform.forward * passiveVisionRange * 0.5f;
            if (Vector3.Distance(origin, point) <= passiveVisionRange) {
                disturbance = point;
                playerVisible = true;
                return point;
            }
            // Converts the shadow pos to the character's Camera space
            Vector3 pos = npcCamera.WorldToViewportPoint(point);
            // Is the point in the frustrum
            if (pos.x >= 0 && pos.x <= 1) {
                if (pos.y >= 0 && pos.y <= 1) {
                    if (pos.z >= 0) {
                        // Draw a ray to the point
                        Ray ray = new Ray(transform.position + Vector3.up, point - transform.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit)) {
                            if (hit.transform.tag == "Player") {
                                disturbance = point;
                                playerVisible = true;
                                return point;
                            }
                        }
                    }
                }
            }
        }
        // Is the player not cloaked
        if (player.cloaked == false) {
            Vector3 point = player.transform.position;
            Vector3 origin = transform.position + transform.forward * passiveVisionRange * 0.5f;
            if (Vector3.Distance(origin, point) <= passiveVisionRange) {
                disturbance = point;
                playerVisible = true;
                return point;
            }
            // Converts the player pos to the character's Camera space
            Vector3 pos = npcCamera.WorldToViewportPoint(point);
            // Is the point in the frustrum
            if (pos.x >= 0 && pos.x <= 1) {
                if (pos.y >= 0 && pos.y <= 1) {
                    if (pos.z >= 0) {
                        // Draw a ray to the point
                        Ray ray = new Ray(transform.position + Vector3.up, point - transform.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit)) {
                            if (hit.transform.tag == "Player") {
                                disturbance = point;
                                playerVisible = true;
                                return point;
                            }
                        }
                    }
                }
            }
        }
        return Vector3.positiveInfinity;
    }

    // Increases the alertness of nearby npcs
    protected void alert() {
        foreach (BaseControl npc in FindObjectsOfType<BaseControl>()) {
            if (npc != this) {
                if (Vector3.Distance(transform.position, npc.transform.position) <= alertRange) {
                    npc.alertCounter += alertnessIncrease * Time.deltaTime;
                }
            }
        }
    }

    // Sets the NavMeshAgent destination
    protected void setDestination(Vector3 destination) {
        if (agent.destination != destination) {
            agent.SetDestination(destination);
        }
    }
}
