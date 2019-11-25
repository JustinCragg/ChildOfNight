using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum which holds each of the different states the slayer can be in
public enum SlayerState { WANDERING, INVESTIGATING, SEARCHING };

/************************************************************************************************************
Class which handles the slayer
Including the AI, movement and attacking
************************************************************************************************************/
public class SlayerControl : BaseControl {
    [Header("Slayer Stuff")]
    [Tooltip("The amount of time between Slayer attacks")]
    public float attackSpeed = 3.0f;
    [Tooltip("The amount of damage inflicted by the Slayer")]
    public int attackDamage = 1;
    // The current timer for the attacks
    float attackCool = 0;

    [Tooltip("The current state the AI is in")]
    public SlayerState slayerState = SlayerState.WANDERING;

    // The radius of the circle the slayer patrols on
    float wanderRadius = 15;
    // The number of segments to the circle
    int numWanderSegments = 8;
    // The size of the segment
    float wanderSegment;
    // The current segment the slayer is on
    int currentSegment = 0;

    public override void init() {
        base.init();
        charType = GoalType.SLAYER;
        wanderSegment = (2 * Mathf.PI) / numWanderSegments;
    }

    protected override void Update() {
        if (gameManager != null) {
            base.Update();

            // Updates values
            updateAlertness();
            updateState();

            Vector3 velocity = Vector3.zero;
            switch (slayerState) {
                // The slayer's wander state
                // Moves around in an approximate circle around the town, with a bias towards the player
                case SlayerState.WANDERING:
                    agent.speed = normalSpeed;

                    if (agent.remainingDistance <= 1.5f) {
                        currentSegment++;
                        if (currentSegment >= numWanderSegments) {
                            currentSegment = 0;
                        }

                        float rotGoal = currentSegment * wanderSegment;
                        Vector3 direc = new Vector3(Mathf.Cos(rotGoal), 0, Mathf.Sin(rotGoal)).normalized * wanderRadius;
                        direc += (player.transform.position - transform.position).normalized * Random.Range(0, 10);
                        direc.y = 0;
                        setDestination(direc);
                    }

                    // Updates animator
                    velocity = agent.velocity.normalized;
                    velocity = transform.InverseTransformDirection(velocity);
                    animator.SetFloat("VelX", velocity.x);
                    animator.SetFloat("VelY", velocity.z);
                    break;
                // The slayer's investigating state
                // The slayer moves towards the location of the disturbance he 'heard' then looks around that area passively
                case SlayerState.INVESTIGATING:
                    agent.speed = slowSpeed;

                    if (playerVisible == true) {
                        gameManager.soundManager.slayerAlertSFX(GetComponentInChildren<AudioSource>());
                        setDestination(Disturbance);
                    }
                    else {
                        // Head towards Disturbance
                        if (Vector3.Distance(transform.position, Disturbance) > 1) {
                            setDestination(Disturbance);
                        }
                        else {
                            // Rotate around searching
                            transform.Rotate(Vector3.up, 10 * Time.deltaTime, Space.World);
                        }
                    }

                    // Updates animator
                    velocity = agent.velocity.normalized;
                    velocity = transform.InverseTransformDirection(velocity);
                    animator.SetFloat("VelX", velocity.x);
                    animator.SetFloat("VelY", velocity.z);
                    break;
                // The slayer's searching state
                // The slayer moves towards the location of the disturbance he 'heard' then actively looks around that area for the player
                case SlayerState.SEARCHING:
                    agent.speed = fastSpeed;

                    // If player is visible
                    if (playerVisible == true) {
                        // Move towards player
                        if (Vector3.Distance(transform.position, Disturbance) > 1.5f) {
                            setDestination(Disturbance);
                        }
                        else {
                            attack();
                        }
                        
                    }
                    // If player is not visible
                    else {
                        // Randomly move around in area around Disturbance
                        if (agent.remainingDistance <= 1.0f) {
                            Vector3 rand = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 10)).normalized * 10.0f;
                            rand += Disturbance;

                            setDestination(rand);
                        }
                    }

                    // Updates animator
                    velocity = agent.velocity.normalized * 2.0f;
                    velocity = transform.InverseTransformDirection(velocity);
                    animator.SetFloat("VelX", velocity.x);
                    animator.SetFloat("VelY", velocity.z);
                    break;
            }
            if (attackCool > 0 && grappled == false) {
                attackCool -= Time.deltaTime;
            }
        }
    }

    // Determines what levels of alertness trigger which states
    void updateState() {
        switch (alertness) {
            // Wandering
            case 0:
            case 1:
            case 2:
                slayerState = SlayerState.WANDERING;
                break;
            // Investigating
            case 3:
            case 4:
            case 5:
                slayerState = SlayerState.INVESTIGATING;
                break;
            // Searching
            case 6:
            case 7:
            case 8:
            case 9:
                slayerState = SlayerState.SEARCHING;
                break;
        }
    }

    // Stops moving and attacks the target
    void attack() {
        Vector3 dir = Disturbance - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, fastSpeed * Time.deltaTime);

        setDestination(transform.position);
        if (attackCool <= 0) {
            animator.SetTrigger("Attack");
            gameManager.soundManager.slayerAttackSFX(GetComponentInChildren<AudioSource>());
            // Attacks the shadow
            if (player.shadowActive == true) {
                if (Vector3.Distance(transform.position, player.shadowObject.transform.position) <= 1.5f) {
                    player.shadow();
                    attackCool = attackSpeed;
                }
            }
            // Attacks the player
            else {
                player.GetComponent<PlayerControl>().takeDamage(1);
                attackCool = attackSpeed;
            }
        }
    }
}
