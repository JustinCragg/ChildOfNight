using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Enum which holds each of the different staes the victims can be in
public enum VictimState { WANDERING, WARY, FLEEING, ALERTING }

/************************************************************************************************************
Child class for NPC Victims which handles AI behaviours
************************************************************************************************************/
public class VictimControl : BaseControl {
    [Header("Victim Stuff")]
    [Tooltip("The current state the Victim is in")]
    public VictimState victimState = VictimState.WANDERING;

    // The object the victim flees to
    [Tooltip("The location the Victim flees to when scared")]
    public GameObject home;

    [Header("Random Movement")]
    // For Random Movement
    [Tooltip("Affects how much the direction changes")]
    public float radius = 1;
    [Tooltip("Affects the randomness of the movement")]
    public float jitter = 1;
    [Tooltip("Affects the distance before recalculation")]
    public float forwardDistance = 1;

    [Tooltip("Affects how much the Victim is pulled towards their home normally")]
    public float homePush = 1;

    // Is the npc currently in their home
    protected bool inHome = false;
    [Tooltip("The amount of time the npc hides in their home for")]
    public float timeHiding = 5.0f;
    // The current amount of time the npc has been hiding for
    protected float inHomeCount = 0.0f;

    public override void init() {
        base.init();
        setRandomDestination();
    }

    protected override void Update() {
        base.Update();

        if (inHome == true) {
            inHomeCount -= Time.deltaTime;
        }
    }

    // When selected in the editor, displays the values for random wandering
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * forwardDistance, radius);
        if (agent != null) {
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }

    // Determines a random destination using a wander steering behaviour
    protected virtual void setRandomDestination() {
        // Gets a random direction on the circumphrence
        Vector3 target = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)).normalized * radius;

        // Adds a random jitter
        target += new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)).normalized * jitter;
        target = target.normalized * radius;
        // Adds a forward direction
        target += transform.forward * forwardDistance;
        target.y = 0;

        // Sets destination
        setDestination(transform.position + target);
    }

    // Returns whether the npc is currently outside their home
    protected bool isAtHome() {
        float dist = agent.remainingDistance;
        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= 1 && Vector3.Distance(transform.position, home.transform.position) <= 5) {
            return true;
        }
        else {
            return false;
        }
    }

    // Hides the npc and sets it to be in their home
    protected void enterHome() {
        foreach (Collider collider in GetComponentsInChildren<Collider>()) {
            collider.enabled = false;
        }
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            renderer.enabled = false;
        }
        GetComponent<NavMeshAgent>().enabled = false;
        alertness = minCalmDown;
        inHome = true;
        inHomeCount = timeHiding;
    }

    // Reveals the npc and removes it from their home
    protected void exitHome() {
        foreach (Collider collider in GetComponentsInChildren<Collider>()) {
            collider.enabled = true;
        }
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            renderer.enabled = true;
        }
        GetComponent<NavMeshAgent>().enabled = true;
        // Reset alertness to minimum level
        alertness = minCalmDown;
        inHome = false;
    }
}