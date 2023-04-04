using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    public static SoundSetting Instance;

    public Button button;
    public AudioSource music;
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
        if (music.isPlaying)
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
        if (music.isPlaying)
        {
            theImage.sprite = disableSoundSprite;
            theImage.color = Color.yellow;
            music.Pause();
        }
        else
        {
            theImage.sprite = enableSoundSprite;
            theImage.color = Color.yellow;
            music.Play();
        }
    }
}
