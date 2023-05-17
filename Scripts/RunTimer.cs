using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RunTimer : MonoBehaviour
{
    private readonly float timeDuration = 0f;

    private float timer;

    private bool timerLock;

    public bool keyboardVisibility;

    [SerializeField] private TextMeshProUGUI firstMinute;
    [SerializeField] private TextMeshProUGUI secondMinute;
    [SerializeField] private TextMeshProUGUI firstSecond;
    [SerializeField] private TextMeshProUGUI secondSecond;
   

    // Start is called before the first frame update
    void Start()
    {
        keyboardVisibility = true;
        timerLock = false;
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 99f * 60f  && !timerLock)
        {
            //Keep keyboard out
            timer += Time.deltaTime;
            UpdateTimerDisplay(timer);
        }

        else
        {
        
            Flash();
        }

        if (timerLock) {

            //Keep keyboard out
            keyboardVisibility = true;
        }

    }

    public float GetTimer()
    {
        return timer;
    }

    public void lockUnlockTimer()
    {
        if(!timerLock)
        {
            timerLock = true;
        }
        else
        {
            timerLock = false;
        }
        
    }

    public bool getTimerState()
    {
        return timerLock;
    }

    private void ResetTimer()
    {
        timer = timeDuration;
    }
    private void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

       string currenttime = string.Format("{00:00}{1:00}", minutes, seconds);
        

       firstMinute.text = currenttime[0].ToString();
       secondMinute.text = currenttime[1].ToString();
       firstSecond.text = currenttime[2].ToString();
       secondSecond.text = currenttime[3].ToString();

    }
    private void Flash()
    {
        timerLock = true;
    }
}
