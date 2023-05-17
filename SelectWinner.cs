using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.UIElements;

using TMPro;

public class SelectWinner : NetworkBehaviour
{
   /* private NetworkVariable<ulong> winnerClientId = new NetworkVariable<ulong>(5);
    private NetworkVariable<float> progresss = new NetworkVariable<float>();
    private NetworkVariable<ushort> Winnertime = new NetworkVariable<ushort>();
    RunTimerOnline timeObj;
    GameObject Timer;

   [SerializeField] private GameObject resultScreen;
    //private Transform progress;

    //[SerializeField] private GameObject client;

    void Start()
    {
        //Save timer
        Timer = GameObject.Find("Timer");
        winnerClientId.OnValueChanged += OnWinnerSelected;

    }

    private void OnWinnerSelected(ulong oldClientId, ulong newClientId)
    {
        
        ulong newWinnerClientId = 5;

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            float clientProgress = client.PlayerObject.GetComponent<PlayerData>().get_Progress();

            if (clientProgress==100f || timeObj.GetTimer()/60 == 0f)
            {
                
                newWinnerClientId= client.ClientId;
               

                
                timeObj = Timer.GetComponent<RunTimerOnline>();

                progresss.Value =(client.PlayerObject.GetComponent<PlayerData>().get_Progress());
                Winnertime.Value = (ushort)(timeObj.GetTimer() / 60);

               

            }
        }                                                                                                   

        if (newWinnerClientId != 5)
        {

            winnerClientId.Value = newWinnerClientId;
            // Find child objects of resultScreen
            Transform progress = resultScreen.transform.Find("progress");
            Transform winnerName = resultScreen.transform.Find("Winner");
            Transform time = resultScreen.transform.Find("Time");

            // Get TextMeshProUGUI components from the child objects
            TextMeshProUGUI progressText = progress.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI winnerNameText = winnerName.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = time.GetComponent<TextMeshProUGUI>();



            // Update the text of the child objects
            progressText.text = "Progress: " + (progresss.Value).ToString() + "%";
            winnerNameText.text = "Winner: " + "P" + winnerClientId.Value.ToString();
            timeText.text = "Time: " + Winnertime.ToString();

            // Set resultScreen active
            resultScreen.SetActive(true);

            GameObject other = GameObject.Find("Timer");
            timeObj = other.GetComponent<RunTimerOnline>();

            Debug.Log("Done");
            //timeObj.lockUnlockTimer();
        }




    }

*/




}
