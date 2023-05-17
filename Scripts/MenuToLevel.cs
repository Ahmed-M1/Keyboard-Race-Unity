using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuToLevel : MonoBehaviour
{


    //BG music to start
    [SerializeField] private AudioSource music;
   

    public void StartGame()
    {
        
      
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void Update()
    {
        music.volume = Settings_Values.volume / 100;
    }
   
}
