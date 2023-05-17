using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InGameOptions : MonoBehaviour
{
    private RunTimer script;
    public void ReturnToMenu()
    {
        GameObject other = GameObject.Find("Timer");
        script=other.GetComponent<RunTimer>();
        script.lockUnlockTimer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
}
