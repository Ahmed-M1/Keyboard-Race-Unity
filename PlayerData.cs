using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.UIElements;

using TMPro;

//You have to set up winner, and result

/*Things  that dont work*/
/*
 * Result screen doesnt appear
 * Time doesn't start
 * 
 
 
 */


// A script inside the Player prefab
public class PlayerData : NetworkBehaviour
{
    private GameObject Getprogress;
    private OnlinePlaying script;


    [SerializeField] private TextMeshProUGUI progress;
    [SerializeField] private TextMeshProUGUI winnerName;
    [SerializeField] private TextMeshProUGUI time;
    bool initialize = false;



    [SerializeField] private TextMeshProUGUI prog;

    private NetworkVariable<float> WinnerProgress = new NetworkVariable<float>(0.00f);

    private NetworkVariable<float> current_Progress = new NetworkVariable<float>(0.00f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<ushort> Winnertime = new NetworkVariable<ushort>();
    private NetworkVariable<ulong> winnerClientId = new NetworkVariable<ulong>(68);
    private NetworkVariable<bool> stopForAll = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    RunTimerOnline timeObj;


    GameObject Timer;
    //[SerializeField] private GameObject resultScreen;
    bool open = false;


    //Initialize Value

    public override void OnNetworkSpawn()
    {
        current_Progress.Value = 0 ;
        prog.text = current_Progress.Value.ToString("0.0") + "%";
        stopForAll.Value = false;

    }



    // Start is called before the first frame update
    void Start()
    {

        Getprogress = GameObject.Find("Test Text");
        script = Getprogress.GetComponent<OnlinePlaying>();
        Timer = GameObject.Find("Timer");
        timeObj = Timer.GetComponent<RunTimerOnline>();


        current_Progress.Value = 0;

        // Add a callback for ValueChanged event of current_Progress NetworkVariable
        current_Progress.OnValueChanged += OnProgressValueChanged;
       


    }




    // Update is called once per frame
    void Update()
    {

        // Check if the timer has reached

        if (IsOwner && !stopForAll.Value)
        {

            UpdateValue(current_Progress.Value);

            // Update UI
            prog.text = current_Progress.Value.ToString("0.0") + "%";
            


        }


        else if(current_Progress.Value==100f && !stopForAll.Value)
        {
            
            SetWinnerServerRpc();
            SetWinner();


            UpdateResultScreen();
        }

        else if ((timeObj.GetTimer() < 1) && !stopForAll.Value)
        {



            SetWinner();

            UpdateResultScreen();
           
            //Debug.Log("Value for stop: " + stopForAll.Value);
           
          

        }

        /*else if(stopForAll.Value)
        {
            gameObject.SetActive(false);
        }*/



        if (NetworkManager.Singleton.IsServer)
        {

            Debug.Log("Server time and open: " + timeObj.GetTimer() + "-" + stopForAll.Value);
        }

        else
        {
            Debug.Log("Client time and open: " + timeObj.GetTimer() + "-" + stopForAll.Value);

        }


        UpdateResultScreen();

    }

    [ServerRpc (RequireOwnership=false)]
    private void SetWinnerServerRpc()
    {
        /*if (NetworkManager.Singleton.IsServer)
        {*/

            Debug.Log("RPC called");
            Debug.Log("Client won!");
            stopForAll.Value = true;
            // Determine the client with the highest progress
            float highestProgress = 0;
            //Debug.Log("Has enetered client");
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                //Debug.Log("Entered");
                float clientProgress = client.PlayerObject.GetComponent<PlayerData>().get_Progress();
                Debug.Log("---");
                Debug.Log(clientProgress);


                if (clientProgress > highestProgress)
                {
                    highestProgress = clientProgress;
                    winnerClientId.Value = client.ClientId;
                    // Debug.Log("Reach here 3");
                }

                Debug.Log("---");
            }



            // Get the timer component and calculate the winning time and progress
            timeObj = Timer.GetComponent<RunTimerOnline>();
            WinnerProgress.Value = highestProgress;
            Winnertime.Value = (ushort)(timeObj.GetTimer());

        //}
    }

    private void SetWinner()
    {

            Debug.Log("Server won!");
            float highestProgress = 0;

            if (NetworkManager.Singleton.IsServer)
                stopForAll.Value = true;
            // Determine the client with the highest progress

            //Debug.Log("Has enetered client");
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                //Debug.Log("Entered");
                float clientProgress = client.PlayerObject.GetComponent<PlayerData>().get_Progress();
                Debug.Log("---");
                Debug.Log(clientProgress);


                if (clientProgress > highestProgress)
                {
                    highestProgress = clientProgress;
                    winnerClientId.Value = client.ClientId;
                    // Debug.Log("Reach here 3");
                }

                Debug.Log("---");
            }



            // Get the timer component and calculate the winning time and progress
            timeObj = Timer.GetComponent<RunTimerOnline>();
            WinnerProgress.Value = highestProgress;
            Winnertime.Value = (ushort)(timeObj.GetTimer());
        

    }

    public float get_Progress()
    {
        return current_Progress.Value;
    }

    public bool get_stopForAll()
    {
        return stopForAll.Value;
    }

    void UpdateValue(float newValue)
    {

        //Debug.Log("Called Server!");
        float newVal = CalculateNewValue();
        current_Progress.Value = newVal;

    }



   

    // Example method for updating the attribute value
    private float CalculateNewValue()
    {
        if (script != null)
        {
            return script.progress_float;
        }
        else
        {
            return 0.0f; // Or any default value you want to use
        }
    }

    // Callback for ValueChanged event of current_Progress NetworkVariable
    private void OnProgressValueChanged(float oldValue, float newValue)
    {
        // Update UI with the new value     
        prog.text = newValue.ToString("0.0") + "%";

    }


    private void UpdateResultScreen()
    {
   
        
        if(stopForAll.Value)
        {

            Debug.Log("Entered StopForAll");


            script.get_ResultScreen().SetActive(true);
            // Set resultScreen active
            Debug.Log("End: " + WinnerProgress.Value + " -" + winnerClientId.Value + "-" + Winnertime.Value);
            // Update the text of the child objects
            script.progress.text = "Progress: " + (WinnerProgress.Value).ToString("0.0") + "%";
            script.winnerName.text = "Winner: " + "P" + (winnerClientId.Value + 1).ToString();
            script.time.text = "Time: " + Winnertime.Value.ToString("0.00");
            //gameObject.SetActive(false);
            // Lock the timer
            timeObj.lockTimer();

            Invoke(nameof(DestroyObject), 0.5f);


        }


    }


    private void DestroyObject()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            //Debug.Log("Entered");
            client.PlayerObject.Despawn();
           
        }
    }
}
