using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{

  public GameObject loadingscreen;
  private RunTimer script;

    public void loadScene(int levelindex)
  {
        StartCoroutine(loadSceneAsynchronously(levelindex));
    
  }

  IEnumerator loadSceneAsynchronously(int levelindex)
    {
        AsyncOperation operation= SceneManager.LoadSceneAsync(levelindex);
        while(!operation.isDone)
        { 
            loadingscreen.SetActive(true);

            if (levelindex == 1 && operation.isDone==true)
            {
                //Start timer
                GameObject other = GameObject.Find("Timer");
                script = other.GetComponent<RunTimer>();
                script.getTimerState();

            }

                yield return null;
        }

     }

  

 
}
