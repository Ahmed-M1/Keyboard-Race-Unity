using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;



//Synchronize time
public class RunTimerOnline : NetworkBehaviour
{
    private float timeDuration = 60f;

    private float timer;

    private bool timerLock;

    public bool keyboardVisibility;

    [SerializeField] private GameObject NetworkUI;
    private NetworkVariable<float> ServerTime = new NetworkVariable<float>(60f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkUI uiScript;

    [SerializeField] private TextMeshProUGUI firstMinute;
    [SerializeField] private TextMeshProUGUI secondMinute;
    [SerializeField] private TextMeshProUGUI firstSecond;
    [SerializeField] private TextMeshProUGUI secondSecond;
   


  

    // Start is called before the first frame update
    void Start()
    {
        uiScript = NetworkUI.GetComponent<NetworkUI>();

        if (uiScript.get_start_Game())
        {
            keyboardVisibility = true;
            timerLock = false;
            ResetTimer();
        }

        
    }

    // Update is called once per frame
    void Update()
    {

        if (uiScript.get_start_Game())
        {
            //For CountUp
            /*if (timer <= 99f * 60f && !timerLock)
            {
                //Keep keyboard out
                timer += Time.deltaTime;
                UpdateTimerDisplay(timer);
            }*/

            if (ServerTime.Value > 1 && !timerLock )
            {
                //Server sets Value for time
                /*if(NetworkManager.Singleton.IsServer)
                {
                    ServerTime.Value -= Time.deltaTime;
                 }*/

                ServerTime.Value -= Time.deltaTime;

                if (!NetworkManager.Singleton.IsServer)
                    Debug.Log("Client -- " + ServerTime.Value);
                
               
                UpdateTimerDisplay(ServerTime.Value);
            }

            else
            {

                Flash();
            }

            if (timerLock)
            {

                //Keep keyboard out
                keyboardVisibility = true;
            }

        }
       

    }

    public float GetTimer()
    {
        return ServerTime.Value;
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

    public void lockTimer()
    {
        if (!timerLock)
        {
            timerLock = true;
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
