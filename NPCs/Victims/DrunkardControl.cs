using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/************************************************************************************************************
Child class for NPC drunkards which handles AI behaviours
************************************************************************************************************/
public class DrunkardControl : VictimControl {

    // Initalises
    public override void init() {
        base.init();

        charType = GoalType.DRUNKARD;
    }

    // Handles how the drunkard acts according to its alertness/state
    protected override void Update() {
        if (gameManager != null) {
            base.Update();

            if (inHome == false) {
                // Updates values
                updateAlertness();
                updateState();

                // If the npc is not grappled or hypnotised
                if (grappled == false && hypnotised == false) {
                    animator.SetBool("Grappled", false);
                    Vector3 velocity = Vector3.zero;
                    // The drunkards's wandering state
                    // Moves around according to a random wander steering behaviour
                    switch (victimState) {
                        case VictimState.WANDERING:
                            agent.speed = slowSpeed;

                            fleeing = false;
                            if (agent.remainingDistance <= 1.0f) {
                                setRandomDestination();
                            }

                            // Updates animator
                            animator.SetBool("Confused", false);
                            velocity = agent.velocity.normalized;
                            velocity = transform.InverseTransformDirection(velocity);
                            animator.SetFloat("VelX", velocity.x);
                            animator.SetFloat("VelY", velocity.z);
                            break;
                        // The drunkard's wary state
                        // Stops moving
                        case VictimState.WARY:
                            agent.speed = 0;
                            Vector3 dir = Disturbance - transform.position;
                            Quaternion rot = Quaternion.LookRotation(dir);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rot, fastSpeed * Time.deltaTime);
                            fleeing = false;

                            // Updates animator
                            animator.SetBool("Confused", true);
                            velocity = agent.velocity.normalized;
                            velocity = transform.InverseTransformDirection(velocity);
                            animator.SetFloat("VelX", velocity.x);
                            animator.SetFloat("VelY", velocity.z);
                            break;
                        // The drunkard's fleeing state
                        // Runs towards their home
                        case VictimState.FLEEING:
                            agent.speed = normalSpeed;

                            if (fleeing == false) {
                                gameManager.soundManager.drunkardAlertSFX(GetComponentInChildren<AudioSource>());
                                setDestination(home.transform.position);
                                fleeing = true;
                            }
                            if (isAtHome()) {
                                if (SceneManager.GetActiveScene().name != "Tutorial") {
                                    enterHome();
                                }
                            }

                            // Updates animator
                            animator.SetBool("Confused", false);
                            velocity = agent.velocity.normalized;
                            velocity = transform.InverseTransformDirection(velocity);
                            animator.SetFloat("VelX", velocity.x);
                            animator.SetFloat("VelY", velocity.z);
                            break;
                        // The drunkard's alerting state
                        // Runs towards their home, same as fleeing state but also alerts nearby villagers
                        case VictimState.ALERTING:
                            // Alert nearby npcs
                            agent.speed = fastSpeed;
                            if (fleeing == false) {
                                setDestination(home.transform.position);
                                fleeing = true;
                            }
                            if (isAtHome()) {
                                enterHome();
                            }

                            // Updates animator
                            animator.SetBool("Confused", false);
                            velocity = agent.velocity.normalized * 2.0f;
                            velocity = transform.InverseTransformDirection(velocity);
                            animator.SetFloat("VelX", velocity.x);
                            animator.SetFloat("VelY", velocity.z);
                            break;
                    }
                }
                else {
                    animator.SetBool("Grappled", true);
                    agent.speed = 0.0f;
                }
            }
            else {
                if (inHomeCount <= 0) {
                    exitHome();
                }
            }
        }
    }

    // Updates the state the drunkard is in depending on the current alertness of the noble
    void updateState() {
        switch (alertness) {
            // Wander
            case 0:
            case 1:
            case 2:
            case 3:
                victimState = VictimState.WANDERING;
                break;
            // Wary
            case 4:
            case 5:
                victimState = VictimState.WARY;
                break;
            // Flee
            case 6:
                victimState = VictimState.FLEEING;
                break;
            // Alert
            case 7:
            case 8:
            case 9:
                victimState = VictimState.ALERTING;
                break;
        }
    }
}