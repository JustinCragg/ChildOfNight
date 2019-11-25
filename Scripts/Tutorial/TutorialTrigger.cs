using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************************************************
Handles triggering the tutorial events
************************************************************************************************************/
public class TutorialTrigger : MonoBehaviour {
    [Tooltip("Which tutorial event does this trigger trigger")]
    public TutorialType tutorialType;
    [HideInInspector]
    // Reference to the tutorial manager
    public TutorialManager tutorialManager = null;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.GetComponent<PlayerControl>() != null) {
            // Displays the tutorial popup and sets gamemanager to be in dialogue
            tutorialManager.displayTutorial(tutorialType);
            tutorialManager.gameManager.inDialogue = true;
        }
    }
}
