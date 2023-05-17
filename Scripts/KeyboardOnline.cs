using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardOnline : MonoBehaviour
{
    private RunTimerOnline script;

    public void GetKeyboard()
    {
        //if time locked then dont open keyboard
        //Start timer
        GameObject other = GameObject.Find("Timer");
        script = other.GetComponent<RunTimerOnline>();
        
        if(!script.getTimerState())
        {
            TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }
       
    }
   
}
