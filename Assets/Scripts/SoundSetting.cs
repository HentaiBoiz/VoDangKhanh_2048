using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    public static SoundSetting Instance;

    public Button button;

    public AudioSource backgroundMusic;
    public AudioSource sfxWin;
    public AudioSource SfxLose;

    public Image theImage;

    public Sprite enableSoundSprite;
    public Sprite disableSoundSprite;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Debug.Log("Found more than one Sound Setting in the scene");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (backgroundMusic.isPlaying)
        {
            theImage.sprite = enableSoundSprite;
            theImage.color = Color.yellow;
        }
        else
        {
            theImage.sprite = disableSoundSprite;
            theImage.color = Color.yellow;
        }
        AddOnClickEvent(); 
    }

    private void AddOnClickEvent()
    {
        this.button.onClick.AddListener(this.SetSoundIcon);
    }

    public void SetSoundIcon()
    {
        if (backgroundMusic.isPlaying)
        {
            OffBackGroundMusic();
        }
        else
        {
            OnBackGroundMusic();
        }
    }
    public void OffBackGroundMusic()
    {
        theImage.sprite = disableSoundSprite;
        theImage.color = Color.yellow;
        backgroundMusic.Pause();
    }

    public void OnBackGroundMusic()
    {
        theImage.sprite = enableSoundSprite;
        theImage.color = Color.yellow;
        backgroundMusic.Play();
    }

    public void OnSFXSound(AudioSource sfxMusic)
    {
        sfxMusic.Play();
    }

    public void OffSFXSound(AudioSource sfxMusic)
    {
        sfxMusic.Stop();
    }

}
