using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Enum which holds each of the different states the guard can be in
public enum GuardState { PATROLING, INVESTIGATING, ATTACKING, ALERTING };

/************************************************************************************************************
Child class for NPC guards which handles AI behaviours and attacking
************************************************************************************************************/
public class GuardControl : BaseControl {
    [Header("Guard Stuff")]
    // Max attack cooldown
    [Tooltip("The amount of time between Guard attacks")]
    public float attackSpeed = 3.0f;
    // The amount of damage that is done each hit
    [Tooltip("The amount of damage inflicted by the Guard")]
    public int attackDamage = 1;
    // Attack cooldown
    float attackCool = 0;

    [Tooltip("The current state the AI is in")]
    public GuardState guardState = GuardState.PATROLING;
    [Space(10)]
    [Tooltip("The list of position the Guard will patrol between")]
    public List<GameObject> waypoints = new List<GameObject>();
    int currentTarget = 0;

    public override void init() {
        base.init();
        charType = GoalType.GUARD;
        // Moves towards first waypoint
        setDestination(waypoints[currentTarget].transform.position);
    }

    protected override void Update() {
        if (gameManager != null) {
            base.Update();

            // Update values
            updateAlertness();
            updateState();

            if (grappled == false && hypnotised == false) {
                Vector3 velocity = Vector3.zero;
                switch (guardState) {
                    // The guard's patrolling state
                    // Moves around from waypoint to waypoint
                    case GuardState.PATROLING:
                        agent.speed = normalSpeed;
                        if (agent.remainingDistance <= 1.0f) {
                            currentTarget++;
                            if (currentTarget == waypoints.Count) {
                                currentTarget = 0;
                            }
                            setDestination(waypoints[currentTarget].transform.position);
                        }

                        // Updates animator
                        velocity = agent.velocity.normalized;
                        velocity = transform.InverseTransformDirection(velocity);
                        animator.SetFloat("VelX", velocity.x);
                        animator.SetFloat("VelY", velocity.z);
                        break;
                    // The guard's investigating state
                    // Moves to the location of the disturbance
                    case GuardState.INVESTIGATING:
                        agent.speed = slowSpeed;
                        if (playerVisible == true) {
                            gameManager.soundManager.guardAlertSFX(GetComponentInChildren<AudioSource>());
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
                    // The guard's attacking state
                    // Investigates the area around the disturbance
                    case GuardState.ATTACKING:
                        agent.speed = fastSpeed;
                        // If player is visible
                        if (playerVisible == true) {
                            // Move towards player
                            setDestination(Disturbance);
                        }
                        // If player is not visible
                        else {
                            // Randomly move around in area around Disturbance
                            if (agent.remainingDistance <= 1.0f) {
                                Vector3 rand = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 10)).normalized * 5.0f;
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
                    // The guard's alerting state
                    // Investigates the area around the disturbance, same as attacking state but also alerts nearby villagers
                    case GuardState.ALERTING:
                        agent.speed = fastSpeed;
                        // Alert nearby npcs
                        alert();
                        // If player is visible
                        if (playerVisible == true) {
                            // Move towards player and attack
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
                                Vector3 rand = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 10)).normalized * 3.0f;
                                rand += (Vector3.zero - transform.position).normalized;
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
            }
            else {
                agent.speed = 0.0f;
            }

            // Reduces attack cooldown
            if (attackCool > 0 && grappled == false && hypnotised == false) {
                attackCool -= Time.deltaTime;
            }
        }
    }

    // Determines what levels of alertness trigger which states
    void updateState() {
        switch (alertness) {
            // Patrolling
            case 0:
            case 1:
            case 2:
            case 3:
                guardState = GuardState.PATROLING;
                break;
            // Investigating
            case 4:
            case 5:
                guardState = GuardState.INVESTIGATING;
                break;
            // Searching
            case 6:
            case 7:
                guardState = GuardState.ATTACKING;
                break;
            // Alerting
            case 8:
            case 9:
                guardState = GuardState.ALERTING;
                break;
        }
    }

    // Attacks the player if able
    void attack() {
        Vector3 dir = Disturbance - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, fastSpeed * Time.deltaTime);

        setDestination(transform.position);
        if (attackCool <= 0) {
            animator.SetTrigger("Attack");
            gameManager.soundManager.guardAttackSFX(GetComponentInChildren<AudioSource>());
            if (player.shadowActive == true) {
                if (Vector3.Distance(transform.position, player.shadowObject.transform.position) <= 1.5f) {
                    player.shadow();
                    attackCool = attackSpeed;
                }
            }
            else {               
                attackCool = attackSpeed;
            }
        }
    }

    // Damages the player
    // Called when the animation's swing reaches its apex
    public void hurtVampire() {
        if (player.shadowActive == false) {
            player.GetComponent<PlayerControl>().takeDamage(attackDamage);
        }
    }
}