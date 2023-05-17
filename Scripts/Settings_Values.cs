using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings_Values : MonoBehaviour
{
    //Attributes
    public static float volume=100;
    static int fontcolor=1;
    [SerializeField] private Slider vol_Slider;

    //BG music to start
    [SerializeField] private AudioSource music;


    void Start()
    {
// Debug.Log("Hello");
       // Debug.Log(volume);
        vol_Slider.value = volume;
        // Set the volume of the audio source to 0.5
        music.volume = volume / 100;

        // Subscribe to the onValueChanged event
        vol_Slider.onValueChanged.AddListener(OnSliderValueChanged);
    }


    void OnSliderValueChanged(float value)
    {
        volume = value;
        // Set the volume of the audio source to 0.5
        music.volume = volume / 100;
       
    }
  
}
