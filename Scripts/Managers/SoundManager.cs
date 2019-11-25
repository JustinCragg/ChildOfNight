using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/************************************************************************************************************
Class for handling game sounds
************************************************************************************************************/
public class SoundManager : MonoBehaviour {
    // Sound Sources
    [Header("Audio Sources")]
    public AudioSource backgroundSFX;
    public AudioSource gameSFX;
    public AudioSource playerSFX;
    List<AudioSource> villagerSFX = new List<AudioSource>();

    // Background
    [Header("Background Sounds")]
    public AudioClip[] backgroundMusic;

    // Player
    [Header("Player Sounds")]
    public AudioClip cloak;
    public AudioClip shadow;
    public AudioClip dash;
    public AudioClip hypnotise;
    public AudioClip bite;
    public AudioClip consume;
    public AudioClip[] playerDamage;
    public AudioClip playerDeath;

    // Game
    [Header("Game Sounds")]
    public AudioClip dayEnd;
    public AudioClip gameover;
    public AudioClip objWon;
    public AudioClip victory;
    public AudioClip footSteps;

    // Guards
    [Header("Guard Sounds")]
    public AudioClip[] guardAlert;
    public AudioClip[] guardAttack;
    public AudioClip guardDamage;
    public AudioClip guardDeath;

    // Drunkards
    [Header("Drunkard Sounds")]
    public AudioClip[] drunkAlert;
    public AudioClip[] drunkWalk;
    public AudioClip drunkDamage;
    public AudioClip drunkDeath;

    // Nobles
    [Header("Noble Sounds")]
    public AudioClip[] nobleAlert;
    public AudioClip nobleDamage;
    public AudioClip nobleDeath;

    // Slayer
    [Header("Slayer Sounds")]
    public AudioClip[] slayerAlert;
    public AudioClip slayerAttack;

    // Find audio sources
    public void init() {
        // Game
        playerSFX = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<AudioSource>();
        backgroundSFX = GameObject.FindGameObjectWithTag("BackgroundSFX").GetComponent<AudioSource>();
        gameSFX = GameObject.FindGameObjectWithTag("GameSFX").GetComponent<AudioSource>();

        // Villagers
        foreach (AudioSource source in FindObjectsOfType<AudioSource>()) {
            if (source.gameObject.tag == "Villager") {
                villagerSFX.Add(source);
            }
        }
    }

    /********************************************************************************************************
     Settings
    ********************************************************************************************************/
    // Apply background music settings
    public void setBGMVolume(float volume) {
        backgroundSFX.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }
    
    // Apply game sound settings
    public void setSFXVolume(float volume) {
        // Set volume of each villager
        foreach (AudioSource source in villagerSFX) {
            source.volume = volume;
        }
        // Set game and player volume
        playerSFX.volume = volume;
        gameSFX.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // Pause audio
    public void pauseAudio() {
        AudioListener.pause = true;
        backgroundSFX.ignoreListenerPause = true;
    }

    // Resume audio
    public void unPauseAudio() {
        AudioListener.pause = false;
    }

    /********************************************************************************************************
     Background Sounds
    ********************************************************************************************************/
    // Set and play background music
    public void playBackgroundMusic(int nightNum) {
        backgroundSFX.clip = backgroundMusic[nightNum];
        backgroundSFX.Play();
    }

    // Stop background music
    public void stopBackgroundMusic() {
        backgroundSFX.Stop();
    }

    /********************************************************************************************************
     Player Sounds
    ********************************************************************************************************/
    // Invisiblity sound effect
    public void playerInvisSFX() {
        playerSFX.PlayOneShot(cloak, 1.0f);
    }

    // Shadow sound effect
    public void playerShadowSFX() {
        playerSFX.PlayOneShot(shadow, 1.0f);
    }

    // Dash sound effect
    public void playerDashSFX() {
        playerSFX.PlayOneShot(dash, 1.0f);
    }

    // Hypnotise sound effect
    public void playerHypnoSFX() {
        playerSFX.PlayOneShot(hypnotise, 1.0f);
    }

    // Bite sound effect
    public void playerBiteSFX() {
        playerSFX.PlayOneShot(bite, 1.0f);
    }

    // Consume sound effect
    public void playerConsumeSFX() {
        playerSFX.PlayOneShot(consume, 1.0f);
    }

    // Damage sound effects
    public void playerDamageSFX(int numDamage) {
        playerSFX.PlayOneShot(playerDamage[Random.Range(0, numDamage)], 1.0f);
    }
 
    // Death sound effect
    public void playerDeathSFX() {
        playerSFX.PlayOneShot(playerDeath, 1.0f);
    }

    // Footsteps when walking sound effect
    public void playerWalkFootstepsSFX() {
        playerSFX.PlayOneShot(footSteps, 0.2f);
    }

    // Footsteps when running sound effect
    public void playerRunFootstepsSFX() {
        playerSFX.PlayOneShot(footSteps, 0.4f);
    }

    /********************************************************************************************************
     Game Sounds
    ********************************************************************************************************/
    // Day completed sound effect
    public void dayEndSFX() {
        gameSFX.PlayOneShot(dayEnd, 1.0f);
    }

    // Gameover sound effect
    public void gameoverSFX() {
        gameSFX.PlayOneShot(gameover, 1.0f);
    }

    // Objective completed sound effect
    public void objWonSFX() {
        gameSFX.PlayOneShot(objWon, 1.0f);
    }

    // Victory sound effect
    public void victorySFX() {
        gameSFX.PlayOneShot(victory, 1.0f);
    }

    // Footsteps when NPC's are walking sound effect
    public void NPCSlowFootstepsSFX(AudioSource source) {
        source.PlayOneShot(footSteps, 0.4f);
    }

    // Footsteps when NPC's are running sound effect
    public void NPCFastFootstepsSFX(AudioSource source) {
        source.PlayOneShot(footSteps, 0.8f);
    }

    /********************************************************************************************************
     Guard Sounds
    ********************************************************************************************************/
    // Gaurd alert sound effects
    public void guardAlertSFX(AudioSource source) {
        source.PlayOneShot(guardAlert[Random.Range(0, guardAlert.Length)], 1.0f);
    }

    // Gaurd attack sound effects
    public void guardAttackSFX(AudioSource source) {
        source.PlayOneShot(guardAttack[Random.Range(0, guardAttack.Length)], 1.0f);
    }

    // Gaurd damage sound effect
    public void guardDamageSFX(AudioSource source) {
        source.PlayOneShot(guardDamage, 1.0f);
    }

    // Gaurd death sound effect
    public void guardDeathSFX(AudioSource source) {
        source.PlayOneShot(guardDeath, 1.0f);
    }

    /********************************************************************************************************
     Drunkard Sounds
    ********************************************************************************************************/
    // Drunkard alert sound effects
    public void drunkardAlertSFX(AudioSource source) {
        source.PlayOneShot(drunkAlert[Random.Range(0, drunkAlert.Length)], 1.0f);
    }

    // Drunkard ambient walking sound effects
    public void drunkardWalkSFX(AudioSource source) {
        source.PlayOneShot(drunkWalk[Random.Range(0, drunkWalk.Length)], 1.0f);
    }

    // Drunkard damage sound effect
    public void drunkardDamageSFX(AudioSource source) {
        source.PlayOneShot(drunkDamage, 1.0f);
    }

    // Drunkard death sound effect
    public void drunkardDeathSFX(AudioSource source) {
        source.PlayOneShot(drunkDeath, 1.0f);
    }

    /********************************************************************************************************
     Noble Sounds
    ********************************************************************************************************/
    // Noble alert sound effects
    public void nobleAlertSFX(AudioSource source) {
        source.PlayOneShot(nobleAlert[Random.Range(0, nobleAlert.Length)], 1.0f);
    }

    // Noble damage sound effect
    public void nobleDamageSFX(AudioSource source) {
        source.PlayOneShot(nobleDamage, 1.0f);
    }

    // Noble death sound effect
    public void nobleDeathSFX(AudioSource source) {
        source.PlayOneShot(nobleDeath, 1.0f);
    }

    /********************************************************************************************************
     Slayer Sounds
    ********************************************************************************************************/
    // Slayer alert sound effects
    public void slayerAlertSFX(AudioSource source) {
        source.PlayOneShot(slayerAlert[Random.Range(0, slayerAlert.Length)], 1.0f);
    }

    // Slayer attack sound effect
    public void slayerAttackSFX(AudioSource source) {
        source.PlayOneShot(slayerAttack, 1.0f);
    }
}
