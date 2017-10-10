using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    WorldManager worldManager;
    AudioSource musicSource;

    public AudioMixerGroup groupMusic;
    public AudioMixerGroup groupEffects;

    public Toggle musicToggle;
    public Toggle effectToggle;

    public bool playMusic = true;
    public bool playEffects = true;

    void Start () {
        worldManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<WorldManager>();
        musicSource = Camera.main.GetComponent<AudioSource>();
    }
	
	void Update () {
        if (worldManager.GetPlayer.Alive) { //make music sound as fast as the player goes
            musicSource.pitch = 1 + ((worldManager.worldSpeedMultiplier - 1) * 0.1f);
        }else {
            musicSource.pitch = Mathf.Clamp(musicSource.pitch - Time.deltaTime, 0, 10);
        }
    }
    
    /// <summary>
    /// Toggle music on or off
    /// </summary>
    public void ToggleMusic() {
        playMusic = !playMusic;
        groupMusic.audioMixer.SetFloat("MusicVolume", playMusic ? -10 : -80);
    }

    /// <summary>
    /// Toggle effects on or off
    /// </summary>
    public void ToggleEffects() {
        playEffects = !playEffects;
        groupMusic.audioMixer.SetFloat("EffectsVolume", playEffects ? -20 : -80);
    }
}
