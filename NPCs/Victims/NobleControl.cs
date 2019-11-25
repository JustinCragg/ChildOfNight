using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************************************************
Child class for NPC nobles which handles AI behaviours
************************************************************************************************************/
public class NobleControl : VictimControl {

    // Initalises
    public override void init() {
        base.init();

        if (charType != GoalType.MAYOR) {
            charType = GoalType.NOBLE;
        }
    }

    // Handles how the noble acts according to its alertness/state
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
                    // The noble's wandering state
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
                        // The noble's wary state
                        // Stops moving
                        case VictimState.WARY:
                            agent.speed = 0;
                            fleeing = false;
                            Vector3 dir = Disturbance - transform.position;
                            Quaternion rot = Quaternion.LookRotation(dir);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rot, fastSpeed * Time.deltaTime);

                            // Updates animator
                            animator.SetBool("Confused", true);
                            velocity = agent.velocity.normalized;
                            velocity = transform.InverseTransformDirection(velocity);
                            animator.SetFloat("VelX", velocity.x);
                            animator.SetFloat("VelY", velocity.z);
                            break;
                        // The noble's fleeing state
                        // Runs towards their home
                        case VictimState.FLEEING:
                            agent.speed = normalSpeed;

                            if (fleeing == false) {
                                gameManager.soundManager.nobleAlertSFX(GetComponentInChildren<AudioSource>());
                                setDestination(home.transform.position);
                                fleeing = true;
                            }
                            if (isAtHome()) {
                                enterHome();
                            }

                            // Updates animator
                            animator.SetBool("Confused", false);
                            velocity = agent.velocity.normalized;
                            velocity = transform.InverseTransformDirection(velocity);
                            animator.SetFloat("VelX", velocity.x);
                            animator.SetFloat("VelY", velocity.z);
                            break;
                        // The noble's alerting state
                        // Runs towards their home, same as fleeing state but also alerts nearby villagers
                        case VictimState.ALERTING:
                            agent.speed = fastSpeed;
                            // Alert nearby npcs
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

    // Sets a random destination according to a random wander steering behaviour
    protected override void setRandomDestination() {
        // Gets a random direction on the circumference
        Vector3 target = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)).normalized * radius;

        // Adds a random jitter
        target += new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)).normalized * jitter;
        target = target.normalized * radius;
        // Adds a forward direction
        target += transform.forward * forwardDistance;
        target.y = 0;

        Vector3 homeDirec = (home.transform.position - transform.position).normalized * homePush;
        target += homeDirec;

        // Sets destination
        setDestination(transform.position + target);
    }

    // Updates the state the noble is in depending on the current alertness of the noble
    void updateState() {
        switch (alertness) {
            // Wander
            case 0:
            case 1:
                victimState = VictimState.WANDERING;
                break;
            // Wary
            case 2:
            case 3:
            case 4:
                victimState = VictimState.WARY;
                break;
            // Flee
            case 5:
            case 6:
            case 7:
                victimState = VictimState.FLEEING;
                break;
            // Alert
            case 8:
            case 9:
                victimState = VictimState.ALERTING;
                break;
        }
    }
}