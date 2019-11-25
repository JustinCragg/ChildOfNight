using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************************************************
Class that is attached to animation controllers, finds scripts and plays functions
************************************************************************************************************/
public class AnimationController : MonoBehaviour {
    // References
    [HideInInspector]
    public BaseControl baseControl = null;
    [HideInInspector]
    public GuardControl guardControl = null;
    [HideInInspector]
    public SoundManager soundManager = null;
    [HideInInspector]
    public PlayerControl playerControl = null;
    [HideInInspector]
    public AudioSource audioSource = null;

    // Animation event when player walks
    public void playerFootsteps() {
        if (soundManager == null) {
            soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        }
        if (playerControl == null) {
            playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        }
        if (playerControl.sprinting == true) {
            soundManager.playerRunFootstepsSFX();
        }
        else {
            soundManager.playerWalkFootstepsSFX();
        }
    }

    // Animation event when guard attacks player
    public void attackVampire() {
        if (guardControl == null) {
            guardControl = GetComponentInParent<GuardControl>();
        }
        guardControl.hurtVampire();
    }

    // Animation event when drunkard is walking around town
    public void drunkardWalk() {
        if (soundManager == null) {
            soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        }
        if (baseControl == null) {
            baseControl = GetComponentInParent<BaseControl>();
            audioSource = baseControl.GetComponentInChildren<AudioSource>();
        }
        soundManager.drunkardWalkSFX(audioSource);
    }

    // Animation event when an NPC dies
    public void NPCDeath() {
        if (baseControl == null) {
            baseControl = GetComponentInParent<BaseControl>();
        }
        baseControl.destroyNPC();
    }

    // Animation event when NPC's are walking
    public void NPCSlowFootsteps() {
        if (soundManager == null) {
            soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        }
        if (baseControl == null) {
            baseControl = GetComponentInParent<BaseControl>();
            audioSource = baseControl.GetComponentInChildren<AudioSource>();
        }
        soundManager.NPCSlowFootstepsSFX(audioSource);
    }

    // Animation event when NPC's are running
    public void NPCFastFootsteps() {
        if (soundManager == null) {
            soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        }
        if (baseControl == null) {
            baseControl = GetComponentInParent<BaseControl>();
            audioSource = baseControl.GetComponentInChildren<AudioSource>();
        }
        soundManager.NPCFastFootstepsSFX(audioSource);
    }
}
