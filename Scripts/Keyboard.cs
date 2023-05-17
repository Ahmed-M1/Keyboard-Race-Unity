using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    private RunTimer script;
    public void GetKeyboard()
    {
        //if time locked then dont open keyboard
        //Start timer
        GameObject other = GameObject.Find("Timer");
        script = other.GetComponent<RunTimer>();
        
        if(!script.getTimerState())
        {
            TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }

       
    }
   
}
